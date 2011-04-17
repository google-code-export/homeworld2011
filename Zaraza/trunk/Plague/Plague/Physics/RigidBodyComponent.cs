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
        private Vector3 translation = Vector3.Zero;
        private float yaw;
        private float pitch;
        private float roll;
       
        /****************************************************************************/





        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RigidBodyComponent(GameObjectInstance gameObject, float mass, bool immovable, MaterialProperties material, Vector3 translation,float yaw,float pitch,float roll)
            : base(gameObject)
        {
            this.mass = mass;
            body = new Body();            
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;
            skin.ExternalData = gameObject.ID;
            this.material = material;
            this.translation = translation;
            body.Immovable = immovable;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
         

            physicsManager.rigidBodies.Add(gameObject.ID, this);
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            
            gameObject.World=body.Orientation;
           
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Up, MathHelper.ToRadians(-roll));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Right, MathHelper.ToRadians(-pitch));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Forward, MathHelper.ToRadians(-yaw));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);
            


            gameObject.World.Translation=body.Position - translation;
    
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
            physicsManager.rigidBodies.Remove(this.gameObject.ID);
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
        public Vector3 SkinTranslation { get { return this.translation; } }
        public float Yaw { get { return this.yaw; } }
        public float Pitch { get { return this.pitch; } }
        public float Roll { get { return this.roll; } }
        /****************************************************************************/




    }
    /****************************************************************************/



}
/****************************************************************************/