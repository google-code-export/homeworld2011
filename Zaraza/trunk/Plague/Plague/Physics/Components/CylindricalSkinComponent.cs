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
    /// CylindricalSkinComponent
    /****************************************************************************/
    class CylindricalSkinComponent : CollisionSkinComponent
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
        public CylindricalSkinComponent(GameObjectInstance gameObject,
                                    Matrix world,
                                    float length,
                                    float radius,
                                    MaterialProperties material,
                                    Vector3 skinTranslation,
                                    float yaw,
                                    float pitch,
                                    float roll)
            : base(gameObject, material, skinTranslation, yaw, pitch, roll)
        {
            this.radius = radius;
            this.length = length;

            SetSkin(world);

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
        }
        /****************************************************************************/



        protected override void SetSkin(Matrix world)
        {
            skin = new CollisionSkin();

            Matrix dummyWorld = world;

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

            dummyWorld.Translation += translation;



            float sideLength = 2.0f * radius / (float)Math.Sqrt(2.0d);

            Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);

            Box supply0 = new Box(dummyWorld.Translation + sides, dummyWorld, new Vector3(sideLength, sideLength, length));

            Box supply1 = new Box(dummyWorld.Translation + Vector3.Transform(sides, Matrix.CreateRotationZ(MathHelper.PiOver4)),
                Matrix.CreateRotationZ(MathHelper.PiOver4) * dummyWorld, new Vector3(sideLength, sideLength, length));

            Box supply2 = new Box(dummyWorld.Translation + Vector3.Transform(sides, Matrix.CreateRotationZ(MathHelper.PiOver4 / 2)),
              Matrix.CreateRotationZ(MathHelper.PiOver4 / 2) * dummyWorld, new Vector3(sideLength, sideLength, length));


            Box supply3 = new Box(dummyWorld.Translation + Vector3.Transform(sides, Matrix.CreateRotationZ(-MathHelper.PiOver4 / 2)),
            Matrix.CreateRotationZ(-MathHelper.PiOver4 / 2) * dummyWorld, new Vector3(sideLength, sideLength, length));


            Skin.AddPrimitive(supply3, material);
            Skin.AddPrimitive(supply2, material);
            Skin.AddPrimitive(supply0, material);
            Skin.AddPrimitive(supply1, material);



        }


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        /****************************************************************************/
        public float Radius { get { return radius; } }
        public float Length { get { return length; } }
        /****************************************************************************/



    }
    /****************************************************************************/



}
/****************************************************************************/