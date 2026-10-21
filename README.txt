SPEEKA (dříve Vox)
==================

Pomůcka pro lidi, kteří nemohou mluvit: napíšou text
nebo klepnou na frázi a aplikace ji řekne přirozeným
hlasem v deseti jazycích. Běží celá offline, bez AI
a bez internetu. Samostatný projekt; hlasový kód je
převzatý ze Sentinelu.
V obchodech a v aplikaci se jmenuje Speeka; v kódu,
složkách a souborech zůstává původní název Vox
(Vox.Mobile, %APPDATA%\Vox, identita balíčku
MartinSedrmajer.VoxAAC, ApplicationId ...vox).
Program ve Windows se jmenuje Speeka.exe.


STRUKTURA
---------
Vox.sln
build-portable.ps1  přenosné sestavení s přibaleným hlasem
build-android.ps1   sestavení pro Android (emulátor, Google Play)
Vox.Mobile\         aplikace pro Android (.NET MAUI)
LICENSE             GPL-3.0 (licence Voxu)
LICENSE-EXCEPTION.txt  výjimka pro WebView2
THIRD-PARTY-NOTICES.txt  licence převzatých částí
README.md           popis pro GitHub (anglicky)
Package\            Microsoft Store: šablona manifestu MSIX,
                    údaje z Partner Center (StoreIdentity.json),
                    ikony, screenshoty, texty pro obchod
docs\               webová stránka pro uživatele (GitHub
                    Pages): index.html, screenshoty, ukázky
                    hlasu Muž 2 česky a anglicky
Vox\
  App.xaml(.cs)     start aplikace, zachycení chyb
  MainWindow        okno s WebView2 (stránka z wwwroot)
  Bridge.cs         spojka stránka <-> hlas: dělení textu
                    na věty, syntéza, nastavení, fráze,
                    historie
  Core\
    AppPaths        kde leží data
    Settings        nastavení, fráze, historie (JSON)
    Loc             jazyk aplikace pro texty z C#
    Phrases         výchozí fráze pro první spuštění
  Voice\
    PiperTts        Piper, Kokoro a Supertonic přes
                    sherpa-onnx
    KokoroVoices    jména mluvčích Kokoro
    WindowsTts      hlasy Windows (SAPI)
    Wav             převod vzorků na WAV
  wwwroot\          rozhraní (index.html, app.css, app.js)
  Vox.ico
  WindowsPlatform   části jen pro Windows (hlasy SAPI,
                    Průzkumník) pro sdílenou spojku


DATA
----
%APPDATA%\Vox\settings.json   nastavení, fráze, historie
%APPDATA%\Vox\voices\         vlastní hlasy
voices\ vedle Speeka.exe       přibalený hlas (Supertonic 3)
%LOCALAPPDATA%\Vox\WebView2\  data WebView2


HLASY
-----
Hledají se ve dvou složkách: voices vedle Speeka.exe
(přibalený Supertonic 3) a %APPDATA%\Vox\voices
(hlasy, které si uživatel přidá sám). Každý hlas ve
vlastní podsložce, formát pro sherpa-onnx.
Přibalený je jen Supertonic: mluví všemi 31
jazyky stejným hlasem. Kokoro a Piper fungují dál, ale jen
jako vlastní hlasy (nepřibalují se).
Kokoro: složka obsahuje voices.bin; mluvčí se vybírá
podle jména (kokoro-en-v0_19 a kokoro-multi-lang-v1_0).
Piper: *.onnx, tokens.txt, espeak-ng-data.
Supertonic 3: duration_predictor, text_encoder,
vector_estimator a vocoder (*.onnx, přednost má int8),
tts.json, unicode_indexer.bin a voice.bin. Umí 31
jazyků včetně češtiny, jazyk se mu předává podle
zvoleného jazyka řeči. Mluvčí F1-F5 (ženy) a M1-M5 (muži).
Model: sherpa-onnx-supertonic-3-tts-int8-2026-05-11
z vydání tts-models projektu sherpa-onnx na GitHubu.
Licence modelu OpenRAIL-M.
Windows: hlasy nainstalované v systému (česky
Microsoft Jakub), robotičtější, ale bez stahování.
Hlas je jeden pro všechny jazyky (Voice v nastavení;
VoiceCs zůstává jen ze starších verzí a nepoužívá se):
jeden mluvčí, jedna rychlost a výška.
Při prvním spuštění se vybere Supertonic, mluvčí
Muž 2 (M2), rychlost 1,15, když se mluví česky,
jinak 1,0. Bez Supertonicu Kokoro (Heart), jiný hlas
nebo hlas Windows.
Jednojazyčný hlas (Kokoro, Piper) mluví jen svým
jazykem; ostatními jazyky mluví přibalený Supertonic
se stejnou rychlostí (Bridge.SpeechVoice). Hlas
Windows mluví vším, co uživatel zvolí.
Kvalita hlasu (Nastavení → Hlas): počet kroků
výpočtu Supertonicu. Automaticky = 12 ve Windows,
4 na Androidu; dál Nejvyšší 12, Vyvážená 6,
Nejrychlejší 3. Méně kroků = rychlejší, ale
v intonaci je slyšet chvění.
Po startu a po změně hlasu se hlas načte dopředu
(stav v horní liště), aby první věta nečekala.


MLUVENÍ
-------
Text se dělí na věty, dlouhé věty (nad 70 znaků) ještě
u čárek, středníků a pomlček; první kus zazní, zatímco
se další počítají. Smajlíky: :D, xD a smějící se emoji
se vysloví jako smích v jazyce řeči (hehehe, hahaha,
jajaja…), ostatní smajlíky a emoji se vynechají
(Core\Emoticons.cs).
Od klepnutí do prvního zvuku je na tlačítku Říct
kolečko a další klepnutí se ignorují, aby dvojité
klepnutí promluvu nezrušilo; Zastavit funguje. Nová promluva přeruší předchozí,
Esc nebo Zastavit mluvení utne.
Výška hlasu: přehrávání se zrychlí nebo zpomalí
a syntéza tempo dorovná, takže se mění jen výška.
U hlasů Windows se výška nepoužívá.
Hlasy Piper mají navíc živost intonace (noise scale,
výchozí 0,67) a proměnlivost rytmu (noise w, výchozí
0,80); nastavují se při načtení modelu, takže změna
načte hlas znovu. Kokoro ani Supertonic tyto volby
nemají.
Úpravy zvuku (WebAudio při přehrávání):
- Vyrovnat hlasitost: komprese a zesílení.
- Srozumitelnost: ořez pod 140 Hz, zvýraznění 3 kHz.
- Efekt Robot (kruhová modulace a kovová rezonance)
  nebo Vysílačka (úzké pásmo, lehké zkreslení).


JAZYK
-----
Dva nezávislé jazyky:
- Jazyk řeči: všech 31 jazyků Supertonicu
  (Core\Languages.cs). Deset z nich má přeložené
  připravené fráze: čeština, angličtina,
  slovenština, polština, němčina, španělština,
  francouzština, italština, ukrajinština, ruština.
  Ostatní (Phrases = false) mají zatím jen pole
  pro text a historii, bez skupin frází a bez úprav.
  Výběr v horní liště (na telefonu jen zkratka
  „CZ ▾“) a v Nastavení: nahoře pět naposledy
  použitých jazyků (RecentLanguages), pod čarou
  ostatní podle zkratky. Mění fráze na tlačítkách,
  názvy skupin a hlas. Výběr v liště jde v Nastavení
  schovat. Při prvním spuštění se vybere jazyk
  systému, jen pokud má fráze, jinak angličtina.
  Pole pro text a historie mají směr textu podle
  obsahu (arabština zprava doleva).
- Překlad pod frází (Nastavení → Jazyk): když se
  mluví jiným jazykem, ukáže se pod frází menším
  písmem její znění ve zvoleném jazyce (stejné
  znění se neopakuje). Nabízí se jen jazyky
  s frázemi.
- Jazyk aplikace (Nastavení → Jazyk):
  čeština nebo angličtina pro veškeré ovládání,
  hlášky, stav hlasu a názvy mluvčích. Při prvním
  spuštění se vybere podle jazyka Windows (čeština
  i pro slovenštinu, jinak angličtina); na nové
  instalaci se jazyk řeči vybere podle jazyka systému,
  pokud je mezi podporovanými. Texty v C# jsou
  v Core\Loc.cs (Loc.T), v rozhraní v app.js (MSG,
  UI_EN) a v index.html (atributy data-t).
Jazyk hlasu Piper se pozná podle názvu modelu
(cs_CZ-...), Kokoro mluví anglicky. Nastavení varuje,
když zvolený hlas umí jen jeden jazyk.


VÝSLOVNOST
----------
Slovník v Nastavení pro každý jazyk zvlášť
(Pronunciations; jedno okno a nad ním výběr jazyka,
výchozí je jazyk, kterým se mluví): řádky
„napsáno = vysloví se“. Starší PronunciationCs
a PronunciationEn se při načtení přesunou. Před syntézou se celá
slova nahradí (bez ohledu na velká písmena); na
tlačítkách i v poli zůstává správný pravopis.
Výchozí: včera = v čera (Jirka „včera“ nepřečte)
a shledanou = schledanou (hlasy čtou „sh“ jako „š“
nebo „s-h“, správně je „sch“).


FRÁZE
-----
Skupiny frází s velkými tlačítky. Fráze má znění
česky (Label), anglicky (Text), v dalších jazycích
(Translations) a příznak naléhavá (červené tlačítko).
Na tlačítku je a řekne se znění v zapnutém jazyce;
když chybí, angličtina, pak čeština. Skupina má název
česky (Name), anglicky (NameEn) a v dalších jazycích
(Names). Výchozí fráze jsou přeložené do deseti
jazyků (Core\Phrases.cs, pořadí jako Languages.All);
starším nastavením se překlady k výchozím frázím
doplní samy. Překlady nekontroloval rodilý mluvčí.
Výchozí skupiny: Základní, Potřeby, Zdraví, Rozhovor
a Čas (teď, za minutku, zítra, včera, za týden…).
Klepnutí na frázi ji řekne. Podržení (asi půl sekundy,
tlačítko se postupně podbarví) nebo pravé tlačítko myši
ji jen vloží do pole na místo kurzoru, aby se dala
upravit nebo doplnit. Platí i pro historii.
Úprava: tlačítko Upravit fráze se musí podržet
(klepnutí jen ukáže nápovědu), pak klepnout na frázi
(úprava, smazání), na Přidat frázi, na název aktivní
skupiny (přejmenování, smazání) nebo na + Skupina.
V jiném jazyce než čeština a angličtina má dialog
třetí pole pro zapnutý jazyk.
Historie drží posledních 40 řečených textů (dlouhé
zprávy jsou na tlačítku zkrácené na tři řádky, celý
text je v bublině po najetí myší). Nový text
jde na začátek; zopakování z historie nebo tlačítkem
Znovu pořadí nemění, aby tlačítka neuhýbala.


OVLÁDÁNÍ
--------
Enter řekne text (Shift+Enter nový řádek), lze vypnout.
Po vyslovení se pole vymaže, lze vypnout.
Velikost písma (střední, velká, největší) zvětší celé
rozhraní. Světlý vzhled má vysoký kontrast.
Barva zvýraznění: modrá (výchozí), tyrkysová, jantarová,
korálová, fialová; každá ve variantě pro tmavý
i světlý vzhled.
Zámek pro pečovatele (volba v Nastavení): vstup do
Nastavení a do úprav frází chce vyřešit příklad
(a + b do 49 + 49, nebo a x b + c), pokaždé jiný,
po dvou chybách nový. Zamkne se hned po zavření
Nastavení a po ukončení úprav frází (Hotovo). Během
práce platí odemčení nejvýš 5 minut, každá uložená
úprava fráze je prodlouží; po vypršení se úpravy
frází samy ukončí. Zavřený panel Nastavení nejde
projít klávesou Tab.
Přepínač jazyka a mluvení zůstávají volné.
Flip UI: zrcadlové rozložení – tlačítko Říct vpravo,
skupiny a fráze řazené a zarovnané zprava.


SESTAVENÍ
---------
Pro vývoj (potřebuje .NET 8 Desktop Runtime):
dotnet publish Vox\Vox.csproj -c Release -r win-x64
  --self-contained false -o publish\Vox
Model Supertonic pak patří do %APPDATA%\Vox\voices.

Přenosná verze k vydání: build-portable.ps1
(PowerShell, z kořene repozitáře). Sestaví Vox jako
self-contained (bez instalace .NET), přibalí Supertonic
do voices\supertonic-3, přidá licence a vytvoří
dist\Speeka-<verze>-win-x64.zip. Model vezme
z %APPDATA%\Vox\voices (první složka se Supertonicem),
jinak ho stáhne z vydání tts-models projektu
sherpa-onnx (mezipaměť dist\cache) a rozbalí přes
7-Zip (tar ve Windows formát bzip2 neumí).
Licenci modelu (OpenRAIL-M) stahuje z Hugging Face;
když to nejde, vypíše varování a licenci je potřeba
přidat ručně. Uživatel potřebuje jen WebView2 Runtime
(ve Windows 10/11 bývá).


MICROSOFT STORE
---------------
build-portable.ps1 -Msix vyrobí navíc balíček
dist\Speeka-<verze>-x64.msix. Rozložení je stejné jako
v ZIPu (včetně Supertonicu), přidají se ikony
z Package\Assets a AppxManifest.xml ze šablony
Package\AppxManifest.template.xml. Identitu balíčku
(Name, Publisher, PublisherDisplayName, DisplayName)
bere z Package\StoreIdentity.json, údaje jsou
v Partner Center u aplikace v části Product identity.
Verze balíčku je verze z Vox.csproj s nulou na konci
(0.2.0 -> 0.2.0.0). Balíček není podepsaný, podepíše
ho Store. Potřebuje makeappx.exe z Windows SDK.
Aplikace běží jako full trust (runFullTrust), jazyky
balíčku jsou čeština a angličtina, takže Store chce
stránku obchodu v obou (texty v Package\store-listing.txt).
Ve verzi ze Storu Windows přesměrovává zápisy do
AppData do složky balíčku; AppPaths.VoicesDirForShell
proto pro Průzkumník vrací přesměrovanou cestu.
Zásady ochrany soukromí: docs\privacy.html.


ANDROID (Vox.Mobile)
--------------------
.NET MAUI pro net10.0-android, Android 7.0 (API 24)
a novější, procesory arm64 a x64 (emulátor).
Sdílí kód s Windows verzí: projekt připojuje soubory
Vox\Bridge.cs, Vox\Core\*.cs a Vox\Voice\PiperTts.cs,
KokoroVoices.cs a Wav.cs (nekopíruje je). Co je jen
pro jednu platformu, jde přes Core\IVoxPlatform:
WindowsPlatform (hlasy SAPI, Průzkumník) a v Androidu
AndroidPlatform (bez systémových hlasů a bez složky
pro vlastní hlasy).
Rozhraní je stejný wwwroot jako ve Windows, zobrazený
v HybridWebView. app.js pozná hostitele: WebView2
(window.chrome.webview), nebo HybridWebView (načte
_framework/hybridwebview.js a zprávy posílá jako text
JSON). Init nese platform a stránka na Androidu skryje
volbu hlasu Windows a složku s hlasy. Na úzkém displeji
(do 640 px) jsou Znovu, Smazat a Nastavení jen ikony
a stav hlasu se ukazuje jen při načítání nebo chybě.
Hlas Supertonic je v balíčku jako asset
(voices/supertonic-3, soubor files.txt se seznamem
"název|velikost"). Při prvním spuštění, a když se
seznam změní, ho BundledModel zkopíruje do složky
aplikace, protože sherpa-onnx potřebuje skutečné
soubory. Do té doby spojka čeká.
build-android.ps1 model připraví do
Vox.Mobile\Resources\Raw\voices (v gitu není)
a sestaví aplikaci: bez přepínače ladicí verzi rovnou
do běžícího emulátoru nebo telefonu, s -Release balíček
.aab do dist\android. Potřebuje .NET 10 SDK, workload
maui-android, Android SDK a JDK.
ApplicationId io.github.martinsedrmajer.vox se po
prvním vydání na Google Play už nesmí měnit.
Balíček pro Google Play se podepisuje klíčem pro
nahrávání (Dokumenty\Vox-podpis\vox-upload.keystore,
mimo repozitář, alias vox); heslo se zadává při
sestavení a předává přes proměnnou prostředí.
Jen na Androidu: odsazení od stavového řádku, výřezu
kamery a klávesnice (MainActivity, Android 15+ kreslí
přes celý displej); krátká vibrace při klepnutí
na tlačítko a po dokončeném podržení (odezva na dotyk
systému, bez oprávnění); volitelně tlačítko pro
snížení hlasitosti řekne text z pole. Na telefonu
jsou dlaždice nižší a výběr jazyka v liště ukazuje
jen zkratku.
Aplikace nemá žádná oprávnění (ani k internetu).
