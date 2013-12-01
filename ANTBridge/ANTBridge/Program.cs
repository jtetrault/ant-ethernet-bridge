using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using ANT_Managed_Library;

namespace ANTBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] networkKey;
            ushort channelPeriod;
            byte channelFrequency;
            IPAddress multicastAddress;
            ushort multicastPort;
            bool verbose;

            try
            {
                // Pull Channel Period settings from the settings file.
                networkKey = Convert.FromBase64String(Properties.Settings.Default.NetworkKey);
                channelPeriod = Properties.Settings.Default.ChannelPeriod;
                channelFrequency = Properties.Settings.Default.ChannelFrequency;
                multicastAddress = IPAddress.Parse(Properties.Settings.Default.MulticastAddress);
                multicastPort = Properties.Settings.Default.MulticastPort;
                verbose = Properties.Settings.Default.Verbose;

                ANTBridge bridge = new ANTBridge(networkKey, channelPeriod, channelFrequency, multicastAddress, multicastPort, verbose);

                while (true)
                {

                }
            }
            catch (ANT_Exception ex)
            {
                Console.WriteLine("An ANT Exception has occured: " + Environment.NewLine + ex.Message);
            }
            Console.WriteLine("Exiting...");
        }
    }
}
