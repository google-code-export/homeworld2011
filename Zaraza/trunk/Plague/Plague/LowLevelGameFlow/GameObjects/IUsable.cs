using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    interface IUsable
    {
        void Use(Mercenary mercenary);
        int GetAmount();
    }
}
