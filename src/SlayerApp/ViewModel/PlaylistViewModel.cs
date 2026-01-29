using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SlayerApp.ViewModel
{
    public partial class PlaylistViewModel : ObservableObject
    {
        [ObservableProperty]
        private Playlist _playlist;

        public PlaylistViewModel(Playlist playlist) 
        {
            Playlist = playlist;
        }



        
    }
}
