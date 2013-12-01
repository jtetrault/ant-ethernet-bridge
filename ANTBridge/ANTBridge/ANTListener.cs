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
        static readonly ANT_ReferenceLibrary.ChannelType CHANNEL_TYPE = ANT_ReferenceLibrary.ChannelType.ADV_TxRx_Only_or_RxAlwaysWildCard_0x40; 
        
        /// <summary>
        /// The Network Number that the Network Key will be assigned to.
        /// </summary>
        static readonly byte NETWORK_NUMBER = 0;


        /*********************************************************************/
        /*** Instance Methods ************************************************/
        /*********************************************************************/
        /// <summary>
        /// Initialize an ANT Device connected to this computer via USB.
        /// The Device will be setup in Continuous Scan mode. It will be able to recieve messages from multiple master devices simultaneously.
        /// 
        /// </summary>
        /// <param name="networkKey">
        /// The Network Key to use when reading messages. Without the correct network key, messages will not be read.
        /// An exception will be thrown if the array is not exactly 80 bytes long.
        /// </param>
        /// <param name="channelPeriod">The Channel Period to use for the receiver.</param>
        /// <param name="channelFrequency">
        /// The Frequency to add to the base frequency (2400) to compute the frequency that the receiver should operate on.
        /// The ANT+ frequency is 57.
        /// An exception will be thrown if the value is not between 0 and 124 inclusive.
        /// </param>
        /// <param name="rxDelegate">The delegate method that will be called every time a transmission is received.</param>
        public ANTListener(byte[] networkKey, ushort channelPeriod, byte channelFrequency, Action<ANT_Response> rxDelegate)
        {
            // Validate networkKey and channelPeriod.
            if (networkKey.Length != 8)
                throw new Exception("Network Key is not exactly 8 bytes long");
            if (channelFrequency < 0 || channelFrequency > 124)
                throw new Exception("Channel Period is not in range 0 - 124");

            // Assign our RxDelegate to whatever is passed in.
            RxDelegate = rxDelegate;

            // Automatically find and connect connect ANT USB device.
            Console.Write("Initializing ANT Device... ");
            try
            {
                Device = new ANT_Device();
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
            // Enable reception of Extended messages.
            if (!Device.enableRxExtendedMessages(true, 500))
                throw new Exception("Error enabling Extendced messages");
            // Configure the Device and Channel with default values.
            Console.Write("Configuring ANT Device and Channel with default values... ");
            if (!Device.setNetworkKey(NETWORK_NUMBER, networkKey, 500))
                throw new Exception("Error configuring network key");
            if (!Channel.assignChannel(CHANNEL_TYPE, NETWORK_NUMBER, 500))
                throw new Exception("Error assigning channel");
            // Set the Channel ID with wildcard values (to accept connections from any device).
            if (!Channel.setChannelID(0, false, 0, 0, 500))
                throw new Exception("Error configuring Channel ID");
            // Configure the Period.
            if (!Channel.setChannelPeriod(channelPeriod, 500))
                throw new Exception("Error setting Channel Period");
            // Configure the Frequency.
            if(!Channel.setChannelFreq(channelFrequency, 500))
                throw new Exception("Error setting Channel Frequency");
            Console.WriteLine("Done!");

            // Open the channel in continuous scan mode.
            if (!Device.openRxScanMode(500))
                throw new Exception("Error opening Channel in continuous scan mode");
            Console.WriteLine("ANT Channel Opened in Continuous Scan Mode!");
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
                    // Pass received data off to the RxDelegate method.
                    RxDelegate(response);
                    break;

                // Display information for unrecognized messages.
                default:
                    Console.WriteLine("Unrecognized Message: " + response.responseID.ToString("X"));
                    break;
            }
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

        /// <summary>
        /// The delegate to pass ANT_Responses to whenever data is received.
        /// </summary>
        private Action<ANT_Response> RxDelegate;
    }
}
