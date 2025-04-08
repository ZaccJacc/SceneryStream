using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static SceneryStream.src.Model.ChildRegion;
using static SceneryStream.src.Model.Region;
using SceneryStream.src.View;
using ExCSS;

namespace SceneryStream.src.Model
{
    internal class Region : ObservableObject
    {

        private object? _map;
        internal object? Map
        {
            get => _map;
            set
            {
                _map = value;
                NotifyPropertyChanged(nameof(Map));
            }
        }

        /*private Bitmap _map;
        internal Bitmap Map
        {
            get => _map;
            set
            {
                _map = value;
                NotifyPropertyChanged(nameof(Map));
            }
        }*/

        private object? _regionDisplay;
        public object? RegionDisplay
        {
            get => _regionDisplay;
        }

        private RegionID _ID;
        public RegionID ID
        {
            get => _ID;
            set
            {
                _ID = value;
                NotifyPropertyChanged(nameof(ID));
            }
        }

        internal Region(string MapURI, RegionID regionID, object? regionDisplay)
        {
            Map = string.IsNullOrEmpty(MapURI) ? "" : new Bitmap(AssetLoader.Open(new Uri(MapURI)));
            //Map = new Bitmap(AssetLoader.Open(new Uri(MapURI)));
            ID = regionID;
            _regionDisplay = regionDisplay;
        }

        internal enum RegionID
        {
            GLOBE = 0,
            USA = 1,
            CAN = 2,
            LATAM = 3,
            EUR = 4,
            AFR = 5,
            ASI = 6,
            OCE = 7,
            DEV = 8,
            ERROR = 9
        }
    }

    internal partial class ChildRegion : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [ObservableProperty]
        private ChildRegionID _ID;

        [ObservableProperty]
        private RegionID _parentID;

        internal SceneryItem PrimaryOrtho = null;

        [ObservableProperty]
        private bool _primaryOrthoAvailable = false;
        

        [ObservableProperty]
        private ObservableCollection<SceneryItem> _sceneryItems = new();

        [ObservableProperty]
        private bool selected = false;


        public ChildRegion(ChildRegionID regionID, RegionID parentID)
        {
            ID = regionID;
            ParentID = parentID;
        }

        internal enum ChildRegionID
        {
            WA,
            OR,
            CA,
            NV,
            ID,
            UT,
            AK,
            HI,
            AZ,
            CO,
            NM,
            WY,
            MT,
            ND,
            SD,
            NE,
            KS,
            OK,
            TX
        }
    }


    public class DisplayedRegionValidator : IValueConverter
    {
        public static readonly DisplayedRegionValidator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is RegionID displayedRegionID && parameter is string targetRegion)
            {
                switch (targetRegion)
                {
                    case "USA":
                        return displayedRegionID == RegionID.USA;
                    case "EUR":
                        return displayedRegionID == RegionID.EUR;
                    default:
                        return false;
                }
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class RegionLocator
    {
        private object region;
        public RegionLocator(string _childId, string _parentId)
        {
            region = RegionHandling.Regions.GetRegionByID((ChildRegionID)Enum.Parse(typeof(ChildRegionID), _childId), (RegionID)Enum.Parse(typeof(RegionID), _parentId));
        }

        public RegionLocator(string _Id)
        {
            region = RegionHandling.Regions.GetRegionByID((RegionID)Enum.Parse(typeof(RegionID), _Id));
        }

        public RegionLocator(Binding _childId, Binding _parentId)
        {
            region = new MultiBinding()
            {
                Bindings = new[] { _childId, _parentId },
                Converter = new FuncMultiValueConverter<object, object>(IDs => IDs.Aggregate((childId, parentID) => RegionHandling.Regions.GetRegionByID((ChildRegionID)Enum.Parse(typeof(ChildRegionID), childId.ToString()), (RegionID)Enum.Parse(typeof(RegionID), parentID.ToString())))) //The binding value isn't coming through any more. When you print it, it just says "avalonia.data.binding", which is why it can't then find it from the region enums.
            };
        }

        public object ProvideValue()
        {
            return region;
        }
    }



    internal partial class RegionHandling : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        //--Internal Utility--//

        private static readonly RegionHandling _regions = new();
        public static RegionHandling Regions
        {
            get => _regions;
        }

        private static List<string> SessionGeneratedShellLinksRemote = new();
        private static List<string> SessionGeneratedShellLinksLocal = new();

        //--Selected Sceneries--//

        internal static ObservableCollection<ChildRegion> SelectedChildRegions = new();
        internal static ObservableCollection<SceneryItem> SelectedSceneryItems = new();

        //--Child Regions--//

        internal static ObservableCollection<ChildRegion> ChildRegions = new()
        {
            new(ChildRegionID.WA, RegionID.USA),
            new(ChildRegionID.OR, RegionID.USA),
            new(ChildRegionID.CA, RegionID.USA),
            new(ChildRegionID.NV, RegionID.USA),
            new(ChildRegionID.ID, RegionID.USA),
            new(ChildRegionID.UT, RegionID.USA),
            new(ChildRegionID.AK, RegionID.USA),
            new(ChildRegionID.HI, RegionID.USA),
            new(ChildRegionID.AZ, RegionID.USA),
            new(ChildRegionID.CO, RegionID.USA),
            new(ChildRegionID.NM, RegionID.USA),
            new(ChildRegionID.WY, RegionID.USA),
            new(ChildRegionID.MT, RegionID.USA),
            new(ChildRegionID.ND, RegionID.USA),
            new(ChildRegionID.SD, RegionID.USA),
            new(ChildRegionID.NE, RegionID.USA),
            new(ChildRegionID.KS, RegionID.USA),
            new(ChildRegionID.OK, RegionID.USA),
            new(ChildRegionID.TX, RegionID.USA)
    };

        //--Parent Regions--//
        internal static ObservableCollection<Region> ParentRegions = new()
        {
            new($@"avares://SceneryStream/Assets/Map/worldmap.png", RegionID.GLOBE, new View.SceneryRegions.GLOBERegion()),
            new($@"avares://SceneryStream/Assets/Map/worldmaplined.png", RegionID.DEV, new View.SceneryRegions.GLOBERegion()), //disabled dev world map for now
            new($@"avares://SceneryStream/Assets/Map/USA/USAmap.png", RegionID.USA, new View.SceneryRegions.USARegion()),
            new("", RegionID.EUR, new View.SceneryRegions.EURRegion()),
            new("",RegionID.ERROR, new View.SceneryRegions.ERRORRegion())
        };
        
        //--//

        internal ChildRegion? GetRegionByID(ChildRegionID childID, RegionID parentID)
        {
            foreach (ChildRegion childRegion in ChildRegions)
            {
                if (childRegion.ID == childID && childRegion.ParentID == parentID)
                { return childRegion; }
            }
            return null;
        }

        internal Region? GetRegionByID(RegionID ID)
        {
            foreach (Region region in ParentRegions)
            {
                if (region.ID == ID) { return region; }
            }
                return null;
        }

        /// <summary>
        /// Saves all the selected regions to an XML file with their ID, parentID, and their selection state for posterity (in case they are erroneously added to the list but not actually intended to be selected).
        /// </summary>
        internal void SaveSelectedRegions()
        {
            XElement Root = new("SavedItems");
            XElement originServer = new("OriginServer", ServerFormat.Format.ServerID);
            XElement savedRegions = new("Regions");
            XElement savedScenery = new("Scenery");
            foreach (var region in SelectedChildRegions)
            {
                savedRegions.Add(new XElement("ChildRegion", new XElement("ID", region.ID), new XElement("ParentID", region.ParentID), new XElement("Selected", region.Selected)));
            }
            foreach (SceneryItem item in SelectedSceneryItems)
            {
                savedScenery.Add(new XElement("SceneryItem", new XElement("SceneryID", item.SceneryID), new XElement("RegionID", item.RegionID), new XElement("ParentID", item.ParentID)));
            }
            Root.Add(originServer);
            Root.Add(savedRegions);
            Root.Add(savedScenery);
            try
            {
                using StreamWriter sw = new(File.Open($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SceneryStream/Data/Scenery.xml", FileMode.Create), Encoding.UTF8); //this only saves the first childregion. why?
                Root.Save(sw);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] Fatal saving error, will not retry.\n\t=> {e.Message}");
            }
        }

        internal void LoadSelectedRegions()
        {
            try
            {
                StreamReader sr = new(File.Open($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SceneryStream/Data/Scenery.xml", FileMode.OpenOrCreate), Encoding.UTF8);
                XElement loadedRegions = XElement.Load(sr);
                if (loadedRegions.Descendants("OriginServer").ElementAt(0).Value != ServerFormat.Format.ServerID)
                {
                    Debug.WriteLine("[!] Servers do not match. Cannot load scenery from an unknown server.");
                    return;
                }
                IEnumerable<XElement> NodeChildRegions = loadedRegions.Descendants("ChildRegion");
                foreach (XElement node in NodeChildRegions)
                {
                    try
                    {
                        ChildRegion currentRegion = GetRegionByID((ChildRegionID)Enum.Parse(typeof(ChildRegionID), node.Descendants("ID").ElementAt(0).Value), (RegionID)Enum.Parse(typeof(RegionID), node.Descendants("ParentID").ElementAt(0).Value));
                        currentRegion.Selected = (bool)node.Descendants("Selected").ElementAt(0);
                        if (currentRegion.Selected && !SelectedChildRegions.Contains(currentRegion))
                        {
                            SelectedChildRegions.Add(currentRegion);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[!] Could not locate region.\n\t=>{e.Message}");
                    }

                }
                foreach (XElement SceneryNode in loadedRegions.Descendants("SceneryItem"))
                {
                    try
                    {
                        ChildRegion currentRegion = GetRegionByID((ChildRegionID)Enum.Parse(typeof(ChildRegionID), SceneryNode.Descendants("RegionID").ElementAt(0).Value), (RegionID)Enum.Parse(typeof(RegionID), SceneryNode.Descendants("ParentID").ElementAt(0).Value));
                        foreach (SceneryItem scenery in currentRegion.SceneryItems)
                        {
                            if (scenery.SceneryID == SceneryNode.Descendants("SceneryID").ElementAt(0).Value)
                            {
                                scenery.Selected = true;
                                if (!SelectedSceneryItems.Contains(scenery))
                                {
                                    SelectedSceneryItems.Add(scenery);
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[!] Could not locate scenery item.\n\t=> {e.Message}");
                    }
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("[!] Could not load the savefile.");
            }
                      
        }

        internal async void AddOneShellLink(SceneryItem item)
        {
            if (item.Path.Contains("srvload:"))
            {
                switch (App.ServiceInstance.Platform)
                {
                    default:
                        return;

                    case PlatformID.Win32NT:
                        SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, item.Path.Split("srvload:")[1], @$"{App.Preferences.SimDirectory}\Custom Scenery", item.Type, $"{item.Title}"));
                        foreach (string ExtraPath in App.Preferences.InstallationPathsCollection)
                        {
                            SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, item.Path.Split("srvload:")[1], @$"{ExtraPath}\Custom Scenery", item.Type, $"{item.Title}"));
                        }
                        SessionGeneratedShellLinksRemote.Add(item.Path);
                        break;
                }
            } else
            {
                Debug.WriteLine("[*] Non-server shell uri registered.");
            }
        }

        internal async void GenerateShellLinks(object? devparam)
        {
            foreach (ChildRegion primaryOrthoRegion in SelectedChildRegions)
            {
                if (primaryOrthoRegion.PrimaryOrtho == null || SessionGeneratedShellLinksRemote.Contains(primaryOrthoRegion.PrimaryOrtho.Path) || !primaryOrthoRegion.PrimaryOrtho.Path.Contains("srvload:"))
                {
                    Debug.WriteLine("[*] Skipped an item.\n\t=> Will not create ortho shell link.");
                    continue;
                }
                if (string.IsNullOrEmpty(devparam.ToString()))
                {
                    
                    switch (App.ServiceInstance.Platform)
                    {
                        default:
                            return;

                        case PlatformID.Win32NT:
                            SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, primaryOrthoRegion.PrimaryOrtho.Path.Split("srvload:")[1], @$"{App.Preferences.SimDirectory}\Custom Scenery", primaryOrthoRegion.PrimaryOrtho.Type, $"{primaryOrthoRegion.PrimaryOrtho.Title}"));
                            foreach (string ExtraPath in App.Preferences.InstallationPathsCollection)
                            {
                                SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, primaryOrthoRegion.PrimaryOrtho.Path.Split("srvload:")[1], @$"{ExtraPath}\Custom Scenery", primaryOrthoRegion.PrimaryOrtho.Type, $"{primaryOrthoRegion.PrimaryOrtho.Title}"));
                            }
                            SessionGeneratedShellLinksRemote.Add(primaryOrthoRegion.PrimaryOrtho.Path);
                            break;
                    }
                }
                
            }
            foreach (SceneryItem item in SelectedSceneryItems)
            {
                if (SessionGeneratedShellLinksRemote.Contains(item.Path) || !item.Path.Contains("srvload:"))
                {
                    Debug.WriteLine("[*] Skipped an item.\n\t=> Will not create shell link.");
                    continue;
                }
                if (string.IsNullOrEmpty(devparam.ToString()))
                {
                    switch (App.ServiceInstance.Platform)
                    {
                        default:
                            return;

                        case PlatformID.Win32NT:
                            SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, item.Path.Split("srvload:")[1], @$"{App.Preferences.SimDirectory}\Custom Scenery", item.Type, $"{item.Title}"));
                            foreach (string ExtraPath in App.Preferences.InstallationPathsCollection)
                            {
                                SessionGeneratedShellLinksLocal.Add(await Utility.Win32.createShortcut(App.Preferences.DriveLetter, item.Path.Split("srvload:")[1], @$"{ExtraPath}\Custom Scenery", item.Type, $"{item.Title}"));
                            }
                            SessionGeneratedShellLinksRemote.Add(item.Path);
                            break;
                    }
                }
            }
        }

        internal void RemoveShellLinks()
        {
            foreach (string path in SessionGeneratedShellLinksLocal)
            {
                try
                {
                    File.Delete(path);
                } 
                catch (Exception)
                {
                    Debug.WriteLine("[!] Couldn't delete shortcut.");
                }
                
            }
        }

    }                      
}
