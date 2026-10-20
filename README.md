# Vox

**Vox speaks for people who can't.** Type a sentence or tap a phrase and Vox says it out loud
in a natural voice. Everything runs offline on your computer: no cloud, no account, no data
leaving the device.

Vox is an AAC (augmentative and alternative communication) app for people who can read and
write but have lost their voice, for example after a stroke, with ALS/MND, after laryngeal
surgery, or temporarily after an operation.

## Features

- **Natural offline voice in Czech and English.** Vox ships with
  [Supertonic 3](https://huggingface.co/Supertone/supertonic-3) running locally through
  [sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx). The same voice speaks both languages.
- **One-tap CZ / EN switch.** It changes the phrase buttons, category names and the voice.
- **Phrase board.** Ready-made phrases in categories (basics, needs, health, conversation,
  time), with your own phrases and categories. Urgent phrases get large red buttons.
- **History** of what you said. Tapping an item repeats it. The order doesn't jump around,
  so buttons stay where you expect them.
- **Long-press** (or right-click) a phrase to put it into the text box and finish the sentence.
- **Sound for real rooms.** Volume up to 200 % with a limiter, level matching so quiet words
  don't get lost, a clarity boost for listeners who are hard of hearing, and optional
  robot or radio effects.
- **Pronunciation dictionary** for words the voice gets wrong. Buttons keep the correct
  spelling.
- **Caregiver lock.** Settings and phrase editing can be locked behind a simple arithmetic
  problem.
- **Accessible layout.** Three font sizes, dark and high-contrast light themes, five accent
  colours, and a mirrored layout (Flip UI).
- **Bring your own voices.** Other sherpa-onnx voices (Kokoro, Piper, more Supertonic) and
  installed Windows voices work too.

## Requirements

- Windows 10 or 11, 64-bit
- Microsoft Edge WebView2 Runtime (included in current Windows 10/11)

The portable build includes .NET and the voice, so nothing else needs to be installed.

## Building

Development build (needs the .NET 8 SDK):

```
dotnet publish Vox\Vox.csproj -c Release -r win-x64 --self-contained false -o publish\Vox
```

Put the Supertonic model folder
([sherpa-onnx-supertonic-3-tts-int8-2026-05-11](https://github.com/k2-fsa/sherpa-onnx/releases/download/tts-models/sherpa-onnx-supertonic-3-tts-int8-2026-05-11.tar.bz2))
into `%APPDATA%\Vox\voices`.

Portable release build (self-contained, with the voice bundled and zipped):

```
powershell -ExecutionPolicy Bypass -File build-portable.ps1
```

The result is `dist\Vox-<version>-win-x64.zip`.

## Status

Early version, built and tested by one developer. Feedback from AAC users, speech
therapists and caregivers is very welcome.

## License

Vox is licensed under the [GNU General Public License v3.0](LICENSE) or later, with an
additional permission for the Microsoft Edge WebView2 SDK and Runtime
(see [LICENSE-EXCEPTION.txt](LICENSE-EXCEPTION.txt)).

Third-party components keep their own licenses, see
[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt). The bundled Supertonic 3 model is
licensed under OpenRAIL-M.

---

## Česky

**Vox mluví za ty, kdo nemohou.** Napíšete větu nebo klepnete na frázi a Vox ji řekne
přirozeným hlasem, česky i anglicky. Běží celý offline, bez cloudu a bez účtu. Je určený
lidem, kteří umějí číst a psát, ale přišli o hlas, třeba po mrtvici, s ALS nebo po operaci
hrtanu. Podrobný technický popis je v souboru README.txt.
