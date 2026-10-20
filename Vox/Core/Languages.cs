using System.Globalization;

namespace Vox.Core;

/// <summary>
/// Jazyky řeči a frází. Čeština a angličtina mají vlastní hlas, slovník výslovnosti i texty
/// ovládání; ostatní jazyky mluví vícejazyčným hlasem Supertonic a mají přeložené fráze.
/// </summary>
public static class Languages
{
    /// <param name="Code">kód jazyka (ISO 639-1), stejný, jaký chce Supertonic</param>
    /// <param name="Short">zkratka do výběru (CZ, EN, DE…)</param>
    /// <param name="Native">název jazyka v tom jazyce</param>
    /// <param name="Cs">příslovce pro české texty („česky“, „německy“)</param>
    /// <param name="En">anglický název</param>
    /// <param name="Laugh">jak se vysloví smajlík :D</param>
    /// <param name="Ready">krátké slovo pro rozehřátí hlasu</param>
    /// <param name="Test">věta pro vyzkoušení hlasu v Nastavení</param>
    public sealed record Info(string Code, string Short, string Native, string Cs, string En, string Laugh, string Ready, string Test);

    public static readonly IReadOnlyList<Info> All = new[]
    {
        new Info("cs", "CZ", "Čeština", "česky", "Czech", "hehehe", "Hotovo.", "Dobrý den, takto zní můj hlas."),
        new Info("en", "EN", "English", "anglicky", "English", "hahaha", "Ready.", "Hello, this is how I sound."),
        new Info("sk", "SK", "Slovenčina", "slovensky", "Slovak", "hehehe", "Hotovo.", "Dobrý deň, takto znie môj hlas."),
        new Info("pl", "PL", "Polski", "polsky", "Polish", "hahaha", "Gotowe.", "Dzień dobry, tak brzmi mój głos."),
        new Info("de", "DE", "Deutsch", "německy", "German", "hahaha", "Fertig.", "Guten Tag, so klingt meine Stimme."),
        new Info("es", "ES", "Español", "španělsky", "Spanish", "jajaja", "Listo.", "Hola, así suena mi voz."),
        new Info("fr", "FR", "Français", "francouzsky", "French", "hahaha", "Prêt.", "Bonjour, voici ma voix."),
        new Info("it", "IT", "Italiano", "italsky", "Italian", "ahahah", "Pronto.", "Buongiorno, ecco la mia voce."),
        new Info("uk", "UA", "Українська", "ukrajinsky", "Ukrainian", "хахаха", "Готово.", "Добрий день, ось так звучить мій голос."),
        new Info("ru", "RU", "Русский", "rusky", "Russian", "хахаха", "Готово.", "Здравствуйте, вот так звучит мой голос."),
    };

    public static bool IsSupported(string? code) => All.Any(l => l.Code == code);

    /// <summary>Údaje o jazyce; neznámý kód dostane angličtinu.</summary>
    public static Info Get(string? code) => All.FirstOrDefault(l => l.Code == code) ?? All[1];

    /// <summary>Jazyk řeči podle jazyka Windows nebo telefonu (při prvním spuštění).</summary>
    public static string Detect()
    {
        var code = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        return IsSupported(code) ? code : "en";
    }

    /// <summary>Přesune jazyk na začátek seznamu naposledy použitých.</summary>
    public static List<string> Touch(List<string>? recent, string code)
    {
        var list = (recent ?? new()).Where(c => c != code && IsSupported(c)).ToList();
        list.Insert(0, code);
        return list.Take(All.Count).ToList();
    }
}
