using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using JigLibX.Utils;

using PlagueEngine.LowLevelGameFlow;



/****************************************************************************/
/// PlagueEngine.Physics.Components
/****************************************************************************/
namespace PlagueEngine.Physics.Components
{


    /****************************************************************************/
    /// SphericalSkinComponent
    /****************************************************************************/
    class SphericalSkinComponent : CollisionSkinComponent
    {



        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private float radius;
        private Body body;
        /****************************************************************************/



        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SphericalSkinComponent(GameObjectInstance gameObject,
                                    Matrix world,
                                    float radius,
                                    MaterialProperties material,
                                    Vector3 skinTranslation,
                                    float yaw,
                                    float pitch,
                                    float roll)
            : base(gameObject, material, skinTranslation, yaw, pitch, roll)
        {

            this.radius = radius;
            body = new Body();
            skin = new CollisionSkin(body);
            skin.ExternalData = gameObject;
            body.CollisionSkin = skin;
            body.Immovable = true;
            Matrix dummyWorld = world;


            dummyWorld.Translation += skinTranslation;

            Sphere sphere = new Sphere(Vector3.Zero, radius);

            Skin.AddPrimitive(sphere, material);
            body.MoveTo(dummyWorld.Translation, dummyWorld);
            body.EnableBody();
            Enable();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float Radius { get { return radius; } }
        /****************************************************************************/

    }
    /****************************************************************************/




}
/****************************************************************************/