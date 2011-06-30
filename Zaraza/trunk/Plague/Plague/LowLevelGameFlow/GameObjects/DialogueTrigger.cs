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
    [Serializable]
    class Str
    {
        [CategoryAttribute("Value")]
        public string content { get; set; }

        public Str()
        {
            content = "";
        }

        public Str(string txt)
        {
            content = txt;
        }
    }

    class DialogueTrigger : Trigger
    {
        protected List<string> Messages;
        protected List<TimeSpan> WaitTimes;
        protected List<Mercenary> Characters;

        protected int TextIndex = 0;

        public void Init(SphericalBodyComponent Body, List<string> Messages, List<TimeSpan> WaitTimes, List<Mercenary> Characters)
        {
            base.Init(Body, 1);
            this.Messages = Messages;
            this.WaitTimes = WaitTimes;
            if (Messages.Count != WaitTimes.Count || WaitTimes.Count != Characters.Count)
            {
                throw new ArgumentException("Listy powinny zawierać tyle samo elementów");
            }
            this.Messages = Messages;
            this.WaitTimes = WaitTimes;
            this.Characters = Characters;
        }

        public void NextText()
        {
            Broadcast(new NewDialogMessageEvent(Characters[TextIndex].Name, Messages[TextIndex], Characters[TextIndex].Icon), Priority.Normal);
            if (TextIndex + 1 == WaitTimes.Count)
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
                    if (Calls == 0 && TextIndex == Messages.Count)
                    {
                        SendEvent(new DestroyObjectEvent(this.ID), Priority.Normal, GlobalGameObjects.GameController);
                    }
                    else
                    {
                        Calls--;
                        if (Calls == 0)
                        {
                            Body.CancelSubscribeStartCollisionEvent(typeof(Mercenary));
                        }
                        TimeControlSystem.TimeControl.CreateTimer(WaitTimes[0], 0, delegate() { NextText(); });
                    }
                }

            }
        }

        public override GameObjectInstanceData GetData()
        {
            var data = new DialogueTriggerData();
            base.GetData(data);

            List<int> tmpIDs = new List<int>();
            foreach (KeyValuePair<EventArgs, EventsSystem.IEventsReceiver[]> pair in events)
            {
                tmpIDs.Add((pair.Key as RegisterMercenaryEvent).mercenary.ID);
            }
            data.MercIDs = tmpIDs.ToArray();
            
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
    class DialogueTriggerData : TriggerData
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

        public DialogueTriggerData()
        {
            Messages = new List<Str>();
            WaitTimes = new List<TimeSpan>();
        }


        [CategoryAttribute("TextParams")]
        public List<Str> Messages { get; set; }
        [CategoryAttribute("TextParams")]
        public List<TimeSpan> WaitTimes { get; set; }
        [CategoryAttribute("TextParams")]
        public int[] MercIDs { get; set; }
    }
    
}
