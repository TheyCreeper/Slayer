using Avalonia.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Playlist
{
    public string Name { get; set; } = string.Empty;
    public int SongCount { get; set; }
    public List<Song> trackList { get; set; } = new List<Song>();
    public ImageDrawing playlistImage { get; set; }

    public Playlist()
    {

    }

   
}