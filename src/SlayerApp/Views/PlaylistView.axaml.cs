using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SlayerApp.ViewModel;

namespace SlayerApp;

public partial class PlaylistView : UserControl
{
    public PlaylistView()
    {
        InitializeComponent();
        this.DataContext = new PlaylistViewModel();
    }
}