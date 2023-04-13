using ReactiveUI;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    public class Preferences : ReactiveObject
    {
        public Preferences() 
        {
            this.WhenAnyValue(x => x.SimDirectory, x => x.ServerAddress, x => x.MultipleSims, x => x.DriveLetter, x => x.PreferencesFile)
                .Subscribe(x => 
                {
                    SimDirectory = x.Item1;
                    ServerAddress = x.Item2;
                    MultipleSims= x.Item3;
                    DriveLetter= x.Item4;
                    PreferencesFile = x.Item5;
                });
        }

        private string? _simDirectory;
        public string? SimDirectory { get => _simDirectory; set => this.RaiseAndSetIfChanged(ref _simDirectory, value); }

        private string? _serverAddress;
        public string? ServerAddress { get => _serverAddress; set => this.RaiseAndSetIfChanged(ref _serverAddress, value); }

        private bool _multipleSims;
        public bool MultipleSims { get => _multipleSims; set => this.RaiseAndSetIfChanged(ref _multipleSims, value); }

        private string? _driveLetter;
        public string? DriveLetter { get => _driveLetter; set=> this.RaiseAndSetIfChanged(ref _driveLetter, value); }

        private string? _preferencesFile;
        public string? PreferencesFile { get => _preferencesFile; set => this.RaiseAndSetIfChanged(ref _preferencesFile, value); }

    }

    internal class PreferencesModel
    {
        public PreferencesModel() { }

        public static async Task<bool> loadPreferences(string fileName) //Still requires the error detection method to be changed from terminating because of an exception, to setting a flag variable that is checked after so other values can be loaded.
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
                                    break;

                                case 'D':
                                    if (line.Length < 2)
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
                        Console.WriteLine($"[!] Preferences file fatally misformatted!\n\t=> Preferences loading terminated.");
                        return false;
                    }
                    
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("[!] Could not locate file at read time!");
                    return false;
                }
                
            });

        }

        public static async Task savePreferences()
        {
            try
            {
                string[] lines = new string[4];
                lines[0] = App.Preferences.ServerAddress != null ? $"A-{App.Preferences.ServerAddress}" : $"A-{null}";
                lines[1] = App.Preferences.SimDirectory != null ? $"S-{App.Preferences.SimDirectory}" : $"S-{null}"; //Add this binding to the right window so the preferences can autosave.
                lines[2] = $"D-{App.Preferences.DriveLetter}";
                lines[3] = $"M-Multisim:{App.Preferences.MultipleSims}";
                if (App.Preferences.PreferencesFile != null && File.Exists(App.Preferences.PreferencesFile))
                {
                    File.WriteAllLines(App.Preferences.PreferencesFile, lines);
                }
                else
                {
                    File.WriteAllLines("App.Preferences.setup", lines);
                    App.Preferences.PreferencesFile = "App.Preferences.setup";
                }
                File.WriteAllText("Targets.setup", App.Preferences.PreferencesFile);

            }
            catch (Exception)
            {
                Console.WriteLine("[!] Could not write to preferences file!");
            }
        }
    }
}
