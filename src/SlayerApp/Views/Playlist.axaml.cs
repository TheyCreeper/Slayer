using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using src.SlayerApp.ViewModel;

namespace src.SlayerApp.Views;

public partial class Playlist : UserControl
{
    public Playlist()
    {
        InitializeComponent();
        DataContext = new LibraryViewModel();
    }
}