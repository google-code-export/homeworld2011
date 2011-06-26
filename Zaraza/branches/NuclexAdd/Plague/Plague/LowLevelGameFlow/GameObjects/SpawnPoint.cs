using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class SpawnPoint : GameObjectInstance
    {

        private Vector3 position;
        private GameObjectDefinition MobDefinition;
        private string StartAnimation;
        private bool enabled;
        private List<Vector3> goToPositions;

        private List<EventArgs> spawnOrders;

        public bool Enabled { get; set; }

        public void Spawn()
        {

        }
    }


    [Serializable]
    class SpawnPointData : GameObjectInstanceData
    {

    }
}
