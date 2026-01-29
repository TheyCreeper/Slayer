using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LibVLCSharp.Shared;

namespace SlayerApp.ViewModel;

public class MediaBarViewModel : INotifyPropertyChanged
{
    private static readonly LibVLC s_libVLC = new();
    private readonly MediaPlayer _mediaPlayer;

    private bool _isMediaBarVisible;
    public bool IsMediaBarVisible
    {
        get => _isMediaBarVisible;
        set { _isMediaBarVisible = value; OnPropertyChanged(); }
    }

    private bool _isShuffleEnabled;
    public bool IsShuffleEnabled
    {
        get => _isShuffleEnabled;
        set { _isShuffleEnabled = value; OnPropertyChanged(); }
    }

    private bool _isRepeatEnabled;
    public bool IsRepeatEnabled
    {
        get => _isRepeatEnabled;
        set { _isRepeatEnabled = value; OnPropertyChanged(); }
    }

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
                CurrentTimeFormatted = FormatTime(TimeSpan.FromSeconds(value));
            }
        }
    }

    private double _duration;
    public double Duration
    {
        get => _duration;
        set { _duration = value; OnPropertyChanged(); }
    }

    private double _volume = 100;
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

    // Commands
    public RelayCommand PlayPauseCommand { get; }
    public RelayCommand PreviousCommand { get; }
    public RelayCommand NextCommand { get; }
    public RelayCommand ToggleShuffleCommand { get; }
    public RelayCommand ToggleRepeatCommand { get; }

    public MediaBarViewModel()
    {
        _mediaPlayer = new MediaPlayer(s_libVLC);
        _mediaPlayer.PositionChanged += OnPositionChanged;
        _mediaPlayer.EndReached += OnEndReached;
        _mediaPlayer.Playing += (_, _) => OnPropertyChanged(nameof(IsPlaying));
        _mediaPlayer.Paused += (_, _) => OnPropertyChanged(nameof(IsPlaying));
        _mediaPlayer.Stopped += (_, _) => OnPropertyChanged(nameof(IsPlaying));

        PlayPauseCommand = new RelayCommand(PlayPause);
        PreviousCommand = new RelayCommand(Previous);
        NextCommand = new RelayCommand(Next);
        ToggleShuffleCommand = new RelayCommand(ToggleShuffle);
        ToggleRepeatCommand = new RelayCommand(ToggleRepeat);
    }

    public void PlaySong(Song song)
    {
        Queue.Clear();
        CurrentSong = song;
        _currentIndex = 0;
        PlayCurrentSong(false);
        IsMediaBarVisible = true;
    }

    public void PlaySongs(IEnumerable<Song> songs)
    {
        Queue.Clear();
        var songList = songs.ToList();

        if (IsShuffleEnabled)
        {
            Shuffle(songList);
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
        }
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
        }
    }

    private void PlayCurrentSong(bool useQueue = true)
    {
        if (useQueue)
        {
            if (_currentIndex < 0 || _currentIndex >= Queue.Count)
                return;

            CurrentSong = Queue[_currentIndex];
            
            if (string.IsNullOrEmpty(CurrentSong.Path) || !System.IO.File.Exists(CurrentSong.Path))
                return;
        }

        var media = new Media(s_libVLC, CurrentSong.Path, FromType.FromPath);
        _mediaPlayer.Media = media;
        _mediaPlayer.Play();
        
        Duration = CurrentSong.Duration.TotalSeconds;
        TotalTimeFormatted = FormatTime(CurrentSong.Duration);
    }

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

    private void Previous()
    {
        if (Queue.Count == 0)
            return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = Queue.Count - 1;

        PlayCurrentSong();
    }

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

    private void ToggleShuffle()
    {
        IsShuffleEnabled = !IsShuffleEnabled;
    }

    private void ToggleRepeat()
    {
        IsRepeatEnabled = !IsRepeatEnabled;
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
                CurrentTimeFormatted = FormatTime(TimeSpan.FromSeconds(position));
            }
        });
    }

    private void OnEndReached(object? sender, EventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(Next);
    }

    private static void Shuffle<T>(List<T> list)
    {
        var rng = Random.Shared;
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private static string FormatTime(TimeSpan time)
    {
        return time.TotalHours >= 1
            ? $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}"
            : $"{time.Minutes}:{time.Seconds:D2}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class RelayCommand : System.Windows.Input.ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => _execute();
    public event EventHandler? CanExecuteChanged;
}
