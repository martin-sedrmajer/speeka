namespace Vox.Voice;

public static class Wav
{
    /// <summary>Mono float vzorky (-1..1) → 16bitový PCM WAV.</summary>
    public static byte[] FromFloat(float[] samples, int sampleRate)
    {
        using var ms = new MemoryStream(44 + samples.Length * 2);
        using var w = new BinaryWriter(ms);
        int dataSize = samples.Length * 2;

        w.Write("RIFF"u8.ToArray());
        w.Write(36 + dataSize);
        w.Write("WAVE"u8.ToArray());
        w.Write("fmt "u8.ToArray());
        w.Write(16);                 // velikost bloku fmt
        w.Write((short)1);           // PCM
        w.Write((short)1);           // mono
        w.Write(sampleRate);
        w.Write(sampleRate * 2);     // bajtů za sekundu
        w.Write((short)2);           // zarovnání bloku
        w.Write((short)16);          // bitů na vzorek
        w.Write("data"u8.ToArray());
        w.Write(dataSize);

        foreach (var f in samples)
        {
            var v = Math.Clamp(f, -1f, 1f);
            w.Write((short)(v * short.MaxValue));
        }

        w.Flush();
        return ms.ToArray();
    }
}
