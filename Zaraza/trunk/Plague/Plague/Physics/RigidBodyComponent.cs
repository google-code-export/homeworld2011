using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using PlagueEngine.LowLevelGameFlow;

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
        private bool isEnabled = true;
        public bool Enabled { get { return this.isEnabled; } }

        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInFrame = new Dictionary<GameObjectInstance, int>();
        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInPrevFrame = new Dictionary<GameObjectInstance, int>();
        
        private List<Type> subscribedGameObjectTypesCollisionsEvents = new List<Type>();
        private List<Type> subscribedGameObjectTypesLostCollisionsEvents = new List<Type>();
        private List<Type> subscribedGameObjectTypesStartCollisionEvents = new List<Type>();
        private List<int> subsribedGameObjectStartCollisionEvents = new List<int>();
        private List<int> subsribedGameObjectCollisionsEvents = new List<int>();
        private List<int> subscribedGameObjectLostCollisionsEvents = new List<int>();

        private bool subscribedAnyCollisionEvent = false;
        private bool CollisionOnThisFrame = false;

        private List<Type> gameObjectsTypeToColide = new List<Type>();
        private List<Type> gameObjectsTypeToNotColide = new List<Type>();
        private List<int> gameObjectsToColide       = new List<int>();
        private List<int> gameObjectsToNotColide    = new List<int>();

        public bool dontCollide { get; set; }

        protected Matrix InvertedSkinLocalMatrix;
        protected Matrix SkinLocalMatrix;
        /****************************************************************************/





        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RigidBodyComponent(bool enabled,GameObjectInstance gameObject, float mass, bool immovable, MaterialProperties material, Vector3 translation,float yaw,float pitch,float roll )
            : base(gameObject)
        {
            this.mass = mass;
            body = new BodyExtended();            
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;
            skin.ExternalData = gameObject;
            this.material = material;
            this.translation = translation;

            SkinOrientation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            
            body.SkinOrientation = SkinOrientation;
            SkinInvertedOrientation = Matrix.Invert(SkinOrientation);

            SkinLocalMatrix = Matrix.CreateTranslation(SkinTranslation) * SkinOrientation;
            
            InvertedSkinLocalMatrix = Matrix.Invert(SkinLocalMatrix);

            body.Immovable = immovable;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;         
            skin.callbackFn += HandleCollisionDetection;
            dontCollide=false;
            isEnabled = enabled;
            if (!isEnabled)
            {
                DisableBody();
            }
            else
            {
                physicsManager.rigidBodies.Add(gameObject.ID, this);
            }
        }
        /****************************************************************************/

 
        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            if (collidee.ExternalData != null)
            {

                CollisionOnThisFrame = true;

                if (!gameObjectsCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                {
                    if (subsribedGameObjectCollisionsEvents.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
                    {
                        if (!gameObjectsCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                        {
                            GameObject.SendEvent(
                                new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                                EventsSystem.Priority.Normal,
                                GameObject);

                            
                        }
                    }




                    if (subscribedGameObjectTypesCollisionsEvents.Contains(collidee.ExternalData.GetType()))
                    {

                        GameObject.SendEvent(
                            new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                            EventsSystem.Priority.Normal,
                            GameObject);

                        
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
                    if (gameObjectsCollisionInFrame.ContainsKey(go)) continue;
                    if (subscribedGameObjectLostCollisionsEvents.Contains(go.ID) || subscribedGameObjectTypesLostCollisionsEvents.Contains(go.GetType()))
                    {
                        GameObject.SendEvent(new LostCollisionEvent(go),EventsSystem.Priority.Normal,GameObject);
                    }
                }
            }

            if (gameObjectsCollisionInFrame.Count != 0 && (subsribedGameObjectStartCollisionEvents.Count != 0 || subscribedGameObjectTypesStartCollisionEvents.Count != 0))
            {
                foreach (GameObjectInstance go in gameObjectsCollisionInFrame.Keys)
                {
                    if (gameObjectsCollisionInPrevFrame.ContainsKey(go)) continue;
                    if (subsribedGameObjectStartCollisionEvents.Contains(go.ID) || subscribedGameObjectTypesStartCollisionEvents.Contains(go.GetType()))
                    {
                        GameObject.SendEvent(new StartCollisionEvent(go),EventsSystem.Priority.Normal,GameObject);
                    }
                }
            }
            gameObjectsCollisionInPrevFrame = gameObjectsCollisionInFrame;
            gameObjectsCollisionInFrame = new Dictionary<GameObjectInstance, int>();


            if (subscribedAnyCollisionEvent && CollisionOnThisFrame)
            {
                this.GameObject.SendEvent(new AnyCollisionEvent(), EventsSystem.Priority.Normal, GameObject);
            }


            CollisionOnThisFrame = false;
        }


        /****************************************************************************/
        /// DisableBody
        /****************************************************************************/
        public void DisableBody()
        {
            if (!isEnabled) return;
            body.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
            physicsManager.rigidBodies.Remove(gameObject.ID);
            isEnabled = false;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// EnableBody
        /****************************************************************************/
        public void EnableBody()
        {
            if (isEnabled) return;
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
            body.EnableBody();
            MoveTo(gameObject.World);                
            physicsManager.rigidBodies.Add(gameObject.ID, this);
            isEnabled = true;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Subscribe Any Collision Event
        /****************************************************************************/
        public void SubscribeAnyCollisionEvent()
        {
            subscribedAnyCollisionEvent = true;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Cancel Subscribe Any Collision Event
        /****************************************************************************/
        public void CancelSubscribeAnyCollisionEvent()
        {
            subscribedAnyCollisionEvent = false;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Subscribe Start Collision Event
        /****************************************************************************/
        public void SubscribeStartCollisionEvent(params Type[] gameObjectTypes)
        {
            subscribedGameObjectTypesStartCollisionEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Start Collision Event
        /****************************************************************************/
        public void SubscribeStartCollisionEvent(params int[] gameObjects)
        {
            subsribedGameObjectStartCollisionEvents.AddRange(gameObjects);
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Cancel Subscribe Start Collision Event
        /****************************************************************************/
        public void CancelSubscribeStartCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                subscribedGameObjectTypesStartCollisionEvents.Remove(gameObjectType);
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
                subsribedGameObjectStartCollisionEvents.Remove(gameObject);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// CollideWithGameObjects
        /****************************************************************************/
        public void CollideWithGameObjects(params int[] gameObjects)
        {
            gameObjectsToColide.AddRange(gameObjects);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CancelCollisionWithGameObjects
        /****************************************************************************/
        public void CancelCollisionWithGameObjects(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                gameObjectsToColide.Remove(gameObject);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// DontCollideWithGameObjects
        /****************************************************************************/
        public void DontCollideWithGameObjects(params int[] gameObjects)
        {
            gameObjectsToNotColide.AddRange(gameObjects);
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// CancelNoCollisionWithGameObjects
        /****************************************************************************/
        public void CancelNoCollisionWithGameObjects(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                gameObjectsToNotColide.Remove(gameObject);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Subscribe Collision Event
        /****************************************************************************/
        public void SubscribeCollisionEvent(params int[] gameObjects)
        {
            subsribedGameObjectCollisionsEvents.AddRange(gameObjects);
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                subsribedGameObjectCollisionsEvents.Remove(gameObject);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Subscribe Lost Collision Event
        /****************************************************************************/
        public void SubscribeLostCollisionEvent(params int[] gameObjects)
        {
            subscribedGameObjectLostCollisionsEvents.AddRange(gameObjects);
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Cancel Subscribe Lost Collision Event
        /****************************************************************************/
        public void CancelSubscribeLostCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                subscribedGameObjectLostCollisionsEvents.Remove(gameObject);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// CollideWithGameObjectsType
        /****************************************************************************/
        public void CollideWithGameObjectsType(params Type[] gameObjectTypes)
        {
            gameObjectsTypeToColide.AddRange(gameObjectTypes);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CancelCollisionWithGameObjectsType
        /****************************************************************************/
        public void CancelCollisionWithGameObjectsType(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                subscribedGameObjectTypesCollisionsEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// DontCollideWithGameObjectsType
        /****************************************************************************/
        public void DontCollideWithGameObjectsType(params Type[] gameObjectTypes)
        {
            gameObjectsTypeToNotColide.AddRange(gameObjectTypes);
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// CancelNoCollisionWithGameObjectsType
        /****************************************************************************/
        public void CancelNoCollisionWithGameObjectsType(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                gameObjectsTypeToNotColide.Remove(gameObjectType);
            }
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Subscribe Collision Event
        /****************************************************************************/
        public void SubscribeCollisionEvent(params Type[] gameObjectTypes)
        {
            subscribedGameObjectTypesCollisionsEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                subscribedGameObjectTypesCollisionsEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Lost Collision Event
        /****************************************************************************/
        public void SubscribeLostCollisionEvent(params Type[] gameObjectTypes)
        {
            subscribedGameObjectTypesLostCollisionsEvents.AddRange(gameObjectTypes);
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Cancel Subscribe Lost Collision Event
        /****************************************************************************/
        public void CancelSubscribeLostCollisionEvent(params Type[] gameObjectTypes)
        {
            foreach (Type gameObjectType in gameObjectTypes)
            {
                subscribedGameObjectTypesLostCollisionsEvents.Remove(gameObjectType);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public void Update()
        {
            if (isEnabled && !Immovable)
            {
                gameObject.World             =  body.Orientation;
                gameObject.World.Translation =  body.Position;
                
                gameObject.World = InvertedSkinLocalMatrix * gameObject.World;                
            }
            SendCollisionEvents();
        }
        /****************************************************************************/



        /****************************************************************************/
        /// SetMass
        /****************************************************************************/
        protected Vector3 SetMass()
        {
            var primitiveProperties = new PrimitiveProperties( PrimitiveProperties.MassDistributionEnum.Solid,PrimitiveProperties.MassTypeEnum.Mass, mass);

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
            if(gameObject!=null)physicsManager.rigidBodies.Remove(gameObject.ID);
            base.ReleaseMe();
        }
        /****************************************************************************/





        /****************************************************************************/
        /// MoveTo
        /****************************************************************************/
        public void MoveTo(Matrix matrix)
        {
            matrix = SkinLocalMatrix * matrix;
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

        public Body Body { get { return body; } }
        public CollisionSkin Skin { get { return skin; } }
        public Vector3 SkinTranslation { get { return translation; } }
        public float Yaw { get { return MathHelper.ToDegrees(yaw); } }
        public float Pitch { get { return MathHelper.ToDegrees(pitch); } }
        public float Roll { get { return MathHelper.ToDegrees(roll); } }
        public Matrix SkinOrientation { get; private set; }
        public Matrix SkinInvertedOrientation { get; private set; }
        /****************************************************************************/




    }
    /****************************************************************************/

    class BodyExtended : Body
    {

        public BodyExtended()
        {
            DesiredVelocity = Vector3.Zero;
            DesiredOrientation = Orientation;
            Controllable = false;
           
        }
        public bool Controllable;
        public Vector3 DesiredVelocity;
        public Matrix DesiredOrientation;
        public Matrix SkinOrientation;

        public void SetUpOrientationForController()
        {
            DesiredOrientation = Orientation;
        }
        
        public override void AddExternalForces(float dt)
        {
            ClearForces();
   
            if (Controllable)
            {
                AllowFreezing = false;
                EnableBody();               
              
                Orientation = DesiredOrientation;

                DesiredVelocity = Vector3.Transform(DesiredVelocity, SkinOrientation);
                DesiredVelocity = Vector3.Transform(DesiredVelocity, Orientation);                

                Vector3 deltaVel = DesiredVelocity - Velocity;                

                deltaVel.Y = 0;                

                AddWorldForce((deltaVel * Mass) / dt);                                
            }

            AddGravityToExternalForce();
        }
    }
}
/****************************************************************************/