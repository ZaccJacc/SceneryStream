using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.ObjectModel;
using SceneryStream.src;
using System.Xml.Linq;
using System.IO;
using SceneryStream.src.View;
using System.ComponentModel;
using SceneryStream.src.Model;
using System;
using System.Diagnostics;

namespace SceneryStream.src
{
    public partial class MainWindow : Window
    {

        protected override async void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Preferences.SimDirectory != null && Preferences.ServerAddress != null && Preferences.DriveLetter != null)
            {
                await PreferencesModel.savePreferences();
            }
            else
            {
                Console.WriteLine("[!] Preferences file incomplete - will not autosave.");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
        }

        public void FlyoutHandle_Pointer(object? sender, PointerWheelEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Control);
        }

        public void ThrowCredits(object? sender, PointerPressedEventArgs e)
        {
            CreditsWindow credits = new CreditsWindow();
            credits.Show();
        }


        //Platform-dependent operations below - Here be dragons!

        /*
         * Once the connections page has been completed, or at least the design for it, begin to program the backend to make test connections and receive a hosted piece of data as a handshake gesture
         * of sorts... this kind of thing might take a while now because the tesing is reliant on the whole app running at once, but testing versions of the code will have to be built in then re-written
         * before the actual service goes out. Try to keep the only thing being changed before release the location of inserting values (e.g. a backend constant rather than typing the actual address)
         * but make sure the premise works first.
         * 
         * TODO:
         *      Program an effective mounting method (Test what has already been made in the localmachine section) and attempt to interface through this file and the primary UI logic to make the magic happen.
         *      DON'T FORGET THAT YOU NEED TO AWAIT THE RESPONSE FROM THE PLATFORM CHECK!!!!!!
         */
    }
}