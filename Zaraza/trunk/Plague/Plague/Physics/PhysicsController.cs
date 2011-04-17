using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using JigLibX.Physics;


/****************************************************************************/
/// PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{

    /****************************************************************************/
    /// PhysicsController
    /****************************************************************************/
    class PhysicsController : Controller
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        internal static PhysicsManager physicsManager;
        private Body body;
        private Vector3 worldForce = Vector3.Zero;
        private Vector3 worldTorque = Vector3.Zero;

        private Vector3 localForce = Vector3.Zero;
        private Vector3 localTorque = Vector3.Zero;

        private ConstraintWorldPoint constraintPoint = new ConstraintWorldPoint();
        /****************************************************************************/




        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public PhysicsController(Body body)
        {
            this.body = body;
            physicsManager.controllers.Add(this);
            EnableController();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// RealeseMe
        /****************************************************************************/
        public void RealeseMe()
        {
            DisableController();
            physicsManager.controllers.Remove(this);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// MoveToPoint
        /****************************************************************************/
        public void MoveToPoint(Vector3 pointPosition,bool localPoint)
        {
            if (constraintPoint.IsConstraintEnabled)
            {
                constraintPoint.DisableConstraint();
                constraintPoint.Destroy();
            }

            if (localPoint)
            {
                constraintPoint.Initialise(body, Vector3.Zero, pointPosition + body.Position);
            }
            else
            {
                constraintPoint.Initialise(body, Vector3.Zero, pointPosition);
            }

        }
        /****************************************************************************/




        /****************************************************************************/
        /// DisableMoveToPoint
        /****************************************************************************/
        public void DisableMoveToPoint()
        {
            constraintPoint.DisableConstraint();
            constraintPoint.Destroy();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// AddWorldForce
        /****************************************************************************/
        public void AddWorldForce(Vector3 force)
        {
            this.worldForce = force;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// DisableWorldForce
        /****************************************************************************/
        public void DisableWorldForce()
        {
            this.worldForce = Vector3.Zero;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// AddWorldTorque
        /****************************************************************************/
        public void AddWorldTorque(Vector3 torque)
        {
            this.worldTorque = torque;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// DisableWorldTorque
        /****************************************************************************/
        public void DisableWorldTorque()
        {
            this.worldTorque = Vector3.Zero;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// AddLocalForce
        /****************************************************************************/
        public void AddLocalForce(Vector3 force)
        {
            this.localForce = force;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// DisableLocalForce
        /****************************************************************************/
        public void DisableLocalForce()
        {
            this.localForce = Vector3.Zero;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// AddLocalTorque
        /****************************************************************************/
        public void AddLocalTorque(Vector3 torque)
        {
            this.localTorque = torque;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// DisableLocalTorque
        /****************************************************************************/
        public void DisableLocalTorque()
        {
            this.localTorque = Vector3.Zero;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// UpdateController
        /****************************************************************************/
        public override void UpdateController(float dt)
        {
            if (worldForce != Vector3.Zero)
            {
                body.AddWorldForce(worldForce);
            }
            if (worldTorque != Vector3.Zero)
            {
                body.AddWorldTorque(worldTorque);
            }

            if (localForce != Vector3.Zero)
            {
                body.AddBodyForce(localForce);
            }
            if (localTorque != Vector3.Zero)
            {
                body.AddBodyTorque(localTorque);
            }


            if (constraintPoint.IsConstraintEnabled)
            {
                if (constraintPoint.WorldPosition == body.Position)
                {
                    constraintPoint.DisableConstraint();
                    constraintPoint.Destroy();
                }
            }



        }
        /****************************************************************************/




    }
    /****************************************************************************/



}
/****************************************************************************/