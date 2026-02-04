using CommunityToolkit.Mvvm.ComponentModel;

namespace SlayerApp.Model;

public partial class QueueItem : ObservableObject
{
    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private Song _song;

    [ObservableProperty]
    private bool _isCurrent;

    public QueueItem(int index, Song song, bool isCurrent = false)
    {
        _index = index;
        _song = song;
        _isCurrent = isCurrent;
    }
}
