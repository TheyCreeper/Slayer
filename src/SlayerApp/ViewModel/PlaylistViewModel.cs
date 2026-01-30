using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTunesSearch.Library.Models;
using SlayerApp.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SlayerApp.ViewModel
{
    public partial class PlaylistViewModel : ObservableObject, ISongCollection
    {
        private static readonly HttpClient s_httpClient = new();

        [ObservableProperty]
        private Playlist _playlist;

        [ObservableProperty]
        private ObservableCollection<Song> _trackList;

        [ObservableProperty]
        private bool _shuffle;
        public string TotalDuration => FormatTotalDuration();

        private Task<Bitmap?>? _cover;
        public Task<Bitmap?> Cover => _cover ??= LoadCoverAsync();
        public string Name => Playlist.Name;

        public void RefreshCover()
        {
            _cover = null;
            OnPropertyChanged(nameof(Cover));
        }

        public PlaylistViewModel() { }

        public PlaylistViewModel(string name) 
        {
            Playlist = new Playlist(name);
        }
        public PlaylistViewModel(Playlist playlist) 
        {
            Playlist = playlist;
            TrackList = new ObservableCollection<Song>(); 
            foreach (Song song in playlist.trackList) TrackList.Add(song);
            Shuffle = playlist.Shuffle;
        }
        private string FormatTotalDuration()
        {
            var total = TimeSpan.FromTicks(TrackList.Sum(s => s.Duration.Ticks));
            if (total.TotalHours >= 1)
                return $"{(int)total.TotalHours} hr {total.Minutes} min";
            return $"{total.Minutes} min {total.Seconds} sec";
        }

        private async Task<Bitmap?> LoadCoverAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Playlist.PlaylistImage))
                    return null;

                Stream imageStream;

                if (Playlist.PlaylistImage.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // HTTP streams are not seekable, copy to MemoryStream
                    var httpStream = await s_httpClient.GetStreamAsync(Playlist.PlaylistImage);
                    var memoryStream = new MemoryStream();
                    await httpStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    imageStream = memoryStream;
                }
                else if (File.Exists(Playlist.PlaylistImage))
                {
                    imageStream = File.OpenRead(Playlist.PlaylistImage);
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

        [RelayCommand]
        private async Task ChangePlaylistImageAsync()
        {
            var topLevel = TopLevel.GetTopLevel(App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (topLevel == null)
                return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Playlist Image",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Image Files") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } }
                }
            });

            if (files.Count > 0)
            {
                var file = files[0];
                var localPath = file.TryGetLocalPath();
                if (!string.IsNullOrEmpty(localPath))
                {
                    Playlist.PlaylistImage = localPath;
                    RefreshCover();
                }
            }
        }

        [RelayCommand]
        public void ToggleShuffle()
        {
            Shuffle = !Shuffle;
        }

        [RelayCommand]
        private void PlayPlaylist()
        {
            if (Shuffle)
            {
                App.MediaBar.IsShuffleEnabled = true;
                var shuffledSongs = new ObservableCollection<Song>(TrackList);
                QueueListManager.Shuffle(ref shuffledSongs);
                App.MediaBar.PlaySongs(shuffledSongs);
                return;
            }
            App.MediaBar.PlaySongs(TrackList);
        }

        [RelayCommand]
        private void AddPlaylistToQueue(PlaylistViewModel playlist)
        {
            App.MediaBar.AddToQueue(playlist.TrackList);
        }

        public void AddToPlaylist(AlbumViewModel targetAlbum)
        {
            foreach(Song song in targetAlbum.Songs)
            {
                Playlist.trackList.Add(song.Song);
                this.TrackList.Add(song);
            }
            App.Database.AddData(this.Playlist);
            App.RefreshPlaylists();
        }

    }
}
