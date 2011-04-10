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

namespace PlagueEngine.Physics
{

    class CollisionSkinComponent : GameObjectComponent
    {
        protected CollisionSkin skin = null;

        private MaterialProperties material;
        private bool isEnabled = false;

        internal static PhysicsManager physicsManager = null;

        public CollisionSkinComponent(GameObjectInstance gameObject, MaterialProperties material)
            : base(gameObject)
        {
            skin = new CollisionSkin();
            this.material = material;

            physicsManager.collisionSkins.Add(this);
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
            physicsManager.collisionSkins.Remove(this);
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
        /****************************************************************************/
    }
}
