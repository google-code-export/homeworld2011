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
        protected List<Mercenary> Ignored;

        protected int TextIndex = 0;

        public void Init(SphericalBodyComponent Body, List<string> Messages, List<TimeSpan> WaitTimes, List<Mercenary> Characters, List<Mercenary> Ignored)
        {
            base.Init(Body, 1);
            if (Messages.Count != WaitTimes.Count || WaitTimes.Count != Characters.Count)
            {
                throw new ArgumentException("Listy powinny zawierać tyle samo elementów");
            }
            this.Messages = Messages;
            this.WaitTimes = WaitTimes;
            this.Characters = Characters;
            this.Ignored = Ignored;
        }

        public void NextText()
        {
            Broadcast(new NewDialogMessageEvent(Characters[TextIndex].Name,
                                                GlobalGameObjects.StringManager.Load<string>(Messages[TextIndex]),
                                                Characters[TextIndex].Icon), Priority.Normal);
            if (TextIndex + 1 < WaitTimes.Count)
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

                if (evt.gameObject.GetType().Equals(typeof(Mercenary)) && !Ignored.Contains(evt.gameObject))
                {
                    if (Calls != 0)
                    {
                        Calls--;
                        if (Calls == 0)
                        {
                            Body.CancelSubscribeStartCollisionEvent(typeof(Mercenary));
                        }
                        TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { NextText(); });
                    }
                }

            }
        }

        public override GameObjectInstanceData GetData()
        {
            DialogueTriggerData data = new DialogueTriggerData();
            base.GetData(data);

            List<int> tmpIDs = new List<int>();
            foreach (Mercenary merc in Characters)
            {
                tmpIDs.Add(merc.ID);
            }
            data.MercIDs = tmpIDs;
            
            List<Str> helper = new List<Str>();
            foreach (string txt in Messages)
            {
                helper.Add(new Str(txt));
            }

            List<int> tmpIgnoredIDs = new List<int>();
            foreach (Mercenary merc in Ignored)
            {
                tmpIgnoredIDs.Add(merc.ID);
            }

            data.IgnoredMercIDs = tmpIgnoredIDs;
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
            MercIDs = new List<int>();
            IgnoredMercIDs = new List<int>();
        }


        [CategoryAttribute("TextParams")]
        public List<Str> Messages { get; set; }
        [CategoryAttribute("TextParams")]
        public List<TimeSpan> WaitTimes { get; set; }
        [CategoryAttribute("TextParams")]
        public List<int> MercIDs { get; set; }
        [CategoryAttribute("TextParams")]
        public List<int> IgnoredMercIDs { get; set; }

    }
    
}
