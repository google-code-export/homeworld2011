using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private  PhysicsSystem                  physicsSystem           = null;
        internal List<RigidBodyComponent>       rigidBodies             = new List<RigidBodyComponent>();
        internal List<CollisionSkinComponent>   collisionSkins          = new List<CollisionSkinComponent>();
        internal PhysicsComponentFactory        physicsComponentFactory = null;
        /****************************************************************************/
        

        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsManager(ContentManager content)
        {                       


            physicsComponentFactory       = new PhysicsComponentFactory(content);
            physicsSystem                 = new PhysicsSystem();
            
            physicsSystem.CollisionSystem = new CollisionSystemBrute();
            
            physicsSystem.SolverType = PhysicsSystem.Solver.Accumulated;
            physicsSystem.EnableFreezing                = true;
            physicsSystem.CollisionSystem.UseSweepTests = true;
            
            physicsSystem.NumCollisionIterations           = 10;
            physicsSystem.NumContactIterations             = 10;
            physicsSystem.NumPenetrationRelaxtionTimesteps = 15;

            RigidBodyComponent.physicsManager = this;
            CollisionSkinComponent.physicsManager = this;
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
            
            foreach (RigidBodyComponent rigidBody in rigidBodies)
            {       
                rigidBody.Update();
            }
        }
        /****************************************************************************/
               
    }
    /********************************************************************************/
    
}
/************************************************************************************/