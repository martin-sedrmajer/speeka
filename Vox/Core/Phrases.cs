namespace Vox.Core;

/// <summary>
/// Výchozí fráze pro první spuštění ve všech jazycích řeči. Tady prvních deset jazyků (pořadí jako
/// v <see cref="Languages.All"/>: cs, en, sk, pl, de, es, fr, it, uk, ru), dalších 21 je v PhrasesMore.cs.
/// Výběr jazyka v aplikaci určuje, co je na tlačítkách a čím se mluví.
/// </summary>
public static partial class Phrases
{
    public static List<PhraseCategory> Defaults() => WithMore(new()
    {
        Category(new[] { "Základní", "Basics", "Základné", "Podstawowe", "Grundlagen", "Básico", "Essentiel", "Base", "Основне", "Основное" },
            P("Ano.", "Yes.", "Áno.", "Tak.", "Ja.", "Sí.", "Oui.", "Sì.", "Так.", "Да."),
            P("Ne.", "No.", "Nie.", "Nie.", "Nein.", "No.", "Non.", "No.", "Ні.", "Нет."),
            P("Děkuji.", "Thank you.", "Ďakujem.", "Dziękuję.", "Danke.", "Gracias.", "Merci.", "Grazie.", "Дякую.", "Спасибо."),
            P("Prosím.", "Please.", "Prosím.", "Proszę.", "Bitte.", "Por favor.", "S'il vous plaît.", "Per favore.", "Будь ласка.", "Пожалуйста."),
            P("Ahoj.", "Hello!", "Ahoj.", "Cześć.", "Hallo.", "Hola.", "Bonjour.", "Ciao.", "Привіт.", "Привет."),
            P("Na shledanou.", "Goodbye.", "Dovidenia.", "Do widzenia.", "Auf Wiedersehen.", "Adiós.", "Au revoir.", "Arrivederci.", "До побачення.", "До свидания."),
            P("Nevím.", "I don't know.", "Neviem.", "Nie wiem.", "Ich weiß nicht.", "No lo sé.", "Je ne sais pas.", "Non lo so.", "Не знаю.", "Не знаю."),
            P("Moment, píšu.", "Just a moment, I'm typing.", "Moment, píšem.", "Chwileczkę, piszę.", "Moment, ich schreibe.", "Un momento, estoy escribiendo.", "Un instant, j'écris.", "Un momento, sto scrivendo.", "Хвилинку, я пишу.", "Минутку, я пишу.")),

        Category(new[] { "Potřeby", "Needs", "Potreby", "Potrzeby", "Bedürfnisse", "Necesidades", "Besoins", "Bisogni", "Потреби", "Потребности" },
            P("Mám žízeň.", "I'm thirsty.", "Som smädný.", "Chce mi się pić.", "Ich habe Durst.", "Tengo sed.", "J'ai soif.", "Ho sete.", "Я хочу пити.", "Я хочу пить."),
            P("Mám hlad.", "I'm hungry.", "Som hladný.", "Jestem głodny.", "Ich habe Hunger.", "Tengo hambre.", "J'ai faim.", "Ho fame.", "Я голодний.", "Я голоден."),
            P("Potřebuji na toaletu.", "I need to use the toilet.", "Potrebujem na toaletu.", "Muszę iść do toalety.", "Ich muss auf die Toilette.", "Necesito ir al baño.", "J'ai besoin d'aller aux toilettes.", "Devo andare in bagno.", "Мені потрібно в туалет.", "Мне нужно в туалет."),
            P("Je mi zima.", "I'm cold.", "Je mi zima.", "Jest mi zimno.", "Mir ist kalt.", "Tengo frío.", "J'ai froid.", "Ho freddo.", "Мені холодно.", "Мне холодно."),
            P("Je mi horko.", "I'm too warm.", "Je mi horúco.", "Jest mi za gorąco.", "Mir ist zu warm.", "Tengo calor.", "J'ai trop chaud.", "Ho caldo.", "Мені жарко.", "Мне жарко."),
            P("Jsem unavený, chci si odpočinout.", "I'm tired. I'd like to rest.", "Som unavený, chcem si oddýchnuť.", "Jestem zmęczony, chcę odpocząć.", "Ich bin müde und möchte mich ausruhen.", "Estoy cansado, quiero descansar.", "Je suis fatigué, je voudrais me reposer.", "Sono stanco, vorrei riposare.", "Я втомився, хочу відпочити.", "Я устал, хочу отдохнуть."),
            P("Můžete prosím otevřít okno?", "Could you open the window, please?", "Môžete prosím otvoriť okno?", "Czy można otworzyć okno?", "Können Sie bitte das Fenster öffnen?", "¿Puede abrir la ventana, por favor?", "Pouvez-vous ouvrir la fenêtre, s'il vous plaît ?", "Può aprire la finestra, per favore?", "Відчиніть, будь ласка, вікно.", "Откройте, пожалуйста, окно."),
            P("Chci si lehnout.", "I'd like to lie down.", "Chcem si ľahnúť.", "Chcę się położyć.", "Ich möchte mich hinlegen.", "Quiero acostarme.", "Je voudrais m'allonger.", "Vorrei sdraiarmi.", "Я хочу лягти.", "Я хочу лечь.")),

        Category(new[] { "Zdraví", "Health", "Zdravie", "Zdrowie", "Gesundheit", "Salud", "Santé", "Salute", "Здоров'я", "Здоровье" },
            U("Pomoc!", "Please help me!", "Pomoc!", "Pomocy!", "Hilfe!", "¡Ayuda!", "À l'aide !", "Aiuto!", "Допоможіть!", "Помогите!"),
            U("Zavolejte prosím sestru.", "Please call the nurse.", "Zavolajte prosím sestru.", "Proszę zawołać pielęgniarkę.", "Bitte rufen Sie die Krankenschwester.", "Llame a la enfermera, por favor.", "Appelez l'infirmière, s'il vous plaît.", "Chiami l'infermiera, per favore.", "Покличте, будь ласка, медсестру.", "Позовите, пожалуйста, медсестру."),
            P("Bolí mě to.", "It hurts.", "Bolí ma to.", "To boli.", "Das tut weh.", "Me duele.", "J'ai mal.", "Mi fa male.", "Мені боляче.", "Мне больно."),
            P("Bolí mě tady.", "It hurts here.", "Bolí ma tu.", "Boli mnie tutaj.", "Hier tut es weh.", "Me duele aquí.", "J'ai mal ici.", "Mi fa male qui.", "У мене болить тут.", "У меня болит здесь."),
            P("Potřebuji své léky.", "I need my medicine.", "Potrebujem svoje lieky.", "Potrzebuję swoich leków.", "Ich brauche meine Medikamente.", "Necesito mis medicinas.", "J'ai besoin de mes médicaments.", "Ho bisogno delle mie medicine.", "Мені потрібні мої ліки.", "Мне нужны мои лекарства."),
            P("Je mi špatně.", "I feel sick.", "Je mi zle.", "Źle się czuję.", "Mir ist schlecht.", "Me siento mal.", "Je me sens mal.", "Mi sento male.", "Мені погано.", "Мне плохо."),
            U("Špatně se mi dýchá.", "I'm having trouble breathing.", "Ťažko sa mi dýcha.", "Trudno mi oddychać.", "Ich bekomme schlecht Luft.", "Me cuesta respirar.", "J'ai du mal à respirer.", "Faccio fatica a respirare.", "Мені важко дихати.", "Мне трудно дышать."),
            P("Už je to lepší.", "I feel better now.", "Už je to lepšie.", "Już jest lepiej.", "Es geht mir schon besser.", "Ya me siento mejor.", "Je me sens mieux.", "Mi sento meglio.", "Мені вже краще.", "Мне уже лучше.")),

        Category(new[] { "Rozhovor", "Conversation", "Rozhovor", "Rozmowa", "Gespräch", "Conversación", "Conversation", "Conversazione", "Розмова", "Разговор" },
            P("Jak se máš?", "How are you?", "Ako sa máš?", "Jak się masz?", "Wie geht es dir?", "¿Cómo estás?", "Comment ça va ?", "Come stai?", "Як справи?", "Как дела?"),
            P("Mám se dobře.", "I'm doing well, thanks.", "Mám sa dobre.", "Dobrze, dziękuję.", "Mir geht es gut, danke.", "Estoy bien, gracias.", "Ça va bien, merci.", "Sto bene, grazie.", "У мене все добре, дякую.", "У меня всё хорошо, спасибо."),
            P("Můžeš to prosím zopakovat?", "Could you say that again, please?", "Môžeš to prosím zopakovať?", "Możesz to powtórzyć?", "Kannst du das bitte wiederholen?", "¿Puedes repetirlo, por favor?", "Tu peux répéter, s'il te plaît ?", "Puoi ripetere, per favore?", "Можеш, будь ласка, повторити?", "Можешь, пожалуйста, повторить?"),
            P("Mluv prosím pomaleji.", "Please speak a little slower.", "Hovor prosím pomalšie.", "Mów proszę wolniej.", "Sprich bitte langsamer.", "Habla más despacio, por favor.", "Parle plus lentement, s'il te plaît.", "Parla più lentamente, per favore.", "Говори, будь ласка, повільніше.", "Говори, пожалуйста, медленнее."),
            P("Dej mi chvíli, napíšu odpověď.", "Give me a moment to type my answer.", "Daj mi chvíľu, napíšem odpoveď.", "Daj mi chwilę, napiszę odpowiedź.", "Gib mir einen Moment, ich schreibe die Antwort.", "Dame un momento, escribo la respuesta.", "Donne-moi un instant, j'écris ma réponse.", "Dammi un momento, scrivo la risposta.", "Дай мені хвилинку, я напишу відповідь.", "Дай мне минутку, я напишу ответ."),
            P("To je vtipné.", "That's funny!", "To je vtipné.", "To jest zabawne.", "Das ist lustig.", "¡Qué gracioso!", "C'est drôle !", "Che divertente!", "Це смішно.", "Это смешно."),
            P("Hehehe", "Hahaha", "Hehehe", "Hahaha", "Hahaha", "Jajaja", "Hahaha", "Ahahah", "Хахаха", "Хахаха"),
            P("Souhlasím.", "I agree.", "Súhlasím.", "Zgadzam się.", "Einverstanden.", "Estoy de acuerdo.", "Je suis d'accord.", "Sono d'accordo.", "Я згоден.", "Я согласен."),
            P("O tom bych raději nemluvil.", "I'd rather not talk about that.", "O tom by som radšej nehovoril.", "Wolałbym o tym nie rozmawiać.", "Darüber möchte ich lieber nicht sprechen.", "Prefiero no hablar de eso.", "Je préfère ne pas en parler.", "Preferirei non parlarne.", "Я б краще не говорив про це.", "Я бы предпочёл не говорить об этом."),
            P("Mám tě rád.", "I love you.", "Mám ťa rád.", "Kocham cię.", "Ich hab dich lieb.", "Te quiero.", "Je t'aime.", "Ti voglio bene.", "Я тебе люблю.", "Я тебя люблю.")),

        Category(new[] { "Čas", "Time", "Čas", "Czas", "Zeit", "Tiempo", "Temps", "Tempo", "Час", "Время" },
            P("Teď.", "Now.", "Teraz.", "Teraz.", "Jetzt.", "Ahora.", "Maintenant.", "Adesso.", "Зараз.", "Сейчас."),
            P("Za minutku.", "In a minute.", "O minútku.", "Za minutkę.", "Gleich.", "En un minuto.", "Dans une minute.", "Tra un minuto.", "За хвилинку.", "Через минутку."),
            P("Za hodinu.", "In an hour.", "O hodinu.", "Za godzinę.", "In einer Stunde.", "En una hora.", "Dans une heure.", "Tra un'ora.", "За годину.", "Через час."),
            P("Dnes.", "Today.", "Dnes.", "Dzisiaj.", "Heute.", "Hoy.", "Aujourd'hui.", "Oggi.", "Сьогодні.", "Сегодня."),
            P("Zítra.", "Tomorrow.", "Zajtra.", "Jutro.", "Morgen.", "Mañana.", "Demain.", "Domani.", "Завтра.", "Завтра."),
            P("Pozítří.", "The day after tomorrow.", "Pozajtra.", "Pojutrze.", "Übermorgen.", "Pasado mañana.", "Après-demain.", "Dopodomani.", "Післязавтра.", "Послезавтра."),
            P("Za týden.", "In a week.", "O týždeň.", "Za tydzień.", "In einer Woche.", "En una semana.", "Dans une semaine.", "Tra una settimana.", "Через тиждень.", "Через неделю."),
            P("Včera.", "Yesterday.", "Včera.", "Wczoraj.", "Gestern.", "Ayer.", "Hier.", "Ieri.", "Учора.", "Вчера."),
            P("Předevčírem.", "The day before yesterday.", "Predvčerom.", "Przedwczoraj.", "Vorgestern.", "Anteayer.", "Avant-hier.", "L'altro ieri.", "Позавчора.", "Позавчера."),
            P("Ráno.", "In the morning.", "Ráno.", "Rano.", "Am Morgen.", "Por la mañana.", "Le matin.", "Di mattina.", "Вранці.", "Утром."),
            P("Večer.", "In the evening.", "Večer.", "Wieczorem.", "Am Abend.", "Por la noche.", "Le soir.", "Di sera.", "Увечері.", "Вечером."),
            P("Dobré ráno.", "Good morning.", "Dobré ráno.", "Dzień dobry.", "Guten Morgen.", "Buenos días.", "Bonjour.", "Buongiorno.", "Доброго ранку.", "Доброе утро."),
            P("Dobrou noc.", "Good night.", "Dobrú noc.", "Dobranoc.", "Gute Nacht.", "Buenas noches.", "Bonne nuit.", "Buonanotte.", "На добраніч.", "Спокойной ночи."),
            P("Kolik je hodin?", "What time is it?", "Koľko je hodín?", "Która godzina?", "Wie spät ist es?", "¿Qué hora es?", "Quelle heure est-il ?", "Che ore sono?", "Котра година?", "Который час?"))
    });

    /// <summary>
    /// Doplní překlady do dalších jazyků k výchozím frázím a skupinám, které uživatel má z dřívějška
    /// (poznají se podle stejného českého a anglického znění). Vlastní fráze a už vyplněné překlady
    /// zůstanou, jak jsou.
    /// </summary>
    public static void FillMissingTranslations(List<PhraseCategory> categories)
    {
        var defaults = Defaults();
        var phraseLookup = new Dictionary<string, Phrase>();
        var categoryLookup = new Dictionary<string, PhraseCategory>();
        foreach (var c in defaults)
        {
            categoryLookup.TryAdd(Key(c.Name, c.NameEn), c);
            foreach (var p in c.Phrases) phraseLookup.TryAdd(Key(p.Label, p.Text), p);
        }

        foreach (var c in categories)
        {
            if (categoryLookup.TryGetValue(Key(c.Name, c.NameEn), out var dc))
                foreach (var (code, name) in dc.Names) c.Names.TryAdd(code, name);
            foreach (var p in c.Phrases)
                if (phraseLookup.TryGetValue(Key(p.Label, p.Text), out var dp))
                    foreach (var (code, text) in dp.Translations) p.Translations.TryAdd(code, text);
        }
    }

    private static string Key(string cs, string en) => cs.Trim() + "\u0001" + en.Trim();

    private static PhraseCategory Category(string[] names, params Phrase[] phrases) => new()
    {
        Name = names[0],
        NameEn = names[1],
        Names = Others(names),
        Phrases = phrases.ToList()
    };

    private static Phrase P(params string[] texts) => Make(false, texts);
    private static Phrase U(params string[] texts) => Make(true, texts);

    private static Phrase Make(bool urgent, string[] texts) =>
        new() { Label = texts[0], Text = texts[1], Translations = Others(texts), Urgent = urgent };

    // Čeština a angličtina mají vlastní pole (Label/Name a Text/NameEn), ostatní jazyky jdou do slovníku.
    private static Dictionary<string, string> Others(string[] texts)
    {
        var map = new Dictionary<string, string>();
        for (var i = 2; i < texts.Length && i < Languages.All.Count; i++)
            map[Languages.All[i].Code] = texts[i];
        return map;
    }
}
