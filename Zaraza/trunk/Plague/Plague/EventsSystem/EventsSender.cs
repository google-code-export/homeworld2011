using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.EventsSystem
/************************************************************************************/
namespace PlagueEngine.EventsSystem
{

    /********************************************************************************/
    /// EventsSender
    /********************************************************************************/
    class EventsSender
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public static EventsSystem eventsSystem = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Send Event (1)
        /****************************************************************************/
        public void SendEvent(EventArgs eventArgs,Priority priority, params IEventsReceiver[] receivers)
        {
            foreach (IEventsReceiver receiver in receivers)
            {
                eventsSystem.AddEvent(new Event(receiver, this, eventArgs), priority);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Send Event (2)
        /****************************************************************************/
        public void SendEvent(EventArgs eventArgs, Priority priority, params uint[] receivers)
        {
            foreach (uint receiver in receivers)
            {
                eventsSystem.AddEvent(new Event(eventsSystem.GetGameObject(receiver), this, eventArgs), priority);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Broadcast (sends events only to sniffers)
        /****************************************************************************/
        public void Broadcast(EventArgs eventArgs, Priority priority = Priority.Normal)
        {
            eventsSystem.AddEvent(new Event(null, this, eventArgs),priority);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/