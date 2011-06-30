using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics.Components;
using System.ComponentModel;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class DialogueTrigger : Trigger
    {
        protected List<string> Messages;
        protected List<TimeSpan> WaitTimes;

        protected uint TextIndex = 0;

        public void Init(SphericalBodyComponent Body, List<string> Messages, List<TimeSpan> WaitTimes, List<GameObjectInstance> characters)
        {
            base.Init(Body,1);
            this.Messages = Messages;
            this.WaitTimes = WaitTimes;
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
    class DialogueTriggerData : TriggerData
    {
        [CategoryAttribute("TextParams")]
        public List<string> Messages { get; set; }
        public List<TimeSpan> WaitTimes { get; set; }
    }
}
