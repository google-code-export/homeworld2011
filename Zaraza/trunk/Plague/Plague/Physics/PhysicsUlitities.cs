using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;

/****************************************************************************/
/// PlagueEngine.Physics
/****************************************************************************/
namespace PlagueEngine.Physics
{




    /****************************************************************************/
    /// PhysicsUlitities
    /****************************************************************************/
    static class PhysicsUlitities
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        static public GraphicsDeviceManager graphics = null;
        static public CollisionSystem collisionSystem= null;
        /****************************************************************************/
        

        /****************************************************************************/
        /// RayTest
        /****************************************************************************/

        public static bool RayTest(Vector3 startPosition, Vector3 endPosition, out float distance, out CollisionSkin skin,out Vector3 skinPosition, out Vector3 normal)
        {

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.SegmentIntersect(out distance,out skin,out skinPosition,out normal, new Segment(startPosition, endPosition - startPosition), new ImmovableSkinPredicate());

            if (skin == null || skin.Owner == null) return false;
            return true;
            
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Ray Test
        /****************************************************************************/
        public static bool IsBodyInSquareArea(Vector3 midpoint,Matrix AreaOrientation, float length, float height, float width, Body testedBody)
        {
            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(length, height, width));
            Body areaBody = new Body();
            CollisionSkin skin = new CollisionSkin(areaBody);
            areaBody.CollisionSkin = skin;
            areaBody.Immovable = true;
            skin.AddPrimitive(box,(int) MaterialTable.MaterialID.NotBouncyRough);
            areaBody.MoveTo(midpoint, AreaOrientation);
            areaBody.EnableBody();

            List<Body> bodyList = new List<Body>(); 
            List<CollisionInfo> collisionInfos = new List<CollisionInfo>(); 
            CollisionFunctor collisionFunctor = new BasicCollisionFunctor(collisionInfos); 

            bodyList.Add(areaBody);
            bodyList.Add(testedBody);

            collisionSystem.DetectAllCollisions(bodyList, collisionFunctor, null, 0.05f);
            
            areaBody.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
            if (collisionInfos.Count ==0 ) return false;

            foreach (CollisionInfo info in collisionInfos)
            {
                CollDetectInfo  collInfo=info.SkinInfo;
                if ((collInfo.Skin0 == testedBody.CollisionSkin && collInfo.Skin1 == areaBody.CollisionSkin) || (collInfo.Skin0 == areaBody.CollisionSkin && collInfo.Skin1 == testedBody.CollisionSkin))
                {
                    return true;
                }
            }
            return false;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Is Body In Sphere Area
        /****************************************************************************/
        public static bool IsBodyInSphereArea(Vector3 midpoint, float radius, Body testedBody)
        {
            Sphere sphere = new Sphere(Vector3.Zero, radius);
            Body areaBody = new Body();
            CollisionSkin skin = new CollisionSkin(areaBody);
            areaBody.CollisionSkin = skin;
            areaBody.Immovable = true;
            skin.AddPrimitive(sphere, (int)MaterialTable.MaterialID.NotBouncyRough);
            areaBody.MoveTo(midpoint, Matrix.Identity);
            areaBody.EnableBody();

            List<Body> bodyList = new List<Body>();
            List<CollisionInfo> collisionInfos = new List<CollisionInfo>();
            CollisionFunctor collisionFunctor = new BasicCollisionFunctor(collisionInfos);

            bodyList.Add(areaBody);
            bodyList.Add(testedBody);

            collisionSystem.DetectAllCollisions(bodyList, collisionFunctor, null, 0.05f);

            areaBody.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);


            if (collisionInfos.Count == 0) return false;

            foreach (CollisionInfo info in collisionInfos)
            {
                CollDetectInfo collInfo = info.SkinInfo;
                if ((collInfo.Skin0 == testedBody.CollisionSkin && collInfo.Skin1 == areaBody.CollisionSkin) || (collInfo.Skin0 == areaBody.CollisionSkin && collInfo.Skin1 == testedBody.CollisionSkin))
                {
                    return true;
                }
            }
            return false;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Direction From Mouse Position
        /****************************************************************************/
        public static Vector3 DirectionFromMousePosition(Matrix cameraProjection, Matrix cameraView, float x, float y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, cameraProjection, cameraView, world);
            Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, cameraProjection, cameraView, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return direction;
        }
    }
    /****************************************************************************/






    /****************************************************************************/
    /// ImmovableSkinPredicate
    /****************************************************************************/
    class ImmovableSkinPredicate : CollisionSkinPredicate1
    {
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (skin0 != null)
                return true;
            else
                return false;
        }
    }
    /****************************************************************************/




}
/****************************************************************************/