using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SlayerApp.ViewModel;

namespace SlayerApp;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }
}