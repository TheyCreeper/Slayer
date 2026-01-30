using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using iTunesSearch.Library;
using SlayerApp;
using SlayerApp.ViewModel;
using src.SlayerApp.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagLib.IFD.Tags;

namespace SlayerApp;

public partial class App : Application
{
    public static DBViewModel Database;
    public static iTunesSearchManager s_SearchManager = new();
    public static MediaBarViewModel MediaBar { get; } = new();
    public static ObservableCollection<PlaylistViewModel> Playlists { get; private set; } = new();

    public static void RefreshPlaylists()
    {
        Playlists.Clear();
        foreach (var playlist in Database.GetPlaylists().Select(p => new PlaylistViewModel(p)))
        {
            Playlists.Add(playlist);
        }
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Database = new();
        RefreshPlaylists();
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