using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.ArtificialIntelligence
{
    class AI
    {
        private AIFactory controllersFactory;

        public AI()
        {
            controllersFactory = new AIFactory(this);
        }

        public void Initialize()
        {
        }
    }
}
