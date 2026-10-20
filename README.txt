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
LICENSE             GPL-3.0 (licence Voxu)
LICENSE-EXCEPTION.txt  výjimka pro WebView2
THIRD-PARTY-NOTICES.txt  licence převzatých částí
README.md           popis pro GitHub (anglicky)
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
    Phrases         výchozí fráze pro první spuštění
  Voice\
    PiperTts        Piper, Kokoro a Supertonic přes
                    sherpa-onnx
    KokoroVoices    jména mluvčích Kokoro
    WindowsTts      hlasy Windows (SAPI)
    Wav             převod vzorků na WAV
  wwwroot\          rozhraní (index.html, app.css, app.js)
  Vox.ico


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
Supertonic, mluvčí Muž 2 (M2) s rychlostí 1,15.
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
Přepínač CZ / EN v horní liště mění jazyk tlačítek
s frázemi, názvů skupin i hlas, kterým se mluví.
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
