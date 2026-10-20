using Vox.Core;
using SherpaOnnx;

namespace Vox.Voice;

/// <summary>
/// Lokální neuronové hlasy přes sherpa-onnx: Piper (stejná cesta jako v modu Verity) a Kokoro.
/// Piper: model *.onnx, tokens.txt a podsložka espeak-ng-data.
/// Kokoro: model.onnx, voices.bin, tokens.txt, espeak-ng-data a u vícejazyčných verzí lexicon-*.txt.
/// Supertonic: čtyři modely *.onnx, tts.json, unicode_indexer.bin a voice.bin; umí 31 jazyků
/// včetně češtiny, jazyk se volí při každé syntéze.
/// V paměti zůstávají dva naposledy použité hlasy (typicky dvě persony), ostatní se uvolní.
/// </summary>
public sealed class PiperTts : IDisposable
{
    private readonly object _lock = new();
    private readonly Dictionary<string, OfflineTts> _engines = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _recent = new();   // klíče od naposledy použitého
    private const int MaxLoaded = 2;

    /// <summary>
    /// Počet kroků výpočtu Supertonicu: víc = čistší zvuk, méně = rychlejší (důležité na slabších telefonech).
    /// Nastavuje spojka podle Nastavení a platformy.
    /// </summary>
    public int SupertonicSteps { get; set; } = 8;

    public byte[] Synthesize(string text, VoiceSettings v, string lang)
    {
        lock (_lock)
        {
            var dir = ResolveDir(v.PiperModelDir)
                      ?? throw new InvalidOperationException(
                          Loc.T("Nenašel jsem žádný hlas. Rozbal hlas Supertonic, Kokoro nebo Piper do vlastní podsložky v ",
                                "No voice found. Extract a Supertonic, Kokoro or Piper voice into its own subfolder of ")
                          + AppPaths.VoicesDir + ".");

            var speaker = Math.Max(0, v.PiperSpeaker);
            // Kokoro: britští mluvčí (bf_, bm_) potřebují britský slovník výslovnosti.
            var british = IsKokoro(dir) && File.Exists(Path.Combine(dir, "lexicon-gb-en.txt")) &&
                          (KokoroVoices.Name(dir, speaker)?.StartsWith('b') ?? false);
            // Piper: intonace a rytmus se nastavují při načtení modelu, proto jsou součástí klíče
            var noiseScale = (float)Math.Clamp(v.PiperNoiseScale, 0.1, 1.5);
            var noiseW = (float)Math.Clamp(v.PiperNoiseW, 0.1, 1.5);
            var supertonic = IsSupertonic(dir);
            var key = dir + (british ? "|gb" : "") +
                      (IsKokoro(dir) || supertonic ? "" : FormattableString.Invariant($"|{noiseScale:0.00}|{noiseW:0.00}"));

            if (!_engines.TryGetValue(key, out var tts))
            {
                tts = supertonic ? LoadSupertonic(dir)
                    : IsKokoro(dir) ? LoadKokoro(dir, british)
                    : Load(dir, noiseScale, noiseW);
                _engines[key] = tts;
            }
            Touch(key);

            var speed = (float)Math.Clamp(v.PiperSpeed, 0.5, 2.0);
            var audio = supertonic
                ? GenerateSupertonic(tts, text, speed, Math.Min(speaker, SupertonicSpeakerCount - 1), lang, SupertonicSteps)
                : tts.Generate(text, speed, speaker);
            if (audio.Samples == null || audio.Samples.Length == 0)
                throw new InvalidOperationException(Loc.T("Hlas nevytvořil žádný zvuk.", "The voice produced no sound."));
            return Wav.FromFloat(audio.Samples, audio.SampleRate);
        }
    }

    /// <summary>Označí hlas jako naposledy použitý a uvolní ty, které se dlouho nepoužily.</summary>
    private void Touch(string key)
    {
        _recent.RemoveAll(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
        _recent.Insert(0, key);
        while (_recent.Count > MaxLoaded)
        {
            var old = _recent[^1];
            _recent.RemoveAt(_recent.Count - 1);
            if (_engines.Remove(old, out var engine) && (object)engine is IDisposable d) d.Dispose();
        }
    }

    /// <summary>Nastavená složka, jinak první nalezený hlas (Supertonic má přednost, umí oba jazyky).</summary>
    public static string? ResolveDir(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured) && Directory.Exists(configured) && HasModel(configured))
            return configured;

        var voices = ListVoices();
        return voices.FirstOrDefault(v => IsSupertonic(v.Dir)).Dir ?? voices.FirstOrDefault().Dir;
    }

    /// <summary>
    /// Hlasy přibalené k aplikaci a hlasy ve vlastní složce %APPDATA%\Vox\voices (název + cesta).
    /// Přibalené mají u názvu poznámku.
    /// </summary>
    public static List<(string Name, string Dir)> ListVoices()
    {
        var list = new List<(string, string)>();
        foreach (var root in AppPaths.VoiceRoots)
        {
            var bundled = string.Equals(Path.GetFullPath(root), Path.GetFullPath(AppPaths.BundledVoicesDir), StringComparison.OrdinalIgnoreCase);
            try
            {
                foreach (var d in Directory.GetDirectories(root).Where(HasModel).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
                    list.Add((Path.GetFileName(d) + (bundled ? Loc.T(" (přibalený)", " (bundled)") : ""), d));
            }
            catch { /* složka nejde přečíst */ }
        }
        return list;
    }

    private static bool HasModel(string dir) => Directory.GetFiles(dir, "*.onnx").Length > 0;

    /// <summary>
    /// Jazyk hlasu podle názvu modelu Piper (např. cs_CZ-jirka-medium.onnx → "cs"); Kokoro mluví anglicky.
    /// Null, když se to z názvu nedá poznat.
    /// </summary>
    public static string? LanguageOf(string dir)
    {
        if (IsKokoro(dir)) return "en";
        if (IsSupertonic(dir)) return null;   // vícejazyčný, jazyk se volí při syntéze
        try
        {
            foreach (var f in Directory.GetFiles(dir, "*.onnx"))
            {
                var m = System.Text.RegularExpressions.Regex.Match(Path.GetFileName(f), @"^([a-z]{2})_[A-Z]{2}[-_.]");
                if (m.Success) return m.Groups[1].Value;
            }
        }
        catch { /* složka nejde přečíst */ }
        return null;
    }

    /// <summary>Supertonic poznáme podle tts.json a unicode_indexer.bin.</summary>
    public static bool IsSupertonic(string dir) =>
        File.Exists(Path.Combine(dir, "tts.json")) && File.Exists(Path.Combine(dir, "unicode_indexer.bin"));

    /// <summary>Mluvčí modelu Supertonic 3 (pořadí = číslo mluvčího): 0–4 ženské F1–F5, 5–9 mužské M1–M5.</summary>
    public const int SupertonicSpeakerCount = 10;

    /// <summary>Popisek mluvčího v jazyce rozhraní, např. „Muž 2 (M2)“ / „Man 2 (M2)“.</summary>
    public static string SupertonicSpeakerLabel(int id)
    {
        id = Math.Clamp(id, 0, SupertonicSpeakerCount - 1);
        var female = id < 5;
        var n = female ? id + 1 : id - 4;
        return female ? Loc.T($"Žena {n} (F{n})", $"Woman {n} (F{n})") : Loc.T($"Muž {n} (M{n})", $"Man {n} (M{n})");
    }

    /// <summary>Výchozí mluvčí Supertonicu: Muž 2 (M2).</summary>
    public const int SupertonicDefaultSpeaker = 6;
    /// <summary>Výchozí rychlost Supertonicu pro češtinu (tempo modelu je v češtině na rozhovor trochu pomalé).</summary>
    public const double SupertonicDefaultSpeedCs = 1.15;
    /// <summary>Výchozí rychlost Supertonicu pro angličtinu (přirozené tempo modelu).</summary>
    public const double SupertonicDefaultSpeedEn = 1.0;

    // Jazyky, které Supertonic 3 umí; jiný kód by model odmítl.
    private static readonly HashSet<string> SupertonicLangs = new(StringComparer.OrdinalIgnoreCase)
    {
        "en", "ko", "ja", "ar", "bg", "cs", "da", "de", "el", "es", "et", "fi", "fr", "hi", "hr", "hu",
        "id", "it", "lt", "lv", "nl", "pl", "pt", "ro", "ru", "sk", "sl", "sv", "tr", "uk", "vi"
    };

    private static OfflineTtsGeneratedAudio GenerateSupertonic(OfflineTts tts, string text, float speed, int speaker, string lang, int steps)
    {
        var config = new OfflineTtsGenerationConfig
        {
            Sid = Math.Max(0, speaker),
            Speed = speed,
            NumSteps = Math.Clamp(steps, 2, 12)
        };
        config.Extra["lang"] = SupertonicLangs.Contains(lang) ? lang.ToLowerInvariant() : "en";
        return tts.GenerateWithConfig(text, config, (_, _, _, _) => 1);
    }

    private static OfflineTts LoadSupertonic(string dir)
    {
        // Když je víc variant, přednost má menší a rychlejší int8.
        string Find(string name)
        {
            var file = Directory.GetFiles(dir, name + "*.onnx")
                .OrderBy(f => f.Contains("int8", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .FirstOrDefault();
            return file ?? throw new InvalidOperationException(
                Loc.T($"Ve složce hlasu Supertonic chybí {name}*.onnx.", $"The Supertonic voice folder is missing {name}*.onnx."));
        }

        var config = new OfflineTtsConfig();
        config.Model.Supertonic.DurationPredictor = Find("duration_predictor");
        config.Model.Supertonic.TextEncoder = Find("text_encoder");
        config.Model.Supertonic.VectorEstimator = Find("vector_estimator");
        config.Model.Supertonic.Vocoder = Find("vocoder");
        config.Model.Supertonic.TtsJson = Path.Combine(dir, "tts.json");
        config.Model.Supertonic.UnicodeIndexer = Path.Combine(dir, "unicode_indexer.bin");
        config.Model.Supertonic.VoiceStyle = Path.Combine(dir, "voice.bin");
        if (!File.Exists(config.Model.Supertonic.VoiceStyle))
            throw new InvalidOperationException(Loc.T("Ve složce hlasu Supertonic chybí voice.bin.", "The Supertonic voice folder is missing voice.bin."));
        config.Model.NumThreads = Math.Clamp(Environment.ProcessorCount / 2, 1, 4);
        config.Model.Provider = "cpu";
        return new OfflineTts(config);
    }

    /// <summary>Kokoro poznáme podle souboru voices.bin s hlasy mluvčích.</summary>
    public static bool IsKokoro(string dir) => File.Exists(Path.Combine(dir, "voices.bin"));

    private static OfflineTts LoadKokoro(string dir, bool british)
    {
        var model = Path.Combine(dir, "model.onnx");
        if (!File.Exists(model)) model = Directory.GetFiles(dir, "*.onnx").First();
        var tokens = Path.Combine(dir, "tokens.txt");
        if (!File.Exists(tokens))
            throw new InvalidOperationException(Loc.T("Ve složce hlasu Kokoro chybí tokens.txt. Stáhni hlas ve formátu pro sherpa-onnx.",
                "The Kokoro voice folder is missing tokens.txt. Download the voice in the sherpa-onnx format."));

        var config = new OfflineTtsConfig();
        config.Model.Kokoro.Model = model;
        config.Model.Kokoro.Voices = Path.Combine(dir, "voices.bin");
        config.Model.Kokoro.Tokens = tokens;
        var espeak = Path.Combine(dir, "espeak-ng-data");
        if (Directory.Exists(espeak)) config.Model.Kokoro.DataDir = espeak;

        // Vícejazyčné verze mají slovníky výslovnosti; angličtina americká nebo britská podle mluvčího.
        var english = Path.Combine(dir, british ? "lexicon-gb-en.txt" : "lexicon-us-en.txt");
        var lexicons = new[] { english, Path.Combine(dir, "lexicon-zh.txt") }.Where(File.Exists).ToArray();
        if (lexicons.Length > 0) config.Model.Kokoro.Lexicon = string.Join(",", lexicons);
        var dict = Path.Combine(dir, "dict");
        if (Directory.Exists(dict)) config.Model.Kokoro.DictDir = dict;

        config.Model.NumThreads = Math.Clamp(Environment.ProcessorCount / 2, 1, 4);
        config.Model.Provider = "cpu";
        return new OfflineTts(config);
    }

    private static OfflineTts Load(string dir, float noiseScale, float noiseW)
    {
        var model = Directory.GetFiles(dir, "*.onnx").First();
        var tokens = Path.Combine(dir, "tokens.txt");
        if (!File.Exists(tokens))
            throw new InvalidOperationException(Loc.T("Ve složce hlasu chybí tokens.txt. Hlas musí být ve formátu pro sherpa-onnx.",
                "The voice folder is missing tokens.txt. The voice must be in the sherpa-onnx format."));

        var config = new OfflineTtsConfig();
        config.Model.Vits.Model = model;
        config.Model.Vits.Tokens = tokens;
        var espeak = Path.Combine(dir, "espeak-ng-data");
        if (Directory.Exists(espeak)) config.Model.Vits.DataDir = espeak;
        config.Model.Vits.NoiseScale = noiseScale;
        config.Model.Vits.NoiseScaleW = noiseW;
        config.Model.Vits.LengthScale = 1.0f;
        config.Model.NumThreads = Math.Clamp(Environment.ProcessorCount / 2, 1, 4);
        config.Model.Provider = "cpu";

        return new OfflineTts(config);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var tts in _engines.Values)
                if ((object)tts is IDisposable d) d.Dispose();
            _engines.Clear();
            _recent.Clear();
        }
    }
}
