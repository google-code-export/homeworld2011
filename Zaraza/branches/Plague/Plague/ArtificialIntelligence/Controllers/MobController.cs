﻿using System;
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
                         float angle) : base(person)
        {
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
