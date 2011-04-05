using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow;
using Microsoft.Xna.Framework;
using PlagueEngine.Physics.Components;

/****************************************************************************/
///  PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{

    /****************************************************************************/
    ///  PhysicsComponentFactory
    /****************************************************************************/
    class PhysicsComponentFactory
    {

        /****************************************************************************/
        ///  Fields
        /****************************************************************************/
        PhysicsManager physicsManager = null;
        /****************************************************************************/


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsComponentFactory(PhysicsManager physicsManager)
        {
            this.physicsManager = physicsManager;
        }
        /****************************************************************************/


        /****************************************************************************/
        ///  CreateBoxPhysicsComponent
        /****************************************************************************/
        public BoxPhysicsComponent CreateBoxPhysicsComponent(GameObjectInstance gameObject,float mass,Vector3 boxSize,float elasticity,float staticRoughness,float dynamicRoughness, bool immovable,Matrix world)
        {
            BoxPhysicsComponent bpc = new BoxPhysicsComponent(gameObject,physicsManager,mass,boxSize,elasticity,staticRoughness,dynamicRoughness,immovable,world);
            physicsManager.PhysicsComponents.Add(bpc);
            return bpc;
        }
        /****************************************************************************/
        
    }
    /****************************************************************************/

}
/****************************************************************************/