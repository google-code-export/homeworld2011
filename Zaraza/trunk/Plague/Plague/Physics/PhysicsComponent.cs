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

/************************************************************************************/
/// PlagueEngine.Physics
/************************************************************************************/
namespace PlagueEngine.Physics
{
    /********************************************************************************/
    /// PhysicsComponent
    /********************************************************************************/
    abstract class PhysicsComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/               
        internal static PhysicsManager physicsManager = null;
        
        protected Body          body = null;
        protected CollisionSkin skin = null;
        /****************************************************************************/
      

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public PhysicsComponent(GameObjectInstance gameObject) : base(gameObject)
        {
            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            physicsManager.PhysicsComponents.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update World Matrix
        /****************************************************************************/
        public void UpdateWorldMatrix()
        {
            if (!Body.IsBodyEnabled || Body.Immovable || !Body.IsActive) return;

            gameObject.World             = body.Orientation;
            gameObject.World.Translation = body.Position;            
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Set mass
        /****************************************************************************/
        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                                    PrimitiveProperties.MassDistributionEnum.Solid,
                                    PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk;
            Vector3 centerOfMass;
            Matrix it;
            Matrix itCoM;

            skin.GetMassProperties(primitiveProperties, out junk, out centerOfMass, out it, out itCoM);

            this.body.BodyInertia = itCoM;
            this.body.Mass = junk;

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
        /// ReleaseMe
        /****************************************************************************/
        public override void ReleaseMe()
        {
            physicsManager.ReleaseComponent(this);
            body.DisableBody();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Body          Body { get { return this.body; } }
        public CollisionSkin Skin { get { return this.skin; } }
        /****************************************************************************/
          
    }
    /********************************************************************************/
    
}
/************************************************************************************/

