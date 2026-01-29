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

}