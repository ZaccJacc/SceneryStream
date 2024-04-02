using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SceneryStream.src.Model
{
    internal partial class SceneryItem : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [ObservableProperty]
        private string type = "";

        [ObservableProperty]
        private string sceneryID = "";

        [ObservableProperty]
        private ChildRegion.ChildRegionID regionID;

        [ObservableProperty]
        private Region.RegionID parentID;

        [ObservableProperty]
        private string title = "";

        [ObservableProperty]
        internal string developer = "";

        [ObservableProperty]
        internal string description = "";

        [ObservableProperty]
        internal string _ICAO = "";

        [ObservableProperty]
        internal string path = "";

        [ObservableProperty]
        private bool selected = false;

        public SceneryItem(string _type, string _sceneryID, ChildRegion.ChildRegionID _regionID, Region.RegionID _parentID, string _ICAO, string _title, string _developer, string _description, string _path)
        {
            Type = _type;
            SceneryID = _sceneryID;
            RegionID = _regionID;
            ParentID = _parentID;
            ICAO = _ICAO;
            Title = _title;
            Developer = _developer;
            Description = _description;
            Path = _path;
        }

        public SceneryItem(string _type, string _sceneryID, ChildRegion.ChildRegionID _regionID, Region.RegionID _parentID, string _title, string _developer, string _description, string _path)
        {
            Type = _type;
            SceneryID = _sceneryID;
            RegionID = _regionID;
            ParentID = _parentID;
            Title = _title;
            Developer = _developer;
            Description = _description;
            Path = _path;
        }

        public SceneryItem(string _type, string _sceneryID, string _title, string _developer, string _description, string _path)
        {
            Type = _type;
            SceneryID = _sceneryID;
            Title = _title;
            Developer = _developer;
            Description = _description;
            Path = _path;
        }
    }


    internal partial class ServerFormat : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {

        private static readonly ServerFormat _format = new();
        public static ServerFormat Format { get { return _format; } }

        [ObservableProperty]
        private string _serverID = "";

        [ObservableProperty]
        private string _serverLocation = "";

        [ObservableProperty]
        private string _serverProvider = "";

        [ObservableProperty]
        private string _loadedXML = "Select a config file";

        [ObservableProperty]
        private ObservableCollection<SceneryItem> _sceneryLoaded = new();

        /// <summary>
        /// Will load the server configuration information from the xml file on the server and display it. This allows for servers to have their sceneries attributed to certain regions without the need for a strong file structure.<br/>
        /// The config file will not be loaded if there is an XSS.Block file present, to allow for better data protection on publicly accessible servers.
        /// </summary>
        /// <param name="param">When not empty or null, will trigger loading from the test folder in the AppData directory rather than the server.</param>
        internal async Task LoadServerConfiguration(object? param)
        {
            try
            {
                string location = "";
                switch (App.ServiceInstance.Platform.ToString())
                {
                    case "Win32NT":
                        location =  File.Exists($@"{App.Preferences.DriveLetter}:\.data\ServerConfig.xml") ? $@"{App.Preferences.DriveLetter}:\.data\ServerConfig.xml" : null;
                        break;

                    case "Unix":
                        location = File.Exists($@"~/mnt/{App.Preferences.DriveLetter}:/.data/ServerConfig.xml") ? $@"~/mnt/{App.Preferences.DriveLetter}:/.data/ServerConfig.xml" : null;
                        break;
                }
                if (!string.IsNullOrEmpty(param.ToString()))
                {
                    switch (App.ServiceInstance.Platform.ToString())
                    {
                        case "Win32NT":
                            location = (await Utility.FileBrowser.produceBrowser("File", $@"{App.Preferences.DriveLetter}:\.data\")).ToString();
                            break;

                        case "Unix":
                            location = (await Utility.FileBrowser.produceBrowser("file", $@"~/mnt/{App.Preferences.DriveLetter}:/.data/")).ToString();
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(location))
                {
                    using StreamReader sr = new(File.OpenRead(location), Encoding.UTF8);
                    XElement loadedConfiguration = XElement.Load(sr);
                    if (File.Exists($@"{App.Preferences.DriveLetter}:\.data\.SCAN.Block"))
                    {
                        LoadedXML = "Invalid Request.\n\nViewing this server's configuration\nfile is not allowed.\n\nConnect to a different server\nto access this functionality.\n\n[This does not impact the server\nconfiguration loading internally,\nthe app will still function as normal.\n\nYou can view the loaded sceneries\nin the next tab using the\narrows below.]";
                    } else
                    {
                        LoadedXML = loadedConfiguration.ToString();
                    }
                    AttributeSceneriesFromXML(loadedConfiguration);
                } else
                {
                    throw new Exception("Server config in non-standard location.");
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] Could not locate Server Configuration file.\n\t=> {e.Message}");
            }
        }

        internal void AttributeSceneriesFromXML(XElement loadedData)
        {
            FlushAttributedSceneries();
            List<XElement> Ortho = new();
            List<XElement> Airports = new();
            List<XElement> Libraries = new();

            ServerID = loadedData.Descendants("ServerID").ElementAt(0).Value;
            ServerLocation = loadedData.Descendants("ServerLocation").ElementAt(0).Value;
            ServerProvider = loadedData.Descendants("ServerProvider").ElementAt(0).Value;

            foreach (XElement Node in loadedData.Descendants("Scenery"))
            {
                foreach (XElement OrthoItem in Node.Descendants("OrthoItem"))
                {
                    Ortho.Add(OrthoItem);
                }
                foreach (XElement AirportItem in Node.Descendants("AirportItem"))
                {
                    Airports.Add(AirportItem);
                }
                foreach (XElement LibraryItem in Node.Descendants("LibraryItem"))
                {
                    Libraries.Add(LibraryItem);
                }
            }
            

            new Thread(() =>
            {
                foreach (XElement OrthoItem in Ortho)
                {
                    
                    try
                    {
                        ChildRegion.ChildRegionID id = (ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), OrthoItem.Attribute("RegionID").Value);
                        Region.RegionID parentid = (Region.RegionID)Enum.Parse(typeof(Region.RegionID), OrthoItem.Attribute("ParentID").Value);
                        ChildRegion targetRegion = RegionHandling.Regions.GetRegionByID(id,parentid);
                        SceneryItem item = new(
                                "Ortho",
                                OrthoItem.Attribute("SceneryID").Value,
                                (ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), OrthoItem.Attribute("RegionID").Value),
                                (Region.RegionID)Enum.Parse(typeof(Region.RegionID), OrthoItem.Attribute("ParentID").Value),
                                OrthoItem.Descendants("Title").ElementAt(0).Value,
                                OrthoItem.Descendants("Developer").ElementAt(0).Value,
                                OrthoItem.Descendants("Description").ElementAt(0).Value,
                                OrthoItem.Descendants("Path").ElementAt(0).Value
                            );
                        SceneryLoaded.Add(item);
                        if (bool.Parse(OrthoItem.Attribute("Primary").Value) == true)
                        {
                            targetRegion.PrimaryOrtho = item;
                        }
                        else
                        {
                            targetRegion.SceneryItems.Add(item);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[!] Error loading ortho item.\n\t=> {e.Message}");
                    }

                }
            }).Start();

            new Thread(() =>
            {
                foreach (XElement AirportItem in Airports)
                {
                    try
                    {
                        ChildRegion targetRegion = RegionHandling.Regions.GetRegionByID((ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), AirportItem.Attribute("RegionID").Value), (Region.RegionID)Enum.Parse(typeof(Region.RegionID), AirportItem.Attribute("ParentID").Value));
                        SceneryItem item = new(
                                "Airport",
                                AirportItem.Attribute("SceneryID").Value,
                                (ChildRegion.ChildRegionID)Enum.Parse(typeof(ChildRegion.ChildRegionID), AirportItem.Attribute("RegionID").Value),
                                (Region.RegionID)Enum.Parse(typeof(Region.RegionID), AirportItem.Attribute("ParentID").Value),
                                AirportItem.Descendants("ICAO").ElementAt(0).Value,
                                AirportItem.Descendants("Title").ElementAt(0).Value,
                                AirportItem.Descendants("Developer").ElementAt(0).Value,
                                AirportItem.Descendants("Description").ElementAt(0).Value,
                                AirportItem.Descendants("Path").ElementAt(0).Value
                            );
                        targetRegion.SceneryItems.Add(item);
                        SceneryLoaded.Add(item);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[!] Error loading airport item.\n\t=> {e.Message}");
                    }
                }
            }).Start();


            new Thread(() =>
            {
                foreach (XElement LibraryItem in Libraries)
                {
                    try
                    {
                        //Not yet implemented libraries.
                    }
                    catch
                    {
                        Debug.WriteLine("[!] Libraries not yet implemented.");
                    }
                }
            }).Start();

        }

        internal static void FlushAttributedSceneries()
        {
            foreach (Region.RegionID ID in (Region.RegionID[])Enum.GetValues(typeof(Region.RegionID)))
            {
                foreach (ChildRegion.ChildRegionID ChildID in (ChildRegion.ChildRegionID[])Enum.GetValues(typeof(ChildRegion.ChildRegionID)))
                {
                    ChildRegion region = RegionHandling.Regions.GetRegionByID(ChildID, ID);
                    if (region != null)
                    {
                        region.SceneryItems.Clear();
                    }
                }
            }
            Format.SceneryLoaded.Clear();
        }

        internal void GenerateServerConfig(string locationRoot, string ID, string Location, string Provider, IEnumerable<SceneryItem> Ortho, IEnumerable<SceneryItem> Airports, IEnumerable<SceneryItem> Libraries)
        {
            XElement serverConfig = new("ServerConfig");
            XElement configuration = new("Configuration", new XElement("ServerID", ID), new XElement("ServerLocation", Location), new XElement("ServerProvider", Provider));
            XElement scenery = new("Scenery");
            XElement orthoNode = new("Ortho");
            XElement airportNode = new("Airports");
            XElement libraryNode = new("Libraries");
            foreach (SceneryItem item in Ortho)
            {
                XElement orthoItem = new("OrthoItem");
                orthoItem.Add(new XAttribute("SceneryID", item.SceneryID));
                orthoItem.Add(new XAttribute("Primary", "false"));
                orthoItem.Add(new XAttribute("RegionID", item.RegionID));
                orthoItem.Add(new XAttribute("ParentID", item.ParentID));
                orthoItem.Add(
                    new XElement("ICAO", item.ICAO),
                    new XElement("Title", item.Title),
                    new XElement("Developer", item.Developer),
                    new XElement("Description", item.Description),
                    new XElement("Path", item.Path)
                    );
                orthoNode.Add(orthoItem);
            }
            foreach (SceneryItem item in Airports)
            {
                XElement airportItem = new("AirportItem");
                airportItem.Add(new XAttribute("SceneryID", item.SceneryID));
                airportItem.Add(new XAttribute("Primary", "false"));
                airportItem.Add(new XAttribute("RegionID", item.RegionID));
                airportItem.Add(new XAttribute("ParentID", item.ParentID));
                airportItem.Add(
                    new XElement("ICAO", item.ICAO),
                    new XElement("Title", item.Title),
                    new XElement("Developer", item.Developer),
                    new XElement("Description", item.Description),
                    new XElement("Path", item.Path)
                    );
                airportNode.Add(airportItem);
            }
            foreach (SceneryItem item in Libraries)
            {
                XElement libraryItem = new("LibraryItem");
                libraryItem.Add(new XAttribute("SceneryID", item.SceneryID));
                libraryItem.Add(new XAttribute("Primary", "false"));
                libraryItem.Add(new XAttribute("RegionID", item.RegionID));
                libraryItem.Add(new XAttribute("ParentID", item.ParentID));
                libraryItem.Add(
                    new XElement("ICAO", item.ICAO),
                    new XElement("Title", item.Title),
                    new XElement("Developer", item.Developer),
                    new XElement("Description", item.Description),
                    new XElement("Path", item.Path)
                    );
                libraryNode.Add(libraryItem);
            }
            scenery.Add(orthoNode);
            scenery.Add(airportNode);
            scenery.Add(libraryNode);
            serverConfig.Add(configuration);
            serverConfig.Add(scenery);
            try
            {
                if (!Directory.Exists(@$"{locationRoot}\.data"))
                {
                    Directory.CreateDirectory(@$"{locationRoot}\.data");
                }
                using StreamWriter sw = new(File.Open(@$"{locationRoot}\.data\ServerConfig.xml", FileMode.Create), Encoding.UTF8);
                serverConfig.Save(sw);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] Fatal saving error, will not retry.\n\t=> {e.Message}");
            }
        }


        internal void FormatServer()
        {
            try
            {
                Debug.WriteLine("[*] Attempting to create data directory.");
                switch (App.ServiceInstance.Platform.ToString())
                {
                    case "Win32NT":
                        Directory.CreateDirectory($@"{App.Preferences.DriveLetter}:\.data\");
                        break;

                    case "Unix":
                        Directory.CreateDirectory($@"~/mnt/{App.Preferences.DriveLetter}:\.data\");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[!] Could not create data directory.\n\t {ex.Message}");
            }
        }
    }
}

