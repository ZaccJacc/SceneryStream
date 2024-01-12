using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal static Region GLOBE = new($@"avares://SceneryStream/Assets/Map/worldmap.png", RegionID.GLOBE);
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
            OCE = 7
        }
    }
}
