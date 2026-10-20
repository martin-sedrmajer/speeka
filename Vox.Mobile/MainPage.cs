using Vox.Core;

namespace Vox.Mobile;

/// <summary>
/// Celá aplikace je webová stránka (stejná jako ve Windows) v HybridWebView.
/// Zprávy ze stránky jdou do sdílené spojky (Bridge.cs), odpovědi zpátky jako JSON.
/// </summary>
public sealed class MainPage : ContentPage
{
    private readonly HybridWebView _web;
    private readonly Bridge _bridge;
    private readonly Task _modelReady;

    /// <summary>Právě zobrazená stránka; MainActivity jí předává hardwarová tlačítka.</summary>
    public static MainPage? Current { get; private set; }

    // Vibrace vyřizuje rovnou stránka, nečeká na hlas ani spojku.
    private const string HapticMessage = "{\"type\":\"haptic\"}";

    public MainPage()
    {
        BackgroundColor = Color.FromArgb("#0C1015");
        // Odsazení od stavového řádku, výřezu kamery a klávesnice řeší MainActivity (Platforms/Android).

        // Přibalený hlas se při prvním spuštění rozbalí z balíčku do složky aplikace.
        AppPaths.BundledVoicesOverride = BundledModel.TargetRoot;
        _modelReady = Task.Run(BundledModel.EnsureCopiedAsync);

        _web = new HybridWebView { HybridRoot = "wwwroot", DefaultFile = "index.html" };
        _web.RawMessageReceived += OnRawMessage;
        Content = _web;

        _bridge = new Bridge(
            json => MainThread.BeginInvokeOnMainThread(() =>
            {
                try { _web.SendRawMessage(json); }
                catch { /* stránka se právě zavírá */ }
            }),
            new AndroidPlatform());
        Current = this;
    }

    /// <summary>Tlačítko pro snížení hlasitosti: true = Vox ho převzal a hlasitost se nemění.</summary>
    public bool HandleVolumeDown(bool pressed)
    {
        if (!_bridge.VolumeKeySpeaks) return false;
        if (pressed) _bridge.SpeakComposerFromKey();
        return true;
    }

    private async void OnRawMessage(object? sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        var message = e.Message;
        if (string.IsNullOrEmpty(message)) return;
        if (message == HapticMessage)
        {
            // krátký pulz; respektuje systémové nastavení „Vibrace při dotyku“
            (_web.Handler?.PlatformView as Android.Views.View)
                ?.PerformHapticFeedback(Android.Views.FeedbackConstants.VirtualKey);
            return;
        }
        try
        {
            // dokud se model kopíruje, spojka počká (stránka mezitím ukazuje „Připravuji hlas…“)
            await _modelReady;
        }
        catch
        {
            // model se nepodařilo rozbalit – spojka pak nahlásí, že hlas chybí
        }
        _bridge.Handle(message);
    }
}
