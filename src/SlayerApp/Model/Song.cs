using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using SlayerApp;
using System;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public class Song
{
    public string Checksum { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Artists { get; set; } = Array.Empty<string>();
    public string Album { get; set; } = string.Empty;
    public uint TrackNumber { get; set; }
    public uint Year { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsCurrent { get; set; }

    public Song(Song song) {}

    public Song(string title, string[] artists, string album, TimeSpan duration, string path)
    {
        Title = title;
        Artists = artists;
        Album = album;
        Duration = duration;
        Path = path;
    }

    public void Play()
    {
        App.MediaBar.PlaySong(this);
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