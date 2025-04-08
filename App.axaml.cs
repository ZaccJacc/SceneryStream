using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using SceneryStream.src;
using SceneryStream.src.Model;
using SceneryStream.src.ViewModel;
using System;
using System.Diagnostics;
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

        private static TopLevel _systemLevel;
        internal static TopLevel SystemLevel
        {
            get => _systemLevel;
        }

        private static IStorageProvider _storage;
        internal static IStorageProvider Storage
        {
            get => _storage;
        }

        private const string _VERSION = "v1.0.0-rc2";
        public string VERSION
        {
            get => _VERSION;
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
                //--//
                _systemLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                _storage = SystemLevel.StorageProvider;
                //--//
                Task platformbuild = ServiceInstance.BuildServiceAuthenticity();
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                //----//
                await platformbuild;
                
                if (!ServiceInstance.Platform_Verified)
                {
                    Debug.WriteLine("[!] Unable to complete platform verification.");
                }
                if (!ServiceInstance.Connected)
                {
                    Debug.WriteLine("[!] Drive connection unsuccessful.\n\t=> Application initialisation complete.");
                }
                if (ServiceInstance.Connected && ServiceInstance.Platform_Verified)
                {
                    Debug.WriteLine("[#] Automatic initialisation success!");
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}