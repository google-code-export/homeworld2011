using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using JigLibX.Collision;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics.Components;
using PlagueEngine.Resources;


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
        /// Fields
        /****************************************************************************/
        private ContentManager content = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public PhysicsComponentFactory(ContentManager content)
        {
            this.content = content;
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
                                                                     material,                                                     
                                                                     mass,
                                                                     immovable,
                                                                     radius,
                                                                     length,                                                                     
                                                                     world);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Terrain Skin Component
        /****************************************************************************/
        public TerrainSkinComponent CreateTerrainSkinComponent(GameObjectInstance gameObject,
                                                               String heightMap,
                                                               int width,
                                                               int length,
                                                               float height,
                                                               float scale,
                                                               float elasticity,
                                                               float staticRoughness,
                                                               float dynamicRoughness)
        {
            MaterialProperties material = new MaterialProperties(elasticity,
                                                                staticRoughness,
                                                                dynamicRoughness);

            TerrainSkinComponent result = new TerrainSkinComponent(gameObject,
                                                                   content.LoadTexture2D(heightMap),
                                                                   width,
                                                                   length,
                                                                   height,
                                                                   scale,
                                                                   material);
            return result;
        }
        /****************************************************************************/
                
    }
    /********************************************************************************/

}
/************************************************************************************/