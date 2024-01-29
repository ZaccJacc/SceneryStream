using Avalonia.Interactivity;
using ReactiveUI;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace SceneryStream.src.ViewModel
{
    internal class PreferencesViewModel : ObservableObject
    {

        public PreferencesViewModel() { }

        private static readonly PreferencesViewModel _pViewModel = new();
        public static PreferencesViewModel PViewModel
        {
            get => _pViewModel;
        }


        private bool _installationListVisible = false;
        public bool InstallationListVisible
        {
            set
            {
                _installationListVisible = value;
                NotifyPropertyChanged(nameof(InstallationListVisible));
            }
            get
            {
                return _installationListVisible;
            }
        }

        private string? _installationToAdd;
        public string InstallationToAdd
        {
            get => _installationToAdd ?? string.Empty;
            set
            {
                _installationToAdd= value;
                NotifyPropertyChanged(nameof(InstallationToAdd));
            }
        }

        private string? _selectedExtraInstallationItem;
        public string SelectedExtraInstallationItem
        {
            get => _selectedExtraInstallationItem ?? string.Empty;
            set
            {
                _selectedExtraInstallationItem = value;
                NotifyPropertyChanged(nameof(SelectedExtraInstallationItem));
            }
        }

        
        public PreferencesModel Preferences = App.Preferences;
    
        private ObservableCollection<string> _installationList = new ObservableCollection<string>();
        public ObservableCollection<string> InstallationPathsCollection
        {
            get => _installationList;
            set
            {
                _installationList = value;
                NotifyPropertyChanged(nameof(InstallationPathsCollection));
            }
        }

        private ObservableCollection<string> _sceneryList = new ObservableCollection<string>();
        public ObservableCollection<string> SceneryPathsCollection
        {
            get => _sceneryList;
            set
            {
                _sceneryList = value;
                NotifyPropertyChanged(nameof(SceneryPathsCollection));
            }
        }

        //-//

        public async void LoadPreferences()
        {
            string? prefFile = (await Utility.FileBrowser.produceBrowser("File")).ToString();
            if (prefFile != "" && prefFile != null)
            {
                PreferencesModel.loadPreferences(prefFile);
            }
        }

        public async void SelectSimDirectory(string install_type) //this needs to eventually check if this is for the main sim directory or for other installations
        {
            string? directory = (await Utility.FileBrowser.produceBrowser("Directory")).ToString();
            App.Preferences.SimDirectory = directory != "" ? directory : App.Preferences.SimDirectory;
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

        public void LogInstallationDirectory()
        {
            PViewModel.InstallationListVisible = true;
            PViewModel.InstallationPathsCollection.Add(InstallationToAdd);
            InstallationToAdd = string.Empty;
        }

        public void RemoveExtraInstallation(object? item)
        {
            PViewModel.InstallationPathsCollection.Remove((string)item);
            if(PViewModel.InstallationPathsCollection.Count < 1)
            {
                PViewModel.InstallationListVisible = false;
            }
        }
    }
}
