using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.EventsSystem
/************************************************************************************/
namespace PlagueEngine.EventsSystem
{

    /********************************************************************************/
    /// Events System
    /********************************************************************************/
    class EventsSystem
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<Priority,Queue<Event>> activeQueues   = new Dictionary<Priority,Queue<Event>>();
        private Dictionary<Priority,Queue<Event>> inactiveQueues = new Dictionary<Priority, Queue<Event>>();
        private Level                             level          = null;

        internal List<EventsSniffer>                              globalSniffers           = new List<EventsSniffer>();
        
        internal Dictionary<IEventsReceiver, List<EventsSniffer>> receiverInstanceSniffers = new Dictionary<IEventsReceiver, List<EventsSniffer>>();
        internal Dictionary<Type, List<EventsSniffer>>            receiverTypeSniffers     = new Dictionary<Type,List<EventsSniffer>>();        
        
        internal Dictionary<EventsSender, List<EventsSniffer>>    senderInstanceSniffers   = new Dictionary<EventsSender, List<EventsSniffer>>();
        internal Dictionary<Type, List<EventsSniffer>>            senderTypeSniffers       = new Dictionary<Type,List<EventsSniffer>>();        
        
        internal Dictionary<Type, List<EventsSniffer>>            eventSniffers            = new Dictionary<Type, List<EventsSniffer>>();
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public EventsSystem(Level level)
        {
            this.level = level;
            EventsSender.eventsSystem  = this;
            EventsSniffer.eventsSystem = this;

            activeQueues.Add(Priority.High,   new Queue<Event>());
            activeQueues.Add(Priority.Normal, new Queue<Event>());
            activeQueues.Add(Priority.Low,    new Queue<Event>());

            inactiveQueues.Add(Priority.High,   new Queue<Event>());
            inactiveQueues.Add(Priority.Normal, new Queue<Event>());
            inactiveQueues.Add(Priority.Low,    new Queue<Event>());
        }
        /****************************************************************************/
                       

        /****************************************************************************/
        /// Add Event
        /****************************************************************************/
        public void AddEvent(Event e, Priority p)
        {
            inactiveQueues[p].Enqueue(e);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get GameObject
        /****************************************************************************/
        public GameObjectInstance GetGameObject(int id)
        {
            return level.GameObjects[id];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            SwitchQueues();
        
            Event e;

            for(int i=0 ; i < activeQueues[Priority.High].Count ; i++)
            {
                e = activeQueues[Priority.High].Dequeue();
                PassEvent(e);
            }

            for (int i = 0; i < activeQueues[Priority.Normal].Count; i++)
            {
                e = activeQueues[Priority.Normal].Dequeue();
                PassEvent(e);
            }

            for (int i = 0; i < activeQueues[Priority.Low].Count; i++)
            {
                e = activeQueues[Priority.Low].Dequeue();
                PassEvent(e);
            }                
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pass Event
        /****************************************************************************/
        private void PassEvent(Event e)
        {            

            /***************************/
            // Global Sniffers
            /***************************/
            foreach(EventsSniffer sniffer in globalSniffers)
            {
                sniffer.OnSniffedEvent(e.Sender,e.Receiver,e.EventArgs);
            }
            /***************************/


            /***************************/
            // Global Sniffers
            /***************************/
            if(eventSniffers.ContainsKey(e.EventArgs.GetType()))
            {
                foreach (EventsSniffer sniffer in eventSniffers[e.EventArgs.GetType()])
                {
                    sniffer.OnSniffedEvent(e.Sender, e.Receiver, e.EventArgs);
                }
            }
            /***************************/


            /***************************/
            // Sender Isntance Sniffers
            /***************************/
            if(senderInstanceSniffers.Keys.Contains(e.Sender))
            {
                foreach(EventsSniffer sniffer in senderInstanceSniffers[e.Sender])
                {
                    sniffer.OnSniffedEvent(e.Sender,e.Receiver,e.EventArgs);
                }
            }
            /***************************/


            /***************************/
            // Sender Type Sniffers
            /***************************/
            if(senderTypeSniffers.Keys.Contains(e.Sender.GetType()))
            {
                foreach(EventsSniffer sniffer in senderTypeSniffers[e.Sender.GetType()])
                {
                    sniffer.OnSniffedEvent(e.Sender,e.Receiver,e.EventArgs);
                }
            }
            /***************************/
            
            if (e.Receiver != null)
            {
                if (e.Receiver.IsDisposed()) return; 

                e.Receiver.OnEvent(e.Sender, e.EventArgs);                
                
                /******************************/
                /// Receiver Isntance Sniffers
                /******************************/
                if(receiverInstanceSniffers.Keys.Contains(e.Receiver))
                {
                    foreach(EventsSniffer sniffer in receiverInstanceSniffers[e.Receiver])
                    {
                        sniffer.OnSniffedEvent(e.Sender,e.Receiver,e.EventArgs);
                    }
                }
                /******************************/


                /******************************/
                /// Receiver Type Sniffers
                /******************************/
                if(receiverTypeSniffers.Keys.Contains(e.Receiver.GetType()))
                {
                    foreach(EventsSniffer sniffer in receiverTypeSniffers[e.Receiver.GetType()])
                    {
                        sniffer.OnSniffedEvent(e.Sender,e.Receiver,e.EventArgs);
                    }
                }
                /******************************/
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Switch Queues
        /****************************************************************************/
        private void SwitchQueues()
        {
            Dictionary<Priority,Queue<Event>> tmp = inactiveQueues;
            inactiveQueues = activeQueues;
            activeQueues = tmp;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Reset
        /****************************************************************************/
        public void Reset()
        {
            foreach (KeyValuePair<Priority,Queue<Event>>  queue in activeQueues)   queue.Value.Clear();
            foreach (KeyValuePair<Priority, Queue<Event>> queue in inactiveQueues) queue.Value.Clear();            
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Priority
    /********************************************************************************/
    enum Priority
    {
        High,
        Normal,
        Low
    };
    /********************************************************************************/

}
/************************************************************************************/