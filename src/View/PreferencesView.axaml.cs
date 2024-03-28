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
