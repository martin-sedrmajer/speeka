using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Vox.Mobile;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Android 15+ kreslí aplikaci přes celý displej, i pod stavový řádek a výřez kamery.
        // Obsah se proto odsadí o místo, které zabírají systémové lišty, výřez a klávesnice.
        var root = FindViewById(Android.Resource.Id.Content);
        if (root is null) return;
        root.SetBackgroundColor(Android.Graphics.Color.ParseColor("#0C1015"));
        ViewCompat.SetOnApplyWindowInsetsListener(root, new SystemBarsPadding());
    }

    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        // Volitelně (Nastavení): tlačítko pro snížení hlasitosti řekne text z pole.
        // Bere se jen první stisk, podržení tlačítka větu neopakuje.
        if (e is not null && e.KeyCode == Keycode.VolumeDown && MainPage.Current is { } page)
        {
            var pressed = e.Action == KeyEventActions.Down && e.RepeatCount == 0;
            if (page.HandleVolumeDown(pressed)) return true;
        }
        return base.DispatchKeyEvent(e);
    }

    private sealed class SystemBarsPadding : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(Android.Views.View? v, WindowInsetsCompat? insets)
        {
            if (v is null || insets is null) return insets;
            var area = insets.GetInsets(
                WindowInsetsCompat.Type.SystemBars() |
                WindowInsetsCompat.Type.DisplayCutout() |
                WindowInsetsCompat.Type.Ime());
            if (area is not null) v.SetPadding(area.Left, area.Top, area.Right, area.Bottom);
            return WindowInsetsCompat.Consumed;
        }
    }
}
