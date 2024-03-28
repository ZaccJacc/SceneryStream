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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SceneryStream.src.Model.ChildRegion;
using static SceneryStream.src.Model.Region;

namespace SceneryStream.src.Model
{
    internal class Region
    {
        internal Bitmap Map;
        internal RegionID ID;

        internal Region(string MapURI, RegionID regionID)
        {
            Map = new(AssetLoader.Open(new Uri(MapURI)));
            ID = regionID;
        }
        //--Parent Regions--//

        internal static Region GLOBE = new($@"avares://SceneryStream/Assets/Map/worldmap.png", RegionID.GLOBE);
        internal static Region GLOBELINED = new($@"avares://SceneryStream/Assets/Map/worldmaplined.png", RegionID.DEV);
        internal static Region USA = new($@"avares://SceneryStream/Assets/Map/USAmap.png", RegionID.USA);

        //----//

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

        public ChildRegion(ChildRegionID regionID, Region.RegionID parentID)
        {
            ID = regionID;
            ParentID = parentID;
        }

        //--Child Regions--//
        
        

        internal enum ChildRegionID
        {
            WA = 0,
            OR = 1,
            CA = 2,
            NV = 3,
            ID = 4,
            UT = 5,
            AK = 6,
            HI = 7
        }
    }

    public class DisplayedRegionValidator : IValueConverter
    {
        public static readonly DisplayedRegionValidator Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Bitmap backgroundMap && parameter is string targetRegion)
            {
                switch (targetRegion)
                {
                    case "USA":
                        return backgroundMap == USA.Map;
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
    }
}
