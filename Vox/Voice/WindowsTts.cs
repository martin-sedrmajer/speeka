using System.Speech.Synthesis;
using Vox.Core;

namespace Vox.Voice;

/// <summary>Hlasy nainstalované ve Windows (SAPI). Zdarma a bez internetu.</summary>
public static class WindowsTts
{
    public static byte[] Synthesize(string text, VoiceSettings v)
    {
        using var synth = new SpeechSynthesizer();
        if (!string.IsNullOrWhiteSpace(v.WindowsVoice))
        {
            try { synth.SelectVoice(v.WindowsVoice); }
            catch { /* hlas mezitím zmizel – použije se výchozí */ }
        }
        synth.Rate = Math.Clamp(v.WindowsRate, -10, 10);

        using var ms = new MemoryStream();
        synth.SetOutputToWaveStream(ms);
        synth.Speak(text);
        synth.SetOutputToNull();
        return ms.ToArray();
    }

    /// <summary>První nainstalovaný hlas pro daný jazyk (např. "cs-CZ"), nebo null.</summary>
    public static string? VoiceFor(string culture)
    {
        try
        {
            using var synth = new SpeechSynthesizer();
            return synth.GetInstalledVoices()
                .Where(v => v.Enabled && string.Equals(v.VoiceInfo.Culture.Name, culture, StringComparison.OrdinalIgnoreCase))
                .Select(v => v.VoiceInfo.Name)
                .FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public static List<string> Voices()
    {
        try
        {
            using var synth = new SpeechSynthesizer();
            return synth.GetInstalledVoices()
                .Where(v => v.Enabled)
                .Select(v => v.VoiceInfo.Name)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }
}
