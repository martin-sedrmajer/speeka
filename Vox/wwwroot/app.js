"use strict";

// Vox – stránka: psaní, fráze, nastavení a přehrávání. Hlas vyrábí C# (Bridge.cs).

const $ = (id) => document.getElementById(id);

const state = {
  settings: null,
  voices: [],          // [{ name, dir, kokoro, supertonic, lang, speakers: [{id, label}] | null }]
  windowsVoices: [],
  voicesDir: "",
  activeCat: null,     // id skupiny, nebo "history"
  editing: false,
  formLang: "en",      // pro který jazyk se v nastavení právě upravuje hlas
  draftVoices: null,   // { en, cs } – rozpracované hlasy v nastavení
};

function lang() {
  return state.settings && state.settings.language === "cs" ? "cs" : "en";
}

function send(msg) {
  if (window.chrome && window.chrome.webview) window.chrome.webview.postMessage(msg);
}

if (window.chrome && window.chrome.webview) {
  window.chrome.webview.addEventListener("message", (e) => onMessage(e.data));
}

function onMessage(msg) {
  switch (msg.type) {
    case "init":
      state.settings = msg.settings;
      state.voices = msg.voices || [];
      state.windowsVoices = msg.windowsVoices || [];
      state.voicesDir = msg.voicesDir || "";
      applyLook();
      renderAll();
      $("text").focus();
      break;

    case "settings":
      state.settings = msg.settings;
      applyLook();
      renderAll();
      break;

    case "voices":
      state.voices = msg.voices || [];
      state.windowsVoices = msg.windowsVoices || [];
      if (drawerOpen()) {
        const keep = state.formLang;
        fillSettingsForm(collectSettings());
        if (keep !== state.formLang) {
          state.formLang = keep;
          loadVoiceFields(state.draftVoices[keep]);
        }
      }
      break;

    case "history":
      if (state.settings) state.settings.history = msg.items || [];
      if (state.activeCat === "history") renderBoard();
      break;

    case "voice.status":
      setVoiceStatus(msg.state, msg.text);
      break;

    case "speak.start":
      // nová promluva přerušuje starou
      stopAudio();
      currentSpeech = msg.id;
      break;

    case "audio":
      if (msg.id !== currentSpeech) return;
      enqueueAudio(msg);
      break;

    case "speak.error":
      if (msg.id === currentSpeech) currentSpeech = null;
      toast("Hlas: " + msg.message);
      setSpeaking(!!playing);
      break;

    case "toast":
      toast(msg.text);
      break;
  }
}

// ---------------------------------------------------------------- vzhled

function applyLook() {
  const s = state.settings;
  document.body.classList.remove("fs-m", "fs-l", "fs-xl");
  document.body.classList.add("fs-" + (s.fontSize || "l"));
  document.body.classList.toggle("light", s.theme === "light");
  document.body.classList.toggle("flipped", !!s.flipUi);
  document.body.dataset.accent = s.accent || "blue";
  document.querySelectorAll(".lang button").forEach((b) => b.classList.toggle("active", b.dataset.lang === lang()));
  $("text").lang = lang();
  $("text").placeholder = lang() === "cs"
    ? "Napiš, co chceš říct (česky)…   Enter = říct"
    : "Napiš, co chceš říct (anglicky)…   Enter = říct";
}

// Přepínač CZ / EN: mění jazyk tlačítek i hlas, kterým se mluví.
document.querySelectorAll(".lang button").forEach((b) => b.addEventListener("click", () => {
  if (!state.settings || b.dataset.lang === lang()) return;
  state.settings.language = b.dataset.lang;
  applyLook();
  renderAll();
  send({ type: "language.set", lang: b.dataset.lang });
}));

function catName(c) {
  return (lang() === "en" ? c.nameEn || c.name : c.name || c.nameEn) || "(bez názvu)";
}

// Znění fráze v zapnutém jazyce; když chybí, použije se druhé.
function phraseText(p) {
  return lang() === "en" ? p.text || p.label : p.label || p.text;
}

function phraseOther(p) {
  const other = lang() === "en" ? p.label : p.text;
  return other && other !== phraseText(p) ? other : null;
}

function setVoiceStatus(kind, text) {
  const el = $("voiceStatus");
  el.className = "voice-status " + (kind || "");
  el.textContent = text || "";
}

// ---------------------------------------------------------------- mluvení

// remember = false: zopakování z historie – pořadí se nemění, ať tlačítka neuhýbají pod prstem.
function speak(text, remember = true) {
  text = (text || "").trim();
  if (!text) return;
  send({ type: "speak", text, remember });
}

function speakComposer() {
  const t = $("text").value.trim();
  if (!t) { $("text").focus(); return; }
  speak(t);
  if (state.settings.clearAfterSpeak) $("text").value = "";
  $("text").focus();
}

function stopAll() {
  send({ type: "stop" });
  currentSpeech = null;
  stopAudio();
}

$("btnSpeak").addEventListener("click", speakComposer);
$("btnClear").addEventListener("click", () => { $("text").value = ""; $("text").focus(); });
$("btnRepeat").addEventListener("click", () => {
  const h = (state.settings && state.settings.history) || [];
  if (h.length) speak(h[0], false); else toast("Zatím nebylo co zopakovat.");
});
$("btnStopTop").addEventListener("click", stopAll);

$("text").addEventListener("keydown", (e) => {
  if (e.key === "Enter" && !e.shiftKey && state.settings && state.settings.enterSpeaks) {
    e.preventDefault();
    speakComposer();
  }
});

document.addEventListener("keydown", (e) => {
  if (e.key !== "Escape") return;
  if (!$("lockDialog").hidden) { closeLock(); return; }
  if (!$("dialog").hidden) { closeDialog(); return; }
  if (drawerOpen()) { closeDrawer(); return; }
  stopAll();
});

// ---------------------------------------------------------------- skupiny a fráze

function categories() {
  return (state.settings && state.settings.categories) || [];
}

function renderAll() {
  if (!state.settings) return;
  const cats = categories();
  if (state.activeCat !== "history" && !cats.some((c) => c.id === state.activeCat))
    state.activeCat = cats.length ? cats[0].id : "history";
  renderCats();
  renderBoard();
}

function renderCats() {
  const nav = $("cats");
  nav.innerHTML = "";
  for (const c of categories()) {
    const b = document.createElement("button");
    b.textContent = catName(c);
    b.classList.toggle("active", c.id === state.activeCat);
    b.addEventListener("click", () => {
      if (state.editing && c.id === state.activeCat) { openCategoryDialog(c); return; }
      state.activeCat = c.id;
      renderCats();
      renderBoard();
    });
    nav.appendChild(b);
  }
  const hist = document.createElement("button");
  hist.textContent = "🕘 Historie";
  hist.classList.toggle("active", state.activeCat === "history");
  hist.addEventListener("click", () => { state.activeCat = "history"; renderCats(); renderBoard(); });
  nav.appendChild(hist);

  if (state.editing) {
    const add = document.createElement("button");
    add.className = "add";
    add.textContent = "+ Skupina";
    add.addEventListener("click", () => openCategoryDialog(null));
    nav.appendChild(add);
  }
}

function renderBoard() {
  const board = $("board");
  board.innerHTML = "";

  if (state.activeCat === "history") {
    $("boardTitle").textContent = "Naposledy řečené – klepnutím zopakuješ";
    const items = state.settings.history || [];
    if (!items.length) {
      board.appendChild(emptyNote("Zatím tu nic není. Co řekneš, objeví se tady."));
      return;
    }
    for (const t of items) {
      const b = tile(t, null, false);
      b.classList.add("history");
      b.title = t;   // dlouhá zpráva je na tlačítku zkrácená, celá je v bublině
      onTilePress(b, () => speak(t, false), () => insertIntoComposer(t));
      board.appendChild(b);
    }
    clampHistory();
    return;
  }

  const cat = categories().find((c) => c.id === state.activeCat);
  if (!cat) return;
  $("boardTitle").textContent = state.editing
    ? "Úprava: klepni na frázi, nebo na název skupiny nahoře pro přejmenování"
    : catName(cat);

  cat.phrases.forEach((p, index) => {
    // při úpravě je vidět i druhý jazyk, ať je hned poznat, kde překlad chybí
    const b = tile(phraseText(p), state.editing ? phraseOther(p) || (lang() === "en" ? "(česky chybí)" : "(anglicky chybí)") : null, p.urgent);
    onTilePress(b,
      () => { if (state.editing) openPhraseDialog(cat, index); else speak(phraseText(p)); },
      () => { if (!state.editing) insertIntoComposer(phraseText(p)); });
    board.appendChild(b);
  });

  if (state.editing) {
    const add = document.createElement("button");
    add.className = "tile add";
    add.textContent = "+ Přidat frázi";
    add.addEventListener("click", () => openPhraseDialog(cat, -1));
    board.appendChild(add);
  } else if (!cat.phrases.length) {
    board.appendChild(emptyNote("Skupina je prázdná. Fráze přidáš přes „Upravit fráze“."));
  }
}

// Klepnutí řekne frázi. Podržení (nebo pravé tlačítko) ji jen vloží do pole, aby se dala upravit.
const HOLD_MS = 550;

function onTilePress(button, onTap, onHold) {
  let timer = null;
  let held = false;

  const cancel = () => {
    clearTimeout(timer);
    timer = null;
    button.classList.remove("holding");
  };

  button.addEventListener("pointerdown", (e) => {
    if (e.button !== 0) return;
    held = false;
    button.classList.add("holding");
    timer = setTimeout(() => {
      timer = null;
      held = true;
      button.classList.remove("holding");
      onHold();
    }, HOLD_MS);
  });
  button.addEventListener("pointerup", cancel);
  button.addEventListener("pointerleave", cancel);
  button.addEventListener("pointercancel", cancel);
  button.addEventListener("click", (e) => {
    if (held) { held = false; e.preventDefault(); return; }   // podržení už vložilo text
    onTap();
  });
  button.addEventListener("contextmenu", (e) => {
    e.preventDefault();
    cancel();
    onHold();
  });
}

// Vloží text na místo kurzoru (s mezerou od okolního textu) a nechá ho připravený k úpravě.
function insertIntoComposer(text) {
  // koncová tečka by větu ukončila („Bolí mě to. u kolena“) – při vkládání ji vynecháme, ? a ! zůstávají
  text = text.replace(/\.+\s*$/, "");
  const box = $("text");
  const start = box.selectionStart ?? box.value.length;
  const end = box.selectionEnd ?? box.value.length;
  const before = box.value.slice(0, start);
  const after = box.value.slice(end);
  const pre = before && !/\s$/.test(before) ? " " : "";
  const post = after && !/^\s/.test(after) ? " " : "";
  box.value = before + pre + text + post + after;
  const caret = (before + pre + text).length;
  box.focus();
  box.setSelectionRange(caret, caret);
  toast("Vloženo do pole – uprav a řekni.");
}

// Dlouhé zprávy v historii zkrátí na tři řádky a dají „…“. Dělá to skript, protože
// zkrácení v CSS (line-clamp) u textu zarovnaného doprava (Flip UI) tři tečky ztratí.
function clampHistory() {
  for (const label of document.querySelectorAll(".tile.history .label")) {
    const full = label.parentElement.title || label.textContent;
    label.textContent = full;
    if (label.scrollHeight <= label.clientHeight + 1) continue;
    const words = full.split(" ");
    let lo = 0, hi = words.length;   // nejdelší počet slov, který se ještě vejde
    while (lo < hi) {
      const mid = Math.ceil((lo + hi) / 2);
      label.textContent = words.slice(0, mid).join(" ") + "…";
      if (label.scrollHeight <= label.clientHeight + 1) lo = mid; else hi = mid - 1;
    }
    label.textContent = lo > 0 ? words.slice(0, lo).join(" ").replace(/[\s,;:.-]+$/, "") + "…" : full;
  }
}
let clampTimer = 0;
window.addEventListener("resize", () => {
  clearTimeout(clampTimer);
  clampTimer = setTimeout(() => { if (state.activeCat === "history") clampHistory(); }, 120);
});

function tile(label, spoken, urgent) {
  const b = document.createElement("button");
  b.className = "tile" + (urgent ? " urgent" : "");
  const l = document.createElement("span");
  l.className = "label";
  l.textContent = label;
  b.appendChild(l);
  if (spoken) {
    const s = document.createElement("span");
    s.className = "spoken";
    s.textContent = spoken;
    b.appendChild(s);
  }
  return b;
}

function emptyNote(text) {
  const p = document.createElement("p");
  p.className = "empty";
  p.textContent = text;
  return p;
}

$("btnEdit").addEventListener("click", () => {
  // do úprav jen přes zámek; z úprav ven vždycky
  if (!state.editing) { requireUnlock(toggleEditing); return; }
  toggleEditing();
});

function toggleEditing() {
  state.editing = !state.editing;
  document.body.classList.toggle("editing", state.editing);
  $("btnEdit").textContent = state.editing ? "✓ Hotovo" : "✎ Upravit fráze";
  if (state.editing && state.activeCat === "history") {
    const cats = categories();
    if (cats.length) state.activeCat = cats[0].id;
  }
  renderAll();
}

function savePhrases() {
  send({ type: "phrases.save", categories: categories() });
  // kdo zrovna upravuje, tomu se odemčení prodlužuje
  if (Date.now() < unlockedUntil) unlockedUntil = Date.now() + UNLOCK_MINUTES * 60 * 1000;
}

// ---------------------------------------------------------------- dialog fráze / skupiny

let dialogAction = null;

function openPhraseDialog(cat, index) {
  const p = index >= 0 ? cat.phrases[index] : { label: "", text: "", urgent: false };
  $("dlgTitle").textContent = index >= 0 ? "Upravit frázi" : "Nová fráze";
  $("dlgPhrase").hidden = false;
  $("dlgCategory").hidden = true;
  $("dlgLabel").value = p.label || "";
  $("dlgText").value = p.text || "";
  $("dlgUrgent").checked = !!p.urgent;
  $("dlgDelete").hidden = index < 0;

  dialogAction = (what) => {
    if (what === "delete") {
      cat.phrases.splice(index, 1);
    } else {
      const label = $("dlgLabel").value.trim();
      const text = $("dlgText").value.trim();
      if (!label && !text) { toast("Vyplň frázi aspoň v jednom jazyce."); return false; }
      const item = { label, text, urgent: $("dlgUrgent").checked };
      if (index >= 0) cat.phrases[index] = item; else cat.phrases.push(item);
    }
    savePhrases();
    renderBoard();
    return true;
  };
  openDialog(lang() === "en" ? $("dlgText") : $("dlgLabel"));
}

function openCategoryDialog(cat) {
  $("dlgTitle").textContent = cat ? "Upravit skupinu" : "Nová skupina";
  $("dlgPhrase").hidden = true;
  $("dlgCategory").hidden = false;
  $("dlgCatName").value = cat ? cat.name : "";
  $("dlgCatNameEn").value = cat ? cat.nameEn || "" : "";
  $("dlgDelete").hidden = !cat;

  dialogAction = (what) => {
    const cats = categories();
    if (what === "delete") {
      if (cat.phrases.length && !confirmDelete) {
        confirmDelete = true;
        $("dlgDelete").textContent = "Opravdu smazat i s frázemi?";
        return false;
      }
      cats.splice(cats.indexOf(cat), 1);
      state.activeCat = cats.length ? cats[0].id : "history";
    } else {
      const name = $("dlgCatName").value.trim();
      const nameEn = $("dlgCatNameEn").value.trim();
      if (!name && !nameEn) { toast("Vyplň název skupiny."); return false; }
      if (cat) { cat.name = name; cat.nameEn = nameEn; }
      else {
        const created = { id: Math.random().toString(16).slice(2) + Date.now().toString(16), name, nameEn, phrases: [] };
        cats.push(created);
        state.activeCat = created.id;
      }
    }
    savePhrases();
    renderAll();
    return true;
  };
  openDialog($("dlgCatName"));
}

let confirmDelete = false;

function openDialog(focusEl) {
  confirmDelete = false;
  $("dlgDelete").textContent = "Smazat";
  $("dialog").hidden = false;
  setTimeout(() => focusEl.focus(), 0);
}

function closeDialog() {
  $("dialog").hidden = true;
  dialogAction = null;
}

$("dlgCancel").addEventListener("click", closeDialog);
$("dlgSave").addEventListener("click", () => { if (dialogAction && dialogAction("save") !== false) closeDialog(); });
$("dlgDelete").addEventListener("click", () => { if (dialogAction && dialogAction("delete") !== false) closeDialog(); });
$("dialog").addEventListener("click", (e) => { if (e.target === $("dialog")) closeDialog(); });
["dlgLabel", "dlgText", "dlgCatName", "dlgCatNameEn"].forEach((id) =>
  $(id).addEventListener("keydown", (e) => { if (e.key === "Enter" && !e.shiftKey) { e.preventDefault(); $("dlgSave").click(); } }));

// ---------------------------------------------------------------- zámek nastavení

// Pečovatel odemkne jednoduchým, ale ne úplně triviálním příkladem. Po odemčení zůstane
// odemčeno pár minut, ať se při úpravách nemusí počítat pořád dokola.
const UNLOCK_MINUTES = 5;
let unlockedUntil = 0;
let lockAction = null;
let lockResult = 0;
let lockMisses = 0;

function requireUnlock(action) {
  if (!state.settings || !state.settings.lockSettings || Date.now() < unlockedUntil) { action(); return; }
  lockAction = action;
  lockMisses = 0;
  newLockTask();
  $("lockDialog").hidden = false;
  setTimeout(() => $("lockAnswer").focus(), 0);
}

function newLockTask() {
  const r = (min, max) => min + Math.floor(Math.random() * (max - min + 1));
  let text;
  if (Math.random() < 0.5) {
    const a = r(14, 49), b = r(14, 49);
    text = `${a} + ${b}`;
    lockResult = a + b;
  } else {
    const a = r(3, 9), b = r(3, 9), c = r(2, 9);
    text = `${a} × ${b} + ${c}`;
    lockResult = a * b + c;
  }
  $("lockTask").textContent = text + " = ?";
  $("lockAnswer").value = "";
  $("lockError").hidden = true;
}

function checkLock() {
  if (Number($("lockAnswer").value) === lockResult && $("lockAnswer").value.trim() !== "") {
    unlockedUntil = Date.now() + UNLOCK_MINUTES * 60 * 1000;
    const action = lockAction;
    closeLock();
    if (action) action();
    return;
  }
  lockMisses++;
  $("lockError").hidden = false;
  // po druhé chybě nový příklad, ať se nedá hádat
  if (lockMisses % 2 === 0) { newLockTask(); $("lockError").hidden = false; }
  $("lockAnswer").select();
}

function closeLock() {
  $("lockDialog").hidden = true;
  lockAction = null;
  $("text").focus();
}

// Se zámkem se úpravy frází po vypršení odemčení samy ukončí (ne uprostřed otevřeného dialogu).
setInterval(() => {
  if (state.editing && state.settings && state.settings.lockSettings &&
      Date.now() >= unlockedUntil && $("dialog").hidden) {
    toggleEditing();
    toast("Úpravy frází se zamkly.");
  }
}, 15000);

$("lockOk").addEventListener("click", checkLock);
$("lockCancel").addEventListener("click", closeLock);
$("lockAnswer").addEventListener("keydown", (e) => { if (e.key === "Enter") { e.preventDefault(); checkLock(); } });
$("lockDialog").addEventListener("click", (e) => { if (e.target === $("lockDialog")) closeLock(); });

// ---------------------------------------------------------------- nastavení

function drawerOpen() {
  return $("drawer").classList.contains("open");
}

function openDrawer() {
  if (!state.settings) return;
  fillSettingsForm(state.settings);
  send({ type: "voices.refresh" });
  $("drawer").inert = false;
  $("drawer").classList.add("open");
  $("drawer").setAttribute("aria-hidden", "false");
  $("scrim").hidden = false;
}

function closeDrawer() {
  $("drawer").classList.remove("open");
  $("drawer").setAttribute("aria-hidden", "true");
  $("drawer").inert = true;
  $("scrim").hidden = true;
  $("text").focus();
}

$("btnSettings").addEventListener("click", () => requireUnlock(openDrawer));
$("btnCloseSettings").addEventListener("click", closeDrawer);
$("scrim").addEventListener("click", closeDrawer);

function fillSettingsForm(s) {
  state.draftVoices = { en: { ...s.voice }, cs: { ...s.voiceCs } };
  state.formLang = s.language === "cs" ? "cs" : "en";
  loadVoiceFields(state.draftVoices[state.formLang]);

  $("pitch").value = s.pitch || 1;
  $("volume").value = s.volume ?? 1;
  $("leveler").checked = !!s.leveler;
  $("clarity").checked = !!s.clarity;
  $("effect").value = s.effect || "none";
  $("fontSize").value = s.fontSize || "l";
  $("theme").value = s.theme || "dark";
  $("accent").value = s.accent || "blue";
  $("pronCs").value = s.pronunciationCs || "";
  $("pronEn").value = s.pronunciationEn || "";
  $("flipUi").checked = !!s.flipUi;
  $("enterSpeaks").checked = s.enterSpeaks !== false;
  $("clearAfterSpeak").checked = s.clearAfterSpeak !== false;
  $("lockSettings").checked = !!s.lockSettings;

  $("voiceHint").textContent = state.voices.length
    ? "Vlastní hlas (Kokoro, Piper, Supertonic) rozbal do vlastní podsložky ve složce s hlasy."
    : "Nenašel jsem žádný přirozený hlas. Rozbal Supertonic, Kokoro nebo Piper do: " + state.voicesDir;
  $("voiceHint").className = "hint" + (state.voices.length ? "" : " bad");
  updateValues();
}

// Pole hlasu v nastavení patří vždy jednomu jazyku (přepínač „Hlas pro češtinu / angličtinu“).
function loadVoiceFields(v) {
  document.querySelectorAll(".seg button").forEach((b) => b.classList.toggle("active", b.dataset.vl === state.formLang));
  $("engine").value = v.engine || "local";

  const vd = $("voiceDir");
  vd.innerHTML = "";
  for (const voice of state.voices) {
    const kind = voice.supertonic ? "Supertonic" : voice.kokoro ? "Kokoro" : "Piper";
    const tag = voice.supertonic ? ", vícejazyčný" : voice.lang ? ", " + voice.lang.toUpperCase() : "";
    vd.add(new Option(voice.name + "  · " + kind + tag, voice.dir));
  }
  if (v.piperModelDir && !state.voices.some((x) => x.dir === v.piperModelDir)) vd.add(new Option(v.piperModelDir, v.piperModelDir));
  if (!state.voices.length && !v.piperModelDir) vd.add(new Option("(žádný hlas nenalezen)", ""));
  vd.value = v.piperModelDir || (state.voices[0] ? state.voices[0].dir : "");
  $("speakerNum").value = v.piperSpeaker || 0;
  syncSpeaker();

  const wv = $("windowsVoice");
  wv.innerHTML = "";
  wv.add(new Option("(výchozí)", ""));
  for (const name of state.windowsVoices) wv.add(new Option(name, name));
  wv.value = state.windowsVoices.includes(v.windowsVoice) ? v.windowsVoice : "";
  $("windowsRate").value = v.windowsRate || 0;
  $("speed").value = v.piperSpeed || 1;
  $("noiseScale").value = v.piperNoiseScale ?? 0.667;
  $("noiseW").value = v.piperNoiseW ?? 0.8;

  showEngine();
  updateValues();
}

function readVoiceFields() {
  return {
    engine: $("engine").value,
    piperModelDir: $("voiceDir").value,
    piperSpeaker: Math.max(0, Math.trunc(Number($("speakerNum").value)) || 0),
    piperSpeed: Number($("speed").value),
    piperNoiseScale: Number($("noiseScale").value),
    piperNoiseW: Number($("noiseW").value),
    windowsVoice: $("windowsVoice").value,
    windowsRate: Number($("windowsRate").value),
  };
}

document.querySelectorAll(".seg button").forEach((b) => b.addEventListener("click", () => {
  if (b.dataset.vl === state.formLang) return;
  state.draftVoices[state.formLang] = readVoiceFields();
  state.formLang = b.dataset.vl;
  loadVoiceFields(state.draftVoices[state.formLang]);
}));

// Varování, když hlas neumí jazyk, pro který je nastavený (Kokoro česky, český Piper anglicky).
function updateLangWarn() {
  const el = $("langWarn");
  let text = "";
  if ($("engine").value === "local") {
    const voice = state.voices.find((x) => x.dir === $("voiceDir").value);
    if (voice && voice.lang && voice.lang !== state.formLang) {
      text = state.formLang === "cs"
        ? "Tento hlas není český, česky bude mluvit s přízvukem (Kokoro česky nepřečte vůbec). Správnou výslovnost má Supertonic, Piper cs_CZ (jirka) nebo hlas Windows Microsoft Jakub."
        : "Tento hlas je " + voice.lang.toUpperCase() + ", anglicky bude mluvit s přízvukem nebo špatně.";
    }
  }
  el.textContent = text;
  el.hidden = !text;
}

// Kokoro: mluvčí podle jména; Piper: číslo (většinou 0).
function syncSpeaker() {
  const voice = state.voices.find((x) => x.dir === $("voiceDir").value);
  const list = voice && voice.speakers;
  const sel = $("speakerSel");
  const num = $("speakerNum");
  if (!list || !list.length) {
    sel.hidden = true;
    num.hidden = false;
    $("rowSpeaker").style.display = voice && voice.kokoro ? "" : "none";
    return;
  }
  $("rowSpeaker").style.display = "";
  const id = Number(num.value) || 0;
  sel.innerHTML = "";
  for (const sp of list) sel.add(new Option(sp.label, String(sp.id)));
  if (!list.some((sp) => sp.id === id)) sel.add(new Option("Mluvčí č. " + id, String(id)));
  sel.value = String(id);
  sel.hidden = false;
  num.hidden = true;
}

$("voiceDir").addEventListener("change", () => {
  const voice = state.voices.find((x) => x.dir === $("voiceDir").value);
  // u nového hlasu rovnou nabídnout výchozího mluvčího: Supertonic Muž 2, Kokoro Heart, jinak prvního
  const heart = voice && voice.speakers ? voice.speakers.find((sp) => /^Heart/.test(sp.label)) : null;
  $("speakerNum").value = voice && voice.supertonic ? 6 : heart ? heart.id : 0;
  syncSpeaker();
  showEngine();
});
$("speakerSel").addEventListener("change", () => { $("speakerNum").value = $("speakerSel").value; });

function showEngine() {
  const engine = $("engine").value;
  document.querySelectorAll(".opt").forEach((el) => el.classList.toggle("show", el.dataset.engine === engine));
  // intonace a rytmus jen u Piperu – Kokoro ani Supertonic je nemají
  const voice = state.voices.find((x) => x.dir === $("voiceDir").value);
  const piper = engine === "local" && !(voice && (voice.kokoro || voice.supertonic));
  $("rowPiperFx").style.display = piper ? "" : "none";
  $("piperFxHint").style.display = piper ? "" : "none";
  updateLangWarn();
}
$("engine").addEventListener("change", showEngine);

function updateValues() {
  $("speedVal").textContent = Number($("speed").value).toFixed(2) + "×";
  $("noiseVal").textContent = Number($("noiseScale").value).toFixed(2);
  $("noiseWVal").textContent = Number($("noiseW").value).toFixed(2);
  $("pitchVal").textContent = Math.round(Number($("pitch").value) * 100) + " %";
  $("volumeVal").textContent = Math.round(Number($("volume").value) * 100) + " %";
  $("winRateVal").textContent = $("windowsRate").value;
}
["speed", "pitch", "volume", "windowsRate", "noiseScale", "noiseW"].forEach((id) => $(id).addEventListener("input", updateValues));

function collectSettings() {
  const s = JSON.parse(JSON.stringify(state.settings));
  state.draftVoices[state.formLang] = readVoiceFields();
  s.voice = { ...state.draftVoices.en };
  s.voiceCs = { ...state.draftVoices.cs };
  s.pitch = Number($("pitch").value);
  s.volume = Number($("volume").value);
  s.leveler = $("leveler").checked;
  s.clarity = $("clarity").checked;
  s.effect = $("effect").value;
  s.fontSize = $("fontSize").value;
  s.theme = $("theme").value;
  s.accent = $("accent").value;
  s.pronunciationCs = $("pronCs").value;
  s.pronunciationEn = $("pronEn").value;
  s.enterSpeaks = $("enterSpeaks").checked;
  s.clearAfterSpeak = $("clearAfterSpeak").checked;
  s.lockSettings = $("lockSettings").checked;
  s.flipUi = $("flipUi").checked;
  return s;
}

$("btnTest").addEventListener("click", () => {
  if (!drawerOpen()) return;
  send({ type: "test", settings: collectSettings(), lang: state.formLang });
});
$("btnSaveSettings").addEventListener("click", () => {
  if (!drawerOpen()) return;
  send({ type: "settings.save", settings: collectSettings() });
  closeDrawer();
});
$("btnVoicesFolder").addEventListener("click", () => send({ type: "voices.open" }));

let resetArmed = false;
$("btnResetPhrases").addEventListener("click", () => {
  if (!resetArmed) {
    resetArmed = true;
    $("btnResetPhrases").textContent = "Opravdu? Tvoje úpravy zmizí";
    setTimeout(() => { resetArmed = false; $("btnResetPhrases").textContent = "Vrátit výchozí fráze"; }, 4000);
    return;
  }
  resetArmed = false;
  $("btnResetPhrases").textContent = "Vrátit výchozí fráze";
  send({ type: "phrases.reset" });
});
$("btnClearHistory").addEventListener("click", () => {
  send({ type: "history.clear" });
  toast("Historie vymazána.");
});

// ---------------------------------------------------------------- přehrávání

let audioCtx = null;
let audioQueue = [];
let playing = null;
let busy = false;          // přehrávání běží (včetně dekódování další věty) – brání souběhu dvou vět
let generation = 0;        // zvyšuje se při zastavení; starý běh po dekódování pozná, že má skončit
let currentSpeech = null;

function enqueueAudio(item) {
  audioQueue.push(item);
  if (!busy) playNext();
}

function setSpeaking(on) {
  document.body.classList.toggle("speaking", on);
  $("btnStopTop").hidden = !on;
}

function b64ToBuffer(b64) {
  const bin = atob(b64);
  const bytes = new Uint8Array(bin.length);
  for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);
  return bytes.buffer;
}

async function playNext() {
  const item = audioQueue.shift();
  if (!item) {
    playing = null;
    busy = false;
    setSpeaking(false);
    return;
  }
  busy = true;
  const gen = generation;
  try {
    audioCtx = audioCtx || new AudioContext();
    if (audioCtx.state === "suspended") await audioCtx.resume();
    const buffer = await audioCtx.decodeAudioData(b64ToBuffer(item.wav));
    if (gen !== generation) return;            // mezitím se zastavilo – nová promluva má vlastní běh
    if (item.id !== currentSpeech) { playNext(); return; }

    const fx = item.fx || {};
    const src = audioCtx.createBufferSource();
    src.buffer = buffer;
    const pitch = Number(fx.pitch) || 1;
    src.playbackRate.value = pitch;

    const start = audioCtx.currentTime + 0.02;
    let node = src;
    if (fx.effect === "robot") node = robotChain(audioCtx, node, start, buffer.duration / pitch + 0.1);
    else if (fx.effect === "radio") node = radioChain(audioCtx, node);
    if (fx.clarity) node = clarityChain(audioCtx, node);
    if (fx.leveler) node = levelerChain(audioCtx, node);

    const out = audioCtx.createGain();
    out.gain.value = Number(fx.volume ?? 1);
    // pojistka na konci: hlasitý hlas ano, přetečení a chrčení ne
    const limiter = audioCtx.createDynamicsCompressor();
    limiter.threshold.value = -2; limiter.knee.value = 0; limiter.ratio.value = 20;
    limiter.attack.value = 0.002; limiter.release.value = 0.1;
    node.connect(out).connect(limiter).connect(audioCtx.destination);

    playing = src;
    setSpeaking(true);
    src.onended = () => { if (playing === src) playNext(); };
    src.start(start);
  } catch (err) {
    if (gen !== generation) return;
    toast("Zvuk se nepodařilo přehrát: " + err.message);
    playing = null;
    playNext();
  }
}

function stopAudio() {
  audioQueue = [];
  busy = false;
  generation++;
  if (playing) {
    const p = playing;
    playing = null;
    p.onended = null;
    try { p.stop(); } catch (_) { /* už dohrálo */ }
  }
  setSpeaking(false);
}

// Srozumitelnost: ořez hučení pod 140 Hz a zvýraznění pásma souhlásek kolem 3 kHz.
function clarityChain(ctx, input) {
  const hp = ctx.createBiquadFilter();
  hp.type = "highpass"; hp.frequency.value = 140;
  const presence = ctx.createBiquadFilter();
  presence.type = "peaking"; presence.frequency.value = 3000; presence.Q.value = 0.8; presence.gain.value = 7;
  input.connect(hp).connect(presence);
  return presence;
}

// Vyrovnání hlasitosti: tiché části nahoru, hlasité dolů, pak celé o kus zesílit.
function levelerChain(ctx, input) {
  const comp = ctx.createDynamicsCompressor();
  comp.threshold.value = -26; comp.knee.value = 18; comp.ratio.value = 4;
  comp.attack.value = 0.004; comp.release.value = 0.25;
  const makeup = ctx.createGain();
  makeup.gain.value = 1.6;
  input.connect(comp).connect(makeup);
  return makeup;
}

// Robot: kruhová modulace přimíchaná k hlasu a krátká kovová rezonance (stejné jako u Cye v Sentinelu).
function robotChain(ctx, input, start, duration) {
  const mix = ctx.createGain();
  const dry = ctx.createGain();
  dry.gain.value = 0.75;
  input.connect(dry).connect(mix);

  const ring = ctx.createGain();
  ring.gain.value = 0;
  const osc = ctx.createOscillator();
  osc.frequency.value = 58;
  osc.connect(ring.gain);
  const wet = ctx.createGain();
  wet.gain.value = 0.35;
  input.connect(ring).connect(wet).connect(mix);
  osc.start(start);
  osc.stop(start + duration + 0.5);

  const delay = ctx.createDelay(0.05);
  delay.delayTime.value = 0.0045;
  const feedback = ctx.createGain();
  feedback.gain.value = 0.5;
  const comb = ctx.createGain();
  comb.gain.value = 0.45;
  mix.connect(delay);
  delay.connect(feedback).connect(delay);
  delay.connect(comb);

  const out = ctx.createGain();
  out.gain.value = 0.8;
  mix.connect(out);
  comb.connect(out);
  return out;
}

// Vysílačka: úzké pásmo a lehké zkreslení.
function radioChain(ctx, input) {
  const hp = ctx.createBiquadFilter();
  hp.type = "highpass"; hp.frequency.value = 300;
  const lp = ctx.createBiquadFilter();
  lp.type = "lowpass"; lp.frequency.value = 3400;
  const shaper = ctx.createWaveShaper();
  const n = 1024, curve = new Float32Array(n), k = 6;
  for (let i = 0; i < n; i++) {
    const x = (i * 2) / n - 1;
    curve[i] = ((3 + k) * x * 20 * (Math.PI / 180)) / (Math.PI + k * Math.abs(x));
  }
  shaper.curve = curve;
  input.connect(hp).connect(shaper).connect(lp);
  return lp;
}

// ---------------------------------------------------------------- oznámení

let toastTimer = null;
function toast(text) {
  const t = $("toast");
  t.textContent = text;
  t.classList.add("show");
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => t.classList.remove("show"), 3500);
}

send({ type: "ready" });
