using System.Text.Json;

namespace Vox.Core;

public sealed class AppSettings
{
    /// <summary>Jazyk řeči: en | cs. Každý jazyk má vlastní hlas.</summary>
    public string Language { get; set; } = "en";
    /// <summary>Hlas pro angličtinu.</summary>
    public VoiceSettings Voice { get; set; } = new();
    /// <summary>Hlas pro češtinu (Kokoro česky neumí – Piper cs_CZ nebo hlas Windows).</summary>
    public VoiceSettings VoiceCs { get; set; } = new() { Engine = "windows" };

    // Zvuk
    public double Volume { get; set; } = 1.0;          // 0 .. 2
    /// <summary>Výška hlasu (1 = původní). Mění se rychlostí přehrávání a tempo se dorovná při syntéze.</summary>
    public double Pitch { get; set; } = 1.0;           // 0.8 .. 1.25
    /// <summary>Vyrovnání hlasitosti (komprese), ať nic nezapadne v hluku.</summary>
    public bool Leveler { get; set; } = true;
    /// <summary>Zvýraznění pásma souhlásek a ořez hučení – líp srozumitelné pro nedoslýchavé.</summary>
    public bool Clarity { get; set; } = false;
    public string Effect { get; set; } = "none";       // none | robot | radio

    // Ovládání a vzhled
    public string FontSize { get; set; } = "l";        // m | l | xl
    public string Theme { get; set; } = "dark";        // dark | light
    public string Accent { get; set; } = "blue";       // blue | teal | amber | coral | purple
    public bool EnterSpeaks { get; set; } = true;
    /// <summary>Obrácené rozložení: tlačítko Říct vpravo, fráze zarovnané doprava.</summary>
    public bool FlipUi { get; set; }
    public bool ClearAfterSpeak { get; set; } = true;
    /// <summary>Nastavení a úpravy frází jsou zamčené za jednoduchým příkladem (pro pečovatele).</summary>
    public bool LockSettings { get; set; }

    public List<PhraseCategory> Categories { get; set; } = Phrases.Defaults();
    public List<string> History { get; set; } = new();

    /// <summary>Při prvním spuštění se vybere nejlepší nalezený hlas; pak už volí uživatel.</summary>
    public bool VoiceChosen { get; set; }
    public bool VoiceCsChosen { get; set; }

    /// <summary>
    /// Úprava výslovnosti: řádky „napsáno = vysloví se“. Na tlačítkách i v poli zůstane správný
    /// pravopis, hlas dostane upravený text (třeba Jirka nezvládá „včera“, zvládne „v čera“).
    /// </summary>
    // „sh“ se ve slově shledanou čte jako „sch“, hlasy to samy nepoznají.
    public string PronunciationCs { get; set; } = "včera = v čera\nshledanou = schledanou";
    public string PronunciationEn { get; set; } = "";

    /// <summary>Hlas pro daný jazyk.</summary>
    public VoiceSettings VoiceFor(string? language) => language == "cs" ? VoiceCs : Voice;
}

public sealed class VoiceSettings
{
    public string Engine { get; set; } = "local";      // local (Piper/Kokoro) | windows
    public string PiperModelDir { get; set; } = "";
    public int PiperSpeaker { get; set; }
    public double PiperSpeed { get; set; } = 1.0;
    /// <summary>Piper: živost intonace (noise scale). Víc = živější melodie, méně = plošší.</summary>
    public double PiperNoiseScale { get; set; } = 0.667;
    /// <summary>Piper: proměnlivost rytmu (noise w). Víc = přirozenější nepravidelné tempo.</summary>
    public double PiperNoiseW { get; set; } = 0.8;
    public string WindowsVoice { get; set; } = "";
    public int WindowsRate { get; set; }
}

public sealed class PhraseCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary>Název česky.</summary>
    public string Name { get; set; } = "";
    /// <summary>Název anglicky (prázdné = český).</summary>
    public string NameEn { get; set; } = "";
    public List<Phrase> Phrases { get; set; } = new();
}

public sealed class Phrase
{
    /// <summary>Fráze česky – v českém režimu je na tlačítku a řekne se.</summary>
    public string Label { get; set; } = "";
    /// <summary>Fráze anglicky – v anglickém režimu je na tlačítku a řekne se.</summary>
    public string Text { get; set; } = "";
    /// <summary>Naléhavá fráze (volání o pomoc) – výrazné tlačítko.</summary>
    public bool Urgent { get; set; }
}

public sealed class SettingsStore
{
    public const int MaxHistory = 40;

    public static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly object _lock = new();
    public AppSettings Current { get; private set; }

    public SettingsStore()
    {
        Current = Load();
    }

    public void Save(AppSettings settings)
    {
        lock (_lock)
        {
            Normalize(settings);
            Current = settings;
            File.WriteAllText(AppPaths.SettingsFile, JsonSerializer.Serialize(settings, Json));
        }
    }

    /// <summary>Přidá text na začátek historie (bez duplicit) a uloží.</summary>
    public void Remember(string text)
    {
        lock (_lock)
        {
            var s = Current;
            s.History.RemoveAll(h => string.Equals(h, text, StringComparison.Ordinal));
            s.History.Insert(0, text);
            if (s.History.Count > MaxHistory) s.History.RemoveRange(MaxHistory, s.History.Count - MaxHistory);
            File.WriteAllText(AppPaths.SettingsFile, JsonSerializer.Serialize(s, Json));
        }
    }

    private static AppSettings Load()
    {
        try
        {
            if (File.Exists(AppPaths.SettingsFile))
            {
                var s = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(AppPaths.SettingsFile), Json);
                if (s != null)
                {
                    Normalize(s);
                    return s;
                }
            }
        }
        catch { /* poškozený soubor – výchozí nastavení */ }
        return new AppSettings();
    }

    private static void Normalize(AppSettings s)
    {
        s.Voice ??= new();
        s.VoiceCs ??= new() { Engine = "windows" };
        s.Categories ??= new();
        s.History ??= new();
        s.PronunciationCs ??= "";
        s.PronunciationEn ??= "";
        s.Volume = Math.Clamp(s.Volume, 0, 2);
        s.Pitch = Math.Clamp(s.Pitch, 0.8, 1.25);
        foreach (var v in new[] { s.Voice, s.VoiceCs })
        {
            v.PiperSpeed = Math.Clamp(v.PiperSpeed, 0.5, 2.0);
            v.PiperNoiseScale = Math.Clamp(v.PiperNoiseScale, 0.1, 1.5);
            v.PiperNoiseW = Math.Clamp(v.PiperNoiseW, 0.1, 1.5);
            v.PiperSpeaker = Math.Max(0, v.PiperSpeaker);
            v.WindowsRate = Math.Clamp(v.WindowsRate, -10, 10);
            if (v.Engine is not ("local" or "windows")) v.Engine = "local";
        }
        if (s.Language is not ("en" or "cs")) s.Language = "en";
        if (s.Effect is not ("none" or "robot" or "radio")) s.Effect = "none";
        if (s.FontSize is not ("m" or "l" or "xl")) s.FontSize = "l";
        if (s.Theme is not ("dark" or "light")) s.Theme = "dark";
        if (s.Accent is not ("blue" or "teal" or "amber" or "coral" or "purple")) s.Accent = "blue";

        foreach (var c in s.Categories)
        {
            if (string.IsNullOrWhiteSpace(c.Id)) c.Id = Guid.NewGuid().ToString("N");
            c.Name = (c.Name ?? "").Trim();
            c.NameEn = (c.NameEn ?? "").Trim();
            c.Phrases = (c.Phrases ?? new())
                .Where(p => p != null && !(string.IsNullOrWhiteSpace(p.Label) && string.IsNullOrWhiteSpace(p.Text)))
                .ToList();
            foreach (var p in c.Phrases)
            {
                p.Label = (p.Label ?? "").Trim();
                p.Text = (p.Text ?? "").Trim();
            }
        }
        s.History = s.History.Where(h => !string.IsNullOrWhiteSpace(h)).Take(MaxHistory).ToList();
    }
}
