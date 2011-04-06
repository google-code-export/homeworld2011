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


/************************************************************************************/
/// PlagueEngine.Physics
/************************************************************************************/
namespace PlagueEngine.Physics
{

    /********************************************************************************/
    /// RigidBodyComponent
    /********************************************************************************/
    class RigidBodyComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected PlagueEngineCollisionSkin skin = null;
        protected Body                      body = null;
                
        private MaterialProperties material;
        private float              mass;

        internal static PhysicsManager physicsManager = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RigidBodyComponent(GameObjectInstance gameObject, 
                                  MaterialProperties material,
                                  float mass,
                                  bool immovable)
            : base(gameObject)
        {
            body = new Body();
            skin = new PlagueEngineCollisionSkin(gameObject, body);
            body.CollisionSkin = skin;

            this.mass       = mass;
            this.material   = material;
            body.Immovable  = immovable;

            physicsManager.rigidBodies.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            if (!body.IsBodyEnabled || body.Immovable || !body.IsActive) return;

            gameObject.World = body.Orientation;
            gameObject.World.Translation = body.Position;  
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set mass
        /****************************************************************************/
        protected Vector3 SetMass()
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                                    PrimitiveProperties.MassDistributionEnum.Solid,
                                    PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk;
            Vector3 centerOfMass;
            Matrix it;
            Matrix itCoM;

            skin.GetMassProperties(primitiveProperties, out junk, out centerOfMass, out it, out itCoM);

            body.BodyInertia = itCoM;
            body.Mass = junk;

            return centerOfMass;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Enable
        /****************************************************************************/
        public void Enable()
        {
            body.EnableBody();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable
        /****************************************************************************/
        public void Disable()
        {
            body.DisableBody();
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
        /// Move To
        /****************************************************************************/
        public void MoveTo(Matrix matrix)
        {
            body.MoveTo(matrix.Translation, matrix);   
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            body.DisableBody();
            skin.Release();
            physicsManager.rigidBodies.Remove(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool  IsEnabled        { get { return body.IsBodyEnabled;        } }
        public float Elasticity       { get { return material.Elasticity;       } }
        public float StaticRoughness  { get { return material.StaticRoughness;  } }
        public float DynamicRoughness { get { return material.DynamicRoughness; } }
        public float Mass             { get { return mass;                      } }

        public Body                      Body { get { return this.body; } }
        public PlagueEngineCollisionSkin Skin { get { return this.skin; } }
        /****************************************************************************/
    }
    /********************************************************************************/

}
/************************************************************************************/