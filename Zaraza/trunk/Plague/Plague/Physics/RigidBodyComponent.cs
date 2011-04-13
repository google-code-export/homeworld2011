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
/// Constructor
/****************************************************************************/
namespace PlagueEngine.Physics
{



    /****************************************************************************/
    /// Constructor
    /****************************************************************************/
    class RigidBodyComponent:GameObjectComponent
    {



        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Body body;
        private CollisionSkin skin;
        internal static PhysicsManager physicsManager;
        private float mass;
        private MaterialProperties material;
        /****************************************************************************/





        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RigidBodyComponent(GameObjectInstance gameObject, float mass, bool immovable,MaterialProperties material)
            : base(gameObject)
        {
            this.mass = mass;
            body = new Body();            
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;
            skin.ExternalData = gameObject.ID;
            this.material = material;

            body.Immovable = immovable;

    
            physicsManager.rigidBodies.Add(this);
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            
            gameObject.World=body.Orientation;
            gameObject.World.Translation=body.Position;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// SetMass
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
        /// ReleaseMe
        /****************************************************************************/
        public override void ReleaseMe()
        {
            body.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
            physicsManager.rigidBodies.Remove(this);
        }
        /****************************************************************************/





        /****************************************************************************/
        /// MoveTo
        /****************************************************************************/
        public void MoveTo(Matrix matrix)
        {
            Matrix tmp = matrix;
            tmp.Translation = Vector3.Zero;
            body.MoveTo(matrix.Translation, tmp);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Properties
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

        public bool IsEnabled { get { return body.IsBodyEnabled; } }
        public float Elasticity { get { return material.Elasticity; } }
        public float StaticRoughness { get { return material.StaticRoughness; } }
        public float DynamicRoughness { get { return material.DynamicRoughness; } }
        public float Mass { get { return mass; } }

        public Body Body { get { return this.body; } }
        public CollisionSkin Skin { get { return this.skin; } }
        /****************************************************************************/




    }
    /****************************************************************************/



}
/****************************************************************************/