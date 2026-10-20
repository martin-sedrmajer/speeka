using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Vox.Core;
using Vox.Voice;

namespace Vox;

/// <summary>
/// Spojka mezi stránkou (WebView2) a hlasem. Stránka posílá text, C# ho po větách
/// převádí na zvuk a posílá zpátky k přehrání. Nic neodchází na internet.
/// </summary>
public sealed class Bridge : IDisposable
{
    private readonly Action<string> _post;
    private readonly SettingsStore _settings = new();
    private readonly PiperTts _piper = new();
    private CancellationTokenSource? _speakCts;
    private int _warmupVersion;

    public Bridge(Action<string> post)
    {
        _post = post;
        if (_settings.Current.UiLanguage != "") Loc.Ui = _settings.Current.UiLanguage;
    }

    public async void Handle(string json)
    {
        try
        {
            var msg = JsonNode.Parse(json);
            switch (Str(msg?["type"]))
            {
                case "ready":
                    EnsureUiLanguage();
                    EnsureVoiceChosen();
                    PostInit();
                    _ = WarmUpAsync(_settings.Current);
                    break;

                case "speak":
                    var text = Str(msg?["text"]);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        // zopakování z historie (remember = false) pořadí nemění
                        var remember = msg?["remember"] is not JsonValue rv || !rv.TryGetValue<bool>(out var r) || r;
                        if (remember)
                        {
                            _settings.Remember(text.Trim());
                            Post(new { type = "history", items = _settings.Current.History });
                        }
                        await SpeakAsync(text, _settings.Current, _settings.Current.Language);
                    }
                    break;

                case "test":
                    // vyzkoušení hlasu z rozpracovaného nastavení – nic se neukládá
                    var draft = msg?["settings"]?.Deserialize<AppSettings>(SettingsStore.Json);
                    var testLang = Str(msg?["lang"]) == "cs" ? "cs" : "en";
                    if (draft != null)
                        await SpeakAsync(testLang == "cs"
                            ? "Dobrý den, takto zní můj hlas."
                            : "Hello, this is how I sound.", draft, testLang);
                    break;

                case "language.set":
                    var lang = Str(msg?["lang"]) == "cs" ? "cs" : "en";
                    var ls = _settings.Current;
                    if (ls.Language != lang)
                    {
                        ls.Language = lang;
                        _settings.Save(ls);
                        Post(new { type = "settings", settings = _settings.Current });
                        _ = WarmUpAsync(_settings.Current);
                    }
                    break;

                case "stop":
                    _speakCts?.Cancel();
                    break;

                case "settings.save":
                    var incoming = msg?["settings"]?.Deserialize<AppSettings>(SettingsStore.Json);
                    if (incoming != null)
                    {
                        var voiceChanged = VoiceKey(incoming) != VoiceKey(_settings.Current)
                                           || incoming.UiLanguage != _settings.Current.UiLanguage;   // stav hlasu je v jazyce rozhraní
                        incoming.History = _settings.Current.History;   // historii spravuje jen C#
                        incoming.VoiceChosen = true;
                        incoming.VoiceCsChosen = true;
                        _settings.Save(incoming);
                        if (_settings.Current.UiLanguage != "") Loc.Ui = _settings.Current.UiLanguage;
                        Post(new { type = "settings", settings = _settings.Current });
                        Post(new { type = "toast", text = Loc.T("Nastavení uloženo.", "Settings saved.") });
                        if (voiceChanged) _ = WarmUpAsync(_settings.Current);
                    }
                    break;

                case "phrases.save":
                    var categories = msg?["categories"]?.Deserialize<List<PhraseCategory>>(SettingsStore.Json);
                    if (categories != null)
                    {
                        var s = _settings.Current;
                        s.Categories = categories;
                        _settings.Save(s);
                        Post(new { type = "settings", settings = _settings.Current });
                    }
                    break;

                case "phrases.reset":
                    var cur = _settings.Current;
                    cur.Categories = Phrases.Defaults();
                    _settings.Save(cur);
                    Post(new { type = "settings", settings = _settings.Current });
                    Post(new { type = "toast", text = Loc.T("Fráze vráceny na výchozí.", "Default phrases restored.") });
                    break;

                case "history.clear":
                    var h = _settings.Current;
                    h.History = new();
                    _settings.Save(h);
                    Post(new { type = "history", items = h.History });
                    break;

                case "voices.refresh":
                    Post(new { type = "voices", voices = VoiceList(), windowsVoices = WindowsTts.Voices() });
                    break;

                case "voices.open":
                    try { Process.Start(new ProcessStartInfo("explorer.exe", $"\"{AppPaths.VoicesDirForShell}\"") { UseShellExecute = true }); }
                    catch { /* průzkumník se nespustil */ }
                    break;
            }
        }
        catch (Exception ex)
        {
            Post(new { type = "toast", text = "Chyba: " + ex.Message });
        }
    }

    // ---------- mluvení ----------

    /// <summary>Text po větách převede na zvuk a posílá stránce; první věta zazní, než se dopočítají další.</summary>
    private async Task SpeakAsync(string text, AppSettings s, string lang)
    {
        var cts = new CancellationTokenSource();
        Interlocked.Exchange(ref _speakCts, cts)?.Cancel();
        var token = cts.Token;

        var id = Guid.NewGuid().ToString("N");
        var parts = SplitSentences(text);
        var fx = Effects(s, lang);
        Post(new { type = "speak.start", id, parts = parts.Count });

        try
        {
            for (var i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                var wav = await Task.Run(() => Synthesize(part, s, lang), token);
                if (token.IsCancellationRequested) return;
                Post(new { type = "audio", id, part = i, last = i == parts.Count - 1, wav = Convert.ToBase64String(wav), fx });
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Post(new { type = "speak.error", id, message = ex.Message });
        }
    }

    private byte[] Synthesize(string text, AppSettings s, string lang)
    {
        text = ApplyPronunciation(text, lang == "cs" ? s.PronunciationCs : s.PronunciationEn);
        var v = s.VoiceFor(lang);
        if (v.Engine == "windows") return WindowsTts.Synthesize(text, v);

        // Výšku dělá stránka zrychleným přehráním; aby se nezměnilo i tempo, syntéza mluví o to pomaleji.
        var local = new VoiceSettings
        {
            Engine = v.Engine,
            PiperModelDir = v.PiperModelDir,
            PiperSpeaker = v.PiperSpeaker,
            PiperSpeed = v.PiperSpeed / Math.Clamp(s.Pitch, 0.8, 1.25),
            PiperNoiseScale = v.PiperNoiseScale,
            PiperNoiseW = v.PiperNoiseW
        };
        return _piper.Synthesize(text, local, lang);
    }

    /// <summary>
    /// Nahradí celá slova podle slovníku výslovnosti („napsáno = vysloví se“, jeden řádek na slovo).
    /// Velikost písmen se při hledání nerozlišuje.
    /// </summary>
    public static string ApplyPronunciation(string text, string? dictionary)
    {
        if (string.IsNullOrWhiteSpace(dictionary)) return text;
        foreach (var line in dictionary.Split('\n'))
        {
            var eq = line.IndexOf('=');
            if (eq <= 0) continue;
            var written = line[..eq].Trim();
            var spoken = line[(eq + 1)..].Trim();
            if (written.Length == 0) continue;
            // celé slovo: před ním ani za ním nesmí být písmeno
            var pattern = @"(?<!\p{L})" + Regex.Escape(written) + @"(?!\p{L})";
            text = Regex.Replace(text, pattern, spoken, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        return text;
    }

    /// <summary>Úprava zvuku při přehrávání – posílá se s každou větou, ať platí i pro zkoušku z nastavení.</summary>
    private static object Effects(AppSettings s, string lang) => new
    {
        volume = s.Volume,
        pitch = s.VoiceFor(lang).Engine == "windows" ? 1.0 : s.Pitch,
        leveler = s.Leveler,
        clarity = s.Clarity,
        effect = s.Effect
    };

    /// <summary>
    /// Rozdělí text na věty (. ! ? … nebo nový řádek). Krátké kousky se spojí s následujícím,
    /// ať hlas nemluví trhaně.
    /// </summary>
    public static List<string> SplitSentences(string text)
    {
        var raw = Regex.Split(text.Trim(), @"(?<=[.!?…])\s+|\r?\n+")
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        var parts = new List<string>();
        var buffer = "";
        foreach (var p in raw)
        {
            buffer = buffer.Length == 0 ? p : buffer + " " + p;
            if (buffer.Length >= 25)
            {
                parts.Add(buffer);
                buffer = "";
            }
        }
        if (buffer.Length > 0)
        {
            if (parts.Count > 0 && buffer.Length < 12) parts[^1] += " " + buffer;
            else parts.Add(buffer);
        }
        return parts;
    }

    /// <summary>Načte hlas dopředu, aby první věta nezačínala několikasekundovým čekáním.</summary>
    private async Task WarmUpAsync(AppSettings s)
    {
        var version = Interlocked.Increment(ref _warmupVersion);
        Post(new { type = "voice.status", state = "loading", text = Loc.T("Připravuji hlas…", "Preparing the voice…") });
        try
        {
            await Task.Run(() => Synthesize(s.Language == "cs" ? "Hotovo." : "Ready.", s, s.Language));
            if (version == _warmupVersion)
                Post(new { type = "voice.status", state = "ready", text = VoiceLabel(s) });
        }
        catch (Exception ex)
        {
            if (version == _warmupVersion)
                Post(new { type = "voice.status", state = "error", text = ex.Message });
        }
    }

    private static string VoiceKey(AppSettings s)
    {
        var v = s.VoiceFor(s.Language);
        return FormattableString.Invariant(
            $"{s.Language}|{v.Engine}|{v.PiperModelDir}|{v.PiperSpeaker}|{v.PiperNoiseScale:0.00}|{v.PiperNoiseW:0.00}|{v.WindowsVoice}");
    }

    private static string VoiceLabel(AppSettings s)
    {
        var v = s.VoiceFor(s.Language);
        var prefix = s.Language == "cs" ? Loc.T("Česky – ", "Czech – ") : Loc.T("Anglicky – ", "English – ");
        if (v.Engine == "windows")
            return prefix + (string.IsNullOrWhiteSpace(v.WindowsVoice) ? Loc.T("hlas Windows (výchozí)", "Windows voice (default)") : v.WindowsVoice);
        var dir = PiperTts.ResolveDir(v.PiperModelDir);
        if (dir == null) return prefix + Loc.T("hlas nenalezen", "voice not found");
        var name = Path.GetFileName(dir);
        if (PiperTts.IsSupertonic(dir))
            return $"{prefix}{name} · {PiperTts.SupertonicSpeakerLabel(v.PiperSpeaker)}";
        if (PiperTts.IsKokoro(dir))
        {
            var speaker = KokoroVoices.Name(dir, v.PiperSpeaker);
            var label = speaker == null ? null : KokoroVoices.Label(speaker);
            return $"{prefix}{name} · {label ?? Loc.T("mluvčí ", "speaker ") + v.PiperSpeaker}";
        }
        return prefix + name;
    }

    // ---------- první spuštění a seznamy ----------

    /// <summary>
    /// Jazyk rozhraní se při prvním spuštění vybere podle Windows. Na úplně nové instalaci
    /// se podle něj nastaví i jazyk řeči.
    /// </summary>
    private void EnsureUiLanguage()
    {
        var s = _settings.Current;
        if (s.UiLanguage == "")
        {
            var firstRun = !s.VoiceChosen && !s.VoiceCsChosen;
            s.UiLanguage = Loc.Detect();
            if (firstRun) s.Language = s.UiLanguage;
            _settings.Save(s);
        }
        Loc.Ui = s.UiLanguage;
    }

    /// <summary>
    /// Při prvním spuštění vybere nejlepší dostupný hlas. Supertonic (přibalený) mluví oběma jazyky
    /// stejným hlasem; bez něj čeština Piper cs_CZ nebo Windows, angličtina Kokoro, Piper nebo Windows.
    /// </summary>
    private void EnsureVoiceChosen()
    {
        var s = _settings.Current;
        if (s.VoiceChosen && s.VoiceCsChosen) return;
        var voices = PiperTts.ListVoices();
        var supertonic = voices.FirstOrDefault(v => PiperTts.IsSupertonic(v.Dir));
        VoiceSettings SupertonicVoice() => new()
        {
            Engine = "local",
            PiperModelDir = supertonic.Dir!,
            PiperSpeaker = PiperTts.SupertonicDefaultSpeaker,
            PiperSpeed = PiperTts.SupertonicDefaultSpeed
        };

        if (!s.VoiceCsChosen)
        {
            var czech = voices.FirstOrDefault(v => PiperTts.LanguageOf(v.Dir) == "cs");
            s.VoiceCs = supertonic.Dir != null
                ? SupertonicVoice()
                : czech.Dir != null
                // český Piper (Jirka) je nahraný pomalu – rovnou mu přidat tempo
                ? new VoiceSettings { Engine = "local", PiperModelDir = czech.Dir, PiperSpeed = 1.3 }
                : new VoiceSettings { Engine = "windows", WindowsVoice = WindowsTts.VoiceFor("cs-CZ") ?? "" };
            s.VoiceCsChosen = true;
        }

        if (s.VoiceChosen)
        {
            _settings.Save(s);
            return;
        }

        if (supertonic.Dir != null)
        {
            s.Voice = SupertonicVoice();
            s.VoiceChosen = true;
            _settings.Save(s);
            return;
        }

        var kokoro = voices.FirstOrDefault(v => PiperTts.IsKokoro(v.Dir));
        if (kokoro.Dir != null)
        {
            s.Voice.Engine = "local";
            s.Voice.PiperModelDir = kokoro.Dir;
            var names = KokoroVoices.Names(kokoro.Dir);
            var heart = names == null ? -1 : Array.IndexOf(names, "af_heart");
            s.Voice.PiperSpeaker = heart >= 0 ? heart : 0;
        }
        else if (voices.Any(v => PiperTts.LanguageOf(v.Dir) != "cs"))
        {
            s.Voice.Engine = "local";
            // Supertonic umí i anglicky, ale ať ho bere až po anglických hlasech
            var english = voices.Where(v => PiperTts.LanguageOf(v.Dir) != "cs")
                .OrderBy(v => PiperTts.IsSupertonic(v.Dir) ? 1 : 0).First();
            s.Voice.PiperModelDir = english.Dir;
            s.Voice.PiperSpeaker = 0;
        }
        else
        {
            s.Voice.Engine = "windows";
        }
        s.VoiceChosen = true;
        _settings.Save(s);
    }

    private void PostInit()
    {
        Post(new
        {
            type = "init",
            settings = _settings.Current,
            voices = VoiceList(),
            windowsVoices = WindowsTts.Voices(),
            voicesDir = AppPaths.VoicesDirForShell,
            version = typeof(Bridge).Assembly.GetName().Version?.ToString(3) ?? ""
        });
    }

    private static object VoiceList() =>
        PiperTts.ListVoices().Select(v => new
        {
            name = v.Name,
            dir = v.Dir,
            kokoro = PiperTts.IsKokoro(v.Dir),
            supertonic = PiperTts.IsSupertonic(v.Dir),
            lang = PiperTts.IsSupertonic(v.Dir) ? null : PiperTts.LanguageOf(v.Dir),
            speakers = PiperTts.IsSupertonic(v.Dir) ? SupertonicSpeakers() : KokoroSpeakers(v.Dir)
        }).ToList();

    private static object SupertonicSpeakers() =>
        Enumerable.Range(0, PiperTts.SupertonicSpeakerCount)
            .Select(i => new { id = i, label = PiperTts.SupertonicSpeakerLabel(i) }).ToList();

    /// <summary>Anglicky mluvící hlasy modelu Kokoro pro výběr podle jména (u Piperu null).</summary>
    private static object? KokoroSpeakers(string dir)
    {
        if (!PiperTts.IsKokoro(dir)) return null;
        var names = KokoroVoices.Names(dir);
        if (names == null) return null;
        return names
            .Select((n, i) => new { id = i, label = KokoroVoices.Label(n) })
            .Where(x => x.label != null)
            .ToList();
    }

    private void Post(object payload) => _post(JsonSerializer.Serialize(payload, SettingsStore.Json));

    private static string? Str(JsonNode? n) =>
        n is JsonValue v && v.TryGetValue<string>(out var s) ? s : null;

    public void Dispose()
    {
        _speakCts?.Cancel();
        _piper.Dispose();
    }
}
