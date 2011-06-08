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
/// PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{
    /****************************************************************************/
    /// CollisionSkinComponent
    /****************************************************************************/
    class CollisionSkinComponent : GameObjectComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected CollisionSkin skin = null;
        protected MaterialProperties material;
        private bool isEnabled = false;
        public bool Enabled { get { return this.isEnabled; } }

        internal static PhysicsManager physicsManager = null;
        protected Vector3 translation = Vector3.Zero;
        protected float yaw;
        protected float pitch;
        protected float roll;

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
        private List<int> gameObjectsToColide = new List<int>();
        private List<int> gameObjectsToNotColide = new List<int>();
        public bool DontCollide { get; set; }
        public bool RequireUpdate { get; set; }
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public CollisionSkinComponent(bool enabled,GameObjectInstance gameObject, MaterialProperties material, Vector3 translation, float yaw, float pitch, float roll)
            : base(gameObject)
        {
            skin = new CollisionSkin();
            this.material = material;           
            skin.ExternalData = gameObject;
            this.translation = translation;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            //this.isEnabled = enabled;
            physicsManager.collisionSkins.Add(gameObject.ID,this);

            
            skin.callbackFn += new CollisionCallbackFn(HandleCollisionDetection);
            DontCollide = false;
            RequireUpdate = false;
            this.isEnabled = true;
            if (!isEnabled)
            {
                Disable();
            }
        }
        /****************************************************************************/


        protected virtual void SetSkin(Matrix world)
        {

        }

        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            if (!((GameObjectInstance)(collidee.ExternalData) == null))
            {

                CollisionOnThisFrame = true;

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


                if (DontCollide)
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


            if (subscribedAnyCollisionEvent && CollisionOnThisFrame)
            {
                this.GameObject.SendEvent(new AnyCollisionEvent(), EventsSystem.Priority.Normal, GameObject);
            }


            CollisionOnThisFrame = false;

        }




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

        public void Update()
        {
            SendCollisionEvents();
            if (RequireUpdate)
            {
                this.MoveTo(gameObject.World);
            }
        }


        protected virtual void MoveTo(Matrix world)
        {

        }

        /****************************************************************************/
        /// Enable
        /****************************************************************************/
        public void Enable()
        {

            SetSkin(gameObject.World);
            skin.ExternalData = this.gameObject;
            if (physicsManager.collisionSkins.ContainsValue(this))
            {
                physicsManager.collisionSkins.Remove(this.gameObject.ID); ;
            }
            physicsManager.collisionSkins.Add(gameObject.ID, this);

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
            skin.callbackFn += new CollisionCallbackFn(HandleCollisionDetection);

            isEnabled = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable
        /****************************************************************************/
        public void Disable()
        {
            physicsManager.collisionSkins.Remove(gameObject.ID);
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
            isEnabled = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ReleaseMe
        /****************************************************************************/
        public override void ReleaseMe()
        {
            if (isEnabled) Disable();
            physicsManager.collisionSkins.Remove(gameObject.ID);
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool IsEnabled { get { return isEnabled; } }
        public float Elasticity { get { return material.Elasticity; } }
        public float StaticRoughness { get { return material.StaticRoughness; } }
        public float DynamicRoughness { get { return material.DynamicRoughness; } }

        public CollisionSkin Skin { get { return skin; } }
        public Vector3 SkinTranslation { get { return this.translation; } }
        public float Yaw { get { return this.yaw; } }
        public float Pitch { get { return this.pitch; } }
        public float Roll { get { return this.roll; } }
        /****************************************************************************/
    }
}
