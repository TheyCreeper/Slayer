using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayerApp.ViewModel
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _pathToAdd;


        public SettingsViewModel() { }

        [RelayCommand]
        private void AddToPathList()
        {
            if (!string.IsNullOrEmpty(PathToAdd))  // Changed from IsNullOrEmpty to !IsNullOrEmpty
            {
                App.Database.files.AddLocation(PathToAdd);
            }
        }
    }
}
