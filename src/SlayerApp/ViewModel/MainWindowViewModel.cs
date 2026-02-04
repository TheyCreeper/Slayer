using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlayerApp.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace SlayerApp.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Playlist> _navBarItem = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPlaylistSelected))]
        private PlaylistViewModel? _currentPlaylist;
        
        public bool IsPlaylistSelected => CurrentPlaylist is not null;

        public MainWindowViewModel()
        {
            LoadPinnedPlaylists();
        }

        [RelayCommand]
        private void LoadPinnedPlaylists()
        {
            NavBarItem.Clear();
            foreach (Playlist playlist in App.Database.playlist.GetAllPlaylists().Where(x => x.IsPinned))
            {
                NavBarItem.Add(playlist);
            }
        }

        [RelayCommand]
        private void NavigateToPlaylist(Playlist playlist)
        {
            CurrentPlaylist = new PlaylistViewModel(playlist);
        }
    }
}
