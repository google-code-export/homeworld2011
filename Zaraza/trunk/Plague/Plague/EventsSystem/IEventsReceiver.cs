﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.EventsSystem
/************************************************************************************/
namespace PlagueEngine.EventsSystem
{

    /********************************************************************************/
    /// IEventsReceiver
    /********************************************************************************/
    interface IEventsReceiver
    {

        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        void OnEvent(EventsSender sender, EventArgs e);
        /****************************************************************************/

    }
    /********************************************************************************/
    
}
/************************************************************************************/