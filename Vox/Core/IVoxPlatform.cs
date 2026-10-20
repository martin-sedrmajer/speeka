namespace Vox.Core;

/// <summary>
/// Věci, které umí jen konkrétní platforma (Windows, Android). Spojka (Bridge.cs) je sdílená
/// a o platformě ví jen přes toto rozhraní.
/// </summary>
public interface IVoxPlatform
{
    /// <summary>windows | android – posílá se stránce, ať skryje, co na platformě není.</summary>
    string Name { get; }

    /// <summary>Verze aplikace pro sekci O aplikaci (např. 0.2.0).</summary>
    string AppVersion { get; }

    /// <summary>Výchozí počet kroků výpočtu hlasu (Nastavení „Kvalita hlasu: automaticky“). Telefony méně, ať se nečeká.</summary>
    int DefaultSynthesisSteps { get; }

    /// <summary>Hlasy systému (na Windows SAPI). Prázdný seznam, když platforma žádné nemá.</summary>
    IReadOnlyList<string> SystemVoices();

    /// <summary>První systémový hlas pro jazyk (např. "cs-CZ"), nebo null.</summary>
    string? SystemVoiceFor(string culture);

    /// <summary>Řeč systémovým hlasem jako WAV. Platforma bez systémových hlasů vyhodí výjimku.</summary>
    byte[] SynthesizeSystem(string text, VoiceSettings voice);

    /// <summary>Složka pro vlastní hlasy tak, jak ji uvidí uživatel (prázdné = platforma ji neukazuje).</summary>
    string VoicesFolderDisplay { get; }

    /// <summary>Otevře složku s vlastními hlasy (kde to jde).</summary>
    void OpenVoicesFolder();
}
