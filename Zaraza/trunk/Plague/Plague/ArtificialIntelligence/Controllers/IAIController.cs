using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    interface IAIController
    {
        void OnEvent(EventsSystem.EventsSender sender, EventArgs e);
        void Update(TimeSpan deltaTime);
    }
}
