using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlayerApp;

namespace SlayerApp.ViewModel
{
    public partial class AlbumViewModel : ObservableObject, IEquatable<AlbumViewModel>
    {
        private static readonly HttpClient s_httpClient = new();
        private readonly Album _album;

        public AlbumViewModel(Album album)
        {
            _album = album;
            LoadSongs();
        }

        public string Artist => _album.Artist;
        public string Name => _album.Name;
        public string ReleaseDate => _album.ReleaseDate ?? "Unknown";
        public string ReleaseYear => string.IsNullOrEmpty(_album.ReleaseDate) 
            ? "Unknown" 
            : _album.ReleaseDate.Split('-')[0];

        public Task<Bitmap?> Cover => LoadCoverAsync();

        public ObservableCollection<SongViewModel> Songs { get; } = [];

        public int SongCount => Songs.Count;
        public string TotalDuration => FormatTotalDuration();

        private void LoadSongs()
        {
            var albumSongs = App.Database.GetSongs()
                .Where(s => s.Album.Equals(Name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.TrackNumber)
                .Select(s => new SongViewModel(s));

            foreach (var song in albumSongs)
            {
                Songs.Add(song);
            }
        }

        private string FormatTotalDuration()
        {
            var total = TimeSpan.FromTicks(Songs.Sum(s => s.Duration.Ticks));
            if (total.TotalHours >= 1)
                return $"{(int)total.TotalHours} hr {total.Minutes} min";
            return $"{total.Minutes} min {total.Seconds} sec";
        }

        private async Task<Bitmap?> LoadCoverAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_album.AlbumArt))
                    return null;

                Stream imageStream;

                if (_album.AlbumArt.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // HTTP streams are not seekable, copy to MemoryStream
                    var httpStream = await s_httpClient.GetStreamAsync(_album.AlbumArt);
                    var memoryStream = new MemoryStream();
                    await httpStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    imageStream = memoryStream;
                }
                else if (File.Exists(_album.AlbumArt))
                {
                    imageStream = File.OpenRead(_album.AlbumArt);
                }
                else
                {
                    return null;
                }

                await using (imageStream)
                {
                    return await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
                }
            }
            catch
            {
                return null;
            }
        }

        public bool Equals(AlbumViewModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _album.Equals(other._album);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AlbumViewModel)obj);
        }

        public override int GetHashCode()
        {
            return _album.GetHashCode();
        }
    }

    public partial class SongViewModel : ObservableObject
    {
        private readonly Song _song;

        public SongViewModel(Song song)
        {
            _song = song;
        }

        public uint TrackNumber => _song.TrackNumber;
        public string Title => _song.Title;
        public string Artists => string.Join(", ", _song.Artists);
        public TimeSpan Duration => _song.Duration;
        public string DurationFormatted => Duration.ToString(@"m\:ss");

        [RelayCommand]
        private void Play()
        {
            _song.PlayTrack();
        }
    }
}
