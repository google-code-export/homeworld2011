using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.LowLevelGameFlow
{
    interface IActiveGameObject
    {
        String[] GetActions();
        
        String[] GetActions(Mercenary mercenary);
    }
}
