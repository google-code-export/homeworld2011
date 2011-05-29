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
        private bool enabled = true;


        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInFrame = new Dictionary<GameObjectInstance, int>();
        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInPrevFrame = new Dictionary<GameObjectInstance, int>();
        
        private List<Type> subscribedGameObjectTypesCollisionsEvents = new List<Type>();
        private List<Type> subscribedGameObjectTypesLostCollisionsEvents = new List<Type>();
        private List<Type> subscribedGameObjectTypesStartCollisionEvents = new List<Type>();
        private List<int> subsribedGameObjectStartCollisionEvents = new List<int>();
        private List<int> subsribedGameObjectCollisionsEvents = new List<int>();
        private List<int> subscribedGameObjectLostCollisionsEvents = new List<int>();
        

        private List<Type> gameObjectsTypeToColide = new List<Type>();
        private List<Type> gameObjectsTypeToNotColide = new List<Type>();
        private List<int> gameObjectsToColide       = new List<int>();
        private List<int> gameObjectsToNotColide    = new List<int>();

        public bool dontCollide { get; set; }
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
            dontCollide=false;
        }
        /****************************************************************************/

 
        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            if (!((GameObjectInstance)(collidee.ExternalData) == null))
            {
               
                if (subscribedGameObjectTypesCollisionsEvents.Count != 0)
                {
                    
                }
                if (!gameObjectsCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                {
                    if (subsribedGameObjectCollisionsEvents.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
                    {
                        if (!gameObjectsCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                        {
                            this.GameObject.SendEvent(
                                new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                                EventsSystem.Priority.Normal,
                                this.GameObject);

                            
                        }
                    }




                    if (subscribedGameObjectTypesCollisionsEvents.Contains(collidee.ExternalData.GetType()))
                    {

                        this.GameObject.SendEvent(
                            new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                            EventsSystem.Priority.Normal,
                            this.GameObject);

                        
                    }

                    gameObjectsCollisionInFrame.Add((GameObjectInstance)(collidee.ExternalData), ((GameObjectInstance)(collidee.ExternalData)).ID);

                }

                if (dontCollide)
                {
                    return false;
                }


                if (gameObjectsToNotColide.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
                {
                    return false;
                }

                if (gameObjectsToColide.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
                {
                    return true;
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

            }
            return false;
        }
        /****************************************************************************/

        private void SendCollisionEvents()
        {
            if (gameObjectsCollisionInPrevFrame.Count != 0 && (subscribedGameObjectLostCollisionsEvents.Count != 0 || subscribedGameObjectTypesLostCollisionsEvents.Count != 0))
            {
                foreach (GameObjectInstance go in gameObjectsCollisionInPrevFrame.Keys)
                {
                    if (!gameObjectsCollisionInFrame.ContainsKey(go))
                    {
                        if (subscribedGameObjectLostCollisionsEvents.Contains(go.ID) || subscribedGameObjectTypesLostCollisionsEvents.Contains(go.GetType()))
                        {
                            this.GameObject.SendEvent(
                            new LostCollisionEvent(go),
                            EventsSystem.Priority.Normal,
                            this.GameObject);
                        }
                    }
                }
            }

            if (gameObjectsCollisionInFrame.Count != 0 && (subsribedGameObjectStartCollisionEvents.Count != 0 || subscribedGameObjectTypesStartCollisionEvents.Count != 0))
            {
                foreach (GameObjectInstance go in gameObjectsCollisionInFrame.Keys)
                {
                    if (!gameObjectsCollisionInPrevFrame.ContainsKey(go))
                    {
                        if (subsribedGameObjectStartCollisionEvents.Contains(go.ID) || subscribedGameObjectTypesStartCollisionEvents.Contains(go.GetType()))
                        {
                            this.GameObject.SendEvent(
                            new StartCollisionEvent(go),
                            EventsSystem.Priority.Normal,
                            this.GameObject);
                        }
                    }
                }
            }
            gameObjectsCollisionInPrevFrame = gameObjectsCollisionInFrame;
            gameObjectsCollisionInFrame = new Dictionary<GameObjectInstance, int>();
        }


        /****************************************************************************/
        /// DisableBody
        /****************************************************************************/
        public void DisableBody()
        {
            if (enabled)
            {
                this.body.DisableBody();                
                PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(this.skin);
                physicsManager.rigidBodies.Remove(this.gameObject.ID);
                enabled = false;
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// EnableBody
        /****************************************************************************/
        public void EnableBody()
        {
            if (!enabled)
            {
                UpdateRotation();
                PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(this.skin);
                this.body.EnableBody();
                this.MoveTo(gameObject.World);
                physicsManager.rigidBodies.Add(this.gameObject.ID, this);
                enabled = true;
            }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Subscribe Start Collision Event
        /****************************************************************************/
        public void SubscribeStartCollisionEvent(params Type[] gameObjectTypes)
        {
            this.subscribedGameObjectTypesStartCollisionEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Start Collision Event
        /****************************************************************************/
        public void SubscribeStartCollisionEvent(params int[] gameObjects)
        {
            this.subsribedGameObjectStartCollisionEvents.AddRange(gameObjects);
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Cancel Subscribe Start Collision Event
        /****************************************************************************/
        public void CancelSubscribeStartCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.subscribedGameObjectTypesStartCollisionEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Cancel Subscribe Start Collision Event
        /****************************************************************************/
        public void CancelSubscribeStartCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.subsribedGameObjectStartCollisionEvents.Remove(gameObject);
            }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// CollideWithGameObjects
        /****************************************************************************/
        public void CollideWithGameObjects(params int[] gameObjects)
        {
            this.gameObjectsToColide.AddRange(gameObjects);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CancelCollisionWithGameObjects
        /****************************************************************************/
        public void CancelCollisionWithGameObjects(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.gameObjectsToColide.Remove(gameObject);
            }
        }
        /****************************************************************************/




        /****************************************************************************/
        /// DontCollideWithGameObjects
        /****************************************************************************/
        public void DontCollideWithGameObjects(params int[] gameObjects)
        {
            this.gameObjectsToNotColide.AddRange(gameObjects);
        }
        /****************************************************************************/



        /****************************************************************************/
        /// CancelNoCollisionWithGameObjects
        /****************************************************************************/
        public void CancelNoCollisionWithGameObjects(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.gameObjectsToNotColide.Remove(gameObject);
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Subscribe Collision Event
        /****************************************************************************/
        public void SubscribeCollisionEvent(params int[] gameObjects)
        {
            this.subsribedGameObjectCollisionsEvents.AddRange(gameObjects);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.subsribedGameObjectCollisionsEvents.Remove(gameObject);
            }
        }
        /****************************************************************************/






        /// Subscribe Lost Collision Event
        /****************************************************************************/
        public void SubscribeLostCollisionEvent(params int[] gameObjects)
        {
            this.subscribedGameObjectLostCollisionsEvents.AddRange(gameObjects);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Lost Collision Event
        /****************************************************************************/
        public void CancelSubscribeLostCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.subscribedGameObjectLostCollisionsEvents.Remove(gameObject);
            }
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
                this.subscribedGameObjectTypesCollisionsEvents.Remove(gameObjectType);
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
            this.subscribedGameObjectTypesCollisionsEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.subscribedGameObjectTypesCollisionsEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Lost Collision Event
        /****************************************************************************/
        public void SubscribeLostCollisionEvent(params Type[] gameObjectTypes)
        {
            this.subscribedGameObjectTypesLostCollisionsEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Lost Collision Event
        /****************************************************************************/
        public void CancelSubscribeLostCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                this.subscribedGameObjectTypesLostCollisionsEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/


        private void UpdateRotation()
        {

            Vector3 t = Vector3.Transform(translation, gameObject.World);


            gameObject.World.Translation = t;

            Quaternion quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Up, MathHelper.ToRadians(roll));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Right, MathHelper.ToRadians(pitch));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);

            quaternion = Quaternion.CreateFromAxisAngle(gameObject.World.Forward, MathHelper.ToRadians(yaw));
            gameObject.World.Forward = Vector3.Transform(gameObject.World.Forward, quaternion);
            gameObject.World.Right = Vector3.Transform(gameObject.World.Right, quaternion);
            gameObject.World.Up = Vector3.Transform(gameObject.World.Up, quaternion);


        }



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

            SendCollisionEvents();
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
            if(gameObject!=null)physicsManager.rigidBodies.Remove(this.gameObject.ID);
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


                deltaVel.Y = -5.0f;
                //deltaVel.Y = 0.0f;
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