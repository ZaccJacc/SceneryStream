using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using SceneryStream.src.Model;

namespace SceneryStream.src.ViewModel
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        //--Window Views--//
        private static readonly View.HomeView homeView = new();
        private static readonly View.PreferencesView preferencesView = new();
        private static readonly View.MapView mapView = new();
        private static readonly View.SceneryView sceneryView = new();
        private static readonly View.CreditsView creditsView = new();
        private static readonly View.ServerFormattingView serverFormattingView = new();
        //--//

        [RelayCommand]
        private static void CloseMainWindow(object? window) => ((Window)window).Close();

        [RelayCommand]
        private static void MaximiseMainWindow(object? window) => ((Window)window).WindowState = ((Window)window).WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

        [RelayCommand]
        private static void MinimiseMainWindow(object? window) => ((Window)window).WindowState = WindowState.Minimized;

        private bool _homePageSelected = true;
        public bool HomePageSelected
        {
            get => _homePageSelected;
            set
            {
                _homePageSelected = value;
                if(value != false)
                {
                    PreferencesSelected = false;
                    MapSelected = false;
                    SceneryViewSelected = false;
                    CreditsSelected = false;
                    ConfigSelected = false;
                }
                SetContentByIndex(0);
                NotifyPropertyChanged(nameof(HomePageSelected));
            }
        }

        private bool _preferencesSelected;
        public bool PreferencesSelected
        {
            get => _preferencesSelected;
            set
            {
                if(_preferencesSelected == true != value)
                {
                    _preferencesSelected = value;
                    HomePageSelected = true;
                }
                _preferencesSelected = value;
                if (value != false)
                {
                    HomePageSelected = false;
                    MapSelected = false;
                    SceneryViewSelected = false;
                    CreditsSelected = false;
                    ConfigSelected = false;
                    SetContentByIndex(1);
                }
                NotifyPropertyChanged(nameof(PreferencesSelected));
            }
        }

        private bool _mapSelected;
        public bool MapSelected
        {
            get => _mapSelected;
            set
            {
                if (_mapSelected == true != value)
                {
                    _mapSelected = value;
                    HomePageSelected = true;
                }
                _mapSelected = value;
                if(value != false)
                {
                    HomePageSelected = false;
                    PreferencesSelected = false;
                    SceneryViewSelected =false;
                    CreditsSelected = false;
                    ConfigSelected = false;
                    SetContentByIndex(2);
                }
                NotifyPropertyChanged(nameof(MapSelected));
            }
        }

        private bool _sceneryViewSelected;
        public bool SceneryViewSelected
        {
            get => _sceneryViewSelected;
            set
            {
                if (_sceneryViewSelected == true != value)
                {
                    _sceneryViewSelected = value;
                    HomePageSelected = true;
                }
                _sceneryViewSelected = value;
                if (value != false)
                {
                    HomePageSelected = false;
                    PreferencesSelected = false;
                    MapSelected = false;
                    CreditsSelected = false;
                    ConfigSelected = false;
                    SetContentByIndex(3);
                }
                NotifyPropertyChanged(nameof(SceneryViewSelected));
            }
        }

        private bool _creditsSelected;
        public bool CreditsSelected
        {
            get => _creditsSelected;
            set
            {
                if (_creditsSelected == true != value)
                {
                    _creditsSelected = value;
                    HomePageSelected = true;
                }
                _creditsSelected = value;
                if(value != false)
                {
                    HomePageSelected = false;
                    PreferencesSelected = false;
                    MapSelected = false;
                    SceneryViewSelected = false;
                    ConfigSelected = false;
                    SetContentByIndex(4);
                }
                NotifyPropertyChanged(nameof(CreditsSelected));
            }
        }

        private bool _configSelected;
        public bool ConfigSelected
        {
            get => _configSelected;
            set
            {
                if (_configSelected == true != value)
                {
                    _configSelected = value;
                    HomePageSelected = true;
                }
                _configSelected = value;
                if(value != false)
                {
                    HomePageSelected = false;
                    PreferencesSelected = false;
                    MapSelected = false;
                    SceneryViewSelected = false;
                    CreditsSelected = false;
                    SetContentByIndex(5);
                }
                NotifyPropertyChanged(nameof(ConfigSelected));
            }
        }

        private object? _contentToDisplay = homeView;
        public object? ContentToDisplay
        {
            get => _contentToDisplay;
            set
            {
                _contentToDisplay = value;
                NotifyPropertyChanged(nameof(ContentToDisplay));
            }
        }

       

        private void SetContentByIndex(int index)
        {

            switch (index)
            {
                default:
                case 0:
                    ContentToDisplay = homeView;
                    break;

                case 1:
                    ContentToDisplay = preferencesView;
                    break;

                case 2:
                    ContentToDisplay = mapView;
                    break;

                case 3:
                    ContentToDisplay = sceneryView;
                    break;

                case 4:
                    ContentToDisplay = creditsView;
                    break;

                case 5:
                    ContentToDisplay = serverFormattingView;
                    break;
            }
        }
    }
}
