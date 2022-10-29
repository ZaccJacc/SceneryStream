using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.ObjectModel;

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
    }
}