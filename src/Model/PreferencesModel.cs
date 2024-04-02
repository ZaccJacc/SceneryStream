using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class PreferencesModel : ObservableObject
    {
        public PreferencesModel() { }


        private string? _simDirectory;
        public string? SimDirectory
        {
            get => _simDirectory;
            set
            {
                _simDirectory = value;
                NotifyPropertyChanged(nameof(SimDirectory));
            }
        }

        private string? _serverAddress;
        public string? ServerAddress
        {
            get => _serverAddress;
            

            set
            {
                _serverAddress = value;
                NotifyPropertyChanged(nameof(ServerAddress));
            }
        }


        private bool _multipleSims;
        public bool MultipleSims
        {
            get => _multipleSims;
            set
            {
                _multipleSims = value;
                NotifyPropertyChanged(nameof(MultipleSims));
            }
        }

        private bool _multipleScenes;
        public bool MultipleScenes
        {
            get => _multipleScenes;
            set
            {
                _multipleScenes = value;
                NotifyPropertyChanged(nameof(MultipleScenes));
            }
        }

        private string? _driveLetter;
        public string DriveLetter
        {
            get => _driveLetter;
            set
            {
                _driveLetter = value;
                NotifyPropertyChanged(nameof(DriveLetter));
                NotifyPropertyChanged(nameof(DriveIndex));
            }
        }

        public int? DriveIndex
        {
            get
            {

                return string.IsNullOrEmpty(_driveLetter) ? 0 : _driveLetter[0] - 65;
            }
            set
            {
                _driveLetter = ((char?)(value + 65)).ToString();
                NotifyPropertyChanged(nameof(DriveIndex));
            }
        }


        private string? _preferencesFile;
        public string? PreferencesFile
        {
            get => _preferencesFile;
            set
            {
                _preferencesFile = value;
                NotifyPropertyChanged(nameof(PreferencesFile));
            }
        }

        private ObservableCollection<string> _installationList = new();
        public ObservableCollection<string> InstallationPathsCollection
        {
            get => _installationList;
            set
            {
                _installationList = value;
                NotifyPropertyChanged(nameof(InstallationPathsCollection));
            }
        }

        private ObservableCollection<string> _sceneryList = new();
        public ObservableCollection<string> SceneryPathsCollection
        {
            get => _sceneryList;
            set
            {
                _sceneryList = value;
                NotifyPropertyChanged(nameof(SceneryPathsCollection));
            }
        }

        //--//

        public static async Task SavePreferences()
        {
            await Task.Run(() =>
            {
                Debug.WriteLine("[*] Attempting to save preferences.");
                try
                {
                    List<string> lines = new() 
                    {
                        App.Preferences.ServerAddress != null ? $"A-{App.Preferences.ServerAddress}" : $"A-{null}",
                        App.Preferences.SimDirectory != null ? $"S-{App.Preferences.SimDirectory}" : $"S-{null}", //Add this binding to the right window so the preferences can autosave.
                        $"D-{App.Preferences.DriveLetter}",
                        $"M-Multisim:{App.Preferences.MultipleSims}",
                        $"M-Multiscene:{App.Preferences.MultipleScenes}"
                    };
                    
                    foreach (object installation in App.Preferences.InstallationPathsCollection)
                    {
                        lines.Add($"§-{installation}");
                    }
                    foreach (object directory in App.Preferences.SceneryPathsCollection)
                    {
                        lines.Add($"&-{directory}");
                    }
                    if (App.Preferences.PreferencesFile != null)
                    {
                        File.WriteAllLines(App.Preferences.PreferencesFile, lines);
                    }
                    Debug.WriteLine("[*] Preferences Saved.");

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[!] Could not write to preferences file!\n\t=> {e.Message}");
                }
            });
        }

        public static async Task<bool> loadPreferences(string fileName)
        {
            App.Preferences.PreferencesFile = fileName;
            return await Task.Run(() =>
            {
                try
                {
                    string[] lines = File.ReadAllLines(fileName);
                    bool PropertiesIncomplete = false;
                    try
                    {
                        foreach (string line in lines)
                        {
                            switch (line[0])
                            {
                                case 'A':
                                    if (!(line[2..].Contains('\\') || line[2..].Contains('/')))
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.ServerAddress = line.Substring(2);
                                    Debug.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'S':
                                    if (!(line[2..].Contains('\\') || line[2..].Contains('/')))
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.SimDirectory = line.Substring(2);
                                    Debug.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'M':
                                    string[] split = line.Split(':');
                                    if (!split[1].Contains("True") && !split[1].Contains("False"))
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    switch (split[0].Contains("M-Multisim"))
                                    {
                                        case true:
                                            App.Preferences.MultipleSims = split[1].Equals("True"); //for some reason this is updating fine, but the UI isn't. The variable is observable though?
                                            break;

                                        case false:
                                            App.Preferences.MultipleScenes = split[1].Equals("True");
                                            break;
                                    }
                                    Debug.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'D':
                                    if (line.Length < 3)
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.DriveLetter = line[2].ToString();
                                    Debug.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case '§':
                                    if (!(line[2..].Contains('\\') || line[2..].Contains('/')))
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.InstallationPathsCollection.Add(line.Substring(2));
                                    break;
                                
                                case '&':
                                    if (!(line[2..].Contains('\\') || line[2..].Contains('/')))
                                    {
                                        PropertiesIncomplete = true;
                                        Debug.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.SceneryPathsCollection.Add(line[2..]);
                                    break;
                            }
                        }
                        App.Preferences.PreferencesFile = App.Preferences.PreferencesFile ?? "Preferences.setup";
                        if (PropertiesIncomplete)
                        {
                            Debug.WriteLine("[*] Preferences value is not fully formatted.\n\t=> Did not disrupt loading.");
                        }

                        App.Preferences.MultipleSims = App.Preferences.MultipleSims && App.Preferences.InstallationPathsCollection.Count > 0;
                        App.Preferences.MultipleScenes = App.Preferences.MultipleScenes && App.Preferences.SceneryPathsCollection.Count > 0;

                        return !string.IsNullOrEmpty(App.Preferences.ServerAddress); //return true if there is a server address provided and nothing else went fatally wrong.
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[!] Preferences file fatally misformatted!\n\t=> Preferences loading terminated. {ex.Message}");
                        return false;
                    }

                }
                catch (Exception)
                {
                    Debug.WriteLine("[!] Could not locate file at read time!");
                    return false;
                }

            });

        }


    }
}
