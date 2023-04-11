using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SceneryStream.src;
using SceneryStream.src.Model;
using System;
using System.Threading.Tasks;

namespace SceneryStream
{
    public partial class App : Application
    {
        public static LocalMachine ServiceInstance = new LocalMachine();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Task platformbuild = ServiceInstance.BuildServiceAuthenticity();
                desktop.MainWindow = new MainWindow();
                //----//
                await platformbuild;
                if (!ServiceInstance.Platform_Verified)
                {
                    Console.WriteLine("[!] Unable to complete platform verification.");
                }
                if (!ServiceInstance.Connected)
                {
                    Console.WriteLine("[!] Drive connection unsuccessful.");
                }
                if (ServiceInstance.Connected && ServiceInstance.Platform_Verified)
                {
                    Console.WriteLine("[#] Automatic initialisation success!");
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}