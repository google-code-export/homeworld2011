﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.LowLevelGameFlow
{
    interface IPenetrable : IShootable
    {
        float GetArmorClass();
    }
}
