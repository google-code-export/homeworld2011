﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JigLibX.Physics;
using JigLibX.Collision;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;

/****************************************************************************/
///  PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{
    /****************************************************************************/
    ///  PlagueEngine.Physics
    /****************************************************************************/
    class PhysicsManager
    {


        /****************************************************************************/
        ///  Fields
        /****************************************************************************/
        private PhysicsSystem physicsSystem = null;
        private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
        private PhysicsComponentFactory physicsComponentFactory = null;
        /****************************************************************************/




        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsManager()
        {
            physicsComponentFactory = new PhysicsComponentFactory(this);
            physicsSystem=new PhysicsSystem();
            physicsSystem.CollisionSystem=new CollisionSystemSAP();
            
        }
        /****************************************************************************/





        /****************************************************************************/
        ///  Update
        /****************************************************************************/
        public void Update(float timeStep)
        {
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);
            
            foreach (PhysicsComponent component in physicsComponents)
            {
       
                component.UpdateWorldMatrix();
            }
        }

        /****************************************************************************/



        /****************************************************************************/
        ///  Release Component
        /****************************************************************************/
        public void ReleaseComponent(PhysicsComponent physicsComponent)
        {

            physicsComponents.Remove(physicsComponent);

        }
        /****************************************************************************/




        /****************************************************************************/
        ///  Properties
        /****************************************************************************/
        public List<PhysicsComponent> PhysicsComponents { get { return this.physicsComponents; } }
        public PhysicsComponentFactory PhysicsComponentFactory { get { return this.physicsComponentFactory; } }
        /****************************************************************************/





    }
    /****************************************************************************/



}
/****************************************************************************/