using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SceneryStream.src.Model.ChildRegion;
using static SceneryStream.src.Model.Region;

namespace SceneryStream.src.Model
{
    internal class Region : ObservableObject
    {
        private Bitmap _map;
        internal Bitmap Map
        {
            get => _map;
            set
            {
                _map = value;
                NotifyPropertyChanged(nameof(Map));
            }
        }

        internal RegionID ID;

        internal Region(string MapURI, RegionID regionID)
        {
            Map = new(AssetLoader.Open(new Uri(MapURI)));
            ID = regionID;
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
            DEV = 8
        }
    }

    internal class ChildRegion : ObservableObject
    {
        internal ChildRegionID ID;
        internal RegionID ParentID;

        private bool selected = false;
        internal bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                NotifyPropertyChanged(nameof(Selected));
            }
        }

        public ChildRegion(ChildRegionID regionID, RegionID parentID)
        {
            ID = regionID;
            ParentID = parentID;
        }

        //--Child Regions--//
        
        

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
            WY

        }
    }

    public class DisplayedRegionValidator : IValueConverter
    {
        public static readonly DisplayedRegionValidator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Bitmap displayedRegion && parameter is string targetRegion)
            {
                switch (targetRegion)
                {
                    case "USA":
                        return displayedRegion == RegionHandling.Regions.USA.Map;
                }
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    internal partial class RegionHandling : ObservableObject
    {
        private static readonly RegionHandling _regions = new();
        public static RegionHandling Regions
        {
            get => _regions;
        }

        internal static ObservableCollection<ChildRegion> SelectedChildRegions = new();

        /// <summary>
        /// Saves all the selected regions to an XML file with their ID, parentID, and their selection state for posterity (in case they are erroneously added to the list but not actually intended to be selected).
        /// </summary>
        internal void SaveSelectedRegions()
        {
            XElement savedRegions = new("Regions");
            foreach (var region in SelectedChildRegions)
            {
                savedRegions.Add(new XElement("childRegion", region.ID, region.ParentID, region.Selected));
            }
            try
            {
                savedRegions.Save(AssetLoader.Open(new Uri($"avares://SceneryStream/Assets/Data/Scenery.xml")));
            }
            catch (FileNotFoundException)
            {
                try
                {
                    File.Create($"avares://SceneryStream/Assets/Data/Scenery.xml");
                }
                catch
                {
                    Console.WriteLine("[!] Could not save scenery.");
                    return;
                }
                SaveSelectedRegions();
            }
            
        }

        //--Parent Regions--//
        private Region _GLOBE = new($@"avares://SceneryStream/Assets/Map/worldmap.png", RegionID.GLOBE);
        public Region GLOBE
        {
            get => _GLOBE;
            set
            {
                _GLOBE = value;
                NotifyPropertyChanged(nameof(GLOBE));
            }
        }

        private Region _GLOBELINED = new($@"avares://SceneryStream/Assets/Map/worldmaplined.png", RegionID.DEV);
        public Region GLOBELINED
        {
            get => _GLOBELINED;
            set
            {
                _GLOBELINED = value;
                NotifyPropertyChanged(nameof(GLOBELINED));
            }
        }

        private Region _USA = new($@"avares://SceneryStream/Assets/Map/USAmap.png", RegionID.USA);
        public Region USA
        {
            get => _USA;
            set
            {
                _USA = value;
                NotifyPropertyChanged(nameof(USA));
            }
        }

        //--Child Regions--//

        private ChildRegion _USA_WA = new(ChildRegionID.WA, RegionID.USA);
        public ChildRegion USA_WA
        {
            get => _USA_WA;
            set
            {
                _USA_WA = value;
                NotifyPropertyChanged(nameof(USA_WA));
            }
        }
        
        private ChildRegion _USA_OR = new(ChildRegionID.OR, RegionID.USA);
        public ChildRegion USA_OR
        {
            get => _USA_OR;
            set
            {
                _USA_OR = value;
                NotifyPropertyChanged(nameof(USA_OR));
            }
        }

        private ChildRegion _USA_CA = new(ChildRegionID.CA, RegionID.USA);
        public ChildRegion USA_CA
        {
            get => _USA_CA;
            set
            {
                _USA_CA = value;
                NotifyPropertyChanged(nameof(USA_CA));
            }
        }

        private ChildRegion _USA_NV = new(ChildRegionID.NV, RegionID.USA);
        public ChildRegion USA_NV
        {
            get => _USA_NV;
            set
            {
                _USA_NV = value;
                NotifyPropertyChanged(nameof(USA_NV));
            }
        }

        private ChildRegion _USA_ID = new(ChildRegionID.ID, RegionID.USA);
        public ChildRegion USA_ID
        {
            get => _USA_ID;
            set
            {
                _USA_ID = value;
                NotifyPropertyChanged(nameof(USA_ID));
            }
        }

        private ChildRegion _USA_UT = new(ChildRegionID.UT, RegionID.USA);
        public ChildRegion USA_UT
        {
            get => _USA_UT;
            set
            {
                _USA_UT = value;
                NotifyPropertyChanged(nameof(USA_UT));
            }
        }

        private ChildRegion _USA_AK = new(ChildRegionID.AK, RegionID.USA);
        public ChildRegion USA_AK
        {
            get => _USA_AK;
            set
            {
                _USA_AK = value;
                NotifyPropertyChanged(nameof(USA_AK));
            }
        }

        private ChildRegion _USA_HI = new(ChildRegionID.HI, RegionID.USA);
        public ChildRegion USA_HI
        {
            get => _USA_HI;
            set
            {
                _USA_HI = value;
                NotifyPropertyChanged(nameof(USA_HI));
            }
        }
        
        private ChildRegion _USA_AZ = new(ChildRegionID.AZ, RegionID.USA);
        public ChildRegion USA_AZ
        {
            get => _USA_AZ;
            set
            {
                _USA_AZ = value;
                NotifyPropertyChanged(nameof(USA_AZ));
            }
        }

        private ChildRegion _USA_CO = new(ChildRegionID.CO, RegionID.USA);
        public ChildRegion USA_CO
        {
            get => _USA_CO;
            set
            {
                _USA_CO = value;
                NotifyPropertyChanged(nameof(USA_CO));
            }
        }

        private ChildRegion _USA_NM = new(ChildRegionID.NM, RegionID.USA);
        public ChildRegion USA_NM
        {
            get => _USA_NM;
            set
            {
                _USA_NM = value;
                NotifyPropertyChanged(nameof(USA_NM));
            }
        }

        private ChildRegion _USA_WY = new(ChildRegionID.WY, RegionID.USA);
        public ChildRegion USA_WY
        {
            get => _USA_WY;
            set
            {
                _USA_WY = value;
                NotifyPropertyChanged(nameof(USA_WY));
            }
        }
    }
}
