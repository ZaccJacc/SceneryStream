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
        public ReactiveCommand<string, string> ProduceWindowsBrowser { get;  }
        //--//

        public PreferencesViewModel()
        {
            TestCommand = ReactiveCommand.Create(testCommand);
            ProduceWindowsBrowser = ReactiveCommand.Create<string, string>(produceWindowsBrowser);
        }

        private void testCommand()
        {
            Console.WriteLine("Hello World!");
        }

        public static void popup(object? sender, RoutedEventArgs e)
        {
            var command = ReactiveCommand.Create(() => Console.WriteLine("ReactiveCommand invoked"));
            FileBrowserView fileBrowser = new FileBrowserView();
            fileBrowser.Show();
        }

        private string produceWindowsBrowser(string callback)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            string? path = openFolderDialog.ShowAsync(new FileBrowserView()).ToString();
            //Find a a way to set the directory field in the view model from here i dont remember how.
            return path;
        }

    }
}
