using System.Windows;
using System.Windows.Threading;

namespace Vox;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += (_, args) => args.SetObserved();
        base.OnStartup(e);
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.Exception.ToString(), Core.Loc.T("Vox – neočekávaná chyba", "Vox – unexpected error"), MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
