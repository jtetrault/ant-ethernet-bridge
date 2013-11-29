using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ANTBridge
{
    class MulticastSender
    {
        /*********************************************************************/
        /*** Instance Methods ************************************************/
        /*********************************************************************/

        /// <summary>
        /// Initialize a UDP connection to a multicast address.
        /// </summary>
        /// <param name="multicastAddress">The multicast address to send UDP packets to.</param>
        /// <param name="port">The port to use for UDP datagrams.</param>
        public MulticastSender(IPAddress multicastAddress, ushort port)
        {
            UdpLink = new UdpClient();
            UdpLink.JoinMulticastGroup(multicastAddress);
            EndPoint = new IPEndPoint(multicastAddress, port);
        }

        /// <summary>
        /// Sends a byte[] to the multicast group initialized in the constructor.
        /// </summary>
        public void Send(byte[] message)
        {
            UdpLink.Send(message, message.Length, EndPoint);
        }


        /*********************************************************************/
        /*** Instance Variables **********************************************/
        /*********************************************************************/
        /// <summary>
        /// The UDP link, used to establish a multicast group to send messages to.
        /// </summary>
        private UdpClient UdpLink;

        /// <summary>
        /// The end point (IP address and port) that messages will go to.
        /// </summary>
        private IPEndPoint EndPoint;
    }
}
