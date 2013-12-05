using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using ANTResponseFormatter;

namespace ANTBridge
{
    class ANTBridge
    {

        /*********************************************************************/
        /*** Class Variables and Constants ***********************************/
        /*********************************************************************/
        /// <summary>
        /// The number of bytes needed for the payload of each ANT message.
        /// </summary>
        static readonly byte ANT_PAYLOAD_LENGTH = 8;

        /// <summary>
        /// The number of bytes needed for the Device ID of each ANT message.
        /// Consists of deviceNumber (ushort), deviceTypeID (byte), and transmissionTypeID (byte).
        /// pairingBit (bool) is ignored.
        /// </summary>
        static readonly byte ANT_DEVICE_ID_LENGTH = 4;

        /*********************************************************************/
        /*** Instance Methods ************************************************/
        /*********************************************************************/
        /// <summary>
        /// Initialize the ANTListener and IPSender.
        /// </summary>
        /// <param name="networkKey">The Network Key to pass to ANTListener.</param>
        /// <param name="channelPeriod">The Channel Period to pass to ANTListener.</param>
        /// <param name="channelFrequency">The Channel Frequency to pass along to ANTListener.</param>
        /// <param name="multicastAddress">The multicast IP address of the multicast group to send messages to.</param>
        /// <param name="multicastPort">The port to send messages to.</param>
        /// <param name="verbose">Determines if messages should be written to Console as events happen.</param>
        public ANTBridge(byte[] networkKey, ushort channelPeriod, byte channelFrequency, IPAddress multicastAddress, ushort multicastPort, bool verbose)
        {
            Listener = new ANTListener(networkKey, channelPeriod, channelFrequency, this.ANTListenerDelegate);
            Console.WriteLine("Listening for ANT Messages:\nNetwork Key:\t{0}\nChannel Period:\t{1}\nChannel Freq.:\t{2}\n",
                BitConverter.ToString(networkKey),
                channelPeriod,
                channelFrequency);


            Sender = new MulticastSender(multicastAddress, multicastPort);
            Console.WriteLine("Sending Multicast Messages to {0}:{1}\n",
                multicastAddress.ToString(),
                multicastPort);

            Message = new byte[ANT_PAYLOAD_LENGTH + ANT_DEVICE_ID_LENGTH];
            Verbose = verbose;
        }

        /// <summary>
        /// Receives ANT messages from ANTListener and passes them off to MulticastSender.
        /// </summary>
        /// <param name="response"></param>
        public void ANTListenerDelegate(ANT_Managed_Library.ANT_Response response)
        {
            // Build the message to send.
            // Copy the payload over.
            Array.Copy(response.getDataPayload(), Message, ANT_PAYLOAD_LENGTH);
            if (response.isExtended())
            {
                // Copy extended information into Message immediately after the payload.
                ANT_Managed_Library.ANT_ChannelID deviceID = response.getDeviceIDfromExt();
                Message[ANT_PAYLOAD_LENGTH] = (byte)(deviceID.deviceNumber);
                Message[ANT_PAYLOAD_LENGTH + 1] = (byte)(deviceID.deviceNumber >> 8);
                Message[ANT_PAYLOAD_LENGTH + 2] = deviceID.deviceTypeID;
                Message[ANT_PAYLOAD_LENGTH + 3] = deviceID.transmissionTypeID;
            }
            else // Clear the DeviceID info from any previous Messages.
                Array.Clear(Message, ANT_PAYLOAD_LENGTH, ANT_DEVICE_ID_LENGTH);

            // Print the message on the Console if desired.
            if (Verbose)
            {
                Console.WriteLine(ANTResponseFormatter.Formatter.FormatMessage(Message));
            }
            Sender.Send(Message);
        }

        /*********************************************************************/
        /*** Instance Variables **********************************************/
        /*********************************************************************/

        /// <summary>
        /// Listens for ANT messages and relays them to ANTListenerDelegate().
        /// </summary>
        private ANTListener Listener;

        /// <summary>
        /// Sends messages to the preconfigured multicast group.
        /// </summary>
        private MulticastSender Sender;

        /// <summary>
        /// Holds the message to pass to Sender.
        /// </summary>
        private byte[] Message;

        /// <summary>
        /// Determines if messages should be written to console as events happen.
        /// </summary>
        private bool Verbose;
    }
}
