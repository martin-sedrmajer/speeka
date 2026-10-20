VOX
===

Pomůcka pro lidi, kteří nemohou mluvit: napíšou text
nebo klepnou na frázi a aplikace ji řekne přirozeným
hlasem. Běží celá offline, bez AI a bez internetu.
Samostatný projekt; hlasový kód je převzatý ze Sentinelu.


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
voices\ vedle Vox.exe          přibalený hlas (Supertonic 3)
%LOCALAPPDATA%\Vox\WebView2\  data WebView2


HLASY
-----
Hledají se ve dvou složkách: voices vedle Vox.exe
(přibalený Supertonic 3) a %APPDATA%\Vox\voices
(hlasy, které si uživatel přidá sám). Každý hlas ve
vlastní podsložce, formát pro sherpa-onnx.
Přibalený je jen Supertonic: mluví česky i anglicky
stejným hlasem. Kokoro a Piper fungují dál, ale jen
jako vlastní hlasy (nepřibalují se).
Kokoro: složka obsahuje voices.bin; mluvčí se vybírá
podle jména (kokoro-en-v0_19 a kokoro-multi-lang-v1_0).
Piper: *.onnx, tokens.txt, espeak-ng-data.
Supertonic 3: duration_predictor, text_encoder,
vector_estimator a vocoder (*.onnx, přednost má int8),
tts.json, unicode_indexer.bin a voice.bin. Umí 31
jazyků včetně češtiny, jazyk se mu předává podle
přepínače CZ / EN. Mluvčí F1-F5 (ženy) a M1-M5 (muži).
Model: sherpa-onnx-supertonic-3-tts-int8-2026-05-11
z vydání tts-models projektu sherpa-onnx na GitHubu.
Licence modelu OpenRAIL-M.
Windows: hlasy nainstalované v systému (česky
Microsoft Jakub), robotičtější, ale bez stahování.
Při prvním spuštění se pro oba jazyky vybere
Supertonic, mluvčí Muž 2 (M2), rychlost 1,15
pro češtinu a 1,0 pro angličtinu.
Bez Supertonicu: čeština Piper cs_CZ nebo hlas
Windows, angličtina Kokoro (Heart), jiný hlas nebo
hlas Windows.
Po startu a po změně hlasu se hlas načte dopředu
(stav v horní liště), aby první věta nečekala.


MLUVENÍ
-------
Text se dělí na věty; první věta zazní, zatímco se
další počítají. Nová promluva přeruší předchozí,
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
- Jazyk řeči: přepínač CZ / EN v horní liště mění
  fráze na tlačítkách, názvy skupin a hlas.
- Jazyk aplikace (Nastavení → Ovládání a vzhled):
  čeština nebo angličtina pro veškeré ovládání,
  hlášky, stav hlasu a názvy mluvčích. Při prvním
  spuštění se vybere podle jazyka Windows (čeština
  i pro slovenštinu, jinak angličtina); na nové
  instalaci podle něj i jazyk řeči. Texty v C# jsou
  v Core\Loc.cs (Loc.T), v rozhraní v app.js (MSG,
  UI_EN) a v index.html (atributy data-t).
Každý jazyk má vlastní hlas (Voice a VoiceCs
v nastavení). Kokoro česky neumí; pro češtinu
Supertonic, Piper cs_CZ (jirka) nebo hlas Windows
Microsoft Jakub.
Při prvním spuštění se český hlas vybere sám (viz
Hlasy). Jazyk
hlasu Piper se pozná podle názvu modelu (cs_CZ-...).
Nastavení varuje, když hlas neumí zvolený jazyk.


VÝSLOVNOST
----------
Slovník v Nastavení, zvlášť pro češtinu a angličtinu:
řádky „napsáno = vysloví se“. Před syntézou se celá
slova nahradí (bez ohledu na velká písmena); na
tlačítkách i v poli zůstává správný pravopis.
Výchozí: včera = v čera (Jirka „včera“ nepřečte)
a shledanou = schledanou (hlasy čtou „sh“ jako „š“
nebo „s-h“, správně je „sch“).


FRÁZE
-----
Skupiny frází s velkými tlačítky. Fráze má znění
česky (Label) a anglicky (Text) a příznak naléhavá
(červené tlačítko). Na tlačítku je a řekne se znění
v zapnutém jazyce; když chybí, použije se druhé.
Skupina má název česky (Name) a anglicky (NameEn).
Výchozí skupiny: Základní, Potřeby, Zdraví, Rozhovor
a Čas (teď, za minutku, zítra, včera, za týden…).
Klepnutí na frázi ji řekne. Podržení (asi půl sekundy,
tlačítko se postupně podbarví) nebo pravé tlačítko myši
ji jen vloží do pole na místo kurzoru, aby se dala
upravit nebo doplnit. Platí i pro historii.
Úprava: tlačítko Upravit fráze, pak klepnout na frázi
(úprava, smazání), na Přidat frázi, na název aktivní
skupiny (přejmenování, smazání) nebo na + Skupina.
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
dist\Vox-<verze>-win-x64.zip. Model vezme
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
dist\Vox-<verze>-x64.msix. Rozložení je stejné jako
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
