using Avalonia.Interactivity;
using ReactiveUI;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SceneryStream.src.ViewModel
{
    internal class PreferencesViewModel
    {


        public string Platform
        {
            get { return App.ServiceInstance.Platform.ToString(); }
        }

        public PreferencesModel Preferences = App.Preferences;

        public PreferencesViewModel() { }

        //-//
        public static void Popup(object? sender, RoutedEventArgs e)
        {
            var command = ReactiveCommand.Create(() => Console.WriteLine("ReactiveCommand invoked"));
            FileBrowserView fileBrowser = new FileBrowserView();
            fileBrowser.Show();
        }

        public async void LoadPreferences()
        {
            string prefFile = (await Utility.FileBrowser.produceBrowser("File")).ToString();
            if (prefFile != "" && prefFile != null)
            {
                PreferencesModel.loadPreferences(prefFile);
            }
        }

        public async void SelectSimDirectory(string install_type) //this needs to eventually check if this is for the main sim directory or for other installations
        {
            App.Preferences.SimDirectory = (await Utility.FileBrowser.produceBrowser("Directory")).ToString();
        }

        public async void ResetPreferences()
        {
            await Task.Run(() =>
            {
                App.Preferences.ServerAddress = null;
                App.Preferences.MultipleSims = false;
                App.Preferences.DriveLetter = "A";
                App.Preferences.SimDirectory = null;
            });
        }

        public async void SavePreferences()
        {
            await PreferencesModel.SavePreferences();
        }

    }
}
