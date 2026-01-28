using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace src.SlayerApp.ViewModel;

public class LibraryViewModel : INotifyPropertyChanged
{
    private string _playlistName;
    private int _songCount;
    private int _playlistImage;
    public string PlaylistName
    {
        get => _playlistName;
        set
        {
            if (_playlistName != value)
            {
                _playlistName = value;
                OnPropertyChanged();
            }
        }
    }

    public int SongCount
    {
        get => _songCount;
        set
        {
            if (_songCount != value)
            {
                _songCount = value;
                OnPropertyChanged();
            }
        }
    }

    public PlaylistViewModel()
    {
        _playlistName = "New Playlist";
        SongCount = 10;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}