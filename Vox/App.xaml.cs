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
        MessageBox.Show(e.Exception.ToString(), "Vox – neočekávaná chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
