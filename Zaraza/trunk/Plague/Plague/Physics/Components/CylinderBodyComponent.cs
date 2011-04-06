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
    class CylinderBodyComponent : PhysicsComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public  readonly float              Mass;
        public  readonly float              Radius;
        public  readonly float              Length;
        
        private readonly MaterialProperties material;
        /****************************************************************************/


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public CylinderBodyComponent(GameObjectInstance gameObject,
                                     float mass,
                                     float radius,
                                     float length,
                                     MaterialProperties material,
                                     bool immovable,
                                     Matrix world)
            : base(gameObject)
        {

            this.Mass     = mass;
            this.Radius   = radius;
            this.Length   = length;
            this.material = material;

            Capsule middle = new Capsule(Vector3.Zero, Matrix.Identity, radius, length - 2.0f * radius);

            float sideLength = 2.0f * radius / (float)Math.Sqrt(2.0d);

            Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);

            Box supply0 = new Box(sides, Matrix.Identity,
                new Vector3(sideLength, sideLength, length));

            Box supply1 = new Box(Vector3.Transform(sides, Matrix.CreateRotationZ(MathHelper.PiOver4)),
                Matrix.CreateRotationZ(MathHelper.PiOver4), new Vector3(sideLength, sideLength, length));

            skin.AddPrimitive(middle,  material);
            skin.AddPrimitive(supply0, material);
            skin.AddPrimitive(supply1, material);

            skin.ApplyLocalTransform(new Transform(-SetMass(mass), Matrix.Identity));

            float cylinderMass = body.Mass;

            float comOffs = (length - 2.0f * radius) * 0.5f; ;

            float Ixx = 0.5f  * cylinderMass * radius * radius + cylinderMass * comOffs * comOffs;
            float Iyy = 0.25f * cylinderMass * radius * radius + (1.0f / 12.0f) * cylinderMass * length * length + cylinderMass * comOffs * comOffs;
            float Izz = Iyy;

            body.SetBodyInertia(Ixx, Iyy, Izz);

            body.Immovable = immovable;
            body.MoveTo(world.Translation, world);

            Enable();            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Immovable
        /****************************************************************************/
        public bool Immovable
        {
            get
            {
                return body.Immovable;
            }

            set
            {
                body.Immovable = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        // Properties
        /****************************************************************************/
        public float Elasticity       { get { return material.Elasticity;       } }
        public float StaticRoughness  { get { return material.StaticRoughness;  } }
        public float DynamicRoughness { get { return material.DynamicRoughness; } }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/