using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using iTunesSearch.Library;
using SlayerApp;
using SlayerApp.ViewModel;
using TagLib.IFD.Tags;

namespace SlayerApp;

public partial class App : Application
{
    public static GPViewModel Database;
    public static iTunesSearchManager s_SearchManager = new();
    public static MediaBarViewModel MediaBar { get; } = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Database = new();

    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}