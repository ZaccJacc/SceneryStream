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
                Console.WriteLine(Preferences.ServerAddress);
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
                Console.WriteLine(Preferences.DriveLetter);
            }
        }
        
    }
}
