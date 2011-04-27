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

            if (subscribedGameObjectTypesEvents.Contains(collidee.ExternalData.GetType()))
            {


                this.GameObject.SendEvent(
                    new CollisionEvent((GameObjectInstance)(collidee.ExternalData)),
                    EventsSystem.Priority.Normal,
                    this.GameObject);

            }

            return true;
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
