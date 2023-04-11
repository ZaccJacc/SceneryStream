using System;
using System.IO;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal struct Preferences
    {
        public static string? SimDirectory { get; set; }

        public static string? ServerAddress { get; set; }

        public static bool MultipleSims { get; set; }

        public static string? DriveLetter { get; set; }

        public static string? PreferencesFile { get; set; }
    }

    internal class PreferencesModel
    {
        public PreferencesModel() { }

        public static async Task<bool> loadPreferences(string fileName) //Still requires the error detection method to be changed from terminating because of an exception, to setting a flag variable that is checked after so other values can be loaded.
        {
            Preferences.PreferencesFile = fileName;
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
                                    Preferences.ServerAddress = line.Substring(2);
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;

                                case 'S':
                                    if (!(line.Substring(2).Contains("\\") || line.Substring(2).Contains("/")))
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    Preferences.SimDirectory = line.Substring(2);
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
                                    Preferences.MultipleSims = split[1].Equals("True") ? true : false;
                                    break;

                                case 'D':
                                    if (line.Length < 2)
                                    {
                                        PropertiesIncomplete = true;
                                        Console.WriteLine($"[!] Could not load value {line}\n\t=> Preferences loading will resume");
                                        break;
                                    }
                                    Preferences.DriveLetter = line[2].ToString();
                                    Console.WriteLine($"[*] Preferences value: {line} Loaded");
                                    break;
                            }
                        }
                        Preferences.PreferencesFile = Preferences.PreferencesFile == null ? "Preferences.setup" : Preferences.PreferencesFile;
                        Console.WriteLine(Path.GetFullPath(Preferences.PreferencesFile));
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
                lines[0] = Preferences.ServerAddress != null ? $"A-{Preferences.ServerAddress}" : $"A-{null}";
                lines[1] = Preferences.SimDirectory != null ? $"S-{Preferences.SimDirectory}" : $"S-{null}"; //Add this binding to the right window so the preferences can autosave.
                lines[2] = $"D-{Preferences.DriveLetter}";
                lines[3] = $"M-Multisim:{Preferences.MultipleSims}";
                if (Preferences.PreferencesFile != null && File.Exists(Preferences.PreferencesFile))
                {
                    File.WriteAllLines(Preferences.PreferencesFile, lines);
                }
                else
                {
                    File.WriteAllLines("Preferences.setup", lines);
                    Preferences.PreferencesFile = "Preferences.setup";
                }
                File.WriteAllText("Targets.setup", Preferences.PreferencesFile);

            }
            catch (Exception)
            {
                Console.WriteLine("[!] Could not write to preferences file!");
            }
        }
    }
}
