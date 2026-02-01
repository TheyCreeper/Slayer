using Avalonia.Controls;
using SlayerApp.Model;
using SlayerApp.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SlayerApp;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        MediaBarControl.DataContext = App.MediaBar;

        
    }

    private void HideUserControls()
    {
        LibraryView.IsVisible = false;
        SettingsView.IsVisible = false;
    }

    private void NavigationButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Button button = (Button)sender;
        HideUserControls();
        switch (button.Name)
        {
            case "Home":
                LibraryView.IsVisible = true;
                break;
            case "Library":
                LibraryView.IsVisible = true;
                break;
            case "Settings":
                SettingsView.IsVisible = true;
                break;
            default:
                //CurrentPlaylist = new PlaylistViewModel(NavBarItem
                //    .Where(x => x.Name == button.Name)
                //    .First());
                break;
        }
    }
}