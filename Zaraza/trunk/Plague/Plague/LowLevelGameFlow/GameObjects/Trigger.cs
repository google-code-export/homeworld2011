using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics.Components;
using PlagueEngine.EventsSystem;
using PlagueEngine.Physics;
using System.ComponentModel;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Trigger : GameObjectInstance
    {
        protected SphericalBodyComponent Body;
        protected Dictionary<EventArgs, IEventsReceiver[]> events;
        protected int MaxCalls;
        protected int Calls
        {
            get
            {
                return _calls;
            }
            set
            {
                if(value >= 0)
                {
                    _calls=value;
                }
            }
        }
        private int _calls;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Body"></param>
        public void Init(SphericalBodyComponent Body, Dictionary<EventArgs, IEventsReceiver[]> events, int MaxCalls)
        {
            this.Init(Body, MaxCalls);
            this.events = events;
        }

        public void Init(SphericalBodyComponent Body, int MaxCalls)
        {
            this.MaxCalls = MaxCalls;
            this._calls = MaxCalls;
            this.Status = GameObjectStatus.Passable;
            this.Body = Body;
            this.Body.SubscribeStartCollisionEvent(typeof(Mercenary));
            this.Body.dontCollide = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void GetData(TriggerData data)
        {
            base.GetData(data);

            data.Radius = Body.Radius;
            data.Calls = MaxCalls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        

        public override void ReleaseComponents()
        {
            if (Body != null)
            {
                Body.ReleaseMe();
                Body = null;
            }
            base.ReleaseComponents();
        }


    }

    

    [Serializable]
    class TriggerData : GameObjectInstanceData
    {
        [CategoryAttribute("CollisionSkin")]
        public float Radius { get; set; }

        [CategoryAttribute("TriggerProperties")]
        public int Calls { get; set; }
    }
}
