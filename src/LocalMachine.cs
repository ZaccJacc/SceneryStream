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

namespace SceneryStream.src
{
    public class LocalMachine
    {
        //vvvvvvvvvvvvvv//DEBUG VALUES//vvvvvvvvvvvvvv//
        private const string ADDRESS = @"192.168.1.230\zachary";
        //^^^^^^^^^^^^^^//DEBUG VALUES//^^^^^^^^^^^^^^//
        private bool primary_connection_success;
        public bool Connected { get { return primary_connection_success; } }

        private bool platform_authenticity;
        public bool Platform_Verified { get { return platform_authenticity; } }


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
            Task<bool> verifyPlatform = VerifyLocalPlatform();
            Task<bool> pingServer = AttemptAddressPing(ADDRESS);

            platform_authenticity = await verifyPlatform;

            bool ping_success = await pingServer;
            if (!ping_success)
            {
                Console.WriteLine("[!] Initial server connection could not be made.\nVerify target socket in connection settings."); //Replace with viewable output in final production
            } 
            else
            {
                Task<bool> attempt_mounting = Utility.Windows.System.PerformTargetLocationMounting(ADDRESS, "X");
                primary_connection_success = await attempt_mounting;
            }
        }


        private static async Task<bool> VerifyLocalPlatform() //this is throwing a warning - but don't be concerned, it means the contents ot this method will run sync. but that is fine.
        {
            PlatformID system = Environment.OSVersion.Platform;
            Console.WriteLine("[*] System platform: " + system); //DEBUG
            switch (system)
            {
                case PlatformID.Win32NT:
                    return true;
                default:
                    return false;
            }
        }

        private static async Task<bool> AttemptAddressPing(string address)
        {
            string[] brokenaddress = address.Split("\\");
            address = brokenaddress[0];
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
    namespace MacOS { }
    namespace Linux { }
    namespace Windows
    {
        internal class System
        {
            internal static async Task<bool> PerformTargetLocationMounting(string address, string drive) //the only purpose of this method is to plinky plonk the actual moutning through an async task :p
            {
                Console.WriteLine("[*] Attempting target mounting");
                try
                {
                    return await Task.Run(async () =>
                    {
                        try
                        {
                            NetworkDrive.MapNetworkDrive(drive, address); // add a way to see if it actually has done it right?
                            return true;
                        }
                        catch
                        {
                            Console.WriteLine("[!] Untraced mounting error.");
                            return false;
                        }
                    /*Process p = new Process();
                    await Task.Delay(13000).ConfigureAwait(false);
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = "cmd";
                    p.StartInfo.Arguments = $"/K net use {drive.ToUpper()}: \\\\{address}"; //Might need some better formatting surrounding this address...
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    p.Start();
                    p.WaitForExit();
                    string output = p.StandardOutput.ReadToEnd();
                    Console.WriteLine(output);
                    p.Dispose();
                    Console.WriteLine("[|] Target mounting complete");
                    return true;*/
                    }).WaitAsync(TimeSpan.FromMilliseconds(12000)); //The drive mounting has 12 seconds to complete, or the task will timeout.
                }
                catch(TimeoutException e)
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
            [DllImport("mpr.dll")]
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
        }
    }
}

