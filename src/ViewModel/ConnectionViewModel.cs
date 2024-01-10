using SceneryStream.src.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Utility;
using Avalonia.Media.Imaging;
using System.Threading;

namespace SceneryStream.src.ViewModel
{
    internal class ConnectionViewModel : ObservableObject
    {

        private static ConnectionViewModel _cViewModel = new();
        public static ConnectionViewModel CViewModel
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

        private Bitmap? _source = new($@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Assets/Status/Connecting_Circle.png");
        public Bitmap? Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged(nameof(Source));
            }
        }

        internal void ReviewConnecionStatusIndicator()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            switch (App.ServiceInstance.Connected)
            {
                case true:
                    Source = new Bitmap(@$"{projectDirectory}/Assets/Status/Connected_Circle.png");
                    break;
                case false:
                    Source = new Bitmap(@$"{projectDirectory}/Assets/Status/Disconnected_Circle.png");
                    break;
            }
        }

        internal void ToggleConnection(object? sender, PointerPressedEventArgs args)
        {
            Console.WriteLine("[*] Connection Manually Triggered");
            switch (App.ServiceInstance.Connected)
            {
                case true:
                    NetworkDrive.RemoveDriveByConsole(App.Preferences.DriveLetter);
                    break;

                case false:
                    if (!string.IsNullOrEmpty(App.Preferences.ServerAddress))
                    {
                        Source = new Bitmap(@$"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Assets/Status/Connecting_Circle.png");
                        makeConnection();
                    }
                    break;
            }
        }

        internal async void makeConnection()
        {
            if (App.Preferences.ServerAddress != null && App.Preferences.DriveLetter != null)
            {
                new Thread(async() =>
                {
                    App.ServiceInstance.Connected = await Windows.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter, 0);
                    CViewModel.GatherUpdateInformation();
                }).Start();
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
                Console.WriteLine("[*] Attempting to read server updates info");
                try
                {
                    switch (App.ServiceInstance.Platform.ToString())
                    {
                        case "Win32NT":
                            CViewModel.UpdateText = File.ReadAllText(App.Preferences.ServerAddress + @"\ServerUpdates");
                            break;

                        case "Unix":
                            CViewModel.UpdateText = File.ReadAllText($"~/mnt/{App.Preferences.ServerAddress}/ServerUpdates");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Could not load preferences!\n\t=> {ex.Message}");
                }
            });
        }
    }
}

