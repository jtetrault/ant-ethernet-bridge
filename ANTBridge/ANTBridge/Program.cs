using System;
using System.Collections.Generic;
using System.Linq;
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

            try
            {
                // Pull Channel Period settings from the settings file.
                networkKey = Convert.FromBase64String(Properties.Settings.Default.NetworkKey);
                channelPeriod = Properties.Settings.Default.ChannelPeriod;
                channelFrequency = Properties.Settings.Default.ChannelFrequency;

                ANTBridge bridge = new ANTBridge(networkKey, channelPeriod, channelFrequency);

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
