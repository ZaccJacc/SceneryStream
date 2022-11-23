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


namespace SceneryStream
{
    public partial class MainWindow : Window
    {
        ObservableCollection<string> paths; /*Observable Collections send a notification when their contents are updated. This means 
                                             * objects that don't read through the list but instead use the whole thing as a source (ListBoxes) 
                                             * have to use this, because they need to be triggered to update the whole collection*/
        ObservableCollection<string> scenery_paths;

        public MainWindow()
        {
            InitializeComponent();
            paths = new ObservableCollection<string>();
            scenery_paths = new ObservableCollection<string>();
        }


        public void FlyoutHandle_Pointer(object? sender, PointerWheelEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Control);
        }

        public void UsingMoreInstallations(object? sender, RoutedEventArgs args)
        {
            switch (((CheckBox)sender).Tag.ToString())
            {
                case "0":
                    OtherInstallationField.IsVisible = true;
                    BrowseCustom.IsVisible = true;
                    AddDirectory.IsVisible = true;
                    if ((string)OtherInstallationList.Tag == "1")
                    {
                        OtherDirectoryList.IsVisible = true;
                    }
                    ((CheckBox)sender).Tag = 1;
                    break;
                case "1":
                    OtherInstallationField.IsVisible = false;
                    BrowseCustom.IsVisible = false;
                    OtherInstallationList.IsVisible = false;
                    AddDirectory.IsVisible = false;
                    ((CheckBox)sender).Tag = 0;
                    break;

                    default:
                    break;

            }
            
        }

        public void LogCustomInstallationDirectory(object? sender, RoutedEventArgs args)
        {
            if ((string)OtherInstallationList.Tag == "0")
            {
                OtherInstallationList.Tag = "1";
                OtherInstallationList.IsVisible = true;
            }
            paths.Add(OtherInstallationField.Text);
            OtherInstallationList.Items = paths;
        }

        public void RecallPathSelection(object? sender, SelectionChangedEventArgs e)
        {
            OtherInstallationField.Text = ((ListBox)sender).SelectedItem as string;
        }

        public void HandleBrowser(object? sender, RoutedEventArgs e)
        {
            SimDirectory.Text = ClientSize.Height.ToString() + " " + ClientSize.Width.ToString();
        }

        public void ThrowCredits(object? sender, PointerPressedEventArgs e)
        {
            CreditsWindow credits = new CreditsWindow();
            credits.Show();
        }

        public void UsingCustomLocations(object? sender, RoutedEventArgs args)
        {
            switch (((CheckBox)sender).Tag.ToString())
            {
                case "0":
                    OtherDirectoryField.IsVisible = true;
                    BrowseCustomScenery.IsVisible = true;
                    AddDirectoryScenery.IsVisible = true;
                    if ((string)OtherDirectoryList.Tag == "1")
                    {
                        OtherDirectoryList.IsVisible = true;
                    }
                    ((CheckBox)sender).Tag = 1;
                    break;
                case "1":
                    OtherDirectoryField.IsVisible = false;
                    BrowseCustomScenery.IsVisible = false;
                    OtherDirectoryList.IsVisible = false;
                    AddDirectoryScenery.IsVisible = false;
                    ((CheckBox)sender).Tag = 0;
                    break;

                default:
                    break;

            }

        }

        public void LogCustomSceneryDirectory(object? sender, RoutedEventArgs args)
        {
            if ((string)OtherDirectoryList.Tag == "0")
            {
                OtherDirectoryList.Tag = "1";
                OtherDirectoryList.IsVisible = true;
            }
            scenery_paths.Add(OtherDirectoryField.Text);
            OtherDirectoryList.Items = scenery_paths;
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