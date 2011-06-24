using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PlagueEngine.Physics
{
    static class ExplosionManager
    {
        public static List<Explosion> explosions = new List<Explosion>();


        public static void CreateExplosion(Vector3 position, float force,float radius)
        {
            Explosion ex = new Explosion(position, force,radius);
            explosions.Add(ex);
        }


        public static void Update()
        {
            List<Explosion> wasted = new List<Explosion>();

            foreach (Explosion ex in explosions)
            {
                ex.Update();

                if (ex.wasted)
                {
                    ex.Remove();
                    wasted.Add(ex);
                }

            }

            foreach (Explosion ex in wasted)
            {
                explosions.Remove(ex);
            }
            wasted.Clear();


        }
    }
}
