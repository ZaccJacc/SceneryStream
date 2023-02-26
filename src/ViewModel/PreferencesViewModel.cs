using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ReactiveUI;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SceneryStream.src.ViewModel
{
    internal class PreferencesViewModel
    {
        
        public string? PreferencesFile { get; set; }
        private string? _preferencesFile;

        public string? SimDirectory { get; set; }
        private string? _simDirectory;

        //--//
        public ReactiveCommand<Unit, Unit> TestCommand { get; }
        //--//

        public PreferencesViewModel()
        {
            TestCommand = ReactiveCommand.Create(_testCommand);
        }

        void _testCommand()
        {
            Console.WriteLine("Hello World!");
        }

        public static void popup(object? sender, RoutedEventArgs e)
        {
            var command = ReactiveCommand.Create(() => Console.WriteLine("ReactiveCommand invoked"));
            FileBrowserView fileBrowser = new FileBrowserView();
            fileBrowser.Show();
        }

    }
}
