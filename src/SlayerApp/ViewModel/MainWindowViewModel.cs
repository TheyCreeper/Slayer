using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SlayerApp.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Playlist> _navBarItem;

        [ObservableProperty]
        private PlaylistViewModel? _currentPlaylist;
        public bool IsPlaylistSelected => CurrentPlaylist is not null;

        public ICommand NavigateToPlaylist { get; }

        public MainWindowViewModel()
        {
            foreach (Playlist playlist in App.Database.GetPlaylists().Where(x => x.IsPinned))
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
