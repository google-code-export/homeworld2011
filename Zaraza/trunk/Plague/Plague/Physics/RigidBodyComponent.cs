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
        private BodyExtended body;
        private CollisionSkin skin;
        internal static PhysicsManager physicsManager;
        private float mass;
        private MaterialProperties material;
        private Vector3 translation = Vector3.Zero;
        private float yaw;
        private float pitch;
        private float roll;

        private List<Type> subscribedGameObjectTypesEvents = new List<Type>();
        private List<Type> gameObjectsTypeToColide = new List<Type>();
        private List<Type> gameObjectsTypeToNotColide = new List<Type>();
        /****************************************************************************/





        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RigidBodyComponent(GameObjectInstance gameObject, float mass, bool immovable, MaterialProperties material, Vector3 translation,float yaw,float pitch,float roll )
            : base(gameObject)
        {
            this.mass = mass;
            body = new BodyExtended();            
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;
            skin.ExternalData = gameObject;
            this.material = material;
            this.translation = translation;
            body.Immovable = immovable;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            
            physicsManager.rigidBodies.Add(gameObject.ID, this);
            skin.callbackFn += new CollisionCallbackFn(HandleCollisionDetection);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {

            if (subscribedGameObjectTypesEvents.Contains(collidee.ExternalData.GetType()))
            {


                this.GameObject.SendEvent(
                    new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                    EventsSystem.Priority.Normal,
                    this.GameObject);
               
            }



            if (gameObjectsTypeToNotColide.Contains(collidee.ExternalData.GetType()))
            {
                return false;
            }

            if (gameObjectsTypeToColide.Count == 0) return true;

            if (gameObjectsTypeToColide.Contains(collidee.ExternalData.GetType()))
            {
                return true;
            }

            return false;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// CollideWithGameObjectsType
        /****************************************************************************/
        public void CollideWithGameObjectsType(params Type[] gameObjectTypes)
        {
            this.gameObjectsTypeToColide.AddRange(gameObjectTypes);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CancelCollisionWithGameObjectsType
        /****************************************************************************/
        public void CancelCollisionWithGameObjectsType(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.gameObjectsTypeToColide.Remove(gameObjectType);
            }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// DontCollideWithGameObjectsType
        /****************************************************************************/
        public void DontCollideWithGameObjectsType(params Type[] gameObjectTypes)
        {
            this.gameObjectsTypeToNotColide.AddRange(gameObjectTypes);
        }
        /****************************************************************************/



        /****************************************************************************/
        /// CancelNoCollisionWithGameObjectsType
        /****************************************************************************/
        public void CancelNoCollisionWithGameObjectsType(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.gameObjectsTypeToNotColide.Remove(gameObjectType);
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Subscribe Collision Event
        /****************************************************************************/
        public void SubscribeCollisionEvent(params Type[] gameObjectTypes)
        {
            this.subscribedGameObjectTypesEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.subscribedGameObjectTypesEvents.Remove(gameObjectType);
            }
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

            Vector3 t = Vector3.Transform(translation, gameObject.World);


            gameObject.World.Translation = body.Position - t;
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
            base.ReleaseMe();
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

    class BodyExtended : Body
    {

        public BodyExtended()
            : base()
        {
            DesiredVelocity = Vector3.Zero;
            DesiredOrientation = this.Orientation;
            Controllable = false;
           
        }
        public bool Controllable;
        public Vector3 DesiredVelocity;
        public Matrix DesiredOrientation;
        public bool OrientationSetuped = false;


        public void TransformDesiredVelocity(float yaw,float pitch,float roll)
        {
           DesiredVelocity= Vector3.Transform(DesiredVelocity, Matrix.CreateFromYawPitchRoll(yaw, pitch, roll));
        }

        public void SetUpOrientationForController()
        {
            DesiredOrientation = this.Orientation;
            OrientationSetuped = true;
        }


        
        public override void AddExternalForces(float dt)
        {
            ClearForces();


            if (Controllable)
            {
                this.AllowFreezing = false;
                this.EnableBody();


                AngularVelocity = Vector3.Zero;

                this.Orientation = DesiredOrientation;


                DesiredVelocity = Vector3.Transform(DesiredVelocity, this.Orientation);
                Vector3 deltaVel = DesiredVelocity - Velocity;


                deltaVel.Y = 0.0f;
                deltaVel *= 10.0f;


                float forceFactor = 90.0f;
                AddWorldForce(deltaVel * Mass * dt * forceFactor);
                //AddBodyForce(DesiredVelocity*3 * Mass * dt * forceFactor);

            }
      
            AddGravityToExternalForce();
        }
    }

}
/****************************************************************************/