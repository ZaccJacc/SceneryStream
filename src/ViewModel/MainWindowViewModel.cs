using SceneryStream.src.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryStream.src.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {

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
                }
                setContentByIndex(0);
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
                }
                setContentByIndex(1);
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
                }
                setContentByIndex(2);
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
                }
                setContentByIndex(3);
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
                }
                setContentByIndex(4);
                NotifyPropertyChanged(nameof(CreditsSelected));
            }
        }

        private object _contentToDisplay = new View.ConnectionView();
        public object? ContentToDisplay
        {
            get => _contentToDisplay;
            set
            {
                _contentToDisplay = value;
                NotifyPropertyChanged(nameof(ContentToDisplay));
            }
        }

        private void setContentByIndex(int index)
        {
            switch (index)
            {
                default:
                case 0:
                    ContentToDisplay = new View.ConnectionView();
                    break;

                case 1:
                    ContentToDisplay = new View.PreferencesView();
                    break;

                case 2:
                    ContentToDisplay = new View.MapView();
                    break;

                case 3:
                    ContentToDisplay = new View.SceneryView();
                    break;

                case 4:
                    ContentToDisplay = new View.CreditsView();
                    break;
            }
        }
    }
}
