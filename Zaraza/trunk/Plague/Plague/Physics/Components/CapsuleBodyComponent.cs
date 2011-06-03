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
    /// CapsuleBodyComponent
    /****************************************************************************/
    class CapsuleBodyComponent : RigidBodyComponent
    {


        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private float radius;
        private float length;
        /****************************************************************************/




        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public CapsuleBodyComponent(bool enabled,GameObjectInstance gameObject,
                            float mass,
                            float radius,
                            float length,
                            MaterialProperties material,
                            bool immovable,
                            Matrix world,
                            Vector3 skinTranslation,
                            float yaw,
                            float pitch,
                            float roll)
            : base(enabled, gameObject, mass, immovable, material, skinTranslation,yaw, pitch, roll)
        {

            this.radius = radius;
            this.length = length;

            Capsule middle = new Capsule(Vector3.Zero, Matrix.Identity, radius, length - 2.0f * radius);

            Skin.AddPrimitive(middle, material);
            Skin.ApplyLocalTransform(new Transform(-SetMass(), Matrix.Identity));

            /***************************************/
            // Manually set body inertia
            /***************************************/
            float cylinderMass = Body.Mass;

            float comOffs = (length - 2.0f * radius) * 0.5f; ;

            float Ixx = 0.5f * cylinderMass * radius * radius + cylinderMass * comOffs * comOffs;
            float Iyy = 0.25f * cylinderMass * radius * radius + (1.0f / 12.0f) * cylinderMass * length * length + cylinderMass * comOffs * comOffs;
            float Izz = Iyy;

            Body.SetBodyInertia(Ixx, Iyy, Izz);
            /***************************************/

            //world = Matrix.CreateTranslation(skinTranslation) * world;
            MoveTo(world);

            if (enabled) Enable();
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Radius { get { return radius; } }
        public float Length { get { return length; } }
        /****************************************************************************/


    }
    /****************************************************************************/



}
/****************************************************************************/