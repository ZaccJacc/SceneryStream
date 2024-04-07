using Avalonia.Platform.Storage;
using SceneryStream;
using SceneryStream.src.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class LocalMachine : ObservableObject
    {
        private bool primary_connection_success;
        public bool Connected {
            get => primary_connection_success;

            set
            {
                primary_connection_success = value;
                NotifyPropertyChanged(nameof(Connected));
                HomeViewModel.HViewModel.ReviewConnecionStatusIndicator();
                HomeViewModel.ScanNewUpdates();
                HomeViewModel.RefreshScenerySpotlight();
            }
        }

        private PingReply _serverPingReply;
        public PingReply ServerPingReply
        {
            get => _serverPingReply;
            set
            {
                _serverPingReply = value;
                NotifyPropertyChanged(nameof(ServerPingReply));
            }
        }

        private string _pingReplyTime = "-";
        public string PingReplyTime
        {
            get => _pingReplyTime;
            set
            {
                _pingReplyTime = value;
                NotifyPropertyChanged(nameof(PingReplyTime));
            }
        }

        private bool platform_authenticity;
        public bool Platform_Verified { get { return platform_authenticity; } }

        private PlatformID platform;
        public PlatformID Platform { get { return platform; } }


        /// <summary>
        /// System used for platform verification and non-user-controlled functionality.
        /// </summary>
        public LocalMachine() { }

        /// <summary>
        /// Verify the operating system that .NET is running on so that the correct method for target server mounting can be used.<br/>
        /// Verify that the target socket is responsive.<br/>
        /// Attempt to mount the target server location specified in the application configuration settings.
        /// </summary>
        public async Task BuildServiceAuthenticity()
        {
            Debug.WriteLine("[*] Verifying local platform");
            new Thread(async () =>
            {
                Task<(bool, PlatformID)> platformData = VerifyLocalPlatform();
                (bool, PlatformID) platformdata = await platformData;
                platform_authenticity = platformdata.Item1;
                platform = platformdata.Item2;
            }).Start();
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SceneryStream/Data/Preferences.setup"))
                        {
                            File.Create($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SceneryStream/Data/Preferences.setup");
                            Debug.WriteLine("[*] Created new preferences file.");
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("[!] Could not create preferences file!\n\t=> Connection will not be attempted.");
                        return;
                    }
                    App.Preferences.PreferencesFile = ($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SceneryStream/Data/Preferences.setup");
                    Debug.WriteLine("[*] Preferences file located.");
                    Task<bool> loadPreferenceSuccess = PreferencesModel.loadPreferences(App.Preferences.PreferencesFile); //use preferences model to load the preferences from the file.
                    if (await loadPreferenceSuccess)
                    {
                        await MakeConnection(); //connect to the server if the preferences were not fatally malformed and there is a server address provided.
                    }
                    LoadDataPostConnection();
                });
                
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[!] Fatal error in initialising application resources.\n\t=> {e.Message}");
            }
        }


        private static async Task<(bool, PlatformID)> VerifyLocalPlatform()
        {
            PlatformID system = Environment.OSVersion.Platform;
            Debug.WriteLine("[*] System platform: " + system); //DEBUG
            switch (system)
            {
                case PlatformID.Win32NT:
                    return (true, system);
                default:
                    return (false, system);
            }
        }

        private static async Task<bool> AttemptAddressPing(string address)
        {
            try
            {
                string[] brokenaddress = address.Split("\\");
                address = brokenaddress[2];
            }
            catch (Exception)
            {
                Debug.WriteLine($"[!] Server address is not formatted correctly!\n\t=> Address: {address}");
            }

            try
            {
                App.ServiceInstance.ServerPingReply = new Ping().Send(address, 1000);
                App.ServiceInstance.PingReplyTime = App.ServiceInstance.ServerPingReply.RoundtripTime.ToString();
                if (App.ServiceInstance.ServerPingReply != null)
                {
                    Debug.WriteLine("[*] Ping Results \n\tStatus :  " + App.ServiceInstance.ServerPingReply.Status + " \n\t Time : " + App.ServiceInstance.PingReplyTime + " \n\t Address : " + App.ServiceInstance.ServerPingReply.Address); //DBUG
                }
                return true;
            }
            catch
            {
                Debug.WriteLine("[!] Untraced connection error."); //DBUG
                App.ServiceInstance.PingReplyTime = "-";
                return false;
            }
        }

        internal async Task MakeConnection()
        {
            Task<bool> pingServer = AttemptAddressPing(App.Preferences.ServerAddress);
            if (!await pingServer)
            {
                Debug.WriteLine("[!] Initial server connection could not be made.\n\tVerify target socket in connection settings."); //Replace with viewable output in final production
                Connected = false;
            }
            else
            {
                switch (Platform)
                {
                    case PlatformID.Win32NT:
                        Task<bool> attempt_mounting = Utility.Win32.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter, 0);
                        Connected = await attempt_mounting;
                        break;

                    case PlatformID.Unix:
                        Task<bool> attempt_unix_mounting = Utility.Unix.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter);
                        Connected = await attempt_unix_mounting;
                        break;
                }
            }
        }

        internal async void LoadDataPostConnection()
        {
            if (Connected)
            {
                await ServerFormat.Format.LoadServerConfiguration(string.Empty);
                RegionHandling.Regions.LoadSelectedRegions();
                RegionHandling.Regions.GenerateShellLinks(string.Empty);
            }
        }
    }
}




namespace Utility
{
    [ComImport] //Simon Mourier, Stack Overflow
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    class FileBrowser
    {

        internal static async Task<object?> produceBrowser(string callback)
        {
            switch (callback.ToUpper())
            {
                case "F":
                case "FILE":
                    try
                    {
                        var result = await App.Storage.OpenFilePickerAsync(new FilePickerOpenOptions()
                        {
                            Title = "Select a file.",
                            AllowMultiple = false
                        });
                        return result[0].TryGetLocalPath();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("[!] No could not select file");
                        return "";
                    }

                case "D":
                case "DIRECTORY":
                    try
                    {
                        var result = await App.Storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                        {
                            Title = "Choose a directory.",
                            AllowMultiple = false
                        });
                        
                        if (result.Count < 1)
                        {
                            Debug.WriteLine("[!] No directory selected!");
                            return "";
                        }
                        else
                        {
                            return result[0].TryGetLocalPath();
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("[!] Could not select directory!");
                        return "";
                    }    
            }
            return "";
        }

        internal static async Task<object?> produceBrowser(string callback, string startpoint)
        {
            switch (callback.ToUpper())
            {
                case "F":
                case "FILE":
                    try
                    {
                        var result = await App.Storage.OpenFilePickerAsync(new FilePickerOpenOptions()
                        {
                            Title = "Select a file.",
                            AllowMultiple = false,
                            SuggestedStartLocation = await App.Storage.TryGetFolderFromPathAsync(startpoint)
                        });
                        return result[0].TryGetLocalPath();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("[!] No could not select file");
                        return "";
                    }


                case "D":
                case "DIRECTORY":
                    try
                    {
                        var result = await App.Storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                        {
                            Title = "Choose a directory.",
                            AllowMultiple = false,
                            SuggestedStartLocation = await App.Storage.TryGetFolderFromPathAsync(startpoint)
                        });

                        if (result.Count < 1)
                        {
                            Debug.WriteLine("[!] No directory selected!");
                            return "";
                        }
                        else
                        {
                            return result[0].TryGetLocalPath();
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("[!] Could not select directory!");
                        return "";
                    }
            }
            return "";
        }
    }

    class Unix
    {
        internal static async Task<bool> PerformTargetLocationMounting(string address, string drive)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            if (Directory.Exists($"/mnt/{drive}"))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = $"{projectDirectory}/src/Script/Unmount.sh",
                    Arguments = $"{drive}"
                });
            }
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = $"{projectDirectory}/src/Script/Mount.sh",
                    Arguments = $"{address} {drive}"
                };
                Process process = Process.Start(processStartInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    class Win32
    {
        internal static async Task<string> createShortcut(string mounted_drive,string link_path, string link_location, string scenery_type, string link_name)
        {
            return await Task.Run(() =>
            {
                Debug.WriteLine("[*] Attempting shell link.");
                try
                {
                    IShellLink link = (IShellLink)new ShellLink();

                    // setup shortcut information
                    link.SetDescription($"XSS Mount for {scenery_type}");
                    link.SetPath($@"{mounted_drive}:\{link_path}");
                    

                    // save it
                    IPersistFile file = (IPersistFile)link;
                    switch (scenery_type.ToLower())
                    {
                        case "o":
                        case "ortho":
                            file.Save(Path.Combine(link_location, $"zOrtho - XSS - {link_name}.lnk"), false);
                            Debug.WriteLine("\t=> Made ortho shortcut");
                            return Path.Combine(link_location, $"zOrtho - XSS - {link_name}.lnk");

                        case "a":
                        case "airport":
                            file.Save(Path.Combine(link_location, $"Airport - XSS - {link_name}.lnk"), false);
                            Debug.WriteLine("\t=> Made airport shortcut");
                            return Path.Combine(link_location, $"Airport - XSS-{link_name}.lnk");

                        case "l":
                        case "library":
                            file.Save(Path.Combine(link_location, $"Lib - XSS - {link_name}.lnk"), false);
                            Debug.WriteLine("\t=> Made library shortcut");
                            return Path.Combine(link_location, $"Lib - XSS - {link_name}.lnk");

                        default:
                            return "";
                    }
                }
                catch(Exception e) 
                {
                    Debug.WriteLine($"[!] Could not create shortcuts!\n\t=> {e.Message}");
                    return "";
                }
            });
        }




        internal static async Task<bool> PerformTargetLocationMounting(string address, string drive, int processType)
        {
            Debug.WriteLine("[*] Attempting target mounting");
            try
            {
                return await Task.Run(async () =>
                {
                    try
                    {
                        switch (processType)
                        {
                            case 1:
                                //NetworkDrive.MapNetworkDrive(drive, address);
                                throw new NotImplementedException();
                                Debug.WriteLine("Directories: " + Directory.GetDirectories("X"));
                                return NetworkDrive.IsDriveMapped(drive);


                            default:
                                return NetworkDrive.MapDriveByConsole(drive, address);

                        }

                    }
                    catch
                    {
                        Debug.WriteLine("[!] Untraced mounting error.");
                        return false;
                    }
                }).WaitAsync(TimeSpan.FromMilliseconds(12000)); //The drive mounting has 12 seconds to complete, or the task will timeout.
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("[!] Drive mounting timed out!");
                return false;
            }

        }
    }

    public class NetworkDrive
    {

        public static bool IsDriveMapped(string sDriveLetter)
        {
            string[] DriveList = Environment.GetLogicalDrives();
            for (int i = 0; i < DriveList.Length; i++)
            {
                if (sDriveLetter + ":\\" == DriveList[i].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool MapDriveByConsole(string drive, string address)
        {
            Debug.WriteLine("[*] Attempting to map drive");
            foreach(string s in Environment.GetLogicalDrives())
            {
                if (char.ToUpper(s[0]).ToString().Equals(drive.ToUpper())){
                    Debug.WriteLine($"[*] Drive conflict found!\n\t=> Will override {drive}");
                    RemoveDriveByConsole(drive);
                }
            }

            Process process = new Process();
            string output;
            try
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = $"net",
                    Arguments = $"use {drive}: {address} password /USER:Guest"
                };
                process.Start();
                process.WaitForExit();
                output = process.StandardOutput.ReadToEnd();
                process.Dispose();
                return output.Contains("success");
            }
            catch (Exception ex)
            {
                process.Dispose();
                Debug.WriteLine(ex.Message);
                return false;

            }

        }

        internal static void RemoveDriveByConsole(string drive)
        {
            Debug.WriteLine($"[*] Attempting to remove {drive}");
            Process process = new Process();
            try
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = $"net",
                    Arguments = $"use {drive}: /delete /y"
                };

                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                if (!output.Contains("success"))
                {
                    Debug.WriteLine("[!] Could not remove mounted drive!");
                    throw new Exception("\t=> Drive removal failed");
                }
                process.Dispose();
                App.ServiceInstance.Connected = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                process.Dispose();
            }

        }
    }
}

