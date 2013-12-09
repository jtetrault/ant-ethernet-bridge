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
        const string VALID_SETTINGS_MESSAGE = "Setting must be one of: key, period, frequency, address, port, verbose";

        static void Main(string[] args)
        {
            byte[] networkKey;
            ushort channelPeriod;
            byte channelFrequency;
            IPAddress multicastAddress;
            ushort multicastPort;
            bool verbose;

            switch (args.Length)
            {
                case 0: // Run ANTBridge.
                    try
                    {
                        // Pull Channel Period settings from the settings file.
                        networkKey = BitConverter.GetBytes(UInt64.Parse(Properties.Settings.Default.NetworkKey, System.Globalization.NumberStyles.HexNumber));
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(networkKey);
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
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        Console.WriteLine("A Socket Exception has occured: " + Environment.NewLine + ex.Message);
                    }
                    Console.WriteLine("Exiting...");
                    break;

                case 2: // Change a configuration setting.
                    string setting = args[0];
                    string value = args[1];

                    try
                    {
                        string message;
                        // Determine which setting is being changed and parse the value.
                        switch (setting)
                        {
                            case "key":
                                networkKey = BitConverter.GetBytes(UInt64.Parse(value, System.Globalization.NumberStyles.HexNumber));
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(networkKey);
                                Properties.Settings.Default.NetworkKey = BitConverter.ToString(networkKey).Replace("-","");
                                message = "Network Key changed to " + BitConverter.ToString(networkKey);
                                break;

                            case "period":
                                channelPeriod = Convert.ToUInt16(value);
                                Properties.Settings.Default.ChannelPeriod = channelPeriod;
                                message = "Channel Period changed to " + channelPeriod;
                                break;

                            case "frequency":
                                channelFrequency = Convert.ToByte(value);
                                if (channelFrequency < 0 || channelFrequency > 124)
                                    throw new Exception("frequency must be in range 0 - 124");
                                Properties.Settings.Default.ChannelFrequency = channelFrequency;
                                message = "Channel Frequency changed to " + channelFrequency;
                                break;

                            case "address":
                                multicastAddress = IPAddress.Parse(value);
                                Properties.Settings.Default.MulticastAddress = value;
                                message = "Multicast Address changed to " + value;
                                break;

                            case "port":
                                multicastPort = Convert.ToUInt16(value);
                                Properties.Settings.Default.MulticastPort = multicastPort;
                                message = "Multicast Port changed to " + multicastPort;
                                break;

                            case "verbose":
                                Properties.Settings.Default.Verbose = Boolean.Parse(value);
                                message = "Verbose changed to " + Boolean.Parse(value);
                                break;

                            default:
                                message = String.Format("No setting matches {0}\n{1}", setting, VALID_SETTINGS_MESSAGE);
                                break;
                        }

                        Properties.Settings.Default.Save();
                        Console.WriteLine(message);
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException || ex is OverflowException)
                            Console.WriteLine("Value {1} provided for {0} is not valid", value, setting);
                        else
                            Console.WriteLine("Exception: {0}", ex.Message);
                    }
                    break;

                default: // Print usage statement.
                    string programName = Environment.GetCommandLineArgs()[0];
                    Console.WriteLine("Usage: {0} [setting value]", programName);
                    Console.WriteLine(VALID_SETTINGS_MESSAGE);
                    break;
            }
        }
    }
}
