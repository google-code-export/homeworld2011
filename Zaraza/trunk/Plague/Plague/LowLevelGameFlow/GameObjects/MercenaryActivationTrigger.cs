using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics.Components;
using System.ComponentModel;
using PlagueEngine.EventsSystem;
using PlagueEngine.Physics;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class MercenaryActivationTrigger : Trigger
    {
        static public MercenariesManager MercenariesManager = null;


        public void Init(Mercenary[] Mercenaries, SphericalBodyComponent Body, int Calls)
        {
            base.Init(Body, Calls);
            this.events = new Dictionary<EventArgs, EventsSystem.IEventsReceiver[]>();
            foreach (Mercenary merc in Mercenaries)
            {
                this.events.Add(new RegisterMercenaryEvent(merc), new MercenariesManager[] { MercenariesManager });
            }
        }

        public override GameObjectInstanceData GetData()
        {
            var data = new MercenaryActivationTriggerData();
            base.GetData(data);

            List<int> tmpIDs = new List<int>();
            foreach (KeyValuePair<EventArgs, EventsSystem.IEventsReceiver[]> pair in events)
            {
                tmpIDs.Add((pair.Key as RegisterMercenaryEvent).mercenary.ID);
            }
            data.MercIDs = tmpIDs.ToArray();

            return data;
        }

        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (typeof(StartCollisionEvent).Equals(e.GetType()))
            {
                StartCollisionEvent evt = e as StartCollisionEvent;

                if (evt.gameObject.GetType().Equals(typeof(Mercenary)))
                {
                    if (Calls == 0)
                    {
                        SendEvent(new DestroyObjectEvent(this.ID), Priority.Normal, GlobalGameObjects.GameController);
                    }
                    else
                    {
                        foreach (KeyValuePair<EventArgs, IEventsReceiver[]> pair in events)
                        {
                            RegisterMercenaryEvent tmp = pair.Key as RegisterMercenaryEvent;
                            if (tmp.mercenary == evt.gameObject)
                            {
                                return;
                            }

                        }
                        foreach (KeyValuePair<EventArgs, IEventsReceiver[]> pair in events)
                        {
                            if (pair.Value == null)
                            {
                                Broadcast(pair.Key);
                            }
                            else
                            {
                                foreach (IEventsReceiver receiver in pair.Value)
                                {
                                    SendEvent(pair.Key, Priority.Normal, receiver);
                                }
                            }
                        }
                    }
                }

            }
            base.OnEvent(sender, e);
        }
    }


    [Serializable]
    class MercenaryActivationTriggerData : TriggerData
    {
        [CategoryAttribute("Targets")]
        public int[] MercIDs { get; set; }

        
    }
}
