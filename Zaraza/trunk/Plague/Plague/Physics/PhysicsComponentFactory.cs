using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using JigLibX.Collision;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics.Components;
using PlagueEngine.Resources;
using Microsoft.Xna.Framework.Graphics;

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
        /// Create Capsule Body Component
        /****************************************************************************/
        public CapsuleBodyComponent CreateCapsuleBodyComponent(GameObjectInstance gameObject,
                                    float mass,
                                    float radius,
                                    float length,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness,
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float skinYaw,
                                    float skinPitch,
                                    float skinRoll)
        {
            MaterialProperties material = new MaterialProperties(elasticity, staticRoughness, dynamicRoughness);

            CapsuleBodyComponent result = new CapsuleBodyComponent(gameObject, mass, radius, length, material, immovable, world, skinTranslation, skinYaw, skinPitch, skinRoll);
            return result;
        }
        /****************************************************************************/






        /****************************************************************************/
        /// Create Square Body Component
        /****************************************************************************/
        public SquareBodyComponent CreateSquareBodyComponent(GameObjectInstance gameObject,
                                    float mass,
                                    float length,
                                    float height,
                                    float width,  
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness, 
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float skinYaw,
                                    float skinPitch,
                                    float skinRoll)
        {
            MaterialProperties material = new MaterialProperties(elasticity, staticRoughness, dynamicRoughness);

            SquareBodyComponent result = new SquareBodyComponent(gameObject, mass, length, height, width, material, immovable, world,skinTranslation,skinYaw,skinPitch,skinRoll);
            return result;
        }
        /****************************************************************************/






        /****************************************************************************/
        /// Create Spherical Body Component
        /****************************************************************************/
        public SphericalBodyComponent CreateSphericalBodyComponent(GameObjectInstance gameObject,
                                    float mass,
                                    float radius,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness,
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float skinYaw,
                                    float skinPitch,
                                    float skinRoll)
        {
            MaterialProperties material = new MaterialProperties(elasticity, staticRoughness, dynamicRoughness);

            SphericalBodyComponent result = new SphericalBodyComponent(gameObject, mass, radius, material, immovable, world,skinTranslation,skinYaw,skinPitch,skinRoll);
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





        /****************************************************************************/
        /// Create Square Skin Component
        /****************************************************************************/
        public SquareSkinComponent CreateSquareSkinComponent(GameObjectInstance gameObject,
                                                                float elasticity,
                                                                float staticRoughness,
                                                                float dynamicRoughness,
                                                                Matrix world,
                                                                float length,
                                                                float height,
                                                                float width,
                                                                Vector3 skinTranslation,
                                                                float yaw,
                                                                float pitch, 
                                                                float roll)
        {
            MaterialProperties material = new MaterialProperties(elasticity,
                                                                staticRoughness,
                                                                dynamicRoughness);

            SquareSkinComponent result = new SquareSkinComponent(gameObject,
                                                                 world,
                                                                 length,
                                                                 height,
                                                                 width,
                                                                 material,
                                                                 skinTranslation,
                                                                 yaw,
                                                                 pitch,
                                                                 roll);
            return result;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Create Square Skin Component
        /****************************************************************************/
        public SphericalSkinComponent CreateSphericalSkinComponent(GameObjectInstance gameObject,
                                                                float elasticity,
                                                                float staticRoughness,
                                                                float dynamicRoughness,
                                                                Matrix world,
                                                                float radius,
                                                                Vector3 skinTranslation,
                                                                float yaw,
                                                                float pitch,
                                                                float roll)
        {
            MaterialProperties material = new MaterialProperties(elasticity,
                                                                staticRoughness,
                                                                dynamicRoughness);

            SphericalSkinComponent result = new SphericalSkinComponent(gameObject,
                                                                 world,
                                                                 radius,
                                                                 material,
                                                                 skinTranslation,
                                                                 yaw,
                                                                 pitch,
                                                                 roll);
            return result;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Create Cylindrical Skin Component
        /****************************************************************************/
        public CylindricalSkinComponent CreateCylindricalSkinComponent(GameObjectInstance gameObject,
                                                                float elasticity,
                                                                float staticRoughness,
                                                                float dynamicRoughness,
                                                                Matrix world,
                                                                float length,
                                                                float radius,
                                                                Vector3 skinTranslation,
                                                                float yaw,
                                                                float pitch,
                                                                float roll)
        {
            MaterialProperties material = new MaterialProperties(elasticity,
                                                                staticRoughness,
                                                                dynamicRoughness);

            CylindricalSkinComponent result = new CylindricalSkinComponent(gameObject,
                                                                 world,
                                                                 length,
                                                                 radius,
                                                                 material,
                                                                 skinTranslation,
                                                                 yaw,
                                                                 pitch,
                                                                 roll);
            return result;
        }
        /****************************************************************************/



           
        /****************************************************************************/
        /// Create Cylidrical Body Component
        /****************************************************************************/
        public CylindricalBodyComponent CreateCylindricalBodyComponent(GameObjectInstance gameObject,
                                    float mass,
                                    float radius,
                                    float length,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness, 
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float skinYaw,
                                    float skinPitch,
                                    float skinRoll)
        {
            MaterialProperties material = new MaterialProperties(elasticity, staticRoughness, dynamicRoughness);

            CylindricalBodyComponent result = new CylindricalBodyComponent(gameObject, mass, radius, length, material, immovable, world,skinTranslation,skinYaw,skinPitch,skinRoll);
            return result;
        }
            /********************************************************************************/



        /****************************************************************************/
        /// Create Cylidrical Body Component 2
        /****************************************************************************/
        public CylindricalBodyComponent2 CreateCylindricalBodyComponent2(GameObjectInstance gameObject,
                                    float mass,
                                    float radius,
                                    float length,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness,
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float skinYaw,
                                    float skinPitch,
                                    float skinRoll)
        {
            MaterialProperties material = new MaterialProperties(elasticity, staticRoughness, dynamicRoughness);

            CylindricalBodyComponent2 result = new CylindricalBodyComponent2(gameObject, mass, radius, length, material, immovable, world, skinTranslation, skinYaw, skinPitch, skinRoll);
            return result;
        }
        /********************************************************************************/




    }

        /********************************************************************************/


    }
    /********************************************************************************/




