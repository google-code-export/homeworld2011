using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
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
            SquareBodyMeshData dddtata = new SquareBodyMeshData();
            dddtata.Type = (typeof(SquareBodyMesh));
            dddtata.Model = "Barrel";
            dddtata.Diffuse = "Barrel_diffuse";
            dddtata.Specular = "Barrel_specular";
            dddtata.Normals = "Barrel_normals";
            dddtata.DynamicRoughness = 0.5f;
            dddtata.Elasticity = 0.1f;
            dddtata.StaticRoughness = 0.5f;
            dddtata.Height = 3.3f;
            dddtata.Lenght = 3.3f;
            dddtata.Width = 3.3f;
            dddtata.Immovable = false;
            dddtata.InstancingMode = 1;
            dddtata.Mass = 50;


            dddtata.World = Matrix.CreateTranslation(245, 54, 35);
            dddtata.Pitch = 90;
            gameObjectsFactory.Create(dddtata);

            LinkedCameraData lcdata = new LinkedCameraData();
            lcdata.Type = typeof(LinkedCamera);
            lcdata.position = new Vector3(245, 80, 10);
            lcdata.Target = new Vector3(245, 45, 35);
            lcdata.MovementSpeed = 0.07f;
            lcdata.RotationSpeed = 0.005f;
            lcdata.ZoomSpeed = 0.01f;
            lcdata.FoV = 60;
            lcdata.ZNear = 0.1f;
            lcdata.ZFar = 10000;
            lcdata.ActiveKeyListener = true;
            lcdata.ActiveMouseListener = true;

            LinkedCamera camera = (LinkedCamera)(gameObjectsFactory.Create(lcdata));

            //FreeCameraData fcdata = new FreeCameraData();
            //fcdata.Type = typeof(FreeCamera);
            //fcdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(240, 80, 10),
            //                                                 new Vector3(240, 10, 70),
            //                                                 new Vector3(0, 1, 0)));
            //fcdata.MovementSpeed = 0.05f;
            //fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            //fcdata.FoV = 60;
            //fcdata.ZNear = 0.1f;
            //fcdata.ZFar = 11250;
            //fcdata.ActiveKeyListener = true;
            //fcdata.ActiveMouseListener = true;
            //gameObjectsFactory.Create(fcdata);

            
            TerrainData tdata = new TerrainData();
            tdata.Type = typeof(Terrain);
            tdata.World = Matrix.CreateTranslation(0,0,0);
            tdata.HeightMap = "Heightmap";
            tdata.BaseTexture = "grass";
            tdata.RTexture = "ground";
            tdata.GTexture = "stone";
            tdata.BTexture = "snow";
            tdata.WeightMap = "Weightmap";
            tdata.Width = 256;
            tdata.Length = 256;
            tdata.Height = 60;
            tdata.CellSize = 5;
            tdata.TextureTiling = 40;
            tdata.Elasticity = 0.1f;
            tdata.StaticRoughness = 0.9f;
            tdata.DynamicRoughness = 0.9f;

            gameObjectsFactory.Create(tdata);

            SunlightData sdata = new SunlightData();
            sdata.Type = typeof(Sunlight);
            sdata.World = Matrix.Identity;
            sdata.Enabled = true;
            sdata.Diffuse = new Vector3(0.5f, 0.5f, 0.7f);
            sdata.Specular = new Vector3(0.7f, 0.7f, 0.9f);

            Sunlight s = (Sunlight)gameObjectsFactory.Create(sdata);
            s.Direction = new Vector3(-1,-1,-1);

            CreatureData ssdata = new CreatureData();
            ssdata.Type = typeof(Creature);
            ssdata.Model = "FleshCreature";
            ssdata.TimeRatio = 1.0f;
            ssdata.Diffuse = "flesh_diffuse";
            ssdata.Normals = "flesh_normals";
            ssdata.World *= Matrix.CreateTranslation(245, 56, 30);



            ssdata.Mass = 60;
            ssdata.StaticRoughness = 0.7f;
            ssdata.DynamicRoughness = 0.7f;
            ssdata.Elasticity = 0.3f;

            ssdata.Length = 5;
            ssdata.Radius = 1;
            ssdata.Immovable = false;
            ssdata.SkinPitch = 90;
            ssdata.Translation = new Vector3(0, 2.2f, 0);
            gameObjectsFactory.Create(ssdata);

            GlowStickData pdata = new GlowStickData();
            pdata.Type = typeof(GlowStick);
            pdata.Enabled = true;
            pdata.DynamicRoughness = 0.8f;
            pdata.StaticRoughness = 0.8f;
            pdata.Immovable = false;
            pdata.Mass = 0.3f;
            pdata.Elasticity = 0.2f;
            pdata.Color = new Vector3(0, 1, 0);
            pdata.Texture = "GlowStick_Diffuse";
            pdata.InstancingMode = 3;

            Random random = new Random();
            for (int i = 1; i < 50; i++)
            {                
                pdata.World = Matrix.CreateTranslation(250 + random.Next() % 30,
                                                        60 + random.Next() % 10,
                                                        80 + random.Next() % 30);
                gameObjectsFactory.Create(pdata);
            }

            CylindricalBodyMeshData ddwdtata = new CylindricalBodyMeshData();
            ddwdtata.Type = (typeof(CylindricalBodyMesh));
            ddwdtata.Model = "Barrel";
            ddwdtata.Diffuse = "Barrel_diffuse";
            ddwdtata.Specular = "Barrel_specular";
            ddwdtata.Normals = "Barrel_normals";
            ddwdtata.DynamicRoughness = 0.7f;
            ddwdtata.Elasticity = 0.1f;
            ddwdtata.StaticRoughness = 0.6f;
            ddwdtata.Radius = 1.1f;
            ddwdtata.Lenght = 3.3f;
            ddwdtata.Immovable = false;
            ddwdtata.InstancingMode = 3;
            ddwdtata.Mass = 1;

            for (int j = 0; j < 25; j++)
            {
                ddwdtata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
                                                          80  + random.Next() % 10,
                                                          80  + random.Next() % 70);
                gameObjectsFactory.Create(ddwdtata);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Game Object
        /****************************************************************************/
        public Dictionary<uint, GameObjectInstance> GameObjects
        {
            get
            {
                return gameObjects;
            }
        }
        /****************************************************************************/
        
    }
    /********************************************************************************/

}
/************************************************************************************/