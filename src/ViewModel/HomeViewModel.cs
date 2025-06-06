﻿using SceneryStream.src.Model;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Collections.ObjectModel;
using Avalonia.Platform;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SceneryStream.src.ViewModel
{
    internal partial class HomeViewModel : ObservableObject
    {

        public HomeViewModel() 
        {
            ServerUpdateEntries = new();
            ScenerySpotlightEntries = new();
        }

        private static readonly HomeViewModel _hViewModel = new();
        public static HomeViewModel HViewModel
        {
            get => _hViewModel;
        }

        private ObservableCollection<ServerUpdateEntry> _serverUpdateEntries;
        public ObservableCollection<ServerUpdateEntry> ServerUpdateEntries
        {
            get => _serverUpdateEntries;
            set
            {
                _serverUpdateEntries = value;
                NotifyPropertyChanged(nameof(ServerUpdateEntries));
            }
        }

        private ObservableCollection<SceneryCard> _scenerySpotlightEntries;
        public ObservableCollection<SceneryCard> ScenerySpotlightEntries
        {
            get => _scenerySpotlightEntries;
            set
            {
                _scenerySpotlightEntries = value;
                NotifyPropertyChanged(nameof(ScenerySpotlightEntries));
            }
        }

        private Bitmap? _source = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Status/Connecting_Circle.png")));
        public Bitmap? Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged(nameof(Source));
            }
        }

        public void ReviewConnecionStatusIndicator()
        {
            switch (App.ServiceInstance.Connected)
            {
                case true:
                    Source = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Status/Connected_Circle.png")));
                    break;
                case false:
                    Source = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Status/Disconnected_Circle.png")));
                    break;
            }
        }

        

        /// <summary>
        /// Explanation of the Server Updates formatting.
        /// <para>
        /// The Date and Title of each update are found before <c>//d//</c> in the server updates file, on the line below <c>//br//</c> unless it is the top line of the file.</para>
        /// <para>The newline from the beginning of this is stripped, so as to not end up with the spacing being one line too far down.</para>
        /// <para>The content of the update message begins immediately after <c>//d//</c> on the same line. New lines are then treated as normal in the updates file.</para>
        /// <para>The end of an update message is signified by the presence of <c>//br//</c> on the SAME LINE.
        /// </para>
        /// </summary>
        internal static void ScanNewUpdates()
        {
            Debug.WriteLine("[*] Attempting to read server updates info");
            try
            {
                StreamReader updatesFile = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Resources/ServerUpdates.txt")));
                if (App.ServiceInstance.Connected)
                {
                    switch (App.ServiceInstance.Platform.ToString())
                    {
                        case "Win32NT":
                            updatesFile = new StreamReader(new FileStream(App.Preferences.DriveLetter + @":\ServerUpdates",FileMode.Open, FileAccess.Read));
                            break;

                        case "Unix":
                            updatesFile = new(AssetLoader.Open(new Uri($"~/mnt/{App.Preferences.DriveLetter}/ServerUpdates")));
                            break;
                    }
                }
                HViewModel.ServerUpdateEntries.Clear();
                string[] split = updatesFile.ReadToEnd().Split("//br//");
                foreach (string line in split)
                {
                    string[] content = line.Split("//d//");
                    string dateString = WhitespaceRegex().Replace(content[0], "");
                    if (HViewModel.ServerUpdateEntries.Count > 0 && dateString == HViewModel.ServerUpdateEntries[0].Date)
                    {
                        Debug.WriteLine("[!] Did not refresh server updates\n\t=> Already up to date!");
                        return;
                    }
                    try
                    {
                        ServerUpdateEntry entry = new(dateString, content[1]);
                        HViewModel.ServerUpdateEntries.Add(entry);
                    }
                    catch
                    {
                        throw new Exception("Could not read updates");
                    }
                }
                updatesFile.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[!] Could not load updates!\n\t=> {ex.Message}");
            }
        }

        /// <summary>
        /// Explanation of the Scenery Spotlight formatting.
        /// <para>
        /// <para>The content of the update message begins immediately after <c>//br//</c> on the line below. New lines <c>(\n)</c> are re-grouped in the description only, as they are used to separate the data for each card.</para>
        /// <para>The end of a card's data is signified by the presence of <c>//br//</c> on the SAME LINE.</para>
        /// <para>The ScenerySpotlight file MUST start with an empty line.</para>
        /// </para>
        /// </summary>
        internal static void RefreshScenerySpotlight()
        {
            Debug.WriteLine("[*] Attempting to read spotlight entries.");
            try
            {
                StreamReader spotlightFile = new(AssetLoader.Open(new Uri($@"avares://SceneryStream/Assets/Resources/ScenerySpotlight.txt")));
                if (App.ServiceInstance.Connected)
                {
                    switch (App.ServiceInstance.Platform.ToString())
                    {
                        case "Win32NT":
                            spotlightFile = new StreamReader(new FileStream(App.Preferences.DriveLetter + @":\ScenerySpotlight", FileMode.Open, FileAccess.Read)); //placeholder
                            break;

                        case "Unix":
                            spotlightFile = new(AssetLoader.Open(new Uri($"~/mnt/{App.Preferences.ServerAddress}/ScenerySpotlight"))); //placeholder - this probably won't work as well.
                            break;
                    }
                }
                HViewModel.ScenerySpotlightEntries.Clear();
                string fileData = spotlightFile.ReadToEnd();
                string[] dataSplit = fileData.Split("//br//");
                foreach (string line in dataSplit)
                {
                    string[] lineSplit = line.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    SceneryCard entry = new(new Uri(lineSplit[1]), lineSplit[2], lineSplit[3], string.Join(" ",lineSplit[4..^0]));
                    HViewModel.ScenerySpotlightEntries.Add(entry); 
                    //indexing starts at 1 because when the split by new line is applied, the trailing \n from the revision split or data split is left at the beginning of the following data, meaning an empty line sits at index 0
                }
            } catch (Exception ex)
            {
                Debug.WriteLine($"[!] Could not load spotlight!\n\t=> {ex.Message}\n{ex.InnerException}");
            }
        }

        [GeneratedRegex("\\t|\\n|\\r")]
        private static partial Regex WhitespaceRegex();
    }
}

