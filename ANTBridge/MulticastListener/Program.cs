using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using ANTResponseFormatter;

namespace MulticastListener
{
    /// <summary>
    /// Connects to a multicast group and prints ANT messages received from the group to Console.
    /// </summary>
    class Program
    {
        /*********************************************************************/
        /*** Class Variables and Constants ***********************************/
        /*********************************************************************/
        /// <summary>
        /// A list of the valid settings that can be modified.
        /// </summary>
        const string VALID_SETTINGS_MESSAGE = "<setting> must be one of: address, port";

        /// <summary>
        /// The length of an ANT message.
        /// An ANT message consists of an 8-byte payload followed by a 4-byte device id.
        /// </summary>
        private const short ANT_MESSAGE_LENGTH = 12;

        /// <summary>
        /// Main program entry point.
        /// Runs MulticastListener, or allows for a setting to be changed.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            switch(args.Length)
            {
                case 2: // Change a setting.
                    ChangeSetting(args[0], args[1]);
                    break;

                case 0: // Run MulticastListener.
                    
                    try
                    {
                        // Setup a UDP client for receiving multicast messages.
                        UdpClient client = new UdpClient();
                        IPEndPoint localEp = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.MulticastPort);
                        client.Client.Bind(localEp);

                        IPAddress multicastAddress = IPAddress.Parse(Properties.Settings.Default.MulticastAddress);
                        client.JoinMulticastGroup(multicastAddress);

                        byte[] message = new byte[ANT_MESSAGE_LENGTH];

                        Console.WriteLine("Listening for Multicast Messages on {0}:{1}\n",
                            Properties.Settings.Default.MulticastAddress,
                            Properties.Settings.Default.MulticastPort);

                        // Receive and print ANT messages until the program is killed.
                        while (true)
                        {
                            byte[] tempMessage = client.Receive(ref localEp);

                            // Clear out the message buffer, and then write at most message.Length bytes into it.
                            // Then print out the message using ANTResponseFormatter.
                            Array.Clear(message, 0, message.Length);
                            Array.Copy(tempMessage, message, Math.Max(tempMessage.Length, message.Length));
                            Console.WriteLine(ANTResponseFormatter.Formatter.FormatMessage(message));
                        }
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        Console.WriteLine("A Socket Exception has occured: " + Environment.NewLine + ex.Message);
                    }
                    break;

                default:// Print usage statement.
                    string programName = Environment.GetCommandLineArgs()[0];
                    Console.WriteLine("Usage: {0} [<setting> <value>]", programName);
                    Console.WriteLine(VALID_SETTINGS_MESSAGE);
                    break;
            }
        }

        /// <summary>
        /// Change a setting to the given value. Will not succeed if the setting or value are invalid.
        /// </summary>
        private static void ChangeSetting(string setting, string value)
        {
            try
            {
                string message;
                // Determine which setting is being changed and parse the value.
                switch (setting)
                {
                    case "address":
                        IPAddress multicastAddress = IPAddress.Parse(value);
                        Properties.Settings.Default.MulticastAddress = value;
                        message = "Multicast Address changed to " + value;
                        break;

                    case "port":
                        ushort multicastPort = Convert.ToUInt16(value);
                        Properties.Settings.Default.MulticastPort = multicastPort;
                        message = "Multicast Port changed to " + multicastPort;
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
        }
    }
}
