using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SlayerApp.ViewModel
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        Playlist Playlist { get; set; }


        public PlaylistViewModel() { }

        public void SetPlaylist(Playlist model)
        {
            Playlist = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
