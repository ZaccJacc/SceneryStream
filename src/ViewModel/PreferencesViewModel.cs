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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SceneryStream.src.ViewModel
{
    internal class PreferencesViewModel : INotifyPropertyChanged
    {
        
        public string? PreferencesFile { get; set; }
        private string? _preferencesFile;

        public string? SimDirectory { get; set; }
        private string? _simDirectory;

        private string bindingTest = "hello world";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Platform
        {
            get { return App.ServiceInstance.Platform.ToString(); }
        }

        //--//
        public ReactiveCommand<Unit, Unit> TestCommand { get; }
        public ReactiveCommand<string, Unit> ProduceBrowser { get;  }
        //--//

        public PreferencesViewModel()
        {
            TestCommand = ReactiveCommand.Create(testCommand);
            ProduceBrowser = ReactiveCommand.CreateFromTask<string>(produceBrowser);
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

        private async Task produceBrowser(string callback)
        {
            switch (callback)
            {
                case "ConfigFile":
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Title = "Select Application Configuration File";
                    string[] filePath = await fileDialog.ShowAsync(new Window());
                    try
                    {
                        foreach (string s in filePath) //this doesn't work because the browser keeps returning a null path before something has been chosen
                        {
                            _preferencesFile += s;
                        }
                        break;
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("No path selected");
                        break;
                    }
                    

                case "SimDirectory":
                    OpenFolderDialog simDialog = new OpenFolderDialog();
                    _simDirectory = await simDialog.ShowAsync(new Window());
                    break;
            }
        }

    }
}
