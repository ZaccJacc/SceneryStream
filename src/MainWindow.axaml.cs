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
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void FlyoutHandle_Pointer(object? sender, PointerWheelEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Control);
        }

        public void buttonTest(object? sender, RoutedEventArgs args)
        {
            Console.WriteLine("Button pressed");
        }

        /*
         * Once the connections page has been completed, or at least the design for it, begin to program the backend to make test connections and receive a hosted piece of data as a handshake gesture
         * of sorts... this kind of thing might take a while now because the tesing is reliant on the whole app running at once, but testing versions of the code will have to be built in then re-written
         * before the actual service goes out. Try to keep the only thing being changed before release the location of inserting values (e.g. a backend constant rather than typing the actual address)
         * but make sure the premise works first.
         * 
         */
    }
}