
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.ArtificialInteligence.Controllers;
using System;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MobController : AbstractAIController
    {
        private GameObjectInstance currentObject;
        //TODO: change temporary constructor
        public MobController(AbstractLivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle)
            : base(person)
        {
            this.rotationSpeed = rotationSpeed;
            this.movingSpeed = movingSpeed;
            this.distance = distance;
            this.anglePrecision = angle;
        }



        override public void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(TakeDamage)))
            {
                #region Take Damage
                TakeDamage evt = e as TakeDamage;
                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(this.controlledObject);
                    this.controlledObject.SendEvent(args, Priority.Normal, this.receiver);
                }
                else
                {
                    this.HP -= (uint)evt.amount;
                    if (this.attackTarget == null)
                    {
                        this.attackTarget = evt.attacker;
                        action = PlagueEngine.ArtificialInteligence.Controllers.Action.ENGAGE;
                    }
                }
                
                #endregion
                return;
            }
            else if (e.GetType().Equals(typeof(EnemyNoticed)))
            {
                #region Engage
                EnemyNoticed evt = e as EnemyNoticed;
                action = PlagueEngine.ArtificialInteligence.Controllers.Action.ENGAGE;
                this.attackTarget = evt.ClosestNoticedEnemy;
                #endregion
                return;
            }
            else
            {
                base.OnEvent(sender, e);
            }
            return;
        }



        public override void Update(TimeSpan deltaTime)
        {
            switch (action)
            {
                default:
                    base.Update(deltaTime);
                    return;
            }
        }


        /****************************************************************************/
        /// ACTIONS HANDLING
        /****************************************************************************/
        protected void Attack()
        {
            //TakeDamage dmg = new TakeDamage(attack.minInflictedDamage, this.controlledObject);
            //controlledObject.SendEvent(dmg, EventsSystem.Priority.Normal, this.attackTarget);
            this.useAttack();
            action = PlagueEngine.ArtificialInteligence.Controllers.Action.ATTACK_IDLE;
            cooldownTimer.Reset(attack.cooldown, 1);
        }


        /****************************************************************************/
        /// EVENTS HANDLING
        /****************************************************************************/

        private void engageEvent(EventArgs e)
        {

        }



        /*****************************************************************************************/
    }



}
