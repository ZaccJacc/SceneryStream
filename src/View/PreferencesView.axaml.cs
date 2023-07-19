using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace SceneryStream.src.View
{
    public partial class PreferencesView : UserControl
    {

        ObservableCollection<string> paths; /*Observable Collections send a notification when their contents are updated. This means 
                                             * objects that don't read through the list but instead use the whole thing as a source (ListBoxes) 
                                             * have to use this, because they need to be triggered to update the whole collection*/
        ObservableCollection<string> scenery_paths; //"OtherDirectoryList"


        public PreferencesView()
        {
            InitializeComponent();
            paths = new ObservableCollection<string>();
            scenery_paths = new ObservableCollection<string>();

            //code constructing a contextmenu so it can be assigned an event listener in backend
            ContextMenu DeleteItemMenu = new ContextMenu();
            string[] delete = { "Remove" };
            DeleteItemMenu.Items = delete;
            DeleteItemMenu.PointerPressed += DeleteItemMenu_PointerPressed;
            OtherDirectoryList.ContextMenu = DeleteItemMenu;
            OtherInstallationList.ContextMenu = DeleteItemMenu;

            //getting the checkboxes into their correct state based on the loadeed preferences - currently all this happens before the preferences get read... somehow need to find a way to fix this.
            //could do a re-check when the window comes into focus / is selected?
            switch(App.Preferences.MultipleSims)
            {
                case true:
                    OtherInstallationField.IsVisible = true;
                    BrowseCustom.IsVisible = true;
                    AddDirectory.IsVisible = true;
                    if ((string)OtherInstallationList.Tag == "1")
                    {
                        OtherDirectoryList.IsVisible = true;
                    }
                    OtherInstallationCheck.Tag = 1;
                    break;

                default:
                    OtherInstallationField.IsVisible = false;
                    BrowseCustom.IsVisible = false;
                    OtherInstallationList.IsVisible = false;
                    AddDirectory.IsVisible = false;
                    OtherInstallationCheck.Tag = 0;
                    break;
            }
        }

        private void DeleteItemMenu_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            //((ContextMenu)sender)
            if(OtherDirectoryList.SelectedItem != null)
            {
                scenery_paths.Remove(OtherDirectoryList.SelectedItem.ToString());
            }
            else
            {
                if(OtherInstallationList.SelectedItem != null)
                {
                    paths.Remove(OtherInstallationList.SelectedItem.ToString());
                }
            }
            
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
                    App.Preferences.MultipleSims = true;
                    break;
                case "1":
                    OtherInstallationField.IsVisible = false;
                    BrowseCustom.IsVisible = false;
                    OtherInstallationList.IsVisible = false;
                    AddDirectory.IsVisible = false;
                    ((CheckBox)sender).Tag = 0;
                    App.Preferences.MultipleSims = false;
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
            OtherInstallationField.Text = string.Empty;
        }

        public void RecallPathSelection(object? sender, SelectionChangedEventArgs e)
        {
            switch(((ListBox)sender).Name){
                case "OtherInstallationList":
                    OtherInstallationField.Text = ((ListBox)sender).SelectedItem as string;
                    break;

                case "OtherDirectoryList":
                    OtherDirectoryField.Text= ((ListBox)sender).SelectedItem as string;
                    break;
            }
            
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
            OtherDirectoryField.Text = string.Empty;
        }

        public void SubmitKeyHandler(object? sender, KeyEventArgs args)
        {
            if(args.Key == Key.Return)
            {
                switch (((TextBox)sender).Name)
                {
                    case "OtherDirectoryField":
                        LogCustomSceneryDirectory(sender, args); 
                        break;

                    case "OtherInstallationField":
                        LogCustomInstallationDirectory(sender, args); 
                        break;
                }
            }
        }
    }
}
