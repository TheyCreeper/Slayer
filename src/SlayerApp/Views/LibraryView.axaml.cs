using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SlayerApp.Model;
using SlayerApp.ViewModel;
using src.SlayerApp.ViewModel;
using System.Linq;

namespace SlayerApp;

public partial class LibraryView : UserControl
{
    public LibraryView()
    {
        InitializeComponent();
        DataContext = new LibraryViewModel();
    }

    private void AddAlbumToPlaylist_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && 
            menuItem.SelectedItem is Playlist targetPlaylist &&
            menuItem.DataContext is AlbumViewModel sourceAlbum)
        {
            targetPlaylist.AddTracks(App.Database
                .song
                .GetAllSongs()
                .Where(x => x.Album == sourceAlbum.Name));
        }
    }

    private void AddPlaylistToPlaylist_Click(object? sender, RoutedEventArgs e)
    {
        //if (sender is MenuItem menuItem && 
        //    menuItem.DataContext is PlaylistViewModel targetPlaylist &&
        //    menuItem.Tag is PlaylistViewModel sourcePlaylist)
        //{
        //    sourcePlaylist.AddToPlaylist(targetPlaylist);
        //}
    }
}