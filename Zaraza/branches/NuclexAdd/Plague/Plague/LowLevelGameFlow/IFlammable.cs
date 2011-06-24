using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.LowLevelGameFlow
{
    interface IFlammable : IShootable
    {
        void SetOnFire();
    }
}
