using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    abstract class Controller
    {
        //protected AbstracPerson being {get; set;
        protected void move(TimeSpan deltaTime)
        {
            if (Vector2.Distance(new Vector2(Mercenary.World.Translation.X,
                                                 Mercenary.World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
            {
                moving = 0;
                Mercenary.Controller.StopMoving();
                Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
            }
            else
            {
                Vector3 direction = Mercenary.World.Translation - target;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(Mercenary.World.Forward.X, Mercenary.World.Forward.Z));

                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                if (det < 0)
                {
                    angle = -angle;
                }

                if (Math.Abs(angle) > anglePrecision)
                {
                    Mercenary.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                }

                Mercenary.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                #region Blend to Run
                if (Mercenary.Mesh.CurrentClip != "Run")
                {
                    Mercenary.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                }
                #endregion
            }
        }
    }
}
