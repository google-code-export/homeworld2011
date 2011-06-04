using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.ArtificialInteligence.Controllers;
using PlagueEngine.EventsSystem;
using PlagueEngine.ArtificialIntelligence;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.ArtificialInteligence
{
    class AI : IEventsReceiver
    {
        private List<AbstractAIController> GoodGuys;
        private List<AbstractAIController> BadGuys;

        private List<Mercenary> GoodGuysObjects;
        private List<Creature> BadGuysObjects;

        int counter = 0;

        public AI()
        {
            this.BadGuys            = new List<AbstractAIController>();
            this.GoodGuys = new List<AbstractAIController>();
            AbstractAIController.ai = this;
        }
        
        public void registerController(IAIController controller)
        {
            if(controller.GetType().Equals(typeof (MercenaryController)))
            {
                GoodGuys.Add(controller as MercenaryController);
            }
            else
            {
                BadGuys.Add(controller as MobController);
            }
        }


        public void OnEvent(EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                EnemyKilled evt = e as EnemyKilled;
                if(evt.DeadEnemy.GetType().Equals(typeof (Mercenary)))
                {
                    evt.DeadEnemy.SendEvent(evt, Priority.Normal, BadGuys.ToArray());
                    GoodGuys.Remove(evt.DeadEnemy.ObjectAIController as MercenaryController);
                }
                else
                {
                    evt.DeadEnemy.SendEvent(evt, Priority.Normal, GoodGuys.ToArray());
                    BadGuys.Remove(evt.DeadEnemy.ObjectAIController as MobController);
                }
            }
        }

        public bool IsDisposed()
        {
            return GoodGuys == null && BadGuys == null;
        }

        public void Dispose()
        {
            this.BadGuys.Clear();
            this.BadGuys = null;
            this.GoodGuys.Clear();
            this.GoodGuys = null;
        }

        public void Update()
         {
            if (counter % 2 == 0)
            {
                foreach(MobController contr in BadGuys)
                {
                    if (contr.attackTarget == null)
                    {
                        AbstractAIController found = PlagueEngine.AItest.AI.FindClosestVisible(GoodGuys, contr, contr.controlledObject.World.Forward, (float)30.0, (float)100.0);
                        if(found != null)
                        {
                            Diagnostics.PushLog("=========================MOB SEES!=========================");
                            EnemyNoticed evt = new EnemyNoticed(found.controlledObject);
                            found.controlledObject.SendEvent(evt, Priority.Normal, contr.controlledObject);
                        }
                    }
                }
                counter = 1;
            }
            else
            {
                foreach (MercenaryController contr in GoodGuys)
                {
                    if (contr.attackTarget == null)
                    {
                        AbstractAIController found = PlagueEngine.AItest.AI.FindClosestVisible(BadGuys, contr, contr.controlledObject.World.Forward, (float)30.0, (float)100.0);
                        if (found != null)
                        {
                            Diagnostics.PushLog("=========================MERC SEES!=========================");
                            EnemyNoticed evt = new EnemyNoticed(found.controlledObject);
                            found.controlledObject.SendEvent(evt, Priority.Normal, contr.controlledObject);
                        }
                    }
                }
                counter = 0;
            }

        }
    }
}
