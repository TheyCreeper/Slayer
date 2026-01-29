using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using src.SlayerApp.ViewModel;

namespace SlayerApp;

public partial class LibraryView : UserControl
{
    public LibraryView()
    {
        InitializeComponent();
        DataContext = new LibraryViewModel();
    }
}