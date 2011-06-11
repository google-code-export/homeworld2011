using System.Collections.Generic;
using JigLibX.Physics;
using JigLibX.Collision;
using PlagueEngine.Resources;

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
        private  PhysicsSystem                              physicsSystem;
        internal Dictionary<int, RigidBodyComponent>        rigidBodies              = new Dictionary<int, RigidBodyComponent>();
        internal Dictionary<int, CollisionSkinComponent>    collisionSkins           = new Dictionary<int, CollisionSkinComponent>();
        internal List<Cone>                                 cones = new List<Cone>();
        internal List<PhysicsController>                    controllers              = new List<PhysicsController>();
        internal PhysicsComponentFactory                    physicsComponentFactory;
        /****************************************************************************/


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsManager(ContentManager content)
        {                       
            physicsComponentFactory       = new PhysicsComponentFactory(content);
            physicsSystem                 = new PhysicsSystem
                                                {
                                                    CollisionSystem = new CollisionSystemSAP(),
                                                    SolverType = PhysicsSystem.Solver.Fast,
                                                    EnableFreezing = true,
                                                    NumCollisionIterations = 5,
                                                    NumContactIterations = 5,
                                                    NumPenetrationRelaxtionTimesteps = 5
                                                };
            physicsSystem.CollisionSystem.UseSweepTests = true;
            RigidBodyComponent.physicsManager = this;
            CollisionSkinComponent.physicsManager = this;
            PhysicsUlitities.collisionSystem = physicsSystem.CollisionSystem;
            PhysicsController.physicsManager = this;
            Cone.physicsManager = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        ///  Update
        /****************************************************************************/
        public void Update(float timeStep)
        {
            if (timeStep < 1.0f / 60.0f) physicsSystem.Integrate(timeStep);
            else physicsSystem.Integrate(1.0f / 60.0f);

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





            List<Cone> wastedCones = new List<Cone>();

            foreach (Cone cone in cones)
            {
                cone.Update();
                if (cone.wasted)
                {
                    wastedCones.Add(cone);
                }
            }

            foreach (Cone cone in wastedCones)
            {
                cones.Remove(cone);
            }





            ExplosionManager.Update();
        }
        /****************************************************************************/
               
    }
    /********************************************************************************/
    
}
/************************************************************************************/