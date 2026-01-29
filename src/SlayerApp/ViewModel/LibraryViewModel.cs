using SlayerApp;
using SlayerApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace src.SlayerApp.ViewModel;

public partial class LibraryViewModel : ObservableObject
{
    public ObservableCollection<AlbumViewModel> Albums { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlbumSelected))]
    private AlbumViewModel? _selectedAlbum;

    public bool IsAlbumSelected => SelectedAlbum is not null;

    public LibraryViewModel()
    {
        Albums = new(App.Database.GetAlbums().Select(a => new AlbumViewModel(a)));
    }

    [RelayCommand]
    private void SelectAlbum(AlbumViewModel album)
    {
        SelectedAlbum = album;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedAlbum = null;
    }

    [RelayCommand]
    private void PlayAlbum(AlbumViewModel album)
    {
        _ = album.PlayAlbumCommand;
    }

    [RelayCommand]
    private void AddAlbumToQueue(AlbumViewModel album)
    {
        _ = album.AddAlbumToQueueCommand;
    }

    [ObservableProperty]
    private ObservableCollection<PlaylistViewModel> _playlists = new();

    public bool HasPlaylists => Playlists.Count > 0;

    [ObservableProperty]
    private bool _isCreatePlaylistPromptVisible;

    [ObservableProperty]
    private string _newPlaylistName = string.Empty;

    [RelayCommand]
    private void ShowCreatePlaylistPrompt()
    {
        NewPlaylistName = string.Empty;
        IsCreatePlaylistPromptVisible = true;
    }

    [RelayCommand]
    private void CancelCreatePlaylist()
    {
        IsCreatePlaylistPromptVisible = false;
        NewPlaylistName = string.Empty;
    }

    [RelayCommand]
    private void ConfirmCreatePlaylist()
    {
        if (!string.IsNullOrWhiteSpace(NewPlaylistName))
        {
            var newPlaylist = new PlaylistViewModel(NewPlaylistName.Trim());
            Playlists.Add(newPlaylist);
            OnPropertyChanged(nameof(HasPlaylists));
        }
        IsCreatePlaylistPromptVisible = false;
        NewPlaylistName = string.Empty;
    }

    [RelayCommand]
    private void SelectPlaylist(PlaylistViewModel playlist)
    {
        // Handle playlist selection
    }

    [RelayCommand]
    private void PlayPlaylist(PlaylistViewModel playlist)
    {
        _ = playlist.PlayPlaylistCommand;
    }

    [RelayCommand]
    private void AddPlaylistToQueue(PlaylistViewModel playlist)
    {
        _ = playlist.AddPlaylistToQueueCommand;
    }

    [RelayCommand]
    private void RenamePlaylist(PlaylistViewModel playlist)
    {
        // Show rename dialog
    }

    [RelayCommand]
    private void DeletePlaylist(PlaylistViewModel playlist)
    {
        Playlists.Remove(playlist);
        OnPropertyChanged(nameof(HasPlaylists));
    }

}