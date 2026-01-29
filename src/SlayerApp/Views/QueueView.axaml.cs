using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SlayerApp;

public partial class QueueView : UserControl
{
    public QueueView()
    {
        InitializeComponent();
        this.DataContext = App.MediaBar;
    }
}