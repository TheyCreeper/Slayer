using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using SlayerApp;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace SlayerApp.Model
{
    public class Song : INotifyPropertyChanged
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
        public string DurationFormatted => Duration.ToString(@"m\:ss");
        public ICommand AddToQueue => new RelayCommand(AddThisToQueue);
        public ICommand Play => new RelayCommand(PlayThis);

        public Song() { }

        public Song(Song song) { }

        public Song(string title, string[] artists, string album, TimeSpan duration, string path)
        {
            Title = title;
            Artists = artists;
            Album = album;
            Duration = duration;
            Path = path;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void PlayThis()
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

        public void AddThisToQueue()
        {
            App.MediaBar.AddSingleToQueue(this);
        }
    }
}