using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MobController : Controller
    {
        private GameObjectInstance currentObject;
        private static AI ai;
        //TODO: change temporary constructor
        public MobController(LivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle)
        {
            this.GameObject = person;
            this.rotationSpeed = rotationSpeed;
            this.movingSpeed = movingSpeed;
            this.distance = distance;
            this.anglePrecision = angle;
        }

        override public bool OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (base.OnEvent(sender, e))
            {
                return true;
            }
            else if (e.GetType().Equals(typeof(TakeDamage)))
            {
                takeDamageEvent(e);
                return true;
            }
            else if (e.GetType().Equals(typeof(EnemyNoticed)))
            {
                engageEvent(e);
                return true;
            }
            /*else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                examineEvent(e);
                return true;
            }*/
            return false;
        }

        private void takeDamageEvent(EventArgs e)
        {
            TakeDamage evtArg = e as TakeDamage;
            if(this.GameObject.HP < evtArg.amount)
            {
                this.GameObject.HP = 0;
                this.GameObject.SendEvent(new EnemyKilled(this.GameObject), Priority.Normal, ai);
            }
            else
            {
                this.GameObject.HP -= (uint) evtArg.amount;
            }
 	        
        }

        private void engageEvent(EventArgs e)
        {

        }

        public override void Update(TimeSpan deltaTime)
        {
            switch (action)
            {
                default:
                    base.Update(deltaTime);
                    break;
            }
        }

        /****************************************************************************/
        /// ACTIONS HANDLING
        /****************************************************************************/
        
        

        /****************************************************************************/
        /// EVENTS HANDLING
        /****************************************************************************/

        

        /*****************************************************************************************/
    }

    

}
