using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTBridge
{
    class ANTBridge
    {
        /*********************************************************************/
        /*** Instance Methods ************************************************/
        /*********************************************************************/
        /// <summary>
        /// Initialize the ANTListener and IPSender.
        /// </summary>
        public ANTBridge()
        {
            Listener = new ANTListener(this.ANTListenerDelegate);
        }

        /// <summary>
        /// Receives ANT messages from ANTListener and passes them off to IPSender.
        /// </summary>
        /// <param name="response"></param>
        public void ANTListenerDelegate(ANT_Managed_Library.ANT_Response response)
        {
            Console.WriteLine(BitConverter.ToString(response.getDataPayload()));
            if (response.isExtended())
            {
                ANT_Managed_Library.ANT_ChannelID channelID = response.getDeviceIDfromExt();
                Console.WriteLine("Device ID: {0:D}-{1:D}-{2:D}", channelID.deviceNumber, channelID.deviceTypeID, channelID.transmissionTypeID);
            }
        }

        /*********************************************************************/
        /*** Instance Variables **********************************************/
        /*********************************************************************/

        private ANTListener Listener;
    }
}
