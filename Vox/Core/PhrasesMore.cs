namespace Vox.Core;

/// <summary>
/// Výchozí fráze v dalších 21 jazycích. Tabulka každého jazyka jde ve stejném pořadí jako
/// <see cref="MoreOrder"/> (anglická znění z Phrases.cs) a k frázím se napojí podle angličtiny.
/// Mužský rod jako v češtině. Překlady nekontroloval rodilý mluvčí.
/// </summary>
public static partial class Phrases
{
    // Anglická znění v pořadí tabulek: názvy pěti skupin, pak fráze skupinu po skupině.
    private static readonly string[] MoreOrder =
    {
        // skupiny
        "Basics", "Needs", "Health", "Conversation", "Time",
        // Základní
        "Yes.", "No.", "Thank you.", "Please.", "Hello!", "Goodbye.", "I don't know.", "Just a moment, I'm typing.",
        // Potřeby
        "I'm thirsty.", "I'm hungry.", "I need to use the toilet.", "I'm cold.", "I'm too warm.", "I'm tired. I'd like to rest.", "Could you open the window, please?", "I'd like to lie down.",
        // Zdraví
        "Please help me!", "Please call the nurse.", "It hurts.", "It hurts here.", "I need my medicine.", "I feel sick.", "I'm having trouble breathing.", "I feel better now.",
        // Rozhovor
        "How are you?", "I'm doing well, thanks.", "Could you say that again, please?", "Please speak a little slower.", "Give me a moment to type my answer.", "That's funny!", "Hahaha", "I agree.", "I'd rather not talk about that.", "I love you.",
        // Čas
        "Now.", "In a minute.", "In an hour.", "Today.", "Tomorrow.", "The day after tomorrow.", "In a week.", "Yesterday.", "The day before yesterday.", "In the morning.", "In the evening.", "Good morning.", "Good night.", "What time is it?",
    };

    private static readonly Dictionary<string, string[]> More = new()
    {
        // arabština
        ["ar"] = new[]
        {
            "أساسيات", "احتياجات", "الصحة", "محادثة", "الوقت",
            "نعم.", "لا.", "شكرًا.", "من فضلك.", "مرحبًا!", "مع السلامة.", "لا أعرف.", "لحظة، أنا أكتب.",
            "أنا عطشان.", "أنا جائع.", "أحتاج إلى دخول الحمّام.", "أشعر بالبرد.", "أشعر بالحر.", "أنا متعب، أريد أن أرتاح.", "هل يمكنك فتح النافذة من فضلك؟", "أريد أن أستلقي.",
            "ساعدوني من فضلكم!", "من فضلك، نادِ الممرضة.", "هذا يؤلمني.", "يؤلمني هنا.", "أحتاج إلى أدويتي.", "أشعر بتوعك.", "أجد صعوبة في التنفس.", "أشعر بتحسن الآن.",
            "كيف حالك؟", "أنا بخير، شكرًا.", "هل يمكنك أن تعيد ذلك من فضلك؟", "تكلّم ببطء أكثر من فضلك.", "أعطني لحظة لأكتب إجابتي.", "هذا مضحك!", "ههههه", "أوافق.", "أفضّل ألا أتحدث عن ذلك.", "أحبك.",
            "الآن.", "بعد دقيقة.", "بعد ساعة.", "اليوم.", "غدًا.", "بعد غد.", "بعد أسبوع.", "أمس.", "أول أمس.", "في الصباح.", "في المساء.", "صباح الخير.", "تصبح على خير.", "كم الساعة؟",
        },
        // bulharština
        ["bg"] = new[]
        {
            "Основни", "Нужди", "Здраве", "Разговор", "Време",
            "Да.", "Не.", "Благодаря.", "Моля.", "Здравей!", "Довиждане.", "Не знам.", "Момент, пиша.",
            "Жаден съм.", "Гладен съм.", "Трябва да отида до тоалетната.", "Студено ми е.", "Горещо ми е.", "Уморен съм, искам да си почина.", "Може ли да отворите прозореца, моля?", "Искам да легна.",
            "Помощ!", "Моля, повикайте сестрата.", "Боли ме.", "Боли ме тук.", "Трябват ми лекарствата.", "Зле ми е.", "Трудно ми е да дишам.", "Вече ми е по-добре.",
            "Как си?", "Добре съм, благодаря.", "Можеш ли да повториш, моля?", "Говори по-бавно, моля.", "Дай ми момент да напиша отговора си.", "Това е смешно!", "Хахаха", "Съгласен съм.", "Предпочитам да не говоря за това.", "Обичам те.",
            "Сега.", "След минутка.", "След час.", "Днес.", "Утре.", "Вдругиден.", "След седмица.", "Вчера.", "Онзи ден.", "Сутринта.", "Вечерта.", "Добро утро.", "Лека нощ.", "Колко е часът?",
        },
        // dánština
        ["da"] = new[]
        {
            "Grundlæggende", "Behov", "Helbred", "Samtale", "Tid",
            "Ja.", "Nej.", "Tak.", "Vær så venlig.", "Hej!", "Farvel.", "Det ved jeg ikke.", "Et øjeblik, jeg skriver.",
            "Jeg er tørstig.", "Jeg er sulten.", "Jeg skal på toilettet.", "Jeg fryser.", "Jeg har det for varmt.", "Jeg er træt, jeg vil gerne hvile mig.", "Kan du åbne vinduet, tak?", "Jeg vil gerne lægge mig ned.",
            "Hjælp!", "Tilkald venligst sygeplejersken.", "Det gør ondt.", "Det gør ondt her.", "Jeg har brug for min medicin.", "Jeg har det dårligt.", "Jeg har svært ved at trække vejret.", "Jeg har det bedre nu.",
            "Hvordan har du det?", "Jeg har det godt, tak.", "Kan du sige det igen?", "Tal lidt langsommere, tak.", "Giv mig et øjeblik til at skrive mit svar.", "Det er sjovt!", "Hahaha", "Jeg er enig.", "Det vil jeg helst ikke tale om.", "Jeg elsker dig.",
            "Nu.", "Om et minut.", "Om en time.", "I dag.", "I morgen.", "I overmorgen.", "Om en uge.", "I går.", "I forgårs.", "Om morgenen.", "Om aftenen.", "Godmorgen.", "Godnat.", "Hvad er klokken?",
        },
        // řečtina
        ["el"] = new[]
        {
            "Βασικά", "Ανάγκες", "Υγεία", "Συζήτηση", "Χρόνος",
            "Ναι.", "Όχι.", "Ευχαριστώ.", "Παρακαλώ.", "Γεια σου!", "Αντίο.", "Δεν ξέρω.", "Μια στιγμή, γράφω.",
            "Διψάω.", "Πεινάω.", "Πρέπει να πάω στην τουαλέτα.", "Κρυώνω.", "Ζεσταίνομαι.", "Είμαι κουρασμένος, θέλω να ξεκουραστώ.", "Μπορείτε να ανοίξετε το παράθυρο, παρακαλώ;", "Θέλω να ξαπλώσω.",
            "Βοήθεια!", "Φωνάξτε τη νοσοκόμα, παρακαλώ.", "Πονάει.", "Πονάει εδώ.", "Χρειάζομαι τα φάρμακά μου.", "Δεν νιώθω καλά.", "Δυσκολεύομαι να αναπνεύσω.", "Νιώθω καλύτερα τώρα.",
            "Τι κάνεις;", "Είμαι καλά, ευχαριστώ.", "Μπορείς να το επαναλάβεις, παρακαλώ;", "Μίλα λίγο πιο αργά, παρακαλώ.", "Δώσε μου μια στιγμή να γράψω την απάντηση.", "Αυτό είναι αστείο!", "Χαχαχα", "Συμφωνώ.", "Προτιμώ να μη μιλήσω γι' αυτό.", "Σ' αγαπώ.",
            "Τώρα.", "Σε ένα λεπτό.", "Σε μία ώρα.", "Σήμερα.", "Αύριο.", "Μεθαύριο.", "Σε μία εβδομάδα.", "Χθες.", "Προχθές.", "Το πρωί.", "Το βράδυ.", "Καλημέρα.", "Καληνύχτα.", "Τι ώρα είναι;",
        },
        // estonština
        ["et"] = new[]
        {
            "Põhilised", "Vajadused", "Tervis", "Vestlus", "Aeg",
            "Jah.", "Ei.", "Aitäh.", "Palun.", "Tere!", "Head aega.", "Ma ei tea.", "Hetk, ma kirjutan.",
            "Mul on janu.", "Mul on kõht tühi.", "Mul on vaja tualetti minna.", "Mul on külm.", "Mul on palav.", "Ma olen väsinud, tahaksin puhata.", "Kas te saaksite palun akna avada?", "Tahaksin pikali heita.",
            "Appi!", "Palun kutsuge õde.", "See valutab.", "Siit valutab.", "Mul on vaja oma ravimeid.", "Mul on halb olla.", "Mul on raske hingata.", "Mul on nüüd parem.",
            "Kuidas läheb?", "Mul läheb hästi, aitäh.", "Kas sa saaksid seda palun korrata?", "Räägi palun natuke aeglasemalt.", "Anna mulle hetk, et vastus kirjutada.", "See on naljakas!", "Hahaha", "Olen nõus.", "Ma ei tahaks sellest rääkida.", "Ma armastan sind.",
            "Nüüd.", "Minuti pärast.", "Tunni pärast.", "Täna.", "Homme.", "Ülehomme.", "Nädala pärast.", "Eile.", "Üleeile.", "Hommikul.", "Õhtul.", "Tere hommikust.", "Head ööd.", "Mis kell on?",
        },
        // finština
        ["fi"] = new[]
        {
            "Perusasiat", "Tarpeet", "Terveys", "Keskustelu", "Aika",
            "Kyllä.", "Ei.", "Kiitos.", "Ole kiltti.", "Hei!", "Näkemiin.", "En tiedä.", "Hetkinen, kirjoitan.",
            "Minulla on jano.", "Minulla on nälkä.", "Minun täytyy päästä vessaan.", "Minulla on kylmä.", "Minulla on liian kuuma.", "Olen väsynyt, haluaisin levätä.", "Voisitteko avata ikkunan?", "Haluaisin käydä makuulle.",
            "Apua!", "Kutsukaa hoitaja, kiitos.", "Sattuu.", "Tähän sattuu.", "Tarvitsen lääkkeeni.", "Voin pahoin.", "Minun on vaikea hengittää.", "Voin nyt paremmin.",
            "Mitä kuuluu?", "Hyvää, kiitos.", "Voisitko toistaa?", "Puhu vähän hitaammin, kiitos.", "Anna minulle hetki kirjoittaa vastaukseni.", "Hauskaa!", "Hahaha", "Olen samaa mieltä.", "En haluaisi puhua siitä.", "Rakastan sinua.",
            "Nyt.", "Minuutin päästä.", "Tunnin päästä.", "Tänään.", "Huomenna.", "Ylihuomenna.", "Viikon päästä.", "Eilen.", "Toissapäivänä.", "Aamulla.", "Illalla.", "Hyvää huomenta.", "Hyvää yötä.", "Paljonko kello on?",
        },
        // hindština
        ["hi"] = new[]
        {
            "बुनियादी", "ज़रूरतें", "स्वास्थ्य", "बातचीत", "समय",
            "हाँ।", "नहीं।", "धन्यवाद।", "कृपया।", "नमस्ते!", "अलविदा।", "मुझे नहीं पता।", "एक पल, मैं लिख रहा हूँ।",
            "मुझे प्यास लगी है।", "मुझे भूख लगी है।", "मुझे शौचालय जाना है।", "मुझे ठंड लग रही है।", "मुझे बहुत गर्मी लग रही है।", "मैं थक गया हूँ, मैं आराम करना चाहता हूँ।", "क्या आप कृपया खिड़की खोल सकते हैं?", "मैं लेटना चाहता हूँ।",
            "मदद कीजिए!", "कृपया नर्स को बुलाइए।", "दर्द हो रहा है।", "यहाँ दर्द हो रहा है।", "मुझे मेरी दवाइयाँ चाहिए।", "मेरी तबीयत ठीक नहीं है।", "मुझे साँस लेने में तकलीफ़ हो रही है।", "अब मैं बेहतर महसूस कर रहा हूँ।",
            "आप कैसे हैं?", "मैं ठीक हूँ, धन्यवाद।", "क्या आप कृपया फिर से कह सकते हैं?", "कृपया थोड़ा धीरे बोलिए।", "मुझे जवाब लिखने के लिए एक पल दीजिए।", "यह मज़ेदार है!", "हाहाहा", "मैं सहमत हूँ।", "मैं इस बारे में बात नहीं करना चाहता।", "मैं तुमसे प्यार करता हूँ।",
            "अभी।", "एक मिनट में।", "एक घंटे में।", "आज।", "आने वाला कल।", "परसों।", "एक हफ़्ते में।", "बीता हुआ कल।", "बीता हुआ परसों।", "सुबह।", "शाम को।", "सुप्रभात।", "शुभ रात्रि।", "कितने बजे हैं?",
        },
        // chorvatština
        ["hr"] = new[]
        {
            "Osnovno", "Potrebe", "Zdravlje", "Razgovor", "Vrijeme",
            "Da.", "Ne.", "Hvala.", "Molim.", "Bok!", "Doviđenja.", "Ne znam.", "Trenutak, pišem.",
            "Žedan sam.", "Gladan sam.", "Moram na zahod.", "Hladno mi je.", "Vruće mi je.", "Umoran sam, htio bih se odmoriti.", "Možete li, molim vas, otvoriti prozor?", "Htio bih leći.",
            "Upomoć!", "Molim vas, pozovite sestru.", "Boli me.", "Boli me ovdje.", "Trebam svoje lijekove.", "Loše mi je.", "Teško dišem.", "Sad mi je bolje.",
            "Kako si?", "Dobro sam, hvala.", "Možeš li to ponoviti, molim te?", "Govori malo sporije, molim te.", "Daj mi trenutak da napišem odgovor.", "To je smiješno!", "Hahaha", "Slažem se.", "Radije ne bih o tome.", "Volim te.",
            "Sada.", "Za minutu.", "Za sat vremena.", "Danas.", "Sutra.", "Prekosutra.", "Za tjedan dana.", "Jučer.", "Prekjučer.", "Ujutro.", "Navečer.", "Dobro jutro.", "Laku noć.", "Koliko je sati?",
        },
        // maďarština
        ["hu"] = new[]
        {
            "Alapok", "Szükségletek", "Egészség", "Beszélgetés", "Idő",
            "Igen.", "Nem.", "Köszönöm.", "Kérem.", "Szia!", "Viszontlátásra.", "Nem tudom.", "Egy pillanat, írok.",
            "Szomjas vagyok.", "Éhes vagyok.", "Mosdóba kell mennem.", "Fázom.", "Melegem van.", "Fáradt vagyok, szeretnék pihenni.", "Kinyitná az ablakot, kérem?", "Szeretnék lefeküdni.",
            "Segítség!", "Kérem, hívja a nővért.", "Fáj.", "Itt fáj.", "Szükségem van a gyógyszereimre.", "Rosszul vagyok.", "Nehezen kapok levegőt.", "Már jobban vagyok.",
            "Hogy vagy?", "Jól vagyok, köszönöm.", "Meg tudnád ismételni, kérlek?", "Beszélj kicsit lassabban, kérlek.", "Adj egy pillanatot, megírom a válaszom.", "Ez vicces!", "Hahaha", "Egyetértek.", "Erről inkább nem beszélnék.", "Szeretlek.",
            "Most.", "Egy perc múlva.", "Egy óra múlva.", "Ma.", "Holnap.", "Holnapután.", "Egy hét múlva.", "Tegnap.", "Tegnapelőtt.", "Reggel.", "Este.", "Jó reggelt.", "Jó éjszakát.", "Hány óra van?",
        },
        // indonéština
        ["id"] = new[]
        {
            "Dasar", "Kebutuhan", "Kesehatan", "Percakapan", "Waktu",
            "Ya.", "Tidak.", "Terima kasih.", "Tolong.", "Halo!", "Sampai jumpa.", "Saya tidak tahu.", "Sebentar, saya sedang mengetik.",
            "Saya haus.", "Saya lapar.", "Saya perlu ke toilet.", "Saya kedinginan.", "Saya kepanasan.", "Saya lelah, saya ingin istirahat.", "Bisakah Anda membuka jendela?", "Saya ingin berbaring.",
            "Tolong saya!", "Tolong panggilkan perawat.", "Sakit.", "Sakitnya di sini.", "Saya butuh obat saya.", "Saya merasa tidak enak badan.", "Saya sulit bernapas.", "Saya sudah merasa lebih baik.",
            "Apa kabar?", "Kabar baik, terima kasih.", "Bisa diulangi?", "Tolong bicara sedikit lebih pelan.", "Beri saya waktu sebentar untuk mengetik jawaban.", "Lucu sekali!", "Hahaha", "Saya setuju.", "Saya lebih suka tidak membicarakan itu.", "Aku sayang kamu.",
            "Sekarang.", "Satu menit lagi.", "Satu jam lagi.", "Hari ini.", "Besok.", "Lusa.", "Seminggu lagi.", "Kemarin.", "Kemarin lusa.", "Pagi hari.", "Malam hari.", "Selamat pagi.", "Selamat tidur.", "Jam berapa sekarang?",
        },
        // japonština
        ["ja"] = new[]
        {
            "基本", "必要なこと", "健康", "会話", "時間",
            "はい。", "いいえ。", "ありがとうございます。", "お願いします。", "こんにちは！", "さようなら。", "わかりません。", "少々お待ちください、入力中です。",
            "喉が渇きました。", "お腹が空きました。", "トイレに行きたいです。", "寒いです。", "暑いです。", "疲れたので、休みたいです。", "窓を開けていただけますか？", "横になりたいです。",
            "助けてください！", "看護師さんを呼んでください。", "痛いです。", "ここが痛いです。", "薬が必要です。", "気分が悪いです。", "息が苦しいです。", "もう良くなりました。",
            "元気ですか？", "元気です、ありがとう。", "もう一度言っていただけますか？", "もう少しゆっくり話してください。", "返事を書くので、少し待ってください。", "面白いですね！", "ははは", "賛成です。", "その話はしたくないです。", "大好きです。",
            "今。", "一分後に。", "一時間後に。", "今日。", "明日。", "明後日。", "一週間後に。", "昨日。", "一昨日。", "朝に。", "夜に。", "おはようございます。", "おやすみなさい。", "今何時ですか？",
        },
        // korejština
        ["ko"] = new[]
        {
            "기본", "필요", "건강", "대화", "시간",
            "네.", "아니요.", "감사합니다.", "부탁합니다.", "안녕하세요!", "안녕히 계세요.", "모르겠어요.", "잠깐만요, 쓰고 있어요.",
            "목말라요.", "배고파요.", "화장실에 가야 해요.", "추워요.", "더워요.", "피곤해요, 좀 쉬고 싶어요.", "창문 좀 열어 주시겠어요?", "눕고 싶어요.",
            "도와주세요!", "간호사를 불러 주세요.", "아파요.", "여기가 아파요.", "제 약이 필요해요.", "몸이 안 좋아요.", "숨쉬기가 힘들어요.", "이제 좀 나아졌어요.",
            "잘 지내요?", "잘 지내요, 고마워요.", "다시 한 번 말해 주시겠어요?", "조금만 천천히 말해 주세요.", "대답을 쓸 시간을 조금 주세요.", "재미있네요!", "하하하", "동의해요.", "그 얘기는 하고 싶지 않아요.", "사랑해요.",
            "지금.", "잠시 후에.", "한 시간 후에.", "오늘.", "내일.", "모레.", "일주일 후에.", "어제.", "그저께.", "아침에.", "저녁에.", "좋은 아침이에요.", "안녕히 주무세요.", "지금 몇 시예요?",
        },
        // litevština
        ["lt"] = new[]
        {
            "Pagrindinės", "Poreikiai", "Sveikata", "Pokalbis", "Laikas",
            "Taip.", "Ne.", "Ačiū.", "Prašau.", "Labas!", "Viso gero.", "Nežinau.", "Akimirką, rašau.",
            "Noriu gerti.", "Noriu valgyti.", "Man reikia į tualetą.", "Man šalta.", "Man karšta.", "Esu pavargęs, norėčiau pailsėti.", "Ar galėtumėte atidaryti langą?", "Norėčiau atsigulti.",
            "Padėkite!", "Prašau pakviesti slaugytoją.", "Skauda.", "Skauda čia.", "Man reikia mano vaistų.", "Man bloga.", "Man sunku kvėpuoti.", "Jau geriau.",
            "Kaip sekasi?", "Gerai, ačiū.", "Ar gali pakartoti?", "Kalbėk šiek tiek lėčiau, prašau.", "Duok man akimirką parašyti atsakymą.", "Juokinga!", "Chachacha", "Sutinku.", "Apie tai verčiau nekalbėčiau.", "Myliu tave.",
            "Dabar.", "Po minutės.", "Po valandos.", "Šiandien.", "Rytoj.", "Poryt.", "Po savaitės.", "Vakar.", "Užvakar.", "Ryte.", "Vakare.", "Labas rytas.", "Labos nakties.", "Kiek valandų?",
        },
        // lotyština
        ["lv"] = new[]
        {
            "Pamata", "Vajadzības", "Veselība", "Saruna", "Laiks",
            "Jā.", "Nē.", "Paldies.", "Lūdzu.", "Sveiki!", "Uz redzēšanos.", "Es nezinu.", "Mirkli, es rakstu.",
            "Man slāpst.", "Esmu izsalcis.", "Man jāiet uz tualeti.", "Man ir auksti.", "Man ir karsti.", "Esmu noguris, gribētu atpūsties.", "Vai jūs, lūdzu, varētu atvērt logu?", "Es gribētu apgulties.",
            "Palīgā!", "Lūdzu, pasauciet medmāsu.", "Sāp.", "Sāp šeit.", "Man vajag manas zāles.", "Man ir slikti.", "Man ir grūti elpot.", "Tagad man ir labāk.",
            "Kā tev iet?", "Man iet labi, paldies.", "Vai vari, lūdzu, atkārtot?", "Lūdzu, runā mazliet lēnāk.", "Dod man mirkli uzrakstīt atbildi.", "Tas ir smieklīgi!", "Hahaha", "Es piekrītu.", "Labāk par to nerunāšu.", "Es tevi mīlu.",
            "Tagad.", "Pēc minūtes.", "Pēc stundas.", "Šodien.", "Rīt.", "Parīt.", "Pēc nedēļas.", "Vakar.", "Aizvakar.", "No rīta.", "Vakarā.", "Labrīt.", "Ar labu nakti.", "Cik pulkstenis?",
        },
        // nizozemština
        ["nl"] = new[]
        {
            "Basis", "Behoeften", "Gezondheid", "Gesprek", "Tijd",
            "Ja.", "Nee.", "Dank je wel.", "Alstublieft.", "Hallo!", "Tot ziens.", "Ik weet het niet.", "Een moment, ik ben aan het typen.",
            "Ik heb dorst.", "Ik heb honger.", "Ik moet naar de wc.", "Ik heb het koud.", "Ik heb het te warm.", "Ik ben moe, ik wil graag rusten.", "Kunt u het raam openzetten, alstublieft?", "Ik wil graag gaan liggen.",
            "Help!", "Roep alstublieft de verpleegkundige.", "Het doet pijn.", "Het doet hier pijn.", "Ik heb mijn medicijnen nodig.", "Ik voel me niet goed.", "Ik heb moeite met ademhalen.", "Ik voel me nu beter.",
            "Hoe gaat het?", "Het gaat goed, dank je.", "Kun je dat herhalen, alsjeblieft?", "Praat alsjeblieft wat langzamer.", "Geef me even de tijd om mijn antwoord te typen.", "Dat is grappig!", "Hahaha", "Daar ben ik het mee eens.", "Daar praat ik liever niet over.", "Ik hou van je.",
            "Nu.", "Over een minuutje.", "Over een uur.", "Vandaag.", "Morgen.", "Overmorgen.", "Over een week.", "Gisteren.", "Eergisteren.", "'s Ochtends.", "'s Avonds.", "Goedemorgen.", "Welterusten.", "Hoe laat is het?",
        },
        // portugalština (brazilská)
        ["pt"] = new[]
        {
            "Básico", "Necessidades", "Saúde", "Conversa", "Tempo",
            "Sim.", "Não.", "Obrigado.", "Por favor.", "Olá!", "Até logo.", "Não sei.", "Um momento, vou escrever.",
            "Estou com sede.", "Estou com fome.", "Preciso ir ao banheiro.", "Estou com frio.", "Estou com calor.", "Estou cansado, quero descansar.", "Pode abrir a janela, por favor?", "Gostaria de me deitar.",
            "Socorro!", "Chame a enfermeira, por favor.", "Dói.", "Dói aqui.", "Preciso dos meus medicamentos.", "Não me sinto bem.", "Tenho dificuldade em respirar.", "Já me sinto melhor.",
            "Tudo bem?", "Estou bem, obrigado.", "Pode repetir, por favor?", "Fale mais devagar, por favor.", "Dê-me um momento para escrever a resposta.", "Que engraçado!", "Rárárá", "Concordo.", "Prefiro não falar sobre isso.", "Eu te amo.",
            "Agora.", "Daqui a um minuto.", "Daqui a uma hora.", "Hoje.", "Amanhã.", "Depois de amanhã.", "Daqui a uma semana.", "Ontem.", "Anteontem.", "De manhã.", "À noite.", "Bom dia.", "Boa noite.", "Que horas são?",
        },
        // rumunština
        ["ro"] = new[]
        {
            "De bază", "Nevoi", "Sănătate", "Conversație", "Timp",
            "Da.", "Nu.", "Mulțumesc.", "Vă rog.", "Bună!", "La revedere.", "Nu știu.", "O clipă, scriu.",
            "Mi-e sete.", "Mi-e foame.", "Trebuie să merg la toaletă.", "Mi-e frig.", "Mi-e cald.", "Sunt obosit, aș vrea să mă odihnesc.", "Puteți deschide fereastra, vă rog?", "Aș vrea să mă întind.",
            "Ajutor!", "Vă rog, chemați asistenta.", "Mă doare.", "Mă doare aici.", "Am nevoie de medicamentele mele.", "Mi-e rău.", "Respir greu.", "Mă simt mai bine acum.",
            "Ce mai faci?", "Sunt bine, mulțumesc.", "Poți să repeți, te rog?", "Vorbește puțin mai rar, te rog.", "Dă-mi un moment să scriu răspunsul.", "E amuzant!", "Hahaha", "Sunt de acord.", "Aș prefera să nu vorbesc despre asta.", "Te iubesc.",
            "Acum.", "Peste un minut.", "Peste o oră.", "Azi.", "Mâine.", "Poimâine.", "Peste o săptămână.", "Ieri.", "Alaltăieri.", "Dimineața.", "Seara.", "Bună dimineața.", "Noapte bună.", "Cât e ceasul?",
        },
        // slovinština
        ["sl"] = new[]
        {
            "Osnovno", "Potrebe", "Zdravje", "Pogovor", "Čas",
            "Da.", "Ne.", "Hvala.", "Prosim.", "Živjo!", "Nasvidenje.", "Ne vem.", "Trenutek, pišem.",
            "Žejen sem.", "Lačen sem.", "Moram na stranišče.", "Zebe me.", "Vroče mi je.", "Utrujen sem, rad bi počival.", "Bi lahko, prosim, odprli okno?", "Rad bi se ulegel.",
            "Na pomoč!", "Prosim, pokličite sestro.", "Boli me.", "Tukaj me boli.", "Potrebujem svoja zdravila.", "Slabo mi je.", "Težko diham.", "Zdaj mi je bolje.",
            "Kako si?", "Dobro sem, hvala.", "Lahko prosim ponoviš?", "Prosim, govori malo počasneje.", "Daj mi trenutek, da napišem odgovor.", "To je smešno!", "Hahaha", "Strinjam se.", "O tem raje ne bi govoril.", "Rad te imam.",
            "Zdaj.", "Čez minuto.", "Čez eno uro.", "Danes.", "Jutri.", "Pojutrišnjem.", "Čez en teden.", "Včeraj.", "Predvčerajšnjim.", "Zjutraj.", "Zvečer.", "Dobro jutro.", "Lahko noč.", "Koliko je ura?",
        },
        // švédština
        ["sv"] = new[]
        {
            "Grundläggande", "Behov", "Hälsa", "Samtal", "Tid",
            "Ja.", "Nej.", "Tack.", "Snälla.", "Hej!", "Hej då.", "Jag vet inte.", "Ett ögonblick, jag skriver.",
            "Jag är törstig.", "Jag är hungrig.", "Jag behöver gå på toaletten.", "Jag fryser.", "Jag är för varm.", "Jag är trött, jag vill vila.", "Kan du öppna fönstret, tack?", "Jag vill lägga mig ner.",
            "Hjälp!", "Snälla, ring på sjuksköterskan.", "Det gör ont.", "Det gör ont här.", "Jag behöver min medicin.", "Jag mår illa.", "Jag har svårt att andas.", "Jag mår bättre nu.",
            "Hur mår du?", "Jag mår bra, tack.", "Kan du säga det igen?", "Prata lite långsammare, tack.", "Ge mig en stund att skriva mitt svar.", "Vad roligt!", "Hahaha", "Jag håller med.", "Det vill jag helst inte prata om.", "Jag älskar dig.",
            "Nu.", "Om en minut.", "Om en timme.", "I dag.", "I morgon.", "I övermorgon.", "Om en vecka.", "I går.", "I förrgår.", "På morgonen.", "På kvällen.", "God morgon.", "God natt.", "Vad är klockan?",
        },
        // turečtina
        ["tr"] = new[]
        {
            "Temel", "İhtiyaçlar", "Sağlık", "Sohbet", "Zaman",
            "Evet.", "Hayır.", "Teşekkür ederim.", "Lütfen.", "Merhaba!", "Hoşça kal.", "Bilmiyorum.", "Bir dakika, yazıyorum.",
            "Susadım.", "Acıktım.", "Tuvalete gitmem gerekiyor.", "Üşüyorum.", "Çok sıcakladım.", "Yoruldum, dinlenmek istiyorum.", "Pencereyi açabilir misiniz, lütfen?", "Uzanmak istiyorum.",
            "İmdat!", "Lütfen hemşireyi çağırın.", "Acıyor.", "Burası acıyor.", "İlaçlarıma ihtiyacım var.", "Kendimi kötü hissediyorum.", "Nefes almakta zorlanıyorum.", "Şimdi daha iyiyim.",
            "Nasılsın?", "İyiyim, teşekkürler.", "Tekrar söyler misin, lütfen?", "Lütfen biraz daha yavaş konuş.", "Cevabımı yazmam için bana biraz zaman ver.", "Çok komik!", "Hahaha", "Katılıyorum.", "Bu konuda konuşmamayı tercih ederim.", "Seni seviyorum.",
            "Şimdi.", "Bir dakika sonra.", "Bir saat sonra.", "Bugün.", "Yarın.", "Öbür gün.", "Bir hafta sonra.", "Dün.", "Evvelsi gün.", "Sabah.", "Akşam.", "Günaydın.", "İyi geceler.", "Saat kaç?",
        },
        // vietnamština
        ["vi"] = new[]
        {
            "Cơ bản", "Nhu cầu", "Sức khỏe", "Trò chuyện", "Thời gian",
            "Vâng.", "Không.", "Cảm ơn.", "Làm ơn.", "Xin chào!", "Tạm biệt.", "Tôi không biết.", "Chờ một chút, tôi đang gõ.",
            "Tôi khát nước.", "Tôi đói.", "Tôi cần đi vệ sinh.", "Tôi lạnh.", "Tôi nóng quá.", "Tôi mệt, tôi muốn nghỉ ngơi.", "Bạn có thể mở cửa sổ giúp tôi không?", "Tôi muốn nằm xuống.",
            "Cứu tôi với!", "Làm ơn gọi y tá.", "Đau quá.", "Đau ở đây.", "Tôi cần thuốc của tôi.", "Tôi thấy không khỏe.", "Tôi khó thở.", "Bây giờ tôi thấy đỡ hơn rồi.",
            "Bạn có khỏe không?", "Tôi khỏe, cảm ơn.", "Bạn có thể nói lại được không?", "Làm ơn nói chậm hơn một chút.", "Cho tôi một chút để gõ câu trả lời.", "Buồn cười quá!", "Hahaha", "Tôi đồng ý.", "Tôi không muốn nói về chuyện đó.", "Tôi yêu bạn.",
            "Bây giờ.", "Một phút nữa.", "Một tiếng nữa.", "Hôm nay.", "Ngày mai.", "Ngày kia.", "Một tuần nữa.", "Hôm qua.", "Hôm kia.", "Buổi sáng.", "Buổi tối.", "Chào buổi sáng.", "Chúc ngủ ngon.", "Mấy giờ rồi?",
        },
    };

    /// <summary>Doplní k výchozím skupinám a frázím překlady z tabulek <see cref="More"/>.</summary>
    private static List<PhraseCategory> WithMore(List<PhraseCategory> categories)
    {
        foreach (var (code, texts) in More)
        {
            if (texts.Length != MoreOrder.Length) continue;   // pojistka: posunutá tabulka by přiřadila špatné věty
            var byEnglish = new Dictionary<string, string>();
            for (var i = 0; i < texts.Length; i++) byEnglish[MoreOrder[i]] = texts[i];
            foreach (var c in categories)
            {
                if (byEnglish.TryGetValue(c.NameEn, out var name)) c.Names[code] = name;
                foreach (var p in c.Phrases)
                    if (byEnglish.TryGetValue(p.Text, out var text)) p.Translations[code] = text;
            }
        }
        return categories;
    }
}
