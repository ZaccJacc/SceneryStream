using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace SceneryStream.src.View
{
    public partial class PreferencesView : UserControl
    {



        public PreferencesView()
        {
            InitializeComponent();


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
                    OtherInstallationCheck.IsChecked = true;
                    break;

                default:
                    OtherInstallationField.IsVisible = false;
                    BrowseCustom.IsVisible = false;
                    OtherInstallationList.IsVisible = false;
                    AddDirectory.IsVisible = false;
                    OtherInstallationCheck.Tag = 0;
                    OtherInstallationCheck.IsChecked = false;
                    break;
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

    }
}
