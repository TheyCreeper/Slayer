using System;
using Avalonia.Controls.Primitives;

public class Song
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Artists { get; set; } = Array.Empty<string>();
    public string Album { get; set; } = string.Empty;
    public uint TrackNumber { get; set; }
    public uint Year { get; set; }
    public TimeSpan Duration { get; set; }

    public Song() {}

    public Song(string title, string[] artists, string album, TimeSpan duration, string path)
    {
        Title = title;
        Artists = artists;
        Album = album;
        Duration = duration;
        Path = path;
    }

    // TODO
    /* 
        
    */

    public void PlayTrack()
    {
        throw new NotImplementedException();
    }

    public static Song FromFile(string path)
    {
        var file = TagLib.File.Create(path);
        return new Song
        {
            Path = path,
            Title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(path),
            Artists = file.Tag.Performers ?? Array.Empty<string>(),
            Album = file.Tag.Album ?? string.Empty,
            TrackNumber = file.Tag.Track,
            Year = file.Tag.Year,
            Duration = file.Properties.Duration
        };
    }
}