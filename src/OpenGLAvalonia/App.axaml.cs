using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using OpenGLAvalonia.ViewModels;
using OpenGLAvalonia.Views;

namespace OpenGLAvalonia;

public partial class App : Application
{
    public static double DpiScale { get; private set; } = 0;

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new ShellView {
                DataContext = new ShellViewModel()
            };

            DpiScale = desktop.MainWindow.DesktopScaling;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
