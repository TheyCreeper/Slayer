using Avalonia.Controls;

namespace SlayerApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PlaylistModel playlist = new PlaylistModel();
        playlist.AddFromPath("/home/theycreeper/Documents/Minecraft - Volume Alpha");
    }
}