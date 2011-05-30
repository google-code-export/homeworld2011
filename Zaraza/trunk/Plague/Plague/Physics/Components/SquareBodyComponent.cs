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


/****************************************************************************/
/// PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{



    /****************************************************************************/
    /// SquareBodyComponent
    /****************************************************************************/
    class SquareBodyComponent : RigidBodyComponent
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
        public SquareBodyComponent(bool enabled, GameObjectInstance gameObject,
                                    float mass,
                                    float length,
                                    float height,
                                    float width,  
                                    MaterialProperties material,
                                    bool immovable,
                                    Matrix world,
                                    Vector3 skinTranslation,
                                    float yaw,
                                    float pitch,
                                    float roll)
            : base( enabled,gameObject, mass, immovable, material, skinTranslation,yaw,pitch,roll)
        {
            this.length = length;
            this.width = width;
            this.height = height;

            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(length, height, width));

            Skin.AddPrimitive(box, material);

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
            
            //sdummyWorld.Translation += skinTranslation;
            MoveTo(dummyWorld);

            Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            Enable();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Length { get { return length; } }
        public float Height { get { return height; } }
        public float Width  { get { return width; } }
        /****************************************************************************/

    }
    /****************************************************************************/



}
/****************************************************************************/