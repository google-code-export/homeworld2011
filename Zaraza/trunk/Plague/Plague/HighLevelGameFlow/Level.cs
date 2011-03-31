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
            Clear();

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
            StaticMeshData data = new StaticMeshData();

            data.Type = (typeof(StaticMesh));
            data.Definition = "Rusty Barrel";

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    data.World = Matrix.CreateTranslation(150 * i, 900, 150 * j);
                    gameObjectsFactory.Create(data);
                }                
            }

            //LinkedCameraData lcdata = new LinkedCameraData();
            //lcdata.Type = typeof(LinkedCamera);
            //lcdata.position = new Vector3(-200, 300, -200);
            //lcdata.target = new Vector3(0, 0, 0);
            //lcdata.MovementSpeed = 0.5f;
            //lcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            //lcdata.ZoomSpeed = 0.5f;
            //lcdata.FoV = MathHelper.PiOver4;
            //lcdata.ZNear = 1;
            //lcdata.ZFar = 10000;
            //lcdata.ActiveKeyListener = true;
            //lcdata.ActiveMouseListener = true;
            
            //LinkedCamera camera= (LinkedCamera)(gameObjectsFactory.Create(lcdata));
            
            FreeCameraData fcdata = new FreeCameraData();
            fcdata.Type = typeof(FreeCamera);
            fcdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(200, 2000, 200),
                                                             new Vector3(300, 1990, 300), 
                                                             new Vector3(0, 1, 0)));
            fcdata.MovementSpeed = 1;
            fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            fcdata.FoV = 60;
            fcdata.ZNear = 10;
            fcdata.ZFar = 100000;
            fcdata.ActiveKeyListener = true;
            fcdata.ActiveMouseListener = true;
            gameObjectsFactory.Create(fcdata);


            TerrainData tdata = new TerrainData();
            tdata.Type = typeof(Terrain);
            tdata.World = Matrix.Identity;
            tdata.HeightMap = "Heightmap";
            tdata.BaseTexture = "grass";
            tdata.RTexture = "ground";
            tdata.GTexture = "stone";
            tdata.BTexture = "snow";
            tdata.WeightMap = "Weightmap";
            tdata.Width = 256;
            tdata.Length = 256;
            tdata.Height = 1500;
            tdata.CellSize = 100;
            tdata.TextureTiling = 40;
            tdata.Level = 950;
            tdata.Color = new Vector3(0.2f, 0.2f, 0.6f);
            tdata.ColorAmount = 0.5f;
            tdata.WaveLength = 0.2f;
            tdata.WaveHeight = 0.1f;
            tdata.WaveSpeed = 0.1f;
            tdata.NormalMap = "normalmap";
            tdata.WTextureTiling = 1000;
            tdata.Bias = 0.4f;
            tdata.SpecularStength = 500;
            tdata.ClipPlaneAdjustment = 10;

            gameObjectsFactory.Create(tdata);

            SunLightData sdata = new SunLightData();
            sdata.Type = typeof(SunLight);
            sdata.World = Matrix.Identity;
            sdata.Ambient = new Vector3(0.1f, 0.1f, 0.1f);
            sdata.Diffuse = new Vector3(0.8f, 0.7f, 0.5f);
            sdata.Specular = new Vector3(0.9f, 0.9f, 0.9f);

            ((SunLight)gameObjectsFactory.Create(sdata)).Direction = new Vector3(-1,-1,-1);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/