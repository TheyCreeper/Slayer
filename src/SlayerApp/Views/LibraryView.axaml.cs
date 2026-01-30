using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SlayerApp.ViewModel;
using src.SlayerApp.ViewModel;

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
            menuItem.SelectedItem is PlaylistViewModel targetPlaylist &&
            menuItem.DataContext is AlbumViewModel sourceAlbum)
        {
            targetPlaylist.AddToPlaylist(sourceAlbum.Album);
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