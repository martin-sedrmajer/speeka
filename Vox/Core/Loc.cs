using System.Globalization;

namespace Vox.Core;

/// <summary>
/// Jazyk rozhraní (cs | en) pro texty, které vyrábí C#: hlášky, stav hlasu, názvy mluvčích.
/// Nezávisí na jazyku řeči (přepínač CZ / EN v aplikaci).
/// </summary>
public static class Loc
{
    public static string Ui { get; set; } = Detect();

    public static bool En => Ui == "en";

    /// <summary>Text v jazyce rozhraní.</summary>
    public static string T(string cs, string en) => En ? en : cs;

    /// <summary>Výchozí jazyk podle Windows: čeština (i pro slovenštinu), jinak angličtina.</summary>
    public static string Detect() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName is "cs" or "sk" ? "cs" : "en";

    public static string Normalize(string? value) => value == "cs" ? "cs" : value == "en" ? "en" : "";
}
