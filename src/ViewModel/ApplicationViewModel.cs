using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SceneryStream.src.Model;
using Utility;

namespace SceneryStream.src.ViewModel
{
    internal class ApplicationViewModel : ObservableObject
    {

        public static async void ExitApplication()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
            {
                try
                {
                    NetworkDrive.RemoveDriveByConsole(App.Preferences.DriveLetter);
                    RegionHandling.Regions.RemoveShellLinks();
                }
                catch (Exception)
                {
                    Debug.WriteLine("Shutdown incomplete");
                }
                await PreferencesModel.SavePreferences();
                RegionHandling.Regions.SaveSelectedRegions();
                application.Shutdown();
            }
        }

        public static void ToggleConnection()
        {
            PreferencesViewModel.PViewModel.ToggleConnection();
        }

        public void OpenWindow()
        {
            if(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application){
                application.MainWindow.Show();
            }

        }
    }
}
