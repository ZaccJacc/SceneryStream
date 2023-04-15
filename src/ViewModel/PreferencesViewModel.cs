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
    internal class PreferencesViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Platform
        {
            get { return App.ServiceInstance.Platform.ToString(); }
        }

        public PreferencesViewModel() { }

        //-//
        public string? SimDirectory
        {
            get
            {
                return App.Preferences.SimDirectory;
            }
            set
            {
                App.Preferences.SimDirectory = value;
                NotifyPropertyChanged();
            }
        }

        public int? DriveIndex
        {
            get
            {
                if (App.Preferences.DriveLetter != null)
                {
                    return App.Preferences.DriveLetter[0] - 65;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                App.Preferences.DriveLetter = ((char)(value + 65)).ToString();
                NotifyPropertyChanged();
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

        public async void selectSimDirectory(string install_type) //this needs to eventually check if this is for the main sim directory or for other installations
        {
            SimDirectory = (await Utility.FileBrowser.produceBrowser("Directory")).ToString();
        }

        public async void resetPreferences()
        {
            await Task.Run(() =>
            {
                App.Preferences.ServerAddress = null;
                App.Preferences.MultipleSims = false;
                App.Preferences.DriveLetter = null;
                App.Preferences.SimDirectory = null;
            });
        }
    }
}
