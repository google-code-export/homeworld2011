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
            Vector3 com = SetMass();

            Vector3 translation = world.Translation;

            Matrix dummyWorld = world;
            dummyWorld.M41 = 0;
            dummyWorld.M42 = 0;
            dummyWorld.M43 = 0;

            Vector3 t = Vector3.Transform(skinTranslation, dummyWorld);
            dummyWorld.Translation = translation + t;
            

            Quaternion quaternion = Quaternion.CreateFromAxisAngle(dummyWorld.Forward, MathHelper.ToRadians(yaw));
            dummyWorld.Forward = Vector3.Transform(dummyWorld.Forward, quaternion);
            dummyWorld.Right = Vector3.Transform(dummyWorld.Right, quaternion);
            dummyWorld.Up = Vector3.Transform(dummyWorld.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(dummyWorld.Right, MathHelper.ToRadians(pitch));
            dummyWorld.Forward = Vector3.Transform(dummyWorld.Forward, quaternion);
            dummyWorld.Right = Vector3.Transform(dummyWorld.Right, quaternion);
            dummyWorld.Up = Vector3.Transform(dummyWorld.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(dummyWorld.Up, MathHelper.ToRadians(roll));
            dummyWorld.Forward = Vector3.Transform(dummyWorld.Forward, quaternion);
            dummyWorld.Right = Vector3.Transform(dummyWorld.Right, quaternion);
            dummyWorld.Up = Vector3.Transform(dummyWorld.Up, quaternion);
            //dummyWorld.Translation += skinTranslation;
            MoveTo(dummyWorld);
            Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            Enable();


            
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