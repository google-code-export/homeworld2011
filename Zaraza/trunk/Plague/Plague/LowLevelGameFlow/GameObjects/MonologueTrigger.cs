using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PlagueEngine.Physics;
using PlagueEngine.EventsSystem;
using PlagueEngine.Physics.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class MonologueTrigger : Trigger
    {
        protected List<string> Messages;
        protected List<TimeSpan> WaitTimes;
        Mercenary Activator;

        protected int TextIndex = 0;

        public void Init(SphericalBodyComponent Body, List<string> Messages, List<TimeSpan> WaitTimes)
        {
            base.Init(Body, 1);
            this.Messages = Messages;
            this.WaitTimes = WaitTimes;
            if (Messages.Count != WaitTimes.Count)
            {
                throw new ArgumentException("Listy powinny zawierać tyle samo elementów");
            }
        }

        public void NextText()
        {
            Broadcast(new NewDialogMessageEvent(Activator.Name, Messages[TextIndex], Activator.Icon), Priority.Normal);
            if ((TextIndex + 1) < WaitTimes.Count)
            {
                TimeControlSystem.TimeControl.CreateTimer(WaitTimes[TextIndex], 0, delegate() { NextText(); });
                TextIndex++;
            }
            else
            {
                SendEvent(new DestroyObjectEvent(this.ID), Priority.Normal, GlobalGameObjects.GameController);
            }
        }

        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (typeof(StartCollisionEvent).Equals(e.GetType()))
            {
                StartCollisionEvent evt = e as StartCollisionEvent;

                if (evt.gameObject.GetType().Equals(typeof(Mercenary)))
                {
                    if (Calls != 0)
                    {
                        Calls--;
                        if (Calls == 0)
                        {
                            Body.CancelSubscribeStartCollisionEvent(typeof(Mercenary));
                        }
                        Activator = evt.gameObject as Mercenary;
                        TimeControlSystem.TimeControl.CreateTimer(WaitTimes[0], 0, delegate() { NextText(); });
                    }
                }

            }
        }

        public override GameObjectInstanceData GetData()
        {
            var data = new MonologueTriggerData();
            base.GetData(data);

            List<Str> helper = new List<Str>();
            foreach (string txt in Messages)
            {
                helper.Add(new Str(txt));
            }
            data.Messages = helper; 
            data.WaitTimes = WaitTimes;

            return data;

        }
    }

    [Serializable]
    class MonologueTriggerData : TriggerData
    {
        public List<string> GetMessages()
        {
            List<string> result = new List<string>();
            foreach (Str tmp in Messages)
            {
                result.Add(tmp.content);
            }
            return result;
        }

        public MonologueTriggerData()
        {
            Messages = new List<Str>();
            WaitTimes = new List<TimeSpan>();
        }

        [CategoryAttribute("TextParams")]
        public List<Str> Messages { get; set; }
        [CategoryAttribute("TextParams")]
        public List<TimeSpan> WaitTimes { get; set; }
    }
    
}