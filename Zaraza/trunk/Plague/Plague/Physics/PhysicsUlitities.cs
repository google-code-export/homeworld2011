using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;

namespace PlagueEngine.Physics
{
    static class PhysicsUlitities
    {
        static public GraphicsDeviceManager graphics = null;

        public static bool RayTest(Vector3 startPosition, Vector3 endPosition, out float distance, out CollisionSkin skin,out Vector3 skinPosition, out Vector3 normal)
        {

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.SegmentIntersect(out distance,out skin,out skinPosition,out normal, new Segment(startPosition, endPosition - startPosition), new ImmovableSkinPredicate());

            if (skin == null || skin.Owner == null) return false;
            return true;
        }

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



    class ImmovableSkinPredicate : CollisionSkinPredicate1
    {
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (skin0.Owner != null && !skin0.Owner.Immovable)
                return true;
            else
                return false;
        }
    }
}
