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

        private float mass;
        private Vector3 boxSize;
        private float elasticity;
        private float staticRoughness;
        private float dynamicRoughness;
        private bool immovable;


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




        /****************************************************************************/
        ///  GetData
        /****************************************************************************/
        public override PhysicsComponentData GetData()
        {
            BoxPhysicsComponentData data = new BoxPhysicsComponentData();
            GetData(data);
            data.boxSize = this.boxSize;
            data.dynamicRoughness = this.dynamicRoughness;
            data.elasicity = this.elasticity;
            data.immovable = this.immovable;
            data.mass = this.mass;
            data.staticRoughness = this.staticRoughness;

            return data;
        }

        /****************************************************************************/

    }
    /****************************************************************************/



}
/****************************************************************************/





/****************************************************************************/
///  Box Physics Component Data
/****************************************************************************/
[Serializable]
public class BoxPhysicsComponentData : PhysicsComponentData
{
    public bool immovable               { get; set; }
    public float mass                   { get; set; }
    public Vector3 boxSize              { get; set; }
    public float elasicity              { get; set; }
    public float staticRoughness        { get; set; }
    public float dynamicRoughness       { get; set; }
}
/****************************************************************************/
