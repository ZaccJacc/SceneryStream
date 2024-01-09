using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class PreferencesModel : ObservableObject
    {
        public PreferencesModel() { }

        private string _test = "hello world";
        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                NotifyPropertyChanged(nameof(Test));
            }
        }


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

        //--//

        public static async Task SavePreferences()
        {
            await Task.Run(() =>
            {
                try
                {
                    string[] lines = new string[4];
                    lines[0] = App.Preferences._serverAddress != null ? $"A-{App.Preferences._serverAddress}" : $"A-{null}";
                    lines[1] = App.Preferences._simDirectory != null ? $"S-{App.Preferences._simDirectory}" : $"S-{null}"; //Add this binding to the right window so the preferences can autosave.
                    lines[2] = $"D-{App.Preferences._driveLetter}";
                    lines[3] = $"M-Multisim:{App.Preferences._multipleSims}";
                    if (App.Preferences._preferencesFile != null && File.Exists(App.Preferences._preferencesFile))
                    {
                        File.WriteAllLines(App.Preferences._preferencesFile, lines);
                    }
                    else
                    {
                        File.WriteAllLines("Preferences.setup", lines);
                        App.Preferences._preferencesFile = "Preferences.setup";
                    }
                    File.WriteAllText("Targets.setup", App.Preferences._preferencesFile);

                }
                catch (Exception)
                {
                    Console.WriteLine("[!] Could not write to preferences file!");
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
                                    if (!(line.Substring(2).Contains("\\") || line.Substring(2).Contains("/")))
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.ServerAddress = line.Substring(2);
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'S':
                                    if (!(line.Substring(2).Contains("\\") || line.Substring(2).Contains("/")))
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.SimDirectory = line.Substring(2);
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'M':
                                    string[] split = line.Split(':');
                                    if (!split[1].Contains("True") && !split[1].Contains("False"))
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.MultipleSims = split[1].Equals("True") ? true : false;
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'D':
                                    if (line.Length < 3)
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    App.Preferences.DriveLetter = line[2].ToString();
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;
                            }
                        }
                        App.Preferences.PreferencesFile = App.Preferences.PreferencesFile == null ? "App.Preferences.setup" : App.Preferences.PreferencesFile;
                        Console.WriteLine(Path.GetFullPath(App.Preferences.PreferencesFile));
                        if (PropertiesIncomplete)
                        {
                            Console.WriteLine("[*] Preferences value is not fully formatted.\n\t=> Did not disrupt loading.");
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] Preferences file fatally misformatted!\n\t=> Preferences loading terminated. {ex.Message}");
                        return false;
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("[!] Could not locate file at read time!");
                    return false;
                }

            });

        }


    }
}
