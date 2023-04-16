using SceneryStream.src.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Utility;

namespace SceneryStream.src.ViewModel
{
    internal class ConnectionViewModel : ObservableObject
    {

        private static ConnectionViewModel _cViewModel = new ConnectionViewModel();
        public static ConnectionViewModel cViewModel
        {
            get => _cViewModel;
        }

        private string? _updateText;
        public string? UpdateText
        {
            get
            {
                return string.IsNullOrEmpty(_updateText) ? "Placeholder updates - this box will contain information about the updates to the servers and the app." : _updateText;
            }
            set
            {
                _updateText = value;
                NotifyPropertyChanged(nameof(UpdateText));
            }
        }


        internal async void makeConnection()
        {
            if (App.Preferences.ServerAddress != null && App.Preferences.DriveLetter != null)
            {
                await Windows.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter, 0);
                cViewModel.GatherUpdateInformation();
            }
            else
            {
                Console.WriteLine("[!] Cannot make connection without location and drive.");
            }
        }

        internal async void GatherUpdateInformation()
        {
            await Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("[*] Attempting to read server updates info");
                    cViewModel.UpdateText = File.ReadAllText(App.Preferences.ServerAddress + @"\ServerUpdates");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Could not load preferences!\n\t=> {ex.Message}");
                }
            });
        }

    }
}

