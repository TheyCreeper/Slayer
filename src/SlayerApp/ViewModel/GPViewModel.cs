using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlayerApp.ViewModel
{
    /// <summary>
    /// Handles common data across the app. Helps serialization and deserialization. 
    /// 
    /// It's called a viewmodel but it's much more of a static """database""" to access the data
    /// across the app
    /// 
    /// This way the app stores only the paths to the user's media library, 
    /// and loads it all up when starting the app
    /// </summary>
    public static class GPViewModel
    {
        List<Album> AlbumList = new List<Album>();
        List<Song> SongList = new List<Song>();
        List<Playlist> Playlists = new List<Playlist>();
        List<Genre> GenreList = new List<Genre>();
        List<string> libraryPaths = new();

        public static GPViewModel(string[] libraryPaths)
        {
            
        }
    }
}