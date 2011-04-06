using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.Physics.Components
/************************************************************************************/
namespace PlagueEngine.Physics.Components
{

    /********************************************************************************/
    /// CylinderBodyComponent
    /********************************************************************************/
    class CylinderBodyComponent : RigidBodyComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private float  radius;
        private float  length;
        private Matrix skinTransform;
        /****************************************************************************/


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public CylinderBodyComponent(GameObjectInstance gameObject,
                                     MaterialProperties material,
                                     float              mass,
                                     bool               immovable,
                                     float              radius,
                                     float              length,
                                     Matrix             world,
                                     Matrix             skinTransform)
            : base(gameObject,material,mass,immovable)
        {
            this.radius        = radius;
            this.length        = length;
            this.skinTransform = skinTransform;

            //Capsule middle = new Capsule(new Vector3(0,20,0), 
            //                             skinTransform, 
            //                             radius, 
            //                             length - 2 * radius);

            //float sideLength = 2.0f * radius / (float)Math.Sqrt(2.0d);

            //Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);

            //sides += new Vector3(0, 10, 0);

            Box supply0 = new Box(skinTransform.Translation, skinTransform, new Vector3(radius,length,radius));

            //Box supply1 = new Box(Vector3.Zero, Matrix.CreateRotationZ(MathHelper.PiOver4), new Vector3(sideLength, sideLength, length));

            //skin.AddPrimitive(middle,  material);
            skin.AddPrimitive(supply0, material);
            //skin.AddPrimitive(supply1, material);
                        
            skin.ApplyLocalTransform(new Transform(-SetMass(), Matrix.Identity));


            /***************************************/
            // Manually set body inertia
            /***************************************
            float cylinderMass = body.Mass;

            float comOffs = (length - 2.0f * radius) * 0.5f; ;

            float Ixx = 0.5f  * cylinderMass * radius * radius + cylinderMass * comOffs * comOffs;
            float Iyy = 0.25f * cylinderMass * radius * radius + (1.0f / 12.0f) * cylinderMass * length * length + cylinderMass * comOffs * comOffs;
            float Izz = Iyy;

            body.SetBodyInertia(Ixx, Iyy, Izz);
            /***************************************/

            MoveTo(world);
            Enable();            
        }
        /****************************************************************************/
        

        /****************************************************************************/
        // Properties
        /****************************************************************************/
        public float  Radius        { get { return radius;        } }
        public float  Length        { get { return length;        } }
        public Matrix SkinTransform { get { return skinTransform; } }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/