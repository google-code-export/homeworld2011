using System;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.LowLevelGameFlow
{
    interface IActiveGameObject
    {
        String[] GetActions();
        
        String[] GetActions(Mercenary mercenary);
    }
}
