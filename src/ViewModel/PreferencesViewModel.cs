using Avalonia.Interactivity;
using ReactiveUI;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SceneryStream.src.ViewModel
{
    internal class PreferencesViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Platform
        {
            get { return App.ServiceInstance.Platform.ToString(); }
        }

        public PreferencesViewModel() { }

        public string? SimDirectory
        {
            get
            {
                return Preferences.SimDirectory;
            }
            set
            {
                Preferences.SimDirectory = value;
                NotifyPropertyChanged();
            }
        }

        public int? DriveIndex
        {
            get
            {
                if (Preferences.DriveLetter != null)
                {
                    return Preferences.DriveLetter[0] - 65;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Preferences.DriveLetter = ((char)(value + 65)).ToString();
            }
        }

        //-//
        public static void popup(object? sender, RoutedEventArgs e)
        {
            var command = ReactiveCommand.Create(() => Console.WriteLine("ReactiveCommand invoked"));
            FileBrowserView fileBrowser = new FileBrowserView();
            fileBrowser.Show();
        }

        public async void loadPreferences()
        {
            string prefFile = (await Utility.FileBrowser.produceBrowser("File")).ToString();
            if (prefFile != "" && prefFile != null)
            {
                PreferencesModel.loadPreferences(prefFile);
            }
        }

        public async void selectSimDirectory(string install_type)
        {

                SimDirectory = (await Utility.FileBrowser.produceBrowser("Directory")).ToString();
          
            
        }
    }
}
