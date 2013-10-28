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
            // Automatically finds a connect ANT USB device.
            Device = new ANT_Device();

            // Get a channel object to work with.
            Channel = Device.getChannel(CHANNEL_NUMBER);

            // Configure the Device and Channel with default values.
            Device.setNetworkKey(NETWORK_NUMBER, NETWORK_KEY, 500);
            Channel.assignChannel(CHANNEL_TYPE, NETWORK_NUMBER);
            // Set the Channel ID with wildcard values (to accept connections from any device).
            Channel.setChannelID(0, false, 0, 0);
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
