using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Spawner : GameObjectInstance, IUpdateable
    {
        private static Spawner spawner = null;
        private List<SpawnPoint> spawnPoints = null;

        public bool Enabled { get; set; }
        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder { get; set; }
        public event EventHandler<EventArgs> UpdateOrderChanged;

        private Spawner() 
        {
            this.spawnPoints = new List<SpawnPoint>();
        }

        public static Spawner GetInstance()
        {
            if (spawner == null)
            {
                spawner = new Spawner();
            }
            return spawner;
        }

        public void registerSpawnPoint(SpawnPoint point)
        {
            this.spawnPoints.Add(point);
        }

        public void unregisterSpawnPoint(SpawnPoint point)
        {
            this.spawnPoints.Remove(point);
        }

        public void disableSpawnPoints(SpawnPoint[] points)
        {
            foreach (SpawnPoint point in points)
            {
                point.Enabled = false;
            }
        }

        public void enableSpawnPoints(SpawnPoint[] points)
        {
            foreach (SpawnPoint point in points)
            {
                point.Enabled = true;
            }
        }

        public void disableSpawnPoints(int[] indices)
        {
            foreach (int index in indices)
            {
                this.spawnPoints[index].Enabled = false;
            }
        }

        public void enableSpawnPoints(int[] indices)
        {
            foreach (int index in indices)
            {
                this.spawnPoints[index].Enabled = true;
            }
        }
        
        public void Update(GameTime gameTime)
        {
            
        }

        
    }

    [Serializable]
    class SpawnerData : GameObjectInstanceData
    {
    }
}
