using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace SceneryStream.src
{
    internal class LocalMachine
    {
        //vvvvvvvvvvvvvv//DEBUG VALUES//vvvvvvvvvvvvvv//
        private const string ADDRESS = "192.168.1.143";
        //^^^^^^^^^^^^^^//DEBUG VALUES//^^^^^^^^^^^^^^//
        private bool primary_connection_success;

        public Task<bool> platform_authenticity;
        public bool Connected { get; }

        public LocalMachine()
        {
            platform_authenticity = BuildServiceAuthenticity();
            Connected = primary_connection_success;
        }

        private async Task<bool> VerifyLocalPlatform()
        {
            PlatformID system = Environment.OSVersion.Platform;
            await Task.Delay(4000); //DON'T LEAVE THIS WHEN YOU ACTUALLY NEED TO DO IT!!!!
            Console.WriteLine(system);
            switch (system)
            {
                case PlatformID.Win32NT:
                    return true;
                default:
                    return false;
            }
        }

        public async Task<bool> BuildServiceAuthenticity()
        {
            Console.WriteLine("[*] Verifying local platform");
            Task<bool> verifyPlatform = VerifyLocalPlatform();
            Task<bool> pingServer = AttemptAddressPing(ADDRESS);

            bool platform_verification_success = await verifyPlatform;
            if (!platform_verification_success){
                throw new Exception("Could not verify platform");
            }
            bool ping_success = await pingServer;
            if (!ping_success)
            {
                Console.WriteLine("[*] Initial server connection could not be made.\nVerify target socket in connection settings.");
            } else
            {
                Task<bool> mount_location = PerformTargetLocationMounting(ADDRESS, "X", true);

                bool mount_success = await mount_location;
                if (mount_success)
                {
                    primary_connection_success = true;
                }
            }
            return true;
        }

        private async Task<bool> PerformTargetLocationMounting(string address, string drive, bool debug_window)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "net";
                p.StartInfo.Arguments = $"use {drive.ToUpper()}: \\\\{address}\\";
                p.StartInfo.CreateNoWindow = !debug_window;
                p.Start();
                p.WaitForExit();
                //string output = p.StandardOutput.ReadToEnd();
                p.Dispose();
                return true;
            }
            catch
            {
                Console.WriteLine("[!] Untraced error");
                return false;
            }
            
        }

        private async Task<bool> AttemptAddressPing(string address)
        {
            try
            {
                PingReply reply = new Ping().Send(address, 1000);
                if (reply != null)
                {
                    Console.WriteLine("[*] Ping Results \n\tStatus :  " + reply.Status + " \n\t Time : " + reply.RoundtripTime.ToString() + " \n\t Address : " + reply.Address);
                }
                return true;
            } 
            catch
            {
                Console.WriteLine("[!] Untraced connection error.");
                return false;
            }
            
        }
    }
}
