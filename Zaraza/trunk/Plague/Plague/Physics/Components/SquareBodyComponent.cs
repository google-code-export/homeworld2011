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
        public SquareBodyComponent (GameObjectInstance gameObject,
                                    float mass,
                                    float length,
                                    float height,
                                    float width,  
                                    MaterialProperties material,
                                    bool immovable,
                                    Matrix world):base (gameObject,mass,immovable,material)
        {
            this.length = length;
            this.width = width;
            this.height = height;

            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(length, height, width));

            Skin.AddPrimitive(box, material);

            Vector3 com = SetMass();

            MoveTo(world);

            Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            Enable();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Length { get; set; }
        public float Height { get; set; }
        public float Width  {get; set; }

    }
    /****************************************************************************/



}
/****************************************************************************/