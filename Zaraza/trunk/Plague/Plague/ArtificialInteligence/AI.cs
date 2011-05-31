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
        private List<MercenaryController> GoodGuys;
        private List<MobController> BadGuys;

        private List<Mercenary> GoodGuysObjects;
        private List<Creature> BadGuysObjects;

        public AI()
        {
            this.BadGuys            = new List<MobController>();
            this.GoodGuys           = new List<MercenaryController>();
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
    }
}
