using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using JigLibX.Utils;

using PlagueEngine.LowLevelGameFlow;



/****************************************************************************/
/// PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{


    /****************************************************************************/
    /// SquareSkinComponent
    /****************************************************************************/
    class SquareSkinComponent : CollisionSkinComponent
    {



        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private float length;
        private float height;
        private float width;
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SquareSkinComponent(GameObjectInstance gameObject,
                                    Matrix world,
                                    float length,
                                    float height,
                                    float width,
                                    MaterialProperties material,
                                    Vector3 skinTranslation,
                                    float yaw,
                                    float pitch,
                                    float roll)
            : base(gameObject, material, skinTranslation, yaw, pitch, roll)
        {

            this.length = length;
            this.width = width;
            this.height = height;


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

            Vector3 tr = Vector3.Zero;
            tr.X = translation.X - (length / 2);
            tr.Y = translation.X - (length / 2);
            tr.Z = translation.Z - (width / 2);
            dummyWorld.Translation += tr;

            Box box = new Box(dummyWorld.Translation, dummyWorld, new Vector3(length, height, width));

            Skin.AddPrimitive(box, material);

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
            
        }


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Length { get { return length; } }
        public float Height { get { return height; } }
        public float Width { get { return width; } }
        /****************************************************************************/

    }
    /****************************************************************************/




}
/****************************************************************************/