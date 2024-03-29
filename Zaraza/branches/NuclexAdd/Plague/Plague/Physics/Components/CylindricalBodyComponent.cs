﻿using System;
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
    /// CylindricalBodyComponent
    /****************************************************************************/
    class CylindricalBodyComponent : RigidBodyComponent
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
        public CylindricalBodyComponent(bool enabled, GameObjectInstance gameObject,
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
            : base( enabled,gameObject, mass, immovable, material, skinTranslation, yaw, pitch, roll)
        {

            this.radius = radius;
            this.length = length;   

            Capsule middle = new Capsule(Vector3.Zero, Matrix.Identity, radius, length - 2.0f * radius);

            float sideLength = 2.0f * radius / (float)Math.Sqrt(2.0d);

            Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);  

            Box supply0 = new Box(sides, Matrix.Identity, new Vector3(sideLength, sideLength, length));

            Box supply1 = new Box(Vector3.Transform(sides, Matrix.CreateRotationZ(MathHelper.PiOver4)),
                                                    Matrix.CreateRotationZ(MathHelper.PiOver4), 
                                                    new Vector3(sideLength, sideLength, length));
                        
            Skin.AddPrimitive(middle, material);
            Skin.AddPrimitive(supply0, material);
            Skin.AddPrimitive(supply1, material);

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

            //world = SkinLocalMatrix * world;
            MoveTo(world);

            if(enabled) Enable();
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