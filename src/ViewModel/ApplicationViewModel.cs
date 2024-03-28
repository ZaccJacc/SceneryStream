using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SceneryStream.src.Model;

namespace SceneryStream.src.ViewModel
{
    internal class ApplicationViewModel : ObservableObject
    {

        public static void ExitApplication()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime application)
            {
                application.Shutdown();
            }
        }

        public static void ToggleConnection()
        {
            HomeViewModel.HViewModel.ToggleConnection();
        }
    }
}
