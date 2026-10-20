using System.Text.RegularExpressions;

namespace Vox.Core;

/// <summary>
/// Smajlíky v textu: smích (:D, xD, 😂) se vysloví jako smích, ostatní smajlíky a emoji se tiše vynechají,
/// aby je hlas nečetl po znacích („dvojtečka dé“). Vlastní výslovnost ze slovníku má přednost,
/// protože slovník se použije dřív.
/// </summary>
public static class Emoticons
{
    // Smích: :D :-D =D :'D xD XD xDD a smějící se emoji (😁 😂 😄 😆 😹 🤣), i víc za sebou (:D:D).
    // :D smí být nalepené na slovo (ahoj:D), xD ne (Xbox).
    private static readonly Regex Laugh = new(
        @"(?:(?<![\p{L}\p{N}])[xX][dD]+|(?<!\p{N})[:;=]'?-?[dD]+|\uD83D[\uDE01\uDE02\uDE04\uDE06\uDE39]|\uD83E\uDD23)+(?![\p{L}\p{N}])",
        RegexOptions.CultureInvariant);

    // Ostatní textové smajlíky: :) :-) ;) :( :'( :P :O :/ :* :3 (: <3 ^^ ^_^ -_- o_O …
    // Dvojtečka hned za číslicí nebo písmenem se nebere (10:30, http://).
    private static readonly Regex Other = new(
        @"(?<![\p{L}\p{N}])(?:[:;=]'?-?[()\[\]PpOo3*|/\\]+|\(+[:;=]|</?3+|\^_?\^|-_-|[oO]_[oO])(?![\p{L}\p{N}])",
        RegexOptions.CultureInvariant);

    // Emoji: znaky mimo základní rovinu Unicode (zapsané jako dvojice náhradních znaků),
    // symboly U+2600–U+27BF (☺ ❤ ✌ …), variační selektor a spojovač emoji.
    private static readonly Regex Emoji = new(
        @"[\uD800-\uDBFF][\uDC00-\uDFFF]|[\u2600-\u27BF\uFE0F\u200D]",
        RegexOptions.CultureInvariant);

    private static readonly Regex Spaces = new(@"[ \t]{2,}", RegexOptions.CultureInvariant);
    private static readonly Regex SpaceBeforePunctuation = new(@"[ \t]+(?=[.,!?…])", RegexOptions.CultureInvariant);

    /// <summary>Text upravený pro hlas. Může vyjít prázdný, když obsahoval jen smajlíky.</summary>
    public static string Speakable(string text, string lang)
    {
        var laugh = Languages.Get(lang).Laugh;   // hehehe, hahaha, jajaja…
        text = Laugh.Replace(text, " " + laugh + " ");
        text = Other.Replace(text, "");
        text = Emoji.Replace(text, "");
        text = Spaces.Replace(text, " ");
        text = SpaceBeforePunctuation.Replace(text, "");
        return text.Trim();
    }
}
