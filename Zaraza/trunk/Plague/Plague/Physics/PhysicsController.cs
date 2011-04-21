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
        private ConstraintVelocity constraintVelocity = new ConstraintVelocity();
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
        /// Set Veolicty
        /****************************************************************************/
        public void SetVelocity(Vector3 velocity,Vector3 angularVelocty,bool local)
        {
            if (constraintVelocity.IsConstraintEnabled)
            {
                constraintVelocity.DisableConstraint();
                constraintVelocity.Destroy();
            }


                if (local)
                {
                    constraintVelocity.Initialise(body, ConstraintVelocity.ReferenceFrame.Body, velocity, angularVelocty);
                    constraintVelocity.EnableConstraint();

                }
                else
                {
                    constraintVelocity.Initialise(body, ConstraintVelocity.ReferenceFrame.World, velocity, angularVelocty);
                    constraintVelocity.EnableConstraint();
                }
            
        }
        /****************************************************************************/







        /****************************************************************************/
        /// Disable Veolicty
        /****************************************************************************/
        public void DisableVelocity()
        {
            body.RemoveConstraint(constraintVelocity);
            constraintVelocity.DisableConstraint();
            constraintVelocity.Destroy();

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
                constraintPoint.EnableConstraint();
            }
            else
            {
                constraintPoint.Initialise(body, Vector3.Zero, pointPosition);
                constraintPoint.EnableConstraint();
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
        /// SetWorldForce
        /****************************************************************************/
        public void SetWorldForce(Vector3 force)
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
        /// SetWorldTorque
        /****************************************************************************/
        public void SetWorldTorque(Vector3 torque)
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
        /// SetLocalForce
        /****************************************************************************/
        public void SetLocalForce(Vector3 force)
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
        /// SetLocalTorque
        /****************************************************************************/
        public void SetLocalTorque(Vector3 torque)
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