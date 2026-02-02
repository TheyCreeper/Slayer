using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using iTunesSearch.Library;
using SlayerApp;
using SlayerApp.Model;
using SlayerApp.Store;
using SlayerApp.ViewModel;
using src.SlayerApp.ViewModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using TagLib.IFD.Tags;

namespace SlayerApp;

public partial class App : Application
{
    public static Database Database { get; set; }
    public static iTunesSearchManager s_SearchManager = new();
    public static MediaBarViewModel MediaBar { get; } = new();
    public static ObservableCollection<Playlist> Playlists { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Database = new();
        Playlists = new ObservableCollection<Playlist>(Database.playlist.GetAllPlaylists());
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