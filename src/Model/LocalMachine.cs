#pragma warning disable CS1998
using Avalonia.Controls;
using SceneryStream.src.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    public class LocalMachine
    {
        private bool primary_connection_success;
        public bool Connected { get { return primary_connection_success; } }

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
        /// Attempt to mount the target server location specified in the application configuration settings (TBC)
        /// </summary>
        public async Task BuildServiceAuthenticity()
        {
            Console.WriteLine("[*] Verifying local platform");
            new Thread(async () =>
            {
                Task<(bool, PlatformID)> platformData = VerifyLocalPlatform();
                (bool, PlatformID) platformdata = await platformData;
                platform_authenticity = platformdata.Item1;
                platform = platformdata.Item2;
            }).Start();
            try
            {
                App.Preferences.PreferencesFile = File.ReadAllText("Targets.Setup");
                Console.WriteLine("[*] Preferences file found.");
                Task<bool> loadPreferenceSuccess = PreferencesModel.loadPreferences(App.Preferences.PreferencesFile);
                if (await loadPreferenceSuccess)
                {
                    Task<bool> pingServer = AttemptAddressPing(App.Preferences.ServerAddress);
                    if (!await pingServer)
                    {
                        Console.WriteLine("[!] Initial server connection could not be made.\n\tVerify target socket in connection settings."); //Replace with viewable output in final production
                    }
                    else
                    {
                        switch (platform)
                        {
                            case PlatformID.Win32NT:
                                Task<bool> attempt_mounting = Utility.Windows.PerformTargetLocationMounting(App.Preferences.ServerAddress, App.Preferences.DriveLetter, 0);
                                primary_connection_success = await attempt_mounting;
                                if (primary_connection_success)
                                {
                                    await Task.Run(async () =>
                                    {
                                        Console.WriteLine("[*] Trying to make shortcuts");
                                        Utility.Windows.createShortcut(App.Preferences.DriveLetter, App.Preferences.SimDirectory+@"\Custom Scenery", "airports"); //This will need to be changed at some point to mount for all the different scenery the user has selected. For now, everything though.
                                        //Currently forced to only airports because the server only has airports :p
                                    });
                                }  
                                break;

                            case PlatformID.Unix:
                                Console.WriteLine("[!] Drive mounting is currently only available on Windows.");
                                break;
                        }


                    }
                }

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("[!] Could not locate preferences/target file.");
            }
        }


        private static async Task<(bool, PlatformID)> VerifyLocalPlatform()
        {
            PlatformID system = Environment.OSVersion.Platform;
            Console.WriteLine("[*] System platform: " + system); //DEBUG
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
                Console.WriteLine($"[!] Server address is not formatted correctly!\n\t=> Address: {address}");
            }

            try
            {
                PingReply reply = new Ping().Send(address, 1000);
                if (reply != null)
                {
                    Console.WriteLine("[*] Ping Results \n\tStatus :  " + reply.Status + " \n\t Time : " + reply.RoundtripTime.ToString() + " \n\t Address : " + reply.Address); //DBUG
                }

                return true;
            }
            catch
            {
                Console.WriteLine("[!] Untraced connection error."); //DBUG
                return false;
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
            switch (callback)
            {
                case "File":
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Title = "Select File";
                    string[]? filePath = await fileDialog.ShowAsync(new Window());
                    try
                    {
                        string resultFile = "";
                        foreach (string s in filePath) //this doesn't work because the browser keeps returning a null path before something has been chosen
                        {
                            resultFile += s;
                        }
                        return resultFile;
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("[!] No path selected");
                        return "";
                    }


                case "Directory":
                    OpenFolderDialog simDialog = new OpenFolderDialog();
                    try
                    {
                        string result =  await simDialog.ShowAsync(new Window());
                        if (!result.Contains("\\"))
                        {
                            Console.WriteLine("[!] No directory selected!");
                            return "";
                        }
                        else
                        {
                            return result;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("[!] No directory selected!");
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
    class Windows
    {
        internal static async void createShortcut(string link_path, string link_location, string scenery_type) //code from stackoverflow, credit to Simon Mourier.
        {
            await Task.Run(async () =>
            {
                Console.WriteLine("wapwap");
                try
                {
                    IShellLink link = (IShellLink)new ShellLink();

                    // setup shortcut information
                    link.SetDescription($"XSS Mount for {scenery_type}");
                    link.SetPath($@"{link_path}:\");

                    // save it
                    IPersistFile file = (IPersistFile)link;
                    switch (scenery_type)
                    {
                        case "ortho":
                            file.Save(Path.Combine(link_location, "zOrtho_xss_mount.lnk"), false);
                            Console.WriteLine("\t=> Made ortho shortcut");
                            break;

                        case "airports":
                            file.Save(Path.Combine(link_location, "airports_xss_mount.lnk"), false);
                            Console.WriteLine("\t=> Made airports shortcut");
                            break;
                    }
                }
                catch(Exception) 
                {
                    Console.WriteLine("[!] Could not create shortcuts!");
                }
                
                

            });
            
        }




        internal static async Task<bool> PerformTargetLocationMounting(string address, string drive, int processType)
        {
            Console.WriteLine("[*] Attempting target mounting");
            try
            {
                return await Task.Run(async () =>
                {
                    try
                    {
                        switch (processType)
                        {
                            case 1:
                                NetworkDrive.MapNetworkDrive(drive, address);
                                Console.WriteLine("Directories: " + Directory.GetDirectories("X"));
                                return NetworkDrive.IsDriveMapped(drive);


                            default:
                                return NetworkDrive.MapDriveByConsole(drive, address);

                        }

                    }
                    catch
                    {
                        Console.WriteLine("[!] Untraced mounting error.");
                        return false;
                    }
                }).WaitAsync(TimeSpan.FromMilliseconds(12000)); //The drive mounting has 12 seconds to complete, or the task will timeout.
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[!] Drive mounting timed out!");
                return false;
            }

        }
    }

    public class NetworkDrive //Code sourced from StackOverflow (obviously) from users Mario and Mat
    {

        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }
        private enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }
        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010
        }
        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NETRESOURCE
        {
            public ResourceScope oResourceScope;
            public ResourceType oResourceType;
            public ResourceDisplayType oDisplayType;
            public ResourceUsage oResourceUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComments;
            public string sProvider;
        }
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetAddConnection2
            (ref NETRESOURCE oNetworkResource, string sPassword,
            string sUserName, int iFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2
            (string sLocalName, uint iFlags, int iForce);

        public static void MapNetworkDrive(string sDriveLetter, string sNetworkPath)
        {
            //Checks if the last character is \ as this causes error on mapping a drive.
            if (sNetworkPath.Substring(sNetworkPath.Length - 1, 1) == @"\")
            {
                sNetworkPath = sNetworkPath.Substring(0, sNetworkPath.Length - 1);
            }

            NETRESOURCE oNetworkResource = new NETRESOURCE()
            {
                oResourceType = ResourceType.RESOURCETYPE_DISK,
                sLocalName = sDriveLetter + ":",
                sRemoteName = sNetworkPath
            };

            //If Drive is already mapped disconnect the current 
            //mapping before adding the new mapping
            if (IsDriveMapped(sDriveLetter))
            {
                DisconnectNetworkDrive(sDriveLetter, true);
            }

            WNetAddConnection2(ref oNetworkResource, null, null, 0);
        }

        public static int DisconnectNetworkDrive(string sDriveLetter, bool bForceDisconnect)
        {
            if (bForceDisconnect)
            {
                return WNetCancelConnection2(sDriveLetter + ":", 0, 1);
            }
            else
            {
                return WNetCancelConnection2(sDriveLetter + ":", 0, 0);
            }
        }

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
            foreach(string s in Environment.GetLogicalDrives())
            {
                if (s.ToUpper().Equals(drive.ToUpper())){
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
                    CreateNoWindow = false,
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
                Console.WriteLine(ex.Message);
                return false;

            }

        }

        internal static void RemoveDriveByConsole(string drive)
        {
            Process process = new Process();
            try
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    FileName = $"net",
                    Arguments = $"use {drive}: /delete"
                };

                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                if (!output.Contains("success"))
                {
                    Console.WriteLine("[!] Could not remove mounted drive!");
                    throw new Exception("\t=> Drive removal failed");
                }
                process.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                process.Dispose();
            }

        }
    }
}

