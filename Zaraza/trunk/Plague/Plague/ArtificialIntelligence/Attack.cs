using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    [Serializable]
    public class Attack
    {
        [CategoryAttribute("Attack Distance")]
        public float minAttackDistance;
        [CategoryAttribute("Attack Distance")]
        public float maxAttackDistance;

        [CategoryAttribute("Cooldown")]
        public TimeSpan cooldown;

        [CategoryAttribute("Base Damage")]
        public int maxInflictedDamage;
        [CategoryAttribute("Base Damage")]
        public int minInflictedDamage;
        
        public Attack()
        {
        }
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
