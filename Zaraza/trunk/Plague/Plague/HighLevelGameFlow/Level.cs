using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;

using Microsoft.Xna.Framework;

/************************************************************************************/
/// PlagueEngine.HighLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.HighLevelGameFlow
{

    /********************************************************************************/
    /// Level
    /********************************************************************************/
    class Level
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Dictionary<uint, GameObjectInstance> gameObjects               = new Dictionary<uint,GameObjectInstance>();
        private GameObjectsFactory                   gameObjectsFactory        = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Level(GameObjectsFactory gameObjectsFactory)
        {
            this.gameObjectsFactory                      = gameObjectsFactory;
            gameObjectsFactory.GameObjects               = gameObjects;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Level
        /****************************************************************************/
        public void LoadLevel(LevelData levelData)
        {
            foreach (GameObjectInstanceData goid in levelData.gameObjects)
            {
                gameObjectsFactory.Create(goid);                
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Level
        /****************************************************************************/
        public LevelData SaveLevel()
        {
            LevelData levelData = new LevelData();
            
            foreach (GameObjectInstance goi in gameObjects.Values)
            {
                levelData.gameObjects.Add(goi.GetData());                
            }

            return levelData;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Clear
        /****************************************************************************/
        public void Clear()
        {
            foreach (GameObjectInstance goi in gameObjects.Values) goi.Dispose();
            gameObjects.Clear();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Destructor
        /****************************************************************************/
        ~Level()
        {
            Clear();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Put Some Objects
        /****************************************************************************/
        public void PutSomeObjects()
        {
            GameObjectInstanceData data = new GameObjectInstanceData();

            data.Type = (typeof(StaticMesh));
            data.Definition = "Barrel";
            data.World = Matrix.Identity;

            for (int i = 0; i < 10; i++)
            {
                data.World *= Matrix.CreateTranslation(100, 0, 0);
                gameObjectsFactory.Create(data);
            }

            FreeCameraData fcdata = new FreeCameraData();
            fcdata.Type = typeof(FreeCamera);
            fcdata.World = Matrix.CreateLookAt(new Vector3(-200, 300, -200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            fcdata.MovementSpeed = 1;
            fcdata.RotationSpeed = MathHelper.PiOver4/1000;
            fcdata.FoV = MathHelper.PiOver4;
            fcdata.ZNear = 1;
            fcdata.ZFar = 10000;
            fcdata.ActiveKeyListener = true;

            gameObjectsFactory.Create(fcdata);
            
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/