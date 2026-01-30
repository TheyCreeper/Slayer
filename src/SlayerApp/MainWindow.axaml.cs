using Avalonia.Controls;

namespace SlayerApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MediaBarControl.DataContext = App.MediaBar;
    }

    private void HideUserControls()
    {
        LibraryView.IsVisible = false;
        SettingsView.IsVisible = false;
    }

    public void AddSongToQueue()
    {
        App.MediaBar.AddSingleToQueue(this);
        if (!App.MediaBar.IsQueueVisible) App.MediaBar.IsQueueVisible = true;
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
        }
    }
}