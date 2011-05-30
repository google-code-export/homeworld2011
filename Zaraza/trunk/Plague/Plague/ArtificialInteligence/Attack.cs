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


    }
}
