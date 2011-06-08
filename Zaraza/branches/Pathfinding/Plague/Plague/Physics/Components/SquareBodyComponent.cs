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
            
            Skin.ApplyLocalTransform(new Transform(-SetMass(), Matrix.Identity));

            //world = SkinLocalMatrix * world;
            MoveTo(world);

            if (enabled) Enable();
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