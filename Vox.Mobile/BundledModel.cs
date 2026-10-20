namespace Vox.Mobile;

/// <summary>
/// Hlas Supertonic je v balíčku aplikace jako asset, sherpa-onnx ale potřebuje skutečné soubory.
/// Při prvním spuštění (a po aktualizaci modelu) se proto zkopíruje do složky aplikace.
/// Seznam souborů je ve voices/supertonic-3/files.txt (vytvoří ho build-android.ps1).
/// </summary>
public static class BundledModel
{
    private const string VoiceName = "supertonic-3";

    public static string TargetRoot => Path.Combine(FileSystem.AppDataDirectory, "voices");

    public static async Task EnsureCopiedAsync()
    {
        var target = Path.Combine(TargetRoot, VoiceName);
        var manifestTarget = Path.Combine(target, "files.txt");

        string manifest;
        try
        {
            await using var m = await FileSystem.OpenAppPackageFileAsync($"voices/{VoiceName}/files.txt");
            using var reader = new StreamReader(m);
            manifest = await reader.ReadToEndAsync();
        }
        catch (FileNotFoundException)
        {
            return;   // aplikace sestavená bez modelu
        }

        // stejný seznam = model už je rozbalený a aktuální
        if (File.Exists(manifestTarget) && await File.ReadAllTextAsync(manifestTarget) == manifest)
            return;

        Directory.CreateDirectory(target);
        // řádek: název|velikost (velikost jen kvůli poznání změny modelu)
        foreach (var line in manifest.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var name = line.Split('|')[0];
            await using var source = await FileSystem.OpenAppPackageFileAsync($"voices/{VoiceName}/{name}");
            var temp = Path.Combine(target, name + ".tmp");
            await using (var file = File.Create(temp))
                await source.CopyToAsync(file);
            File.Move(temp, Path.Combine(target, name), overwrite: true);
        }
        // seznam až nakonec: přerušené kopírování se při dalším spuštění zopakuje
        await File.WriteAllTextAsync(manifestTarget, manifest);
    }
}
