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
        /// <summary>
        /// The length of an ANT message.
        /// An ANT message consists of an 8-byte payload followed by a 4-byte device id.
        /// </summary>
        private const short ANT_MESSAGE_LENGTH = 12;

        static void Main(string[] args)
        {
            try
            {
                UdpClient client = new UdpClient();
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.multicastPort);
                client.Client.Bind(localEp);

                IPAddress multicastAddress = IPAddress.Parse(Properties.Settings.Default.multicastAddress);
                client.JoinMulticastGroup(multicastAddress);

                byte[] message = new byte[ANT_MESSAGE_LENGTH];

                Console.WriteLine("Listening for Multicast Messages on {0}:{1}\n",
                    Properties.Settings.Default.multicastAddress,
                    Properties.Settings.Default.multicastPort);

                while (true)
                {
                    byte[] tempMessage = client.Receive(ref localEp);

                    // Clear out the message buffer, and then write at most message.Length bytes into it.
                    // Then print out the message using ANTResponseFormatter.
                    Array.Clear(message, 0, message.Length);
                    Array.Copy(tempMessage, message, Math.Max(tempMessage.Length, message.Length));
                    try
                    {
                        Console.WriteLine(ANTResponseFormatter.Formatter.FormatMessage(message));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine("A Socket Exception has occured: " + Environment.NewLine + ex.Message);
            }
        }
    }
}
