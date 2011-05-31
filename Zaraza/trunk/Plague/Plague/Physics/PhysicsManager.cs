using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using JigLibX.Collision;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.Resources;


/************************************************************************************/
///  PlagueEngine.Physics
/************************************************************************************/
namespace PlagueEngine.Physics
{

    /********************************************************************************/
    ///  PlagueEngine.Physics
    /********************************************************************************/
    class PhysicsManager
    {
        
        /****************************************************************************/
        ///  Fields
        /****************************************************************************/
        private  PhysicsSystem                              physicsSystem            = null;
        internal Dictionary<int, RigidBodyComponent>        rigidBodies              = new Dictionary<int, RigidBodyComponent>();
        internal Dictionary<int, CollisionSkinComponent>    collisionSkins           = new Dictionary<int, CollisionSkinComponent>();
        internal List<PhysicsController>                    controllers              = new List<PhysicsController>();
        internal PhysicsComponentFactory                    physicsComponentFactory  = null;
        /****************************************************************************/


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsManager(ContentManager content)
        {                       


            physicsComponentFactory       = new PhysicsComponentFactory(content);
            physicsSystem                 = new PhysicsSystem();

            //physicsSystem.CollisionSystem = new CollisionSystemBrute();

            physicsSystem.CollisionSystem = new CollisionSystemSAP();

            physicsSystem.SolverType = PhysicsSystem.Solver.Fast;
            physicsSystem.EnableFreezing                = true;
            //physicsSystem.IsShockStepEnabled = true;
            physicsSystem.CollisionSystem.UseSweepTests = true;
            
            physicsSystem.NumCollisionIterations           = 1;
            physicsSystem.NumContactIterations             = 1;
            physicsSystem.NumPenetrationRelaxtionTimesteps = 1;

            RigidBodyComponent.physicsManager = this;
            CollisionSkinComponent.physicsManager = this;
            PhysicsUlitities.collisionSystem = physicsSystem.CollisionSystem;
            PhysicsController.physicsManager = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        ///  Update
        /****************************************************************************/
        public void Update(float timeStep)
        {
            if (timeStep < 1.0f / 60.0f) physicsSystem.Integrate(timeStep);
            else physicsSystem.Integrate(1.0f / 60.0f);

            //PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);



            foreach (RigidBodyComponent rigidBody in rigidBodies.Values)
            {
                rigidBody.Update();
            }


            foreach (CollisionSkinComponent skin in collisionSkins.Values)
            {
                skin.Update();
            }

            foreach (PhysicsController controller in controllers)
            {
                controller.UpdateController(timeStep);
            }

            ExplosionManager.Update();
        }
        /****************************************************************************/
               
    }
    /********************************************************************************/
    
}
/************************************************************************************/