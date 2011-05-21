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
        private MaterialProperties material;
        private bool isEnabled = false;
        internal static PhysicsManager physicsManager = null;
        private Vector3 translation = Vector3.Zero;
        private float yaw;
        private float pitch;
        private float roll;
        private List<Type> subscribedGameObjectTypesEvents = new List<Type>();
        private List<Type> gameObjectsTypeToColide = new List<Type>();
        private List<Type> gameObjectsTypeToNotColide = new List<Type>();


        private List<int> subsribedGameObjectEvents = new List<int>();
        private List<int> gameObjectsToColide       = new List<int>();
        private List<int> gameObjectsToNotColide    = new List<int>();
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public CollisionSkinComponent(GameObjectInstance gameObject, MaterialProperties material, Vector3 translation, float yaw, float pitch, float roll)
            : base(gameObject)
        {
            skin = new CollisionSkin();
            this.material = material;           
            skin.ExternalData = gameObject;
            this.translation = translation;
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            physicsManager.collisionSkins.Add(gameObject.ID,this);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {

            if (subsribedGameObjectEvents.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
            {


                this.GameObject.SendEvent(
                    new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                    EventsSystem.Priority.Normal,
                    this.GameObject);

            }



            if (subscribedGameObjectTypesEvents.Contains(collidee.ExternalData.GetType()))
            {


                this.GameObject.SendEvent(
                    new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                    EventsSystem.Priority.Normal,
                    this.GameObject);

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

            return false;
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
            this.subsribedGameObjectEvents.AddRange(gameObjects);
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Cancel Subscribe Collision Event
        /****************************************************************************/
        public void CancelSubscribeCollisionEvent(params int[] gameObjects)
        {
            foreach (int gameObject in gameObjects)
            {
                this.subsribedGameObjectEvents.Remove(gameObject);
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
        /// Enable
        /****************************************************************************/
        public void Enable()
        {
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
            isEnabled = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable
        /****************************************************************************/
        public void Disable()
        {
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
