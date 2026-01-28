using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PlaylistModel
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
        if (Directory.Exists(filePath))
        {
            var files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (IsAudioFile(file))
                {
                    var song = Song.FromFile(file);
                    trackList.Add(song);
                }
            }
        }
        else if (File.Exists(filePath))
        {
            var song = Song.FromFile(filePath);
            trackList.Add(song);
        }
        
        SongCount = trackList.Count;
    }

    private bool IsAudioFile(string filePath)
    {
        var audioExtensions = new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a" };
        var extension = Path.GetExtension(filePath).ToLower();
        return audioExtensions.Contains(extension);
    }
}