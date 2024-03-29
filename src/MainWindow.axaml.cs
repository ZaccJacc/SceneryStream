using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.ComponentModel;
using System.IO;
using Utility;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SceneryStream.src.ViewModel;

namespace SceneryStream.src
{
    public partial class MainWindow : Window
    {

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                NetworkDrive.RemoveDriveByConsole(App.Preferences.DriveLetter);
                File.Delete(App.Preferences.SimDirectory + @"\Custom Scenery\zOrtho_xss_mount.lnk");
                File.Delete(App.Preferences.SimDirectory + @"\Custom Scenery\airports_xss_mount.lnk");
            }
            catch (Exception)
            {
                Console.WriteLine("Shutdown incomplete");
            }
            //WindowState = WindowState.Minimized;
            //e.Cancel = true;
            await PreferencesModel.SavePreferences();
            ApplicationViewModel.ExitApplication();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

    }
}