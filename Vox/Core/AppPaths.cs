namespace Vox.Core;

/// <summary>Kde aplikace drží svoje data.</summary>
public static class AppPaths
{
    public static string DataDir { get; } = Ensure(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vox"));

    public static string LocalDir { get; } = Ensure(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vox"));

    public static string SettingsFile => Path.Combine(DataDir, "settings.json");
    /// <summary>Hlasy, které si uživatel přidá sám (Kokoro, Piper, další Supertonic).</summary>
    public static string VoicesDir { get; } = Ensure(Path.Combine(DataDir, "voices"));

    public static string AppDir => AppContext.BaseDirectory;

    /// <summary>Hlas přibalený k aplikaci (složka voices vedle Vox.exe).</summary>
    public static string BundledVoicesDir => Path.Combine(AppDir, "voices");

    /// <summary>Složky, ve kterých se hledají hlasy: přibalené a vlastní.</summary>
    public static IEnumerable<string> VoiceRoots =>
        new[] { BundledVoicesDir, VoicesDir }.Where(Directory.Exists);

    public static string WebViewData => Path.Combine(LocalDir, "WebView2");
    public static string WebRoot => Path.Combine(AppDir, "wwwroot");

    private static string Ensure(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}
