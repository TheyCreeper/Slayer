using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Playlist
{
    public string Name { get; set; } = string.Empty;
    public int SongCount { get; set; }
    public List<Song> trackList { get; set; } = new List<Song>();

    /// <summary>
    /// Generates data for a Folder/song and adds it to the playlist (probably not final at all)
    /// </summary>
    /// <param name="filePath"></param>
    public void AddFromPath(string filePath)
    {
        
    }

   
}