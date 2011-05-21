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
    /// EventsSniffer
    /********************************************************************************/
    abstract class EventsSniffer
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public static EventsSystem eventsSystem = null;

        private bool globalSubscription           = false;
        
        private int receiverInstanceSubscription = 0;
        private int receiverTypeSubscription     = 0;
        
        private int senderInstanceSubscription   = 0;
        private int senderTypeSubscription       = 0;
        
        private int eventSubscription            = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public abstract void OnSniffedEvent(EventsSender sender,IEventsReceiver receiver,EventArgs e);
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe All
        /****************************************************************************/
        public void SubscribeAll()
        {
            if (!eventsSystem.globalSniffers.Contains(this))
            {
                eventsSystem.globalSniffers.Add(this);
                globalSubscription = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Receivers (1)
        /****************************************************************************/
        public void SubscribeReceivers(params IEventsReceiver[] receivers)
        {
            foreach (IEventsReceiver receiver in receivers)
            {
                if (eventsSystem.receiverInstanceSniffers.Keys.Contains(receiver))
                {
                    if (!eventsSystem.receiverInstanceSniffers[receiver].Contains(this))
                    {
                        eventsSystem.receiverInstanceSniffers[receiver].Add(this);
                        receiverInstanceSubscription++;
                    }
                }
                else
                {
                    eventsSystem.receiverInstanceSniffers.Add(receiver, new List<EventsSniffer>());
                    eventsSystem.receiverInstanceSniffers[receiver].Add(this);
                    receiverInstanceSubscription++;
                }
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Subscribe Receivers (2)
        /****************************************************************************/
        public void SubscribeReceivers(params int[] receivers)
        {
            IEventsReceiver receiver;
            foreach (int receiverID in receivers)
            {
                receiver = eventsSystem.GetGameObject(receiverID);

                if (eventsSystem.receiverInstanceSniffers.Keys.Contains(receiver))
                {
                    if (!eventsSystem.receiverInstanceSniffers[receiver].Contains(this))
                    {
                        eventsSystem.receiverInstanceSniffers[receiver].Add(this);
                        receiverInstanceSubscription++;
                    }
                }
                else
                {
                    eventsSystem.receiverInstanceSniffers.Add(receiver, new List<EventsSniffer>());
                    eventsSystem.receiverInstanceSniffers[receiver].Add(this);
                    receiverInstanceSubscription++;
                }
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Subscribe Receiver Type
        /****************************************************************************/
        public void SubscribeReceiverTypes(params Type[] ReceiverTypes)
        {
            foreach (Type ReceiverType in ReceiverTypes)
            {
                if (eventsSystem.receiverTypeSniffers.Keys.Contains(ReceiverType))
                {
                    if (!eventsSystem.receiverTypeSniffers[ReceiverType].Contains(this))
                    {
                        eventsSystem.receiverTypeSniffers[ReceiverType].Add(this);
                        receiverTypeSubscription++;
                    }
                }
                else
                {
                    eventsSystem.receiverTypeSniffers.Add(ReceiverType, new List<EventsSniffer>());
                    eventsSystem.receiverTypeSniffers[ReceiverType].Add(this);
                    receiverTypeSubscription++;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Senders (1)
        /****************************************************************************/
        public void SubscribeSenders(params EventsSender[] senders)
        {
            foreach (EventsSender sender in senders)
            {
                if (eventsSystem.senderInstanceSniffers.Keys.Contains(sender))
                {
                    if (!eventsSystem.senderInstanceSniffers[sender].Contains(this))
                    {
                        eventsSystem.senderInstanceSniffers[sender].Add(this);
                        senderInstanceSubscription++;
                    }
                }
                else
                {
                    eventsSystem.senderInstanceSniffers.Add(sender, new List<EventsSniffer>());
                    eventsSystem.senderInstanceSniffers[sender].Add(this);
                    senderInstanceSubscription++;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Senders (2)
        /****************************************************************************/
        public void SubscribeSenders(params int[] senders)
        {
            EventsSender sender;
            foreach (int senderID in senders)
            {
                sender = eventsSystem.GetGameObject(senderID);

                if (eventsSystem.senderInstanceSniffers.Keys.Contains(sender))
                {
                    if (!eventsSystem.senderInstanceSniffers[sender].Contains(this))
                    {
                        eventsSystem.senderInstanceSniffers[sender].Add(this);
                        senderInstanceSubscription++;
                    }
                }
                else
                {
                    eventsSystem.senderInstanceSniffers.Add(sender, new List<EventsSniffer>());
                    eventsSystem.senderInstanceSniffers[sender].Add(this);
                    senderInstanceSubscription++;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Sender Types
        /****************************************************************************/
        public void SubscribeSenderTypes(params Type[] SenderTypes)
        {
            foreach (Type SenderType in SenderTypes)
            {
                if (eventsSystem.senderTypeSniffers.Keys.Contains(SenderType))
                {
                    if (!eventsSystem.senderTypeSniffers[SenderType].Contains(this))
                    {
                        eventsSystem.senderTypeSniffers[SenderType].Add(this);
                        senderTypeSubscription++;
                    }
                }
                else
                {
                    eventsSystem.senderTypeSniffers.Add(SenderType, new List<EventsSniffer>());
                    eventsSystem.senderTypeSniffers[SenderType].Add(this);
                    senderTypeSubscription++;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Event
        /****************************************************************************/
        public void SubscribeEvents(params Type[] EventArgsTypes)
        {
            foreach (Type EventArgsType in EventArgsTypes)
            {
                if (eventsSystem.eventSniffers.Keys.Contains(EventArgsType))
                {
                    if (!eventsSystem.eventSniffers[EventArgsType].Contains(this))
                    {
                        eventsSystem.eventSniffers[EventArgsType].Add(this);
                        eventSubscription++;
                    }
                }
                else
                {
                    eventsSystem.eventSniffers.Add(EventArgsType, new List<EventsSniffer>());
                    eventsSystem.eventSniffers[EventArgsType].Add(this);
                    eventSubscription++;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Global Subscription        
        /****************************************************************************/
        public void CancelGlobalSubscription()
        {
            eventsSystem.globalSniffers.Remove(this);
            globalSubscription = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Receivers Subscription (1)
        /****************************************************************************/
        public void CancelReceiversSubsciption(params IEventsReceiver[] receivers)
        {
            List<IEventsReceiver> delete = new List<IEventsReceiver>();

            foreach (IEventsReceiver receiver in receivers)
            {
                if (eventsSystem.receiverInstanceSniffers.Keys.Contains(receiver))
                {
                    if (eventsSystem.receiverInstanceSniffers[receiver].Remove(this))
                    {
                        receiverInstanceSubscription--;

                        if (eventsSystem.receiverInstanceSniffers[receiver].Count == 0)
                        {
                            delete.Add(receiver);
                        }
                    }
                }
            }

            foreach (IEventsReceiver receiver in delete)
            {
                eventsSystem.receiverInstanceSniffers.Remove(receiver);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Receivers Subscription (2)
        /****************************************************************************/
        public void CancelReceiversSubsciption(params int[] receivers)
        {
            List<IEventsReceiver> delete = new List<IEventsReceiver>();

            foreach (int receiverID in receivers)
            {
                IEventsReceiver receiver = eventsSystem.GetGameObject(receiverID);

                if (eventsSystem.receiverInstanceSniffers.Keys.Contains(receiver))
                {
                    if (eventsSystem.receiverInstanceSniffers[receiver].Remove(this))
                    {
                        receiverInstanceSubscription--;

                        if (eventsSystem.receiverInstanceSniffers[receiver].Count == 0)
                        {
                            delete.Add(receiver);
                        }
                    }
                }
            }

            foreach (IEventsReceiver receiver in delete)
            {
                eventsSystem.receiverInstanceSniffers.Remove(receiver);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Receivers Subscription (3)
        /****************************************************************************/
        public void CancelReceiversSubsciption()
        {
            List<IEventsReceiver> delete = new List<IEventsReceiver>();

            foreach (IEventsReceiver receiver in eventsSystem.receiverInstanceSniffers.Keys)
            {
                if (eventsSystem.receiverInstanceSniffers[receiver].Remove(this))
                {
                    receiverInstanceSubscription--;

                    if (eventsSystem.receiverInstanceSniffers[receiver].Count == 0)
                    {
                        delete.Add(receiver);
                    }
                }
            }

            foreach (IEventsReceiver receiver in delete)
            {
                eventsSystem.receiverInstanceSniffers.Remove(receiver);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Receiver Types Subscription (1)
        /****************************************************************************/
        public void CancelReceiverTypesSubscription(params Type[] receiverTypes)
        {
            foreach (Type receiverType in receiverTypes)
            {
                if (eventsSystem.receiverTypeSniffers.Keys.Contains(receiverType))
                {
                    eventsSystem.receiverTypeSniffers[receiverType].Remove(this);
                    receiverTypeSubscription--;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Receiver Types Subscription (2)
        /****************************************************************************/
        public void CancelReceiverTypesSubscription()
        {
            foreach (List<EventsSniffer> l in eventsSystem.receiverTypeSniffers.Values)
            {
                if (l.Remove(this)) receiverTypeSubscription--;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Sender Subscriiption (1)
        /****************************************************************************/
        public void CancelSenderSubscription(params EventsSender[] senders)
        {
            List<EventsSender> delete = new List<EventsSender>();

            foreach (EventsSender sender in senders)
            {
                if (eventsSystem.senderInstanceSniffers.Keys.Contains(sender))
                {
                    if (eventsSystem.senderInstanceSniffers[sender].Remove(this))
                    {
                        senderInstanceSubscription--;

                        if (eventsSystem.senderInstanceSniffers[sender].Count == 0)
                        {
                            delete.Add(sender);
                        }
                    }
                }
            }

            foreach (EventsSender sender in delete)
            {
                eventsSystem.senderInstanceSniffers.Remove(sender);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Sender Subscriiption (2)
        /****************************************************************************/
        public void CancelSenderSubscription(params int[] senders)
        {
            List<EventsSender> delete = new List<EventsSender>();
            
            foreach (int senderID in senders)
            {
                EventsSender sender = eventsSystem.GetGameObject(senderID);

                if (eventsSystem.senderInstanceSniffers.Keys.Contains(sender))
                {
                    if (eventsSystem.senderInstanceSniffers[sender].Remove(this))
                    {
                        senderInstanceSubscription--;

                        if (eventsSystem.senderInstanceSniffers[sender].Count == 0)
                        {
                            delete.Add(sender);
                        }
                    }
                }
            }

            foreach (EventsSender sender in delete)
            {
                eventsSystem.senderInstanceSniffers.Remove(sender);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Sender Subscriiption (3)
        /****************************************************************************/
        public void CancelSenderSubscription()
        {
            List<EventsSender> delete = new List<EventsSender>();

            foreach (EventsSender sender in eventsSystem.senderInstanceSniffers.Keys)
            {
                if (eventsSystem.senderInstanceSniffers[sender].Remove(this))
                {
                    senderInstanceSubscription--;

                    if (eventsSystem.senderInstanceSniffers[sender].Count == 0)
                    {
                        delete.Add(sender);
                    }
                }
                
            }

            foreach (EventsSender sender in delete)
            {
                eventsSystem.senderInstanceSniffers.Remove(sender);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Sender Type Subscription (1)
        /****************************************************************************/
        public void CancelSenderTypesSubscription(params Type[] senderTypes)
        {
            foreach (Type senderType in senderTypes)
            {
                if (eventsSystem.senderTypeSniffers.Keys.Contains(senderType))
                {
                    if (eventsSystem.senderTypeSniffers[senderType].Remove(this))
                    {
                        senderTypeSubscription--;
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Sender Type Subscription (2)
        /****************************************************************************/
        public void CancelSenderTypesSubscription()
        {
            foreach (List<EventsSniffer> l in eventsSystem.senderTypeSniffers.Values)
            {
                if (l.Remove(this)) senderTypeSubscription--;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Event Subscription (1)
        /****************************************************************************/
        public void CancelEventSubscription(params Type[] EventArgsTypes)
        {
            foreach (Type EventArgsType in EventArgsTypes)
            {
                if (eventsSystem.eventSniffers.Keys.Contains(EventArgsType))
                {
                    eventsSystem.eventSniffers[EventArgsType].Remove(this);
                    eventSubscription--;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Cancel Event Subscription (2)
        /****************************************************************************/
        public void CancelEventSubscription()
        {
            foreach (List<EventsSniffer> l in eventsSystem.eventSniffers.Values)
            {
                if (l.Remove(this)) eventSubscription--;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public virtual void ReleaseMe()
        {
            if (globalSubscription)               CancelGlobalSubscription();
            if (receiverInstanceSubscription > 0) CancelReceiversSubsciption();
            if (receiverTypeSubscription     > 0) CancelReceiverTypesSubscription();
            if (senderInstanceSubscription   > 0) CancelSenderSubscription();
            if (senderTypeSubscription       > 0) CancelSenderSubscription();
            if (eventSubscription            > 0) CancelEventSubscription();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/