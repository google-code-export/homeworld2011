using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.EventsSystem;
using PlagueEngine.ArtificialIntelligence;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using Microsoft.Xna.Framework;

namespace PlagueEngine.ArtificialIntelligence
{
    class AI : EventsSender, IEventsReceiver
    {
        private bool isDisposed;

        private List<AbstractAIController> GoodGuys;
        private List<AbstractAIController> BadGuys;

        private List<Mercenary> GoodGuysObjects;
        private List<Creature> BadGuysObjects;

        int counter = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AI()
        {
            this.isDisposed         = false;
            this.BadGuys            = new List<AbstractAIController>();
            this.GoodGuys           = new List<AbstractAIController>();
            AbstractAIController.ai = this;
        }
        
        /// <summary>
        /// Registers newly created controller in the AI subsystem.
        /// </summary>
        /// <param name="controller">Controller to register in the AI subsystem</param>
        public void registerController(AbstractAIController controller)
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

        /// <summary>
        /// Accepts events from EventsSystem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnEvent(EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                EnemyKilled evt = e as EnemyKilled;
                if(evt.DeadEnemy.GetType().Equals(typeof (Mercenary)))
                {
                    GoodGuys.Remove(evt.DeadEnemy.ObjectAIController);
                    SendEvent(evt, Priority.Normal, BadGuys.ToArray());
                }
                else
                {
                    BadGuys.Remove(evt.DeadEnemy.ObjectAIController);
                    SendEvent(evt, Priority.Normal, GoodGuys.ToArray());
                }
            }
            else if(e.GetType().Equals(typeof(SoundAt)))
            {
                SoundAt evt = e as SoundAt;
                foreach(MobController mob in BadGuys)
                {
                    if (Vector3.Distance(mob.controlledObject.World.Translation, evt.position) < 30)
                    {
                        //EnemyNoticed noticed = new EnemyNoticed();
                        //SendEvent(noticed, Priority.Normal, mob);
                    }
                }
            }
        }

        /// <summary>
        /// Informs whether Object was disposed
        /// </summary>
        /// <returns>True, if object is disposed</returns>
        public bool IsDisposed()
        {
            return isDisposed;
        }

        /// <summary>
        /// Prepares Object for destruction
        /// </summary>
        public void Dispose()
        {
            this.BadGuys.Clear();
            this.GoodGuys.Clear();
            this.BadGuys  = null;
            this.GoodGuys = null;
            this.isDisposed = true;
        }

        /// <summary>
        /// Performs all necessary checks and tests.
        /// </summary>
        public void Update()
         {
            if (counter % 5 == 0)
            {
                counter = 0;

                
                foreach (MobController contr in BadGuys)
                {
                    #region Sight Sensor
                    if (!contr.IsBlinded)
                    {
                        AbstractAIController found = PlagueEngine.AItest.AI.FindClosestVisible(GoodGuys, contr, contr.controlledObject.World.Backward, contr.SightAngle, contr.SightRange);
                        if (found != null && found.controlledObject != contr.AttackTarget)
                        {
                            Diagnostics.PushLog("=========================MOB SEES!=========================");
                            EnemyNoticed evt = new EnemyNoticed(found.controlledObject);
                            SendEvent(evt, Priority.Normal, contr);
                        }
                    }
                    #endregion

                }
               
            }
            else
            {
                foreach (MercenaryController contr in GoodGuys)
                {
                    #region SightSensor
                    if (!contr.IsBlinded)
                    {
                        AbstractAIController found = PlagueEngine.AItest.AI.FindClosestVisible(BadGuys, contr, contr.controlledObject.World.Forward, contr.SightAngle, contr.SightRange);
                        if (found != null && found.controlledObject != contr.AttackTarget)
                        {
                            Diagnostics.PushLog("=========================MERC SEES!=========================");
                            EnemyNoticed evt = new EnemyNoticed(found.controlledObject);
                            SendEvent(evt, Priority.Normal, contr);
                        }
                    }
                    #endregion

                }
              
            }

            counter++;
        }
    }
}
