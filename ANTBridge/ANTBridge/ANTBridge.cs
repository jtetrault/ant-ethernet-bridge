﻿using System;
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
            Listener = new ANTListener();
        }

        /*********************************************************************/
        /*** Instance Variables **********************************************/
        /*********************************************************************/

        private ANTListener Listener;
    }
}