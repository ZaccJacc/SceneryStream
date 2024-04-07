using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SceneryStream.src.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SceneryStream.src.ViewModel
{
    internal partial class ServerFormattingViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private static readonly ServerFormattingViewModel _sFViewModel = new();
        public static ServerFormattingViewModel SFViewModel
        {
            get { return _sFViewModel; }
        }

        [ObservableProperty]
        private string _serverRootPath = "";

        [ObservableProperty]
        private string _serverID = "";
        
        [ObservableProperty]
        private string _serverLocation = "";
        
        [ObservableProperty]
        private string _serverProvider = "";

        [ObservableProperty]
        private ObservableCollection<SceneryItem> _orthoList = new();

        [ObservableProperty]
        private ObservableCollection<SceneryItem> _airportList = new();

        [ObservableProperty]
        private ObservableCollection<SceneryItem> _libraryList = new();

        //--Scenery Item Creation Dialog--//
        [ObservableProperty]
        private bool _sceneryItemDialogOpen = false;

        [ObservableProperty]
        private string _titleToAdd = "";

        [ObservableProperty]
        private string _sceneryIDToAdd = "";

        [ObservableProperty]
        private string _developerToAdd = "";

        [ObservableProperty]
        private string _descriptionToAdd = "";

        [ObservableProperty]
        private string _regionIDToAdd = "";

        [ObservableProperty]
        private string _parentIDToAdd = "";

        [ObservableProperty]
        private string _ICAOToAdd = "";

        [ObservableProperty]
        private string _pathToAdd = "";

        [ObservableProperty]
        private string _typeToAdd = "";

        //--//
        [RelayCommand]
        private static void LoadConfiguration(object? param) => ServerFormat.Format.LoadServerConfiguration(param);

        [RelayCommand]
        private static void ToggleDialog() => SFViewModel.SceneryItemDialogOpen ^= true;
        //--//

        internal async void SelectServerDirectory()
        {
            SFViewModel.ServerRootPath = (await Utility.FileBrowser.produceBrowser("directory")).ToString();
        }

        internal async void CreateNewSceneryItem(object? param)
        {
            if(SFViewModel.SceneryItemDialogOpen== false)
            {
                switch (param.ToString().ToLower())
                {
                    case "ortho":
                        IStorageFolder orthoItem = (await App.Storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                        {
                            Title = "Select Target Path for Ortho Item",
                            AllowMultiple = false,
                        })).ElementAt(0);
                        SFViewModel.SceneryItemDialogOpen = true;
                        SFViewModel.PathToAdd = orthoItem.TryGetLocalPath();
                        SFViewModel.TypeToAdd = "Ortho";
                        break;

                    case "airport":
                        IStorageFolder airportItem = (await App.Storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                        {
                            Title = "Select Target Path for Ortho Item",
                            AllowMultiple = false,
                        })).ElementAt(0);
                        SFViewModel.SceneryItemDialogOpen = true;
                        SFViewModel.PathToAdd = airportItem.TryGetLocalPath();
                        SFViewModel.TypeToAdd = "Airport";
                        break;

                    case "library":
                        IStorageFolder libraryItem = (await App.Storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                        {
                            Title = "Select Target Path for Ortho Item",
                            AllowMultiple = false,
                        })).ElementAt(0);
                        SFViewModel.SceneryItemDialogOpen = true;
                        SFViewModel.PathToAdd = libraryItem.TryGetLocalPath();
                        SFViewModel.TypeToAdd = "Library";
                        break;
                }
            }
            
        }

        internal void CreateFromDialog()
        {
            
            try
            {
                SceneryItem item = new(SFViewModel.TypeToAdd, SFViewModel.SceneryIDToAdd, (ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), SFViewModel.RegionIDToAdd), (Region.RegionID)Enum.Parse(typeof(Region.RegionID), SFViewModel.ParentIDToAdd), SFViewModel.ICAOToAdd, SFViewModel.TitleToAdd, SFViewModel.DeveloperToAdd, SFViewModel.DescriptionToAdd, SFViewModel.PathToAdd);
                if (SFViewModel.ICAOToAdd == "")
                {
                    item = new(SFViewModel.TypeToAdd, SFViewModel.SceneryIDToAdd, (ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), SFViewModel.RegionIDToAdd), (Region.RegionID)Enum.Parse(typeof(Region.RegionID), SFViewModel.ParentIDToAdd), SFViewModel.TitleToAdd, SFViewModel.DeveloperToAdd, SFViewModel.DescriptionToAdd, SFViewModel.PathToAdd);
                }
                
                Debug.WriteLine(SFViewModel.TypeToAdd.ToLower());
                switch (SFViewModel.TypeToAdd.ToLower())
                {
                    case "ortho":
                        SFViewModel.OrthoList.Add(item);
                        break;

                    case "airport":
                        SFViewModel.AirportList.Add(item);
                        break;

                    case "library":
                        SFViewModel.LibraryList.Add(item);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[!] Could not create scenery item\n\t=> {ex.Message}");
            }
            SFViewModel.SceneryItemDialogOpen = false;
            SFViewModel.TitleToAdd = "";
            SFViewModel.SceneryIDToAdd = "";
            SFViewModel.DeveloperToAdd = "";
            SFViewModel.DescriptionToAdd = "";
            SFViewModel.RegionIDToAdd = "";
            SFViewModel.ParentIDToAdd = "";
            SFViewModel.ICAOToAdd = "";
            SFViewModel.PathToAdd = "";
            SFViewModel.TypeToAdd = "";
        }

        internal void RemoveOrthoItem(object? item)
        {
            SFViewModel.OrthoList.Remove((SceneryItem)item);
        }

        internal void RemoveAirportItem(object? item)
        {
            SFViewModel.AirportList.Remove((SceneryItem)item);
        }

        internal void RemoveLibraryItem(object? item)
        {
            SFViewModel.LibraryList.Remove((SceneryItem)item);
        }

        internal void CreateServerConfigFile()
        {

            ServerFormat.Format.GenerateServerConfig($@"{SFViewModel.ServerRootPath}", SFViewModel.ServerID, SFViewModel.ServerLocation, SFViewModel.ServerProvider, SFViewModel.OrthoList, SFViewModel.AirportList, SFViewModel.LibraryList);
        }
    }
}
