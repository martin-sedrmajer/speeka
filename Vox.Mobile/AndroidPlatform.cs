using Vox.Core;

namespace Vox.Mobile;

/// <summary>Android: bez hlasů Windows a bez složky pro vlastní hlasy (přibalený je Supertonic).</summary>
public sealed class AndroidPlatform : IVoxPlatform
{
    public string Name => "android";

    public string AppVersion => AppInfo.Current.VersionString;

    public IReadOnlyList<string> SystemVoices() => Array.Empty<string>();

    public string? SystemVoiceFor(string culture) => null;

    public byte[] SynthesizeSystem(string text, VoiceSettings voice) =>
        throw new InvalidOperationException(Loc.T(
            "Hlasy Windows na Androidu nejsou. V Nastavení vyber přirozený hlas.",
            "Windows voices are not available on Android. Choose a natural voice in Settings."));

    public string VoicesFolderDisplay => "";

    public void OpenVoicesFolder() { }
}
