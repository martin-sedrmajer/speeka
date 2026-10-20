using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Web.WebView2.Core;
using Vox.Core;

namespace Vox;

public partial class MainWindow : Window
{
    // ne .local – ta doména podle Microsoftu zdržuje navigaci
    private const string HostName = "vox.example";
    private readonly Bridge _bridge;
    private CoreWebView2? _core;
    private volatile bool _closed;

    public MainWindow()
    {
        InitializeComponent();
        Web.DefaultBackgroundColor = System.Drawing.Color.FromArgb(255, 12, 16, 21);
        SourceInitialized += (_, _) => ApplyDarkTitleBar();

        _bridge = new Bridge(json =>
        {
            var core = _core;
            if (core == null || _closed) return;
            Dispatcher.InvokeAsync(() =>
            {
                try { if (!_closed) core.PostWebMessageAsJson(json); }
                catch { /* okno se zavírá */ }
            });
        });

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var options = new CoreWebView2EnvironmentOptions
            {
                // hlas musí jít přehrát hned, bez předchozího kliknutí do stránky
                AdditionalBrowserArguments = "--autoplay-policy=no-user-gesture-required"
            };
            var env = await CoreWebView2Environment.CreateAsync(null, AppPaths.WebViewData, options);
            await Web.EnsureCoreWebView2Async(env);

            var core = Web.CoreWebView2;
            core.Settings.IsStatusBarEnabled = false;
            core.Settings.AreDevToolsEnabled = true;
            core.SetVirtualHostNameToFolderMapping(HostName, AppPaths.WebRoot, CoreWebView2HostResourceAccessKind.Allow);

            core.NewWindowRequested += (_, args) =>
            {
                args.Handled = true;
                OpenExternal(args.Uri);
            };
            core.NavigationStarting += (_, args) =>
            {
                if (!args.Uri.StartsWith($"https://{HostName}/", StringComparison.OrdinalIgnoreCase))
                {
                    args.Cancel = true;
                    OpenExternal(args.Uri);
                }
            };

            core.WebMessageReceived += (_, args) => _bridge.Handle(args.WebMessageAsJson);
            _core = core;
            core.Navigate($"https://{HostName}/index.html");
        }
        catch (WebView2RuntimeNotFoundException)
        {
            MessageBox.Show(Core.Loc.T(
                    "Chybí Microsoft Edge WebView2 Runtime. Na Windows 10/11 bývá nainstalovaný; jinak je ke stažení na webu Microsoftu.",
                    "Microsoft Edge WebView2 Runtime is missing. It is usually installed on Windows 10/11; otherwise download it from Microsoft."),
                "Vox", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Vox", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _closed = true;
        _bridge.Dispose();
        base.OnClosed(e);
    }

    private static void OpenExternal(string uri)
    {
        if (!uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return;
        try { Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true }); }
        catch { /* není kam otevřít */ }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    private void ApplyDarkTitleBar()
    {
        try
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int dark = 1;
            DwmSetWindowAttribute(hwnd, 20, ref dark, sizeof(int));      // DWMWA_USE_IMMERSIVE_DARK_MODE
            int caption = 0x0015100C;                                     // COLORREF (BGR) = #0C1015
            DwmSetWindowAttribute(hwnd, 35, ref caption, sizeof(int));   // DWMWA_CAPTION_COLOR (Windows 11)
        }
        catch { /* starší Windows */ }
    }
}
