using Vox.Core;

namespace Vox.Voice;

/// <summary>
/// Jména mluvčích modelů Kokoro (pořadí = číslo mluvčího). Soubor voices.bin jména neobsahuje,
/// proto se verze pozná podle počtu hlasů v něm.
/// </summary>
public static class KokoroVoices
{
    // Jeden hlas ve voices.bin = 510 × 256 čísel float.
    private const long BytesPerVoice = 510L * 256 * 4;

    // kokoro-en-v0_19
    private static readonly string[] V019 =
    {
        "af", "af_bella", "af_nicole", "af_sarah", "af_sky", "am_adam", "am_michael", "bf_emma",
        "bf_isabella", "bm_george", "bm_lewis"
    };

    // kokoro-multi-lang-v1_0
    private static readonly string[] V10 =
    {
        "af_alloy", "af_aoede", "af_bella", "af_heart", "af_jessica", "af_kore", "af_nicole", "af_nova",
        "af_river", "af_sarah", "af_sky", "am_adam", "am_echo", "am_eric", "am_fenrir", "am_liam",
        "am_michael", "am_onyx", "am_puck", "am_santa", "bf_alice", "bf_emma", "bf_isabella", "bf_lily",
        "bm_daniel", "bm_fable", "bm_george", "bm_lewis", "ef_dora", "em_alex", "ff_siwis", "hf_alpha",
        "hf_beta", "hm_omega", "hm_psi", "if_sara", "im_nicola", "jf_alpha", "jf_gongitsune", "jf_nezumi",
        "jf_tebukuro", "jm_kumo", "pf_dora", "pm_alex", "pm_santa", "zf_xiaobei", "zf_xiaoni", "zf_xiaoxiao",
        "zf_xiaoyi", "zm_yunjian", "zm_yunxi", "zm_yunxia", "zm_yunyang"
    };

    /// <summary>Počet mluvčích podle velikosti voices.bin (0 = nejde zjistit).</summary>
    public static int Count(string dir)
    {
        try
        {
            var file = new FileInfo(Path.Combine(dir, "voices.bin"));
            return file.Exists ? (int)Math.Round(file.Length / (double)BytesPerVoice) : 0;
        }
        catch
        {
            return 0;
        }
    }

    // Novější balíčky v1.0 mají navíc španělský hlas em_santa jako mluvčího č. 53 (ostatní čísla beze změny).
    private static readonly string[] V10Santa = V10.Append("em_santa").ToArray();

    /// <summary>Jména mluvčích, pokud verzi modelu známe; jinak null (zbude jen číslo).</summary>
    public static string[]? Names(string dir) => Count(dir) switch
    {
        11 => V019,
        53 => V10,
        54 => V10Santa,
        _ => null
    };

    public static string? Name(string dir, int id)
    {
        var names = Names(dir);
        return names != null && id >= 0 && id < names.Length ? names[id] : null;
    }

    /// <summary>Popisek pro výběr: "Heart (americká žena)" / "Heart (American woman)". Neanglické hlasy vrací null.</summary>
    public static string? Label(string name)
    {
        if (name.Length < 3 || name[2] != '_')
            return name == "af" ? Loc.T("Default (americká žena)", "Default (American woman)") : null;
        var female = name[1] == 'f';
        var who = name[0] switch
        {
            'a' => female ? Loc.T("americká žena", "American woman") : Loc.T("americký muž", "American man"),
            'b' => female ? Loc.T("britská žena", "British woman") : Loc.T("britský muž", "British man"),
            _ => null
        };
        if (who == null) return null;
        var title = char.ToUpperInvariant(name[3]) + name.Substring(4);
        return $"{title} ({who})";
    }
}
