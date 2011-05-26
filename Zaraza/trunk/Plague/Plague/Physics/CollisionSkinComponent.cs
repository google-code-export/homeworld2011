﻿using System;
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
        private List<Type> subscribedGameObjectTypesCollisionsEvents = new List<Type>();
        private List<Type> subscribedGameObjectTypesLostCollisionsEvents = new List<Type>();
        private Dictionary<GameObjectInstance, Type> typesCollisionInFrame = new Dictionary<GameObjectInstance, Type>();
        private Dictionary<GameObjectInstance, Type> typesCollisionInPrevFrame = new Dictionary<GameObjectInstance, Type>();

        private List<Type> gameObjectsTypeToColide = new List<Type>();
        private List<Type> gameObjectsTypeToNotColide = new List<Type>();


        private List<int> subsribedGameObjectCollisionsEvents = new List<int>();
        private List<int> subscribedGameObjectLostCollisionsEvents = new List<int>();
        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInFrame = new Dictionary<GameObjectInstance, int>();
        private Dictionary<GameObjectInstance, int> gameObjectsCollisionInPrevFrame = new Dictionary<GameObjectInstance, int>();
        private List<int> gameObjectsToColide = new List<int>();
        private List<int> gameObjectsToNotColide = new List<int>();

        private int frame = 0;
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
            if (!((GameObjectInstance)(collidee.ExternalData) == null))
            {

                if (subsribedGameObjectCollisionsEvents.Contains(((GameObjectInstance)(collidee.ExternalData)).ID))
                {
                    if (++frame == 1)
                    {
                        this.GameObject.SendEvent(
                            new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                            EventsSystem.Priority.Normal,
                            this.GameObject);
                       if(!gameObjectsCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                       {
                           gameObjectsCollisionInFrame.Add((GameObjectInstance)(collidee.ExternalData),((GameObjectInstance)(collidee.ExternalData)).ID);
                       }
                    }
                }



                if (subscribedGameObjectTypesCollisionsEvents.Contains(collidee.ExternalData.GetType()))
                {
                    if (++frame == 1)
                    {
                        this.GameObject.SendEvent(
                            new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                            EventsSystem.Priority.Normal,
                            this.GameObject);

                       if(!typesCollisionInFrame.ContainsKey((GameObjectInstance)(collidee.ExternalData)))
                       {
                           typesCollisionInFrame.Add((GameObjectInstance)(collidee.ExternalData),collidee.ExternalData.GetType());
                       }

                    }
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

        private void SendLostCollisionEvents()
        {
            if (typesCollisionInPrevFrame.Count != 0)
            {
               foreach(GameObjectInstance GO in typesCollisionInPrevFrame.Keys)
                {
                    if (!typesCollisionInFrame.ContainsKey(GO))
                    {
                        if(subscribedGameObjectTypesLostCollisionsEvents.Contains(typesCollisionInPrevFrame[GO]))
                        {
                            this.GameObject.SendEvent(
                            new LostCollisionEvent(GO),
                            EventsSystem.Priority.Normal,
                            this.GameObject);
                        }
                    }
                }
            }

            typesCollisionInPrevFrame=typesCollisionInFrame;
            typesCollisionInFrame.Clear();

            if (gameObjectsCollisionInPrevFrame.Count != 0)
            {
               foreach(GameObjectInstance GO in gameObjectsCollisionInPrevFrame.Keys)
                {
                    if (!gameObjectsCollisionInFrame.ContainsKey(GO))
                    {
                        if(subscribedGameObjectLostCollisionsEvents.Contains(GO.ID))
                        {
                            this.GameObject.SendEvent(
                            new LostCollisionEvent(GO),
                            EventsSystem.Priority.Normal,
                            this.GameObject);
                        }
                    }
                }
            }

            gameObjectsCollisionInPrevFrame=gameObjectsCollisionInFrame;
            gameObjectsCollisionInFrame.Clear();
        }




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
            frame = 0;
            SendLostCollisionEvents();
        }


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
