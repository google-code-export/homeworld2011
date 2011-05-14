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
        private Dictionary<uint, GameObjectInstance> gameObjects        = new Dictionary<uint,GameObjectInstance>();
        private GameObjectsFactory                   gameObjectsFactory = null;
        private List<GameObjectInstance>             updatableObjects   = new List<GameObjectInstance>();
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Level(GameObjectsFactory gameObjectsFactory)
        {
            this.gameObjectsFactory             = gameObjectsFactory;
            gameObjectsFactory.GameObjects      = gameObjects;
            gameObjectsFactory.UpdatableObjects = updatableObjects;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Level
        /****************************************************************************/
        public void LoadLevel(LevelData levelData)
        {
            Clear();

            Dictionary<uint,KeyValuePair<GameObjectInstance,GameObjectInstanceData>> waitroom = new Dictionary<uint,KeyValuePair<GameObjectInstance,GameObjectInstanceData>>();

            gameObjectsFactory.WaitingRoom = waitroom;

            foreach (GameObjectInstanceData goid in levelData.gameObjects)
            {
                gameObjectsFactory.Create(goid);                
            }

            gameObjectsFactory.ProcessWaitingRoom = true;
            gameObjectsFactory.ProcessedObjects   = 0;

            foreach (KeyValuePair<uint, KeyValuePair<GameObjectInstance, GameObjectInstanceData>> goid in waitroom)
            {
                gameObjectsFactory.Create(goid.Value.Value);
            }

            if (waitroom.Count > gameObjectsFactory.ProcessedObjects)
            {
                throw new Exception("Game Objects (" + waitroom.Count.ToString() + ") stucked in waitroom");
            }
            
            gameObjectsFactory.WaitingRoom = null;
            gameObjectsFactory.ProcessWaitingRoom = false;
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
            foreach (GameObjectInstance goi in updatableObjects) goi.Dispose();
            updatableObjects.Clear();
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
        ///  Update
        /****************************************************************************/
        public void Update(TimeSpan deltaTime)
        {
            foreach (GameObjectInstance go in updatableObjects)
            {
                go.Update(deltaTime);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Put Some Objects
        /****************************************************************************/
        public void PutSomeObjects()
        {




            CylindricalBodyMeshData2 dddtata = new CylindricalBodyMeshData2();
            dddtata.Type = (typeof(CylindricalBodyMesh2));
            dddtata.Model = "tire01";
            dddtata.Diffuse = "tire01.diff";
            dddtata.DynamicRoughness = 0.5f;
            dddtata.Elasticity = 0.1f;
            dddtata.StaticRoughness = 0.5f;

            dddtata.Lenght = 0.4f;
            dddtata.Radius = 0.7f;
            dddtata.Immovable = false;
            dddtata.InstancingMode = 1;
            dddtata.Mass = 50;
            dddtata.SkinPitch = 90;

            dddtata.World = Matrix.CreateTranslation(245, 54, 35);
            dddtata.Pitch = 90;
            gameObjectsFactory.Create(dddtata);

            //FreeCameraData fcdata = new FreeCameraData();
            //fcdata.Type = typeof(FreeCamera);
            //fcdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(240, 80, 10),
            //                                                 new Vector3(240, 10, 70),
            //                                                 new Vector3(0, 1, 0)));
            //fcdata.MovementSpeed = 0.05f;
            //fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            //fcdata.FoV = 60;
            //fcdata.ZNear = 1;
            //fcdata.ZFar = 200;
            //fcdata.ActiveKeyListener = true;
            //fcdata.ActiveMouseListener = true;
            //gameObjectsFactory.Create(fcdata);


            TerrainData tdata = new TerrainData();
            tdata.Type = typeof(Terrain);
            tdata.World = Matrix.CreateTranslation(0, 0, 0);
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
            tdata.Status = 5;

            gameObjectsFactory.Create(tdata);

            SunlightData sdata = new SunlightData();
            sdata.Type = typeof(Sunlight);
            sdata.World = Matrix.Identity;
            sdata.Enabled = true;
            sdata.Diffuse = new Vector3(1f, 1f, 1f);
            sdata.Intensity = 1;

            Sunlight s = (Sunlight)gameObjectsFactory.Create(sdata);
            s.Direction = new Vector3(-1, -1, -1);

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
            pdata.Status = 2;

            Random random = new Random();
            for (int i = 1; i < 4; i++)
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

            for (int j = 0; j < 4; j++)
            {
                ddwdtata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
                                                          80 + random.Next() % 10,
                                                          80 + random.Next() % 70);
                gameObjectsFactory.Create(ddwdtata);
            }

            SpotLightData spdata = new SpotLightData();
            spdata.Type = typeof(SpotLight);
            spdata.Color = new Vector3(1, 0, 0);
            spdata.Enabled = true;
            spdata.FarPlane = 90;
            spdata.NearPlane = 1f;
            spdata.Radius = 60f;
            spdata.Texture = "RadialAttenuation";
            spdata.LinearAttenuation = 0;
            spdata.Intensity = 2;
            spdata.ShadowsEnabled = true;
            spdata.LocalTransform = Matrix.Identity;
            spdata.QuadraticAttenuation = 20;
            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(245, 60, 95), new Vector3(285, 50, 100), Vector3.Up));

            gameObjectsFactory.Create(spdata);

            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(315, 60, 95), new Vector3(285, 50, 100), Vector3.Up));
            spdata.Color = new Vector3(0, 1, 0);
            gameObjectsFactory.Create(spdata);

            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(275, 60, 120), new Vector3(285, 50, 100), Vector3.Up));
            spdata.Color = new Vector3(0, 0, 1);
            spdata.Specular = true;
            gameObjectsFactory.Create(spdata);

            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(275, 60, 150), new Vector3(285, 50, 120), Vector3.Up));
            spdata.Color = new Vector3(1, 1, 0);
            spdata.Specular = true;
            gameObjectsFactory.Create(spdata);

            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(295, 80, 110), new Vector3(285, 50, 120), Vector3.Up));
            spdata.Color = new Vector3(1, 0, 1);
            spdata.Texture = "Horse";
            spdata.Specular = true;
            gameObjectsFactory.Create(spdata);

            SquareBodyMeshData sssdata = new SquareBodyMeshData();
            sssdata.Type = typeof(SquareBodyMesh);
            sssdata.Model = "woodbox01";
            sssdata.Diffuse = "woodbox01.diff";
            sssdata.Specular = "woodbox01.spec";
            sssdata.Normals = "woodbox01.norm";
            sssdata.InstancingMode = 3;

            sssdata.DynamicRoughness = 0.8f;
            sssdata.Elasticity = 0.1f;
            sssdata.Mass = 3f;
            sssdata.Height = 1.3f;
            sssdata.Lenght = 1.3f;
            sssdata.Width = 1.3f;
            sssdata.Immovable = false;
            sssdata.StaticRoughness = 0.8f;
            sssdata.Translation = new Vector3(0, 0.65f, 0);

            for (int j = 0; j < 3; j++)
            {
                sssdata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
                                                         100 + random.Next() % 10,
                                                         80 + random.Next() % 70);
                gameObjectsFactory.Create(sssdata);
            }


            ddwdtata.Type = (typeof(CylindricalBodyMesh));
            ddwdtata.Model = "gasbottle01";
            ddwdtata.Diffuse = "gasbottle01.diff";
            ddwdtata.Specular = "gasbottle01.spec";
            ddwdtata.Normals = "gasbottle01.norm";
            ddwdtata.DynamicRoughness = 0.7f;
            ddwdtata.Elasticity = 0.1f;
            ddwdtata.StaticRoughness = 0.6f;
            ddwdtata.Radius = 0.38f;
            ddwdtata.Lenght = 2.6f;
            ddwdtata.Translation = new Vector3(0, 1.3f, 0);
            ddwdtata.Immovable = false;
            ddwdtata.InstancingMode = 3;
            ddwdtata.SkinPitch = 90;
            ddwdtata.Mass = 1;

            for (int j = 0; j < 4; j++)
            {
                ddwdtata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
                                                          50 + random.Next() % 10,
                                                          70 + random.Next() % 70);
                gameObjectsFactory.Create(ddwdtata);
            }


            sssdata.Type = typeof(SquareBodyMesh);
            sssdata.Model = "pavelow";
            sssdata.Diffuse = "pavelow.diff";
            sssdata.Specular = "pavelow.spec";
            sssdata.Normals = "pawelow.norm";
            sssdata.InstancingMode = 1;

            sssdata.DynamicRoughness = 0.8f;
            sssdata.Elasticity = 0.1f;
            sssdata.Mass = 1000f;
            sssdata.Height = 6f;
            sssdata.Lenght = 50f;
            sssdata.Width = 24f;
            sssdata.Immovable = false;
            sssdata.StaticRoughness = 0.8f;
            sssdata.Translation = new Vector3(0, 3, 0);

            sssdata.World = Matrix.CreateTranslation(351, 100, 152);
            gameObjectsFactory.Create(sssdata);


            sssdata.Type = typeof(SquareBodyMesh);
            sssdata.Model = "buldozer";
            sssdata.Diffuse = "buldozer.diff";
            sssdata.Specular = String.Empty;
            sssdata.Normals = String.Empty;
            sssdata.InstancingMode = 1;

            sssdata.DynamicRoughness = 0.8f;
            sssdata.Elasticity = 0.1f;
            sssdata.Mass = 1000f;
            sssdata.Height = 2f;
            sssdata.Lenght = 7.1f;
            sssdata.Width = 12f;
            sssdata.Immovable = false;
            sssdata.StaticRoughness = 0.8f;
            sssdata.Translation = new Vector3(0, 1, 0);
            sssdata.Status = 1;
            sssdata.World = Matrix.CreateTranslation(240, 60, 60);
            gameObjectsFactory.Create(sssdata);





            BurningBarrelData ddxdtata = new BurningBarrelData();
            ddxdtata.Type = (typeof(BurningBarrel));
            ddxdtata.particleTexture = "fire";
            ddxdtata.maxParticles = 500;
            ddxdtata.duration = 3;
            ddxdtata.durationRandomnes = 1;
            ddxdtata.minHorizontalVelocity = 0;
            ddxdtata.maxHorizontalVelocity = 0;
            ddxdtata.minVerticalVelocity = 0;
            ddxdtata.maxVerticalVelocity = 10;
            ddxdtata.gravity = new Vector3(0, 5, 0);
            ddxdtata.minColor = new Color(255, 255, 255, 10);
            ddxdtata.maxColor = new Color(255, 255, 255, 255);
            ddxdtata.minStartSize = 5;
            ddxdtata.maxStartSize = 10;
            ddxdtata.minEndSize = 1;
            ddxdtata.maxEndSize = 2;
            ddxdtata.blendState = 1;
            ddxdtata.particlesPerSecond = 10;
            ddxdtata.Model = "Barrel";
            ddxdtata.Diffuse = "Barrel_diffuse";
            ddxdtata.Specular = "Barrel_specular";
            ddxdtata.Normals = "Barrel_normals";
            ddxdtata.DynamicRoughness = 0.7f;
            ddxdtata.Elasticity = 0.1f;
            ddxdtata.StaticRoughness = 0.6f;
            ddxdtata.Radius = 1.1f;
            ddxdtata.Lenght = 3.3f;
            ddxdtata.Immovable = false;
            ddxdtata.InstancingMode = 3;
            ddxdtata.Mass = 1;
            ddxdtata.World = Matrix.CreateTranslation(255, 77, 35);

            gameObjectsFactory.Create(ddxdtata);
            





            FlashlightData fldata = new FlashlightData();
            fldata.Type = (typeof(Flashlight));
            fldata.AttenuationTexture = "FlashlightBeam";
            fldata.Color = new Vector3(1, 1, 0.75f);
            fldata.DynamicRoughness = 1;
            fldata.Elasticity = 1;
            fldata.Enabled = true;
            fldata.FarPlane = 20;
            fldata.Immovable = false;
            fldata.InstancingMode = 3;
            fldata.Intensity = 2;
            fldata.LinearAttenuation = 0;
            fldata.Mass = 1;
            fldata.NearPlane = 1;
            fldata.QuadraticAttenuation = 5;
            fldata.Radius = 45;
            fldata.ShadowsEnabled = true;
            fldata.Specular = true;
            fldata.StaticRoughness = 1;
            fldata.World = Matrix.CreateTranslation(250, 60, 30);
            fldata.Status = 2;
            gameObjectsFactory.Create(fldata);


            MercenaryData mddd = new MercenaryData();
            mddd.Type = typeof(Mercenary);
            mddd.Model = "mie";
            mddd.TimeRatio = 1.0f;
            mddd.Diffuse = "Miesniak_diff";
            mddd.Normals = "miesniak_norm";
            mddd.World = Matrix.CreateTranslation(245, 56, 30);

            mddd.Mass = 90;
            mddd.StaticRoughness = 1f;
            mddd.DynamicRoughness = 1f;
            mddd.Elasticity = 0f;

            mddd.Length = 5;
            mddd.Radius = 1;
            mddd.Immovable = false;
            mddd.SkinPitch = 90;
            mddd.Translation = new Vector3(0, 2.2f, 0);
            mddd.Status = 3;
            mddd.MarkerPosition = new Vector3(0, 6.2f, 1);

            gameObjectsFactory.Create(mddd);


            mddd.World = Matrix.CreateTranslation(260, 56, 30);
            gameObjectsFactory.Create(mddd);


            MercenariesManagerData mcmd = new MercenariesManagerData();
            mcmd.Type = typeof(MercenariesManager);
            mcmd.SelectedMercenaries = new List<uint>();
            
            GameObjectInstance mm = gameObjectsFactory.Create(mcmd);

            LinkedCameraData lcdata = new LinkedCameraData();
            lcdata.Type = typeof(LinkedCamera);
            lcdata.position = new Vector3(245, 80, 10);
            lcdata.Target = new Vector3(245, 45, 35);
            lcdata.MovementSpeed = 0.07f;
            lcdata.RotationSpeed = 0.005f;
            lcdata.ZoomSpeed = 0.01f;
            lcdata.FoV = 60;
            lcdata.ZNear = 1f;
            lcdata.ZFar = 200;
            lcdata.ActiveKeyListener = true;
            lcdata.ActiveMouseListener = true;
            lcdata.MercenariesManager = mm.ID;

            LinkedCamera camera = (LinkedCamera)(gameObjectsFactory.Create(lcdata));

            (mm as MercenariesManager).LinkedCamera = camera;



            CreatureData ssdata = new CreatureData();
            ssdata.Type = typeof(Creature);
            ssdata.Model = "FleshCreature";
            ssdata.TimeRatio = 1.0f;
            ssdata.Diffuse = "flesh_diffuse";
            ssdata.Normals = "flesh_normals";
            ssdata.World *= Matrix.CreateTranslation(245, 66, 33);



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