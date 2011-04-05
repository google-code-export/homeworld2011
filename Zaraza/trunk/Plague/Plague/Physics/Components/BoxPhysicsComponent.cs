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

        public float    mass;
        public Vector3  boxSize;
        public float    elasticity;
        public float    staticRoughness;
        public float    dynamicRoughness;
        public bool     immovable;


        /****************************************************************************/
        ///  Constructor
        /****************************************************************************/
        public BoxPhysicsComponent( GameObjectInstance gameObject,
                                    PhysicsManager physicsManager,
                                    float mass,
                                    Vector3 boxSize,
                                    float elasticity,
                                    float staticRoughness,
                                    float dynamicRoughness,
                                    bool immovable,
                                    Matrix world)
            :base(gameObject, physicsManager)
        {

            this.boxSize = boxSize;
            this.dynamicRoughness = dynamicRoughness;
            this.elasticity = elasticity;
            this.immovable = immovable;
            this.mass = mass;
            this.staticRoughness = staticRoughness;


            body = new Body();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;

            Box box = new Box(Vector3.Zero, Matrix.Identity, boxSize);

            skin.AddPrimitive(box, new MaterialProperties(elasticity, staticRoughness, dynamicRoughness));

            Matrix tmp = world;
            tmp.Translation = Vector3.Zero;
            //body.MoveTo(world.Translation, world);
            body.MoveTo(world.Translation, tmp);

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





