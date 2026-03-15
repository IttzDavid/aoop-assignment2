using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LibraryApp.Services;
using LibraryApp.ViewModels;

namespace LibraryApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        var dataService = new DataService();
        var authService = new AuthService(dataService);
        var libraryService = new LibraryService(dataService, authService.Data);
        var mainViewModel = new MainViewModel(authService, libraryService);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = mainViewModel };
            desktop.ShutdownRequested += (_, _) => libraryService.Save();
        }

        base.OnFrameworkInitializationCompleted();
    }
}