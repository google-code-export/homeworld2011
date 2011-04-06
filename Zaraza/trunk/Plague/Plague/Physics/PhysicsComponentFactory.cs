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
                                                                     material,                                                     
                                                                     mass,
                                                                     immovable,
                                                                     radius,
                                                                     length,                                                                     
                                                                     world);

            return result;
        }
        /****************************************************************************/
                
    }
    /********************************************************************************/

}
/************************************************************************************/