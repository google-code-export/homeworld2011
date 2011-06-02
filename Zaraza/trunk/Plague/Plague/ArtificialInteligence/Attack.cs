using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    struct Attack
    {
        public float minAttackDistance;
        public float maxAttackDistance;

        public TimeSpan cooldown;
        public int maxInflictedDamage;
        public int minInflictedDamage;

        public Attack(float minDist, float maxDist, int minDmg, int maxDmg, long ticks)
        {
            this.minAttackDistance = minDist;
            this.minInflictedDamage = minDmg;
            this.maxAttackDistance = maxDist;
            this.maxInflictedDamage = maxDmg;
            this.cooldown = new TimeSpan(ticks);
        }
    }
}
