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
