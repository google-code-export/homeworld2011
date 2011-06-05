
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.ArtificialIntelligence.Controllers;


namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MobController : AbstractAIController
    {
        private GameObjectInstance currentObject;
        //TODO: change temporary constructor
        public MobController(AbstractLivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         uint MaxHP,
                         uint HP)
            : base(person, MaxHP, HP)
        {
            RotationSpeed   = rotationSpeed;
            MovingSpeed     = movingSpeed;
            Distance        = distance;
            AnglePrecision  = angle;
            animationBinding = new Dictionary<Action, string>();
            animationBinding.Add(Action.IDLE, "Idle");
            animationBinding.Add(Action.MOVE, "Run");
            animationBinding.Add(Action.ATTACK, "Attack03");
            ai.registerController(this);
        }



        override public void OnEvent(EventsSystem.EventsSender sender, System.EventArgs e)
        {
            /*if (e.GetType().Equals(typeof(TakeDamage)))
            {
               
                return;
            }
            else */
            /*if (e.GetType().Equals(typeof(EnemyNoticed)))
            {
                #region Engage
                EnemyNoticed evt = e as EnemyNoticed;
                action = Action.ENGAGE;
                this.attackTarget = evt.ClosestNoticedEnemy;
                #endregion
                return;
            }
            else
            {*/
                base.OnEvent(sender, e);
            //}
            return;
        }



        public override void Update(System.TimeSpan deltaTime)
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
        /*protected void Attack()
        {
            //TakeDamage dmg = new TakeDamage(attack.minInflictedDamage, this.controlledObject);
            //controlledObject.SendEvent(dmg, EventsSystem.Priority.Normal, this.attackTarget);
            this.useAttack();
            action = PlagueEngine.ArtificialInteligence.Controllers.Action.ATTACK_IDLE;
            cooldownTimer.Reset(attack.cooldown, 1);
        }*/


        /****************************************************************************/
        /// EVENTS HANDLING
        /****************************************************************************/

        private void engageEvent(System.EventArgs e)
        {

        }



        /*****************************************************************************************/
    }



}
