using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Wiem że shootable oznacza że można z tego strzelać a nie ze może być strzelone,
 * ale nie wiem jak to nazwac zeby było że może być strzelone ;P
 */


namespace PlagueEngine.LowLevelGameFlow
{
    interface IShootable
    {
        void OnShoot(float damage, float stoppingPower);
    }
}
