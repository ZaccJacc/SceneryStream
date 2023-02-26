using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SceneryStream.src.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace SceneryStream.src.View
{
    public partial class PreferencesView : UserControl
    {

        ObservableCollection<string> paths; /*Observable Collections send a notification when their contents are updated. This means 
                                             * objects that don't read through the list but instead use the whole thing as a source (ListBoxes) 
                                             * have to use this, because they need to be triggered to update the whole collection*/
        ObservableCollection<string> scenery_paths;


        public PreferencesView()
        {
            InitializeComponent();
            paths = new ObservableCollection<string>();
            scenery_paths = new ObservableCollection<string>();
        }

        public void loadPreferences(object? sender, RoutedEventArgs args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory(), ((Button)sender).Tag.ToString());
            _ = new Model.PreferencesModel().loadPreferences(Path.Combine(Directory.GetCurrentDirectory(), ((Button)sender).Tag.ToString()));
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

        /// <summary>
        /// Create a new select folder dialog
        /// </summary>
        /// <returns>A string representing the absolute path of the target directory</returns>
        public string HandleBrowser(object? sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            string? path = openFolderDialog.ShowAsync(new FileBrowserView()).ToString();
            //Find a a way to set the directory field in the view model from here i dont remember how.
            return path;
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
