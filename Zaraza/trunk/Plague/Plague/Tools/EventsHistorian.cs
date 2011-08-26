using System;
using System.Collections.Generic;
using System.Text;

using PlagueEngine.EventsSystem;

/************************************************************************************/
/// PlagueEngine
/************************************************************************************/

namespace PlagueEngine
{
    /********************************************************************************/
    /// Events Historian
    /********************************************************************************/

    internal class EventsHistorian : EventsSniffer
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Queue<String> events = new Queue<String>();
        private uint capacity = 0;
        /****************************************************************************/

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/

        public EventsHistorian(uint capacity)
        {
            this.capacity = capacity;
            SubscribeAll();
        }

        /****************************************************************************/

        /****************************************************************************/
        /// On Sniffed Event
        /****************************************************************************/

        public override void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Diagnostics.RunTime.ToString(@"hh\:mm\:ss"));

            builder.Append(": ");

            builder.Append("Sender: ");
            builder.Append(sender == null ? "null" : sender.ToString());
            builder.Append(";");

            if (receiver == null)
            {
                builder.Append("Broadcast;");
            }
            else
            {
                builder.Append("Receiver: ");
                builder.Append(receiver.ToString());
                builder.Append(";");
            }

            if (e != null)
            {
                builder.Append(e.GetType().Name);
                builder.Append(" - ");
                builder.Append(e.ToString());
            }

            events.Enqueue(builder.ToString());

            if (events.Count > capacity)
            {
                events.Dequeue();
            }

#if DEBUG
            Diagnostics.PushLog(builder.ToString());
#endif
        }

        /****************************************************************************/

        /****************************************************************************/
        /// Flush
        /****************************************************************************/

        public void Flush()
        {
            StringBuilder builder = new StringBuilder();

            foreach (String eventData in events)
            {
                builder.AppendLine(eventData);
            }
#if DEBUG
            Diagnostics.PushLog(builder.ToString());
#endif
        }

        /****************************************************************************/
    }

    /********************************************************************************/
}

/************************************************************************************/