﻿using System;
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
        private RigidBodyComponent rigidBodyComponent;
        private BodyExtended body;
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
        public PhysicsController(RigidBodyComponent rigidBodyComponent)
        {
            this.rigidBodyComponent = rigidBodyComponent;
            body = (BodyExtended)rigidBodyComponent.Body;
            physicsManager.controllers.Add(this);
            EnableController();
            body.Immovable = true;             
            RotationConstraint rotationConstraint = new RotationConstraint();
            rotationConstraint.Init(body);
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


        public void MoveUp(float dt)
        {
            //body.Controllable = true;
            body.Immovable = false;
            body.DesiredVelocity = Vector3.Up * dt;            
        }

        public void MoveDown(float dt)
        {            
            //body.Controllable = true;
            body.Immovable = false;
            body.DesiredVelocity = Vector3.Down * dt;         
        }
        public void MoveLeft(float dt)
        {            
            //body.Controllable = true;
            body.Immovable = false;
            body.DesiredVelocity = Vector3.Left * dt;         
        }

        public void MoveRight(float dt)
        {            
            //body.Controllable = true;
            body.Immovable = false;
            body.DesiredVelocity = Vector3.Right * dt;         
        }

        public void MoveBackward(float dt)
        {            
            //body.Controllable = true;
            body.Immovable = false;
            body.DesiredVelocity = Vector3.Backward * dt;            
        }

        public void MoveForward(float dt)
        {            
            body.Immovable = false;
            //body.Controllable = true;
            body.DesiredVelocity = Vector3.Forward * dt;            
        }
          

        public void StopMoving()
        {
            body.DesiredVelocity = new Vector3(0,0,0);
            //body.Controllable = false;            
            //body.Immovable = true;
        }

        public bool IsControlEnabled
        {
            get { return body.Controllable; }
        }
        public void DisableControl()
        {
            body.Controllable = false;
            StopMoving();
        }

        public void EnableControl()
        {
            body.Controllable = true;
            body.SetUpOrientationForController();
        }


        public void Rotate(float dt)
        {
            body.Controllable = true;
            body.Immovable = false;
            body.DesiredOrientation *= Matrix.CreateRotationY(MathHelper.ToRadians(dt));

        }

        public Matrix Desiredorientation { get { return body.DesiredOrientation; } set { body.DesiredOrientation = value; } }

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


    class RotationConstraint : Constraint
    {

        private Body Body = null;

        public void Init(Body body)
        {
            Body = body;
            Satisfied = false;
            body.AddConstraint(this);
            EnableConstraint();
        }

        public override bool Apply(float dt)
        {
            Body.AngularVelocity *= Vector3.Up;
            Body.Velocity *= new Vector3(1, 0, 1);
            return true;
        }

        public override void Destroy()
        {            
            Body = null;
            DisableConstraint();            
        }

        public override void PreApply(float dt)
        {
               
        }
    }
    
}
/****************************************************************************/