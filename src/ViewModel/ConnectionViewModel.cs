using SceneryStream.src.Model;
using System;
using Utility;

namespace SceneryStream.src.ViewModel
{
    internal class ConnectionViewModel
    {
        public string? ConnectionAddress
        {
            get
            {
                return App.Preferences.ServerAddress;
            }
            set
            {
                App.Preferences.ServerAddress = value;
            }
        }



        internal async void makeConnection()
        {
            if (App.Preferences.ServerAddress != null && App.Preferences.DriveLetter != null)
            {
                await Windows.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter, 0);
            }
            else
            {
                Console.WriteLine("[!] Cannot make connection without location and drive.");
            }
        }

        internal void ForceConnection()
        {
            NetworkDrive.MapNetworkDrive("X", "\\\\86.141.55.2\\Scenery");
            Console.WriteLine(NetworkDrive.IsDriveMapped("X"));
        }

    }
}

