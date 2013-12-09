using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTResponseFormatter
{
    /// <summary>
    /// Contains methods for formatting ANT messages into strings.
    /// </summary>
    public class Formatter
    {
        /*********************************************************************/
        /*** Class Variables and Constants ***********************************/
        /*********************************************************************/
        /// <summary>
        /// The number of bytes needed for the payload of each ANT message.
        /// </summary>
        private const byte ANT_PAYLOAD_LENGTH = 8;

        /// <summary>
        /// The number of bytes needed for the device id of each ANT message. 
        /// </summary>
        private const byte ANT_DEVICE_ID_LENGTH = 4;

        /*********************************************************************/
        /*** Class Methods ***************************************************/
        /*********************************************************************/
        /// <summary>
        /// Takes an ANT message and returns it as a string of the following format:
        /// Received Payload: XX-XX-XX-XX-XX-XX-XX-XX
        /// Device ID: [device number]-[device type]-[transmission type]
        /// 
        /// Throws an exception if the message is of the wrong length.
        /// </summary>
        public static string FormatMessage(byte[] message)
        {
            if (message.Length == ANT_PAYLOAD_LENGTH + ANT_DEVICE_ID_LENGTH)
                return "Received Payload: " + BitConverter.ToString(message, 0, ANT_PAYLOAD_LENGTH)
                    + string.Format("\nDevice ID: {0:D}-{1:D}-{2:D}",
                        BitConverter.ToUInt16(message, ANT_PAYLOAD_LENGTH),
                        message[ANT_PAYLOAD_LENGTH + 2],
                        message[ANT_PAYLOAD_LENGTH + 3]);
            else
                throw new Exception("Message is of incorrect length");
        }
    }
}
