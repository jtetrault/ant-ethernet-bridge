using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANT_Managed_Library;

namespace ANTBridge
{
    class ANTListener
    {
        /*********************************************************************/
        /*** Class Variables and Constants ***********************************/
        /*********************************************************************/
        /// <summary>
        /// The channel number to use when initializing Channel.
        /// </summary>
        static readonly byte CHANNEL_NUMBER = 0;

        /// <summary>
        /// The slave (primarily receiving) channel type.
        /// </summary>
        static readonly ANT_ReferenceLibrary.ChannelType CHANNEL_TYPE = ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00; 
        
        /// <summary>
        /// The public network number.
        /// </summary>
        static readonly byte NETWORK_NUMBER = 0;

        /// <summary>
        /// Blank network key.
        /// </summary>
        static readonly byte[] NETWORK_KEY = { 0, 0, 0, 0, 0, 0, 0, 0 };


        /*********************************************************************/
        /*** Instance Methods ************************************************/
        /*********************************************************************/
        /// <summary>
        /// Initialize the Device and Channel controlled by this ANTListener.
        /// </summary>
        public ANTListener()
        {
            // Automatically find and connect connect ANT USB device.
            Console.Write("Initializing ANT Device... ");
            try
            {
                Device = new ANT_Device();
                // Device.deviceResponse += new ANT_Device.dDeviceResponseHandler(this.DeviceResponseHandler);
                Console.WriteLine("Done!");
            }
            catch (ANT_Exception ex)
            {
                Console.WriteLine("Failed!");
                throw ex;
            }

            // Get a channel object to work with and install a handler for channel messages.
            Console.Write("Initializing ANT Channel... ");
            try
            {
                Channel = Device.getChannel(CHANNEL_NUMBER);
                Channel.channelResponse += new dChannelResponseHandler(this.ChannelResponseHandler);
                Console.WriteLine("Done!");
            }
            catch (ANT_Exception ex)
            {
                Console.WriteLine("Failed!");
                throw ex;
            }
            // Configure the Device and Channel with default values.
            Console.Write("Configuring ANT Device and Channel with default values... ");
            if (!Device.setNetworkKey(NETWORK_NUMBER, NETWORK_KEY, 500))
                throw new Exception("Error configuring network key");
            if (!Channel.assignChannel(CHANNEL_TYPE, NETWORK_NUMBER, 500))
                throw new Exception("Error assigning channel");
            // Set the Channel ID with wildcard values (to accept connections from any device).
            if (!Channel.setChannelID(0, false, 0, 0, 500))
                throw new Exception("Error configuring Channel ID");
            Console.WriteLine("Done!");
            // Open the channel
            if(!Channel.openChannel(500))
                throw new Exception("Error opening Channel");
            Console.WriteLine("ANT Channel Opened!");
        }

        /// <summary>
        /// Handles ANT Channel events.
        /// </summary>
        public void ChannelResponseHandler(ANT_Response response)
        {
            switch ((ANT_ReferenceLibrary.ANTMessageID)response.responseID)
            {
                // Handle received messages
                case ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E:
                case ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
                case ANT_ReferenceLibrary.ANTMessageID.BURST_DATA_0x50:
                case ANT_ReferenceLibrary.ANTMessageID.EXT_BROADCAST_DATA_0x5D:
                case ANT_ReferenceLibrary.ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E:
                case ANT_ReferenceLibrary.ANTMessageID.EXT_BURST_DATA_0x5F:
                    Console.WriteLine(BitConverter.ToString(response.getDataPayload()));
                    break;

                // Display information for unrecognized messages.
                default:
                    Console.WriteLine("Unrecognized Message: " + response.responseID.ToString("X"));
                    break;
            }
        }

        /// <summary>
        /// Handle ANT Device responses.
        /// </summary>
        public void DeviceResponseHandler(ANT_Response response)
        {
            Console.WriteLine(response.getDataPayload());
        }

        /*********************************************************************/
        /*** Instance Variables **********************************************/
        /*********************************************************************/
        /// <summary>
        /// The ANT Device that the listener uses to receive ANT messages.
        /// </summary>
        private ANT_Device Device;

        /// <summary>
        /// The ANT Channel that Device is currently using to connect to other ANT Nodes.
        /// </summary>
        private ANT_Channel Channel;
    }
}
