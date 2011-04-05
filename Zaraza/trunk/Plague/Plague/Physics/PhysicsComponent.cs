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
///  PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{
    /****************************************************************************/
    ///  PhysicsComponent
    /****************************************************************************/
    abstract class PhysicsComponent
    {


        /****************************************************************************/
        ///  Fields
        /****************************************************************************/
        protected Body body = null;                         public Body Body            { get { return this.body; } }
        protected CollisionSkin skin = null;                public CollisionSkin Skin   { get { return this.skin; } }
        protected GameObjectInstance gameobject = null;
        protected PhysicsManager physicsManager = null;


        


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public PhysicsComponent(GameObjectInstance gameobject,PhysicsManager physicsManager)
        {

            this.gameobject = gameobject;
            this.physicsManager = physicsManager;
        }



        /****************************************************************************/
        ///  Update World Matrix
        /****************************************************************************/
        public void UpdateWorldMatrix()
        {
            
            gameobject.World=body.Orientation;
            gameobject.World.Translation=body.Position;
        }




        /****************************************************************************/
        ///  Set mass
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
        ///  ReleaseMe
        /****************************************************************************/
        public void ReleaseMe()
        {
            this.physicsManager.ReleaseComponent(this);
        }

        /****************************************************************************/



    }
    /****************************************************************************/
    


}
/****************************************************************************/


