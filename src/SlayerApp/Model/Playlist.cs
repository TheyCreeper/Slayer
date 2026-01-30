using Avalonia.Media;
using SlayerApp.Model;
using SlayerApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SlayerApp.Model
{
    public class Playlist
    {
        public string Name { get; set; } = string.Empty;
        public int SongCount { get; set; }
        public ObservableCollection<Song> trackList { get; set; } = new ObservableCollection<Song>();
        public ImageDrawing playlistImage { get; set; }
        public string PlaylistImage { get; set; }
        public bool Shuffle { get; set; }
        public Playlist() { }

        public Playlist(string name)
        {
            Name = name;
        }

        public void AddTracks(IEnumerable<Song> list)
        {
            foreach (Song song in list)
            {
                trackList.Add(song);
            }
            App.Database.AddData(this);
        }
    }
}
