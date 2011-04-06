using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using JigLibX.Collision;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics.Components;


/************************************************************************************/
///  PlagueEngine.Physics
/************************************************************************************/
namespace PlagueEngine.Physics
{

    /********************************************************************************/
    ///  PhysicsComponentFactory
    /********************************************************************************/
    class PhysicsComponentFactory
    {

        /****************************************************************************/
        ///  CreateBoxPhysicsComponent
        /****************************************************************************
        public BoxPhysicsComponent CreateBoxPhysicsComponent(GameObjectInstance gameObject,float mass,Vector3 boxSize,float elasticity,float staticRoughness,float dynamicRoughness, bool immovable,Matrix world)
        {
            BoxPhysicsComponent bpc = new BoxPhysicsComponent(gameObject,physicsManager,mass,boxSize,elasticity,staticRoughness,dynamicRoughness,immovable,world);            
            return bpc;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Cylinder Body Component
        /****************************************************************************/
        public CylinderBodyComponent CreateCylinderBodyComponent(   GameObjectInstance gameObject,
                                                                    float mass,
                                                                    float radius,
                                                                    float length,
                                                                    float elasticity,
                                                                    float staticRoughness,
                                                                    float dynamicRoughness,                                
                                                                    bool  immovable,
                                                                    Matrix world)
        {
            MaterialProperties material = new MaterialProperties(elasticity,
                                                                 staticRoughness,
                                                                 dynamicRoughness);
            
            CylinderBodyComponent result = new CylinderBodyComponent(gameObject,
                                                                     mass,
                                                                     radius,
                                                                     length,
                                                                     material,
                                                                     immovable,
                                                                     world);

            return result;
        }
        /****************************************************************************/

        
    }
    /********************************************************************************/

}
/************************************************************************************/