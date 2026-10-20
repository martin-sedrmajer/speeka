namespace Vox.Core;

/// <summary>
/// Výchozí fráze pro první spuštění, každá česky i anglicky.
/// Přepínač jazyka v aplikaci určuje, co je na tlačítkách a čím se mluví.
/// </summary>
public static class Phrases
{
    public static List<PhraseCategory> Defaults() => new()
    {
        Category("Základní", "Basics",
            P("Ano.", "Yes."),
            P("Ne.", "No."),
            P("Děkuji.", "Thank you."),
            P("Prosím.", "Please."),
            P("Ahoj.", "Hello!"),
            P("Na shledanou.", "Goodbye."),
            P("Nevím.", "I don't know."),
            P("Moment, píšu.", "Just a moment, I'm typing.")),

        Category("Potřeby", "Needs",
            P("Mám žízeň.", "I'm thirsty."),
            P("Mám hlad.", "I'm hungry."),
            P("Potřebuji na toaletu.", "I need to use the toilet."),
            P("Je mi zima.", "I'm cold."),
            P("Je mi horko.", "I'm too warm."),
            P("Jsem unavený, chci si odpočinout.", "I'm tired. I'd like to rest."),
            P("Můžete prosím otevřít okno?", "Could you open the window, please?"),
            P("Chci si lehnout.", "I'd like to lie down.")),

        Category("Zdraví", "Health",
            P("Pomoc!", "Please help me!", urgent: true),
            P("Zavolejte prosím sestru.", "Please call the nurse.", urgent: true),
            P("Bolí mě to.", "It hurts."),
            P("Bolí mě tady.", "It hurts here."),
            P("Potřebuji své léky.", "I need my medicine."),
            P("Je mi špatně.", "I feel sick."),
            P("Špatně se mi dýchá.", "I'm having trouble breathing.", urgent: true),
            P("Už je to lepší.", "I feel better now.")),

        Category("Rozhovor", "Conversation",
            P("Jak se máš?", "How are you?"),
            P("Mám se dobře.", "I'm doing well, thanks."),
            P("Můžeš to prosím zopakovat?", "Could you say that again, please?"),
            P("Mluv prosím pomaleji.", "Please speak a little slower."),
            P("Dej mi chvíli, napíšu odpověď.", "Give me a moment to type my answer."),
            P("To je vtipné.", "That's funny!"),
            P("Hehehe", "Hahaha"),
            P("Souhlasím.", "I agree."),
            P("O tom bych raději nemluvil.", "I'd rather not talk about that."),
            P("Mám tě rád.", "I love you.")),

        Category("Čas", "Time",
            P("Teď.", "Now."),
            P("Za minutku.", "In a minute."),
            P("Za hodinu.", "In an hour."),
            P("Dnes.", "Today."),
            P("Zítra.", "Tomorrow."),
            P("Pozítří.", "The day after tomorrow."),
            P("Za týden.", "In a week."),
            P("Včera.", "Yesterday."),
            P("Předevčírem.", "The day before yesterday."),
            P("Ráno.", "In the morning."),
            P("Večer.", "In the evening."),
            P("Dobré ráno.", "Good morning."),
            P("Dobrou noc.", "Good night."),
            P("Kolik je hodin?", "What time is it?"))
    };

    private static PhraseCategory Category(string name, string nameEn, params Phrase[] phrases) =>
        new() { Name = name, NameEn = nameEn, Phrases = phrases.ToList() };

    private static Phrase P(string czech, string english, bool urgent = false) =>
        new() { Label = czech, Text = english, Urgent = urgent };
}
