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

    /// <summary>Verze z Microsoft Store (MSIX) běží ze složky WindowsApps.</summary>
    public static bool IsPackaged =>
        AppDir.Contains(@"\WindowsApps\", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Složka vlastních hlasů tak, jak ji vidí Průzkumník. Verze ze Storu zapisuje do AppData
    /// přesměrovaně (…\Packages\&lt;balíček&gt;\LocalCache\Roaming), Průzkumník mimo balíček by
    /// skutečnou cestu nenašel.
    /// </summary>
    public static string VoicesDirForShell
    {
        get
        {
            if (!IsPackaged) return VoicesDir;
            try
            {
                // složka balíčku: Název_Verze_Architektura_Prostředky_IdVydavatele
                var parts = Path.GetFileName(AppDir.TrimEnd('\\', '/')).Split('_');
                if (parts.Length < 5) return VoicesDir;
                var familyName = parts[0] + "_" + parts[^1];
                var redirected = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Packages", familyName, "LocalCache", "Roaming", "Vox", "voices");
                return Directory.Exists(redirected) ? redirected : VoicesDir;
            }
            catch { return VoicesDir; }
        }
    }

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
