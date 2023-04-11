#pragma warning disable CS1998
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Tmds.DBus;
using System.Drawing;
using System.Net;

namespace SceneryStream.src.Model
{
    public class LocalMachine
    {
        private bool primary_connection_success;
        public bool Connected { get { return primary_connection_success; } }

        private bool platform_authenticity;
        public bool Platform_Verified { get { return platform_authenticity; } }

        private PlatformID platform;
        public PlatformID Platform { get { return platform;  } }


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
                Preferences.PreferencesFile = File.ReadAllText("Targets.Setup");
                Console.WriteLine("[*] Preferences file found.");
                Task<bool> loadPreferenceSuccess = PreferencesModel.loadPreferences(Preferences.PreferencesFile);
                if (await loadPreferenceSuccess)
                {
                    Task<bool> pingServer = AttemptAddressPing(Preferences.ServerAddress);
                    if (!await pingServer)
                    {
                        Console.WriteLine("[!] Initial server connection could not be made.\n\tVerify target socket in connection settings."); //Replace with viewable output in final production
                    }
                    else
                    {
                        switch (platform)
                        {
                            case PlatformID.Win32NT:
                                Task<bool> attempt_mounting = Utility.Windows.PerformTargetLocationMounting(Preferences.ServerAddress, Preferences.DriveLetter, 0);
                                primary_connection_success = await attempt_mounting;
                                break;

                            case PlatformID.Unix:
                                Console.WriteLine("[!] Drive mounting is currently only available on Windows.");
                                break;
                        }
                        
                       
                    }
                }

            } catch (FileNotFoundException)
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
                Console.WriteLine($"[!] Server address is not formatted correctly!\nAddress: {address}");
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
    class FileBrowser
    {

        ObservableCollection<Node> Items { get; }
        ObservableCollection<Node> SelectedItems { get; }
        public string strFolder { get; set; }

        public FileBrowser()
        {
            strFolder = @"A:"; // EDIT THIS FOR AN EXISTING FOLDER

            Items = new ObservableCollection<Node>();

            Node rootNode = new Node(strFolder);
            rootNode.Subfolders = GetSubfolders(strFolder);

            Items.Add(rootNode);
        }

        public ObservableCollection<Node> GetSubfolders(string strPath)
        {
            ObservableCollection<Node> subfolders = new ObservableCollection<Node>();
            string[] subdirs = Directory.GetDirectories(strPath, "*", SearchOption.TopDirectoryOnly);

            foreach (string dir in subdirs)
            {
                Node thisnode = new Node(dir);

                try
                {
                    if (Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly).Length > 0)
                    {
                        thisnode.Subfolders = new ObservableCollection<Node>();

                        thisnode.Subfolders = GetSubfolders(dir);
                    }
                }
                catch (Exception e) { }


                subfolders.Add(thisnode);
            }

            return subfolders;
        }

        public class Node
        {
            public ObservableCollection<Node> Subfolders { get; set; }

            public string strNodeText { get; }
            public string strFullPath { get; }

            public Node(string _strFullPath)
            {
                strFullPath = _strFullPath;
                strNodeText = Path.GetFileName(_strFullPath);
            }
        }

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
                        Console.WriteLine("No path selected");
                        return "";
                    }


                case "Directory":
                    OpenFolderDialog simDialog = new OpenFolderDialog();
                    return await simDialog.ShowAsync(new Window());
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
            if(Environment.GetLogicalDrives().Contains(drive))
            {
                RemoveDriveByConsole(drive);
            }

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            Process process = new Process();
            string output;
            try
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    FileName = $"{projectDirectory}/src/Script/WinMount.cmd",
                    Arguments = $"{drive} {address}"
                };
                process.Start();
                process.WaitForExit();
                output = process.StandardOutput.ReadToEnd();
                process.Dispose();
                return output.Contains("success");
            } 
            catch(Exception ex)
            {
                process.Dispose();
                Console.WriteLine(ex.Message);
                return false;

            }
            
        }

        internal static void RemoveDriveByConsole(string drive)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            Process process = new Process();
            try
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    FileName = $"{projectDirectory}/src/Script/WinUnmount.cmd",
                    Arguments = $"{drive}"
                };

                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                if(!output.Contains("success"))
                {
                    Console.WriteLine("[!] Could not remove mounted drive!");
                }
                process.Dispose();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"[!] {ex.Message}");
                process.Dispose();
            }
            
        }
    }
}

