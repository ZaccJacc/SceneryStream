using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SceneryStream.src;
using SceneryStream.src.Model;
using SceneryStream.src.ViewModel;
using System;
using System.Threading.Tasks;

namespace SceneryStream
{
    public partial class App : Application
    {
        private static LocalMachine _serviceInstance = new();
        internal static LocalMachine ServiceInstance
        {
            get { return _serviceInstance; }
        }
        private static PreferencesModel _preferences = new();
        internal static PreferencesModel Preferences
        {
            get { return _preferences; }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                Task platformbuild = ServiceInstance.BuildServiceAuthenticity();
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                //----//
                await platformbuild;
                
                if (!ServiceInstance.Platform_Verified)
                {
                    Console.WriteLine("[!] Unable to complete platform verification.");
                }
                if (!ServiceInstance.Connected)
                {
                    Console.WriteLine("[!] Drive connection unsuccessful.\n\t=> Application initialisation complete.");
                }
                if (ServiceInstance.Connected && ServiceInstance.Platform_Verified)
                {
                    Console.WriteLine("[#] Automatic initialisation success!");
                }
                /*
                Preferences.PreferencesFile = File.ReadAllText("Targets.Setup");
                Console.WriteLine("[*] Preferences file found.");
                PreferencesModel.loadPreferences(Preferences.PreferencesFile);*/
                HomeViewModel.HViewModel.ScanNewUpdates();
                HomeViewModel.HViewModel.RefreshScenerySpotlight();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void QuitAppTrayCall(object? sender, RoutedEventArgs args)
        {
            Console.WriteLine("Feature not yet implemented.");
        }
    }
}