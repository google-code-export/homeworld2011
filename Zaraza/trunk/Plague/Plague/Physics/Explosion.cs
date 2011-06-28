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
    class Explosion
    {
        Vector3 position;
        float force;
        float radius;
        Body body;
        CollisionSkin skin;
        public bool wasted = false;
        public Explosion(Vector3 pos, float force,float radius)
        {
            this.position = pos;
            this.force = force;
            this.radius = radius;

            
            body = new BodyExtended();
            skin = new CollisionSkin(body);
            body.CollisionSkin = skin;
            Sphere sphere = new Sphere(Vector3.Zero, radius);

            skin.AddPrimitive(sphere,(int)MaterialTable.MaterialID.NormalNormal);
            skin.callbackFn += new CollisionCallbackFn(HandleCollisionDetection);
            body.MoveTo(pos, Matrix.Identity);
            body.EnableBody();
            
        }


        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            if (collidee.Owner != null)
            {
                
                Vector3 forceDir = Vector3.Normalize(collidee.Owner.Position - position);
                float dist = Vector3.Distance(collidee.Owner.Position, position);
                collidee.Owner.AddWorldForce(forceDir*force* (float)(Math.Abs((1.0-dist/radius))));
                //collidee.Owner.Velocity += forceDir * force * (float)(Math.Abs((1.0 - dist / radius))) / collidee.Owner.Mass;
            }
            return false;
        }

        public void Update()
        {
            wasted = true;
        }

        public void Remove()
        {
            body.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
           
        }

        
    }
}
