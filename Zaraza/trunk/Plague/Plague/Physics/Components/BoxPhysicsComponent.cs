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
///  PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{



    /****************************************************************************/
    ///  BoxPhysicsComponent
    /****************************************************************************/
    class BoxPhysicsComponent : PhysicsComponent
    {




        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public BoxPhysicsComponent(
                                    GameObjectInstance gameObject,
                                    PhysicsManager physicsManager,
                                    float mass,
                                    Vector3 boxSize,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness,
                                    bool immovable)
            :base(gameObject, physicsManager)
        {

            

            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            Box box = new Box(Vector3.Zero, Matrix.Identity, boxSize);

            skin.AddPrimitive(box, new MaterialProperties(elasticity, staticRoughness, dynamicRoughness));

            body.MoveTo(gameObject.World.Translation, gameObject.World);
            //body.MoveTo(gameObject.World.Translation, Matrix.Identity);

            Vector3 CenterMassPosition = SetMass(mass);
            skin.ApplyLocalTransform(new Transform(-CenterMassPosition, Matrix.Identity));
            body.EnableBody();

            body.Immovable = immovable;


        }
        /****************************************************************************/



    }
    /****************************************************************************/



}
/****************************************************************************/