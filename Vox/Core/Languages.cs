using System.Globalization;

namespace Vox.Core;

/// <summary>
/// Jazyky řeči a frází. Ovládání je jen česky a anglicky; mluví se všemi 31 jazyky,
/// které umí přibalený hlas Supertonic 3, a všechny mají přeložené připravené fráze.
/// Jazyk přidaný bez překladů frází dostane Phrases: false a bude mít jen pole pro text a historii.
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
    /// <param name="Phrases">jazyk má přeložené připravené fráze</param>
    public sealed record Info(string Code, string Short, string Native, string Cs, string En, string Laugh, string Ready, string Test,
        bool Phrases = true);

    /// <summary>Kolik jazyků se pamatuje jako naposledy použité (nahoře ve výběru).</summary>
    public const int RecentCount = 5;

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

        // fráze v PhrasesMore.cs
        new Info("ar", "AR", "العربية", "arabsky", "Arabic", "ههههه", "جاهز.", "مرحبًا، هكذا يبدو صوتي."),
        new Info("bg", "BG", "Български", "bulharsky", "Bulgarian", "хахаха", "Готово.", "Здравейте, така звучи моят глас."),
        new Info("da", "DK", "Dansk", "dánsky", "Danish", "hahaha", "Klar.", "Hej, sådan lyder min stemme."),
        new Info("el", "GR", "Ελληνικά", "řecky", "Greek", "χαχαχα", "Έτοιμο.", "Γεια σας, έτσι ακούγεται η φωνή μου."),
        new Info("et", "EE", "Eesti", "estonsky", "Estonian", "hahaha", "Valmis.", "Tere, nii kõlab minu hääl."),
        new Info("fi", "FI", "Suomi", "finsky", "Finnish", "hahaha", "Valmis.", "Hei, tältä ääneni kuulostaa."),
        new Info("hi", "HI", "हिन्दी", "hindsky", "Hindi", "हाहाहा", "तैयार।", "नमस्ते, मेरी आवाज़ ऐसी सुनाई देती है।"),
        new Info("hr", "HR", "Hrvatski", "chorvatsky", "Croatian", "hahaha", "Spremno.", "Dobar dan, ovako zvuči moj glas."),
        new Info("hu", "HU", "Magyar", "maďarsky", "Hungarian", "hahaha", "Kész.", "Jó napot, így szól a hangom."),
        new Info("id", "ID", "Bahasa Indonesia", "indonésky", "Indonesian", "hahaha", "Siap.", "Halo, beginilah suara saya."),
        new Info("ja", "JP", "日本語", "japonsky", "Japanese", "ははは", "準備ができました。", "こんにちは、これが私の声です。"),
        new Info("ko", "KR", "한국어", "korejsky", "Korean", "하하하", "준비됐습니다.", "안녕하세요, 제 목소리는 이렇습니다."),
        new Info("lt", "LT", "Lietuvių", "litevsky", "Lithuanian", "chachacha", "Paruošta.", "Laba diena, štai kaip skamba mano balsas."),
        new Info("lv", "LV", "Latviešu", "lotyšsky", "Latvian", "hahaha", "Gatavs.", "Labdien, lūk, kā skan mana balss."),
        new Info("nl", "NL", "Nederlands", "nizozemsky", "Dutch", "hahaha", "Klaar.", "Hallo, zo klinkt mijn stem."),
        new Info("pt", "PT", "Português", "portugalsky", "Portuguese", "rárárá", "Pronto.", "Olá, é assim que soa a minha voz."),
        new Info("ro", "RO", "Română", "rumunsky", "Romanian", "hahaha", "Gata.", "Bună ziua, așa sună vocea mea."),
        new Info("sl", "SI", "Slovenščina", "slovinsky", "Slovenian", "hahaha", "Pripravljeno.", "Pozdravljeni, tako zveni moj glas."),
        new Info("sv", "SE", "Svenska", "švédsky", "Swedish", "hahaha", "Klar.", "Hej, så här låter min röst."),
        new Info("tr", "TR", "Türkçe", "turecky", "Turkish", "hahaha", "Hazır.", "Merhaba, sesim böyle duyuluyor."),
        new Info("vi", "VN", "Tiếng Việt", "vietnamsky", "Vietnamese", "hahaha", "Sẵn sàng.", "Xin chào, giọng của tôi nghe như thế này."),
    };

    public static bool IsSupported(string? code) => All.Any(l => l.Code == code);

    /// <summary>Údaje o jazyce; neznámý kód dostane angličtinu.</summary>
    public static Info Get(string? code) => All.FirstOrDefault(l => l.Code == code) ?? All[1];

    /// <summary>
    /// Jazyk řeči podle jazyka Windows nebo telefonu (při prvním spuštění). Jen jazyk s frázemi,
    /// ať první dojem není prázdná obrazovka.
    /// </summary>
    public static string Detect()
    {
        var code = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        return IsSupported(code) && Get(code).Phrases ? code : "en";
    }

    /// <summary>Přesune jazyk na začátek seznamu naposledy použitých.</summary>
    public static List<string> Touch(List<string>? recent, string code)
    {
        var list = (recent ?? new()).Where(c => c != code && IsSupported(c)).ToList();
        list.Insert(0, code);
        return list.Take(RecentCount).ToList();
    }
}
