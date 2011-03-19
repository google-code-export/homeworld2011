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

            LinkedCameraData lcdata = new LinkedCameraData();
            lcdata.Type = typeof(LinkedCamera);
            lcdata.position = new Vector3(-200, 300, -200);
            lcdata.target = new Vector3(0, 0, 0);
            lcdata.MovementSpeed = 0.5f;
            lcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            lcdata.ZoomSpeed = 0.5f;
            lcdata.FoV = MathHelper.PiOver4;
            lcdata.ZNear = 1;
            lcdata.ZFar = 10000;
            lcdata.ActiveKeyListener = true;
            lcdata.ActiveMouseListener = true;
            gameObjectsFactory.Create(lcdata);

            //FreeCameraData fcdata = new FreeCameraData();
            //fcdata.Type = typeof(FreeCamera);
            //fcdata.World = fcdata.World = Matrix.CreateLookAt(new Vector3(-200, 300, -200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //fcdata.MovementSpeed = 1;
            //fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            //fcdata.FoV = MathHelper.PiOver4;
            //fcdata.ZNear = 1;
            //fcdata.ZFar = 10000;
            //fcdata.ActiveKeyListener = true;
            //fcdata.ActiveMouseListener = true;
            //gameObjectsFactory.Create(fcdata);
            
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/