using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class FlammableBarrel : GameObjectInstance, IFlammable
    {
        public void SetOnFire()
        {
            throw new NotImplementedException();
        }

        public void OnShoot(float damage, float stoppingPower)
        {
            throw new NotImplementedException();
        }
    }
}
