using SceneryStream.src.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SceneryStream.src.ViewModel
{
    internal class ConnectionViewModel
    {
        public string? ConnectionAddress 
        {
            get
            {
                return Preferences.ServerAddress;
            }
            set
            {
                Preferences.ServerAddress = value;
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

        internal async void makeConnection()
        {
            if(Preferences.ServerAddress!=null&&Preferences.DriveLetter!=null) 
            {
                await Utility.Windows.Local.PerformTargetLocationMounting(Preferences.ServerAddress, Preferences.DriveLetter);
            }
            else
            {
                Console.WriteLine("[!] Cannot make connection without location and drive.");
            }
        }
        
    }
}
