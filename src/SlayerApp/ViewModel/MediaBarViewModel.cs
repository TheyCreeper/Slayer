using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using SlayerApp.utils;

namespace SlayerApp.ViewModel;

public partial class MediaBarViewModel : ObservableObject
{
    private static readonly LibVLC s_libVLC = new();
    private readonly MediaPlayer _mediaPlayer;

    [ObservableProperty]
    private bool _isMediaBarVisible, _isQueueVisible, _isShuffleEnabled, _isRepeatEnabled;

    private Song? _currentSong;
    public Song? CurrentSong
    {
        get => _currentSong;
        set { _currentSong = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsPlaying)); }
    }

    private double _currentPosition;
    public double CurrentPosition
    {
        get => _currentPosition;
        set
        {
            if (Math.Abs(_currentPosition - value) > 0.01)
            {
                _currentPosition = value;
                OnPropertyChanged();
                
                if (Duration > 0 && Math.Abs(_mediaPlayer.Position * Duration - value) > 1)
                {
                    _mediaPlayer.Position = (float)(value / Duration);
                }
                CurrentTimeFormatted = QueueListManager.FormatTime(TimeSpan.FromSeconds(value));
            }
        }
    }
    [ObservableProperty]
    private double _duration;

    private double _volume = 50;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            OnPropertyChanged();
            _mediaPlayer.Volume = (int)value;
        }
    }

    private string _currentTimeFormatted = "0:00";
    public string CurrentTimeFormatted
    {
        get => _currentTimeFormatted;
        set { _currentTimeFormatted = value; OnPropertyChanged(); }
    }

    private string _totalTimeFormatted = "0:00";
    public string TotalTimeFormatted
    {
        get => _totalTimeFormatted;
        set { _totalTimeFormatted = value; OnPropertyChanged(); }
    }
    public bool IsPlaying => _mediaPlayer.IsPlaying;
    public ObservableCollection<Song> Queue { get; } = [];

    private int _currentIndex;

    public MediaBarViewModel()
    {
        _mediaPlayer = new MediaPlayer(s_libVLC);
        _mediaPlayer.PositionChanged += OnPositionChanged;
        _mediaPlayer.EndReached += OnEndReached;
        _mediaPlayer.Playing += (_, _) => OnPropertyChanged(nameof(IsPlaying));
        _mediaPlayer.Paused += (_, _) => OnPropertyChanged(nameof(IsPlaying));
        _mediaPlayer.Stopped += (_, _) => OnPropertyChanged(nameof(IsPlaying));
    }

    [RelayCommand]
    public void PlaySong(Song song)
    {
        Queue.Clear();
        CurrentSong = song;
        _currentIndex = 0;
        PlayCurrentSong(false);
        IsMediaBarVisible = true;
        IsQueueVisible = false;
    }

    [RelayCommand]
    public void PlaySongs(IEnumerable<Song> songs)
    {
        Queue.Clear();
        var songList = songs.ToList();

        if (IsShuffleEnabled)
        {
            QueueListManager.Shuffle(ref songList);
        }

        foreach (var song in songList)
        {
            Queue.Add(song);
        }

        if (Queue.Count > 0)
        {
            _currentIndex = 0;
            PlayCurrentSong();
            IsMediaBarVisible = true;
            IsQueueVisible = true;
        }
    }

    public void AddSingleToQueue(Song song)
    {
        Queue.Add(song);
    }

    public void AddToQueue(IEnumerable<Song> songs)
    {
        foreach (var song in songs)
        {
            Queue.Add(song);
        }

        if (!IsPlaying && Queue.Count > 0 && CurrentSong == null)
        {
            _currentIndex = 0;
            PlayCurrentSong();
            IsMediaBarVisible = true;
            IsQueueVisible = true;
        }
    }

    [RelayCommand]
    private void PlayCurrentSong(bool useQueue = true)
    {
        if (useQueue)
        {
            if (_currentIndex < 0 || _currentIndex >= Queue.Count)
                return;

            // Update IsCurrent for all queue items
            for (int i = 0; i < Queue.Count; i++)
            {
                Queue[i].IsCurrent = i == _currentIndex;
            }

            CurrentSong = Queue[_currentIndex];
            
            if (string.IsNullOrEmpty(CurrentSong.Path) || !System.IO.File.Exists(CurrentSong.Path))
                return;
        }

        var media = new Media(s_libVLC, CurrentSong.Path, FromType.FromPath);
        _mediaPlayer.Media = media;
        _mediaPlayer.Play();
        
        Duration = CurrentSong.Duration.TotalSeconds;
        TotalTimeFormatted = QueueListManager.FormatTime(CurrentSong.Duration);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (_mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Pause();
        }
        else if (CurrentSong != null)
        {
            _mediaPlayer.Play();
        }
    }

    [RelayCommand]
    private void Previous()
    {
        if (Queue.Count == 0)
            return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = Queue.Count - 1;

        PlayCurrentSong();
    }

    [RelayCommand]
    private void Next()
    {
        if (Queue.Count == 0)
            return;

        _currentIndex++;
        if (_currentIndex >= Queue.Count)
        {
            if (IsRepeatEnabled)
                _currentIndex = 0;
            else
            {
                _currentIndex = Queue.Count - 1;
                _mediaPlayer.Stop();
                return;
            }
        }

        PlayCurrentSong();
    }

    [RelayCommand]
    private void ToggleShuffle()
    {
        IsShuffleEnabled = !IsShuffleEnabled;
        if (IsShuffleEnabled)
        {
            var tempQueue = new ObservableCollection<Song>(Queue);
            QueueListManager.Shuffle<Song>(ref tempQueue);
        }
    }

    [RelayCommand]
    private void ToggleRepeat()
    {
        IsRepeatEnabled = !IsRepeatEnabled;
    }

    [RelayCommand]
    public void PlayFromQueue(Song song)
    {
        var index = Queue.IndexOf(song);
        if (index >= 0)
        {
            _currentIndex = index;
            PlayCurrentSong();
        }
    }

    private void OnPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var position = e.Position * Duration;
            if (Math.Abs(_currentPosition - position) > 0.5)
            {
                _currentPosition = position;
                OnPropertyChanged(nameof(CurrentPosition));
                CurrentTimeFormatted = QueueListManager.FormatTime(TimeSpan.FromSeconds(position));
            }
        });
    }

    private void OnEndReached(object? sender, EventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(Next);
    }
}

//public class RelayCommand : System.Windows.Input.ICommand
//{
//    private readonly Action _execute;
//    private readonly Func<bool>? _canExecute;

//    public RelayCommand(Action execute, Func<bool>? canExecute = null)
//    {
//        _execute = execute;
//        _canExecute = canExecute;
//    }

//    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
//    public void Execute(object? parameter) => _execute();
//    public event EventHandler? CanExecuteChanged;
//}

