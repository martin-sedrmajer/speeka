using System.Diagnostics;
using Vox.Core;
using Vox.Voice;

namespace Vox;

/// <summary>Windows: hlasy SAPI a Průzkumník.</summary>
public sealed class WindowsPlatform : IVoxPlatform
{
    public string Name => "windows";

    public string AppVersion => typeof(WindowsPlatform).Assembly.GetName().Version?.ToString(3) ?? "";

    public IReadOnlyList<string> SystemVoices() => WindowsTts.Voices();

    public string? SystemVoiceFor(string culture) => WindowsTts.VoiceFor(culture);

    public byte[] SynthesizeSystem(string text, VoiceSettings voice) => WindowsTts.Synthesize(text, voice);

    public string VoicesFolderDisplay => AppPaths.VoicesDirForShell;

    public void OpenVoicesFolder()
    {
        try { Process.Start(new ProcessStartInfo("explorer.exe", $"\"{AppPaths.VoicesDirForShell}\"") { UseShellExecute = true }); }
        catch { /* průzkumník se nespustil */ }
    }
}
