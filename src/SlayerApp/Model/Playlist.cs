using Avalonia.Media;
using SlayerApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

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
}