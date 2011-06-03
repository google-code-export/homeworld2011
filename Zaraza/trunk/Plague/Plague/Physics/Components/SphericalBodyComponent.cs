using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;



/****************************************************************************/
/// PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{

    /****************************************************************************/
    /// SphericalBodyComponent
    /****************************************************************************/
    class SphericalBodyComponent : RigidBodyComponent
    {


        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private float radius;
        /****************************************************************************/




        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SphericalBodyComponent(bool enabled, GameObjectInstance gameObject,
                            float mass,
                            float radius,
                            MaterialProperties material,
                            bool immovable,
                            Matrix world,
                            Vector3 skinTranslation,
                            float yaw,
                            float pitch,
                            float roll)
            : base( enabled,gameObject, mass, immovable, material, skinTranslation,yaw,pitch,roll)
    {
            this.radius = radius;
            Sphere sphere = new Sphere(Vector3.Zero, radius);
            Skin.AddPrimitive(sphere, material);

            Skin.ApplyLocalTransform(new Transform(-SetMass(), Matrix.Identity));

            world = SkinLocalMatrix * world;
            MoveTo(world);

            if (enabled) Enable();                        
    }
        /****************************************************************************/



 


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Radius { get; set; }
        /****************************************************************************/



    }
    /****************************************************************************/



}
/****************************************************************************/