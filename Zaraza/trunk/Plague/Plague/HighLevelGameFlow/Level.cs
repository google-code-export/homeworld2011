using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.Resources;
using PlagueEngine.EventsSystem;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;


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
        private List<GameObjectInstance>  updatableObjects = new List<GameObjectInstance>();
        private ContentManager            contentManager   = null;
        private EventsSystem.EventsSystem eventsSystem     = null;

        private int       lastGlobalID  = 0;
        private List<int> freeGlobalIDs = new List<int>();
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Dictionary<int, GameObjectInstance>  GameObjects         { get; private set; }
        public GameObjectsFactory                   GameObjectsFactory  { get; private set; }

        private Dictionary<String, List<GameObjectInstanceData>> GlobalGameObjectsData = new Dictionary<String, List<GameObjectInstanceData>>();

        public String CurrentLevel { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Level(ContentManager contentManager)
        {
            this.contentManager = contentManager;

            eventsSystem = new EventsSystem.EventsSystem(this);

            GameObjects        = new Dictionary<int, GameObjectInstance>();
            GameObjectsFactory = new GameObjectsFactory(GameObjects,updatableObjects);


            // TODO: usunąc linię 66 i 67 
            RegisterGlobalObject(new GameControllerData(),false,false);
            SaveGlobalGameObjectsData();
            LoadGlobalGameObjectsData();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init 
        /****************************************************************************/
        public void Init()
        {
            SpawnGlobalGameObjects(String.Empty);        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// New Level
        /****************************************************************************/
        public void NewLevel(String levelName)
        {
            Clear(true);
            eventsSystem.Reset();
            CurrentLevel = levelName;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Level
        /****************************************************************************/
        public void LoadLevel(String levelName)
        {
            LevelData levelData = contentManager.LoadLevel(levelName);

            Clear(true);
            eventsSystem.Reset();
                        
            // TODO: dodać wywoływanie metody w globalnych przy przejsci do kolejngo levelu, tak by mogły się dostosować

            Dictionary<int,KeyValuePair<GameObjectInstance,GameObjectInstanceData>> waitroom = new Dictionary<int,KeyValuePair<GameObjectInstance,GameObjectInstanceData>>();

            GameObjectsFactory.WaitingRoom = waitroom;

            SpawnGlobalGameObjects(levelName);

            foreach (GameObjectInstanceData goid in levelData.gameObjects)
            {
                GameObjectsFactory.Create(goid);                
            }

            GameObjectsFactory.ProcessWaitingRoom = true;
            GameObjectsFactory.ProcessedObjects   = 0;

            foreach (KeyValuePair<int, KeyValuePair<GameObjectInstance, GameObjectInstanceData>> goid in waitroom)
            {
                GameObjectsFactory.Create(goid.Value.Value);
            }

            if (waitroom.Count > GameObjectsFactory.ProcessedObjects)
            {
                throw new Exception("Game Objects (" + waitroom.Count.ToString() + ") stucked in waitroom");
            }
            
            GameObjectsFactory.WaitingRoom        = null;
            GameObjectsFactory.ProcessWaitingRoom = false;

            CurrentLevel = levelName;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Level
        /****************************************************************************/
        public void SaveLevel(String levelName = null)
        {
            if (String.IsNullOrEmpty(levelName)) levelName = CurrentLevel;

            LevelData levelData = new LevelData();
            
            foreach (GameObjectInstance goi in GameObjects.Values)
            {
                if(goi.ID > 0) levelData.gameObjects.Add(goi.GetData());                
            }

            contentManager.SaveLevel(levelName, levelData);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Spawn Global Game Objects
        /****************************************************************************/
        public void SpawnGlobalGameObjects(String levelName)
        {
            if (GlobalGameObjectsData.ContainsKey(levelName))
            {
                foreach (GameObjectInstanceData goid in GlobalGameObjectsData[levelName])
                {
                    if(!GameObjects.ContainsKey(goid.ID)) GameObjectsFactory.Create(goid);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Register Global Object
        /****************************************************************************/
        public GameObjectInstance RegisterGlobalObject(GameObjectInstanceData data, bool currentLevel,bool spawn)
        {
            int id;
            if (freeGlobalIDs.Count != 0)
            {
                id = freeGlobalIDs[freeGlobalIDs.Count - 1];
                freeGlobalIDs.RemoveAt(freeGlobalIDs.Count - 1);                
            }
            else
            {
                id = --lastGlobalID;
            }

            String levelName;
            if (currentLevel) levelName = CurrentLevel;
            else levelName = String.Empty;

            if (!GlobalGameObjectsData.ContainsKey(levelName))
            {
                GlobalGameObjectsData.Add(levelName, new List<GameObjectInstanceData>());
            }

            data.ID = id;
            GlobalGameObjectsData[levelName].Add(data);

            if (spawn)
            {
                if (!GameObjects.ContainsKey(data.ID)) return GameObjectsFactory.Create(data);
                else return null;
            }
            else return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Spawn Global Object
        /****************************************************************************/
        public GameObjectInstance SpawnGlobalObject(GameObjectInstanceData data)
        {
            int id;
            if (freeGlobalIDs.Count != 0)
            {
                id = freeGlobalIDs[freeGlobalIDs.Count - 1];
                freeGlobalIDs.RemoveAt(freeGlobalIDs.Count - 1);
            }
            else
            {
                id = --lastGlobalID;
            }

            return GameObjectsFactory.Create(data);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unregister Global Object
        /****************************************************************************/
        public void UnregisterGlobalObject(int id)
        {
            if (freeGlobalIDs.Contains(id)) return;

            if (id > lastGlobalID) return;
            else if (id == lastGlobalID)
            {
                ++lastGlobalID;
                return;
            }

            freeGlobalIDs.Add(id);

            GameObjectInstanceData delete = null;
            foreach (KeyValuePair<String, List<GameObjectInstanceData>> global in GlobalGameObjectsData)
            {                
                foreach (GameObjectInstanceData goid in global.Value)
                {
                    if (goid.ID == id)
                    {
                        delete = goid;
                        break;
                    }
                }
                if (delete != null)
                {
                    global.Value.Remove(delete);
                    break;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Clear
        /****************************************************************************/
        public void Clear(bool onlyLocalObjects)
        {
            if (!onlyLocalObjects)
            {
                foreach (GameObjectInstance goi in GameObjects.Values) goi.Dispose();
                GameObjects.Clear();

                foreach (GameObjectInstance goi in updatableObjects) goi.Dispose();
                updatableObjects.Clear();
            }
            else
            {
                List<GameObjectInstance> delete = new List<GameObjectInstance>();

                foreach (GameObjectInstance goi in GameObjects.Values)
                {
                    if (goi.ID > 0)
                    {
                        goi.Dispose();
                        delete.Add(goi);
                    }
                }

                foreach (GameObjectInstance goi in delete)
                {
                    GameObjects.Remove(goi.ID);
                    updatableObjects.Remove(goi);
                }

                delete.Clear();                
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Destructor  
        /****************************************************************************/
        ~Level()
        {
            Clear(false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
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
        /// Update Events System
        /****************************************************************************/
        public void UpdateEventsSystem()
        {
            eventsSystem.Update();
        }
        /****************************************************************************/


        /****************************************************************************/
        // Save Global Game Objects Data
        /****************************************************************************/
        public void SaveGlobalGameObjectsData()
        {
            GlobalGameObjectsData globalGameObjectsData = new GlobalGameObjectsData();

            globalGameObjectsData.FreeGlobalIDs = freeGlobalIDs;
            globalGameObjectsData.LastGlobalID  = lastGlobalID;

            foreach (KeyValuePair<String, List<GameObjectInstanceData>> pair in GlobalGameObjectsData)
            { 
                StringListPair slpair = new StringListPair();
                slpair.String      = pair.Key;
                slpair.GameObjects = pair.Value;

                globalGameObjectsData.GameObjectsData.Add(slpair);
            }

            contentManager.SaveGlobalGameObjectsData(globalGameObjectsData);
        }
        /****************************************************************************/
        
        
        /****************************************************************************/
        /// LoadGlobalGameObjectsData
        /****************************************************************************/
        private void LoadGlobalGameObjectsData()
        {
            GlobalGameObjectsData globalGameObjectsData;
            globalGameObjectsData = contentManager.LoadGlobalGameObjectsData();

            freeGlobalIDs = globalGameObjectsData.FreeGlobalIDs;
            lastGlobalID  = globalGameObjectsData.LastGlobalID;

            GlobalGameObjectsData = new Dictionary<String, List<GameObjectInstanceData>>();

            foreach (StringListPair pair in globalGameObjectsData.GameObjectsData)
            {
                GlobalGameObjectsData.Add(pair.String, pair.GameObjects);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Put Some Objects
        /****************************************************************************/
        public void PutSomeObjects()
        {

//            /***************************************/
//            /// Terrain
//            /***************************************/





//            CylindricalBodyMeshData2 dddtata = new CylindricalBodyMeshData2();
//            dddtata.Type = (typeof(CylindricalBodyMesh2));
//            dddtata.Model = "tire01";
//            dddtata.Diffuse = "tire01.diff";
//            dddtata.DynamicRoughness = 0.5f;
//            dddtata.Elasticity = 0.1f;
//            dddtata.StaticRoughness = 0.5f;

//            dddtata.Lenght = 0.4f;
//            dddtata.Radius = 0.7f;
//            dddtata.Immovable = false;
//            dddtata.InstancingMode = 1;
//            dddtata.Mass = 50;
//            dddtata.SkinPitch = 90;

//            dddtata.World = Matrix.CreateTranslation(245, 54, 35);
//            dddtata.Pitch = 90;
//            dddtata.EnabledMesh = true;
//            GameObjectsFactory.Create(dddtata);




//>>>>>>> .r363
//            TerrainData tdata = new TerrainData();
//            tdata.Type = typeof(Terrain);
//            tdata.World       = Matrix.CreateTranslation(0, 0, 0);
//            tdata.HeightMap   = "Terrain\\Level1";
//            tdata.BaseTexture = "Terrain\\snow";
//            tdata.WeightMap   = "Terrain\\BlankWeights";
            
//            tdata.Width = 350.0f;
//            tdata.Height = 57.512f;
//            tdata.Segments = 256;
//            tdata.TextureTiling = 0.2f;
//            tdata.Elasticity = 0.1f;
//            tdata.StaticRoughness = 0.9f;
//            tdata.DynamicRoughness = 0.9f;            
//            tdata.Status = 5;
            
//            GameObjectsFactory.Create(tdata);
//            /***************************************/


//            SunlightData sdata = new SunlightData();
//            sdata.Type = typeof(Sunlight);
//            sdata.World = Matrix.Identity;
//            sdata.Enabled = true;
//            sdata.Diffuse = new Vector3(1f, 1f, 1f);
//            sdata.Intensity = 1;
            
//            Sunlight s = (Sunlight)GameObjectsFactory.Create(sdata);
//            s.Direction = new Vector3(-1, -1, -1);
            

//            FreeCameraData fcdata = new FreeCameraData();
//            fcdata.Type = typeof(FreeCamera);
//            fcdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(240, 80, 10),
//                                                             new Vector3(240, 10, 70),
//                                                             new Vector3(0, 1, 0)));
//            fcdata.MovementSpeed = 0.05f;
//            fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
//            fcdata.FoV = 60;
//            fcdata.ZNear = 1;
//            fcdata.ZFar = 200;
//            fcdata.ActiveKeyListener = true;
//            fcdata.ActiveMouseListener = true;
//            GameObjectsFactory.Create(fcdata);
//            pdata.EnabledMesh = true;
//<<<<<<< .mine
//=======
//            Random random = new Random();
//            for (int i = 0; i < 30; i++)
//            {
//                pdata.World = Matrix.CreateTranslation(250 + random.Next() % 30,
//                                                        60 + random.Next() % 10,
//                                                        120 + random.Next() % 30);
//                GameObjectsFactory.Create(pdata);
//            }

//            CylindricalBodyMeshData ddwdtata = new CylindricalBodyMeshData();
//            ddwdtata.Type = (typeof(CylindricalBodyMesh));
//            ddwdtata.Model = "Barrel";
//            ddwdtata.Diffuse = "Barrel_diffuse";
//            ddwdtata.Specular = "Barrel_specular";
//            ddwdtata.Normals = "Barrel_normals";
//            ddwdtata.DynamicRoughness = 0.7f;
//            ddwdtata.Elasticity = 0.1f;
//            ddwdtata.StaticRoughness = 0.6f;
//            ddwdtata.Radius = 1.1f;
//            ddwdtata.Lenght = 3.3f;
//            ddwdtata.Immovable = false;
//            ddwdtata.InstancingMode = 3;
//            ddwdtata.Mass = 1;
//            ddwdtata.EnabledMesh = true;
//            for (int j = 0; j < 4; j++)
//            {
//                ddwdtata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
//                                                          80 + random.Next() % 10,
//                                                          80 + random.Next() % 70);
//                GameObjectsFactory.Create(ddwdtata);
//            }

//            SpotLightData spdata = new SpotLightData();
//            spdata.Type = typeof(SpotLight);
//            spdata.Color = new Vector3(1, 0, 0);
//            spdata.Enabled = true;
//            spdata.FarPlane = 90;
//            spdata.NearPlane = 1f;
//            spdata.Radius = 60f;
//            spdata.Texture = "RadialAttenuation";
//            spdata.LinearAttenuation = 0;
//            spdata.Intensity = 2;
//            spdata.ShadowsEnabled = true;
//            spdata.LocalTransform = Matrix.Identity;
//            spdata.QuadraticAttenuation = 20;
//            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(245, 60, 95), new Vector3(285, 50, 100), Vector3.Up));

//            GameObjectsFactory.Create(spdata);

//            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(315, 60, 95), new Vector3(285, 50, 100), Vector3.Up));
//            spdata.Color = new Vector3(0, 1, 0);
//            GameObjectsFactory.Create(spdata);

//            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(275, 60, 120), new Vector3(285, 50, 100), Vector3.Up));
//            spdata.Color = new Vector3(0, 0, 1);
//            spdata.Specular = true;
//            GameObjectsFactory.Create(spdata);

//            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(275, 60, 150), new Vector3(285, 50, 120), Vector3.Up));
//            spdata.Color = new Vector3(1, 1, 0);
//            spdata.Specular = true;
//            GameObjectsFactory.Create(spdata);

//            spdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(295, 80, 110), new Vector3(285, 50, 120), Vector3.Up));
//            spdata.Color = new Vector3(1, 0, 1);
//            spdata.Texture = "Horse";
//            spdata.Specular = true;
//            GameObjectsFactory.Create(spdata);

//            CylindricalSkinMeshData d2 = new CylindricalSkinMeshData();

//            d2.Type = typeof(CylindricalSkinMesh);
//            d2.Model = "woodbox01";
//            d2.Diffuse = "woodbox01.diff";
//            d2.Specular = "woodbox01.spec";
//            d2.Normals = "woodbox01.norm";
//            d2.InstancingMode = 3;
//            d2.EnabledMesh = true;
//            d2.DynamicRoughness = 0.8f;
//            d2.Elasticity = 0.1f;
//            d2.Radius = 0.5f;
//            d2.Lenght = 1.3f;
            
//            d2.StaticRoughness = 0.8f;
//            d2.Translation = new Vector3(245, 66, 66);

//            GameObjectsFactory.Create(d2);

//            SquareBodyMeshData sssdata = new SquareBodyMeshData();
//            sssdata.Type = typeof(SquareBodyMesh);
//            sssdata.Model = "woodbox01";
//            sssdata.Diffuse = "woodbox01.diff";
//            sssdata.Specular = "woodbox01.spec";
//            sssdata.Normals = "woodbox01.norm";
//            sssdata.InstancingMode = 3;
//            sssdata.EnabledMesh = true;
//            sssdata.DynamicRoughness = 0.8f;
//            sssdata.Elasticity = 0.1f;
//            sssdata.Mass = 3f;
//            sssdata.Height = 1.3f;
//            sssdata.Lenght = 1.3f;
//            sssdata.Width = 1.3f;
//            sssdata.Immovable = false;
//            sssdata.StaticRoughness = 0.8f;
//            sssdata.Translation = new Vector3(0, 0.65f, 0);

//            for (int j = 0; j < 3; j++)
//            {
//                sssdata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
//                                                         100 + random.Next() % 10,
//                                                         80 + random.Next() % 70);
//                GameObjectsFactory.Create(sssdata);
//            }


//            ddwdtata.Type = (typeof(CylindricalBodyMesh));
//            ddwdtata.Model = "gasbottle01";
//            ddwdtata.Diffuse = "gasbottle01.diff";
//            ddwdtata.Specular = "gasbottle01.spec";
//            ddwdtata.Normals = "gasbottle01.norm";
//            ddwdtata.DynamicRoughness = 0.7f;
//            ddwdtata.Elasticity = 0.1f;
//            ddwdtata.StaticRoughness = 0.6f;
//            ddwdtata.Radius = 0.38f;
//            ddwdtata.Lenght = 2.6f;
//            ddwdtata.Translation = new Vector3(0, 1.3f, 0);
//            ddwdtata.Immovable = false;
//            ddwdtata.InstancingMode = 3;
//            ddwdtata.SkinPitch = 90;
//            ddwdtata.Mass = 1;
//            ddwdtata.EnabledMesh = true;
//            for (int j = 0; j < 4; j++)
//            {
//                ddwdtata.World = Matrix.CreateTranslation(250 + random.Next() % 70,
//                                                          50 + random.Next() % 10,
//                                                          70 + random.Next() % 70);
//                GameObjectsFactory.Create(ddwdtata);
//            }


//            sssdata.Type = typeof(SquareBodyMesh);
//            sssdata.Model = "pavelow";
//            sssdata.Diffuse = "pavelow.diff";
//            sssdata.Specular = "pavelow.spec";
//            sssdata.Normals = "pawelow.norm";
//            sssdata.InstancingMode = 1;
//            sssdata.EnabledMesh = true;
//            sssdata.DynamicRoughness = 0.8f;
//            sssdata.Elasticity = 0.1f;
//            sssdata.Mass = 1000f;
//            sssdata.Height = 6f;
//            sssdata.Lenght = 50f;
//            sssdata.Width = 24f;
//            sssdata.Immovable = false;
//            sssdata.StaticRoughness = 0.8f;
//            sssdata.Translation = new Vector3(0, 3, 0);

//            sssdata.World = Matrix.CreateTranslation(351, 100, 152);
//            GameObjectsFactory.Create(sssdata);


//            sssdata.Type = typeof(SquareBodyMesh);
//            sssdata.Model = "buldozer";
//            sssdata.Diffuse = "buldozer.diff";
//            sssdata.Specular = String.Empty;
//            sssdata.Normals = String.Empty;
//            sssdata.InstancingMode = 1;
//            sssdata.EnabledMesh = true;
//            sssdata.DynamicRoughness = 0.8f;
//            sssdata.Elasticity = 0.1f;
//            sssdata.Mass = 10000f;
//            sssdata.Height = 6f;
//            sssdata.Lenght = 7.1f;
//            sssdata.Width = 12f;
//            sssdata.Immovable = false;
//            sssdata.StaticRoughness = 0.8f;
//            sssdata.Translation = new Vector3(0, 1, 0);
//            sssdata.Status = 1;
//            sssdata.World = Matrix.CreateTranslation(240, 60, 60);
//            GameObjectsFactory.Create(sssdata);





//            //BurningBarrelData ddxdtata = new BurningBarrelData();
//            //ddxdtata.Type = (typeof(BurningBarrel));
//            //ddxdtata.particleTexture = "fire";
//            //ddxdtata.maxParticles = 500;
//            //ddxdtata.duration = 3;
//            //ddxdtata.durationRandomnes = 1;
//            //ddxdtata.minHorizontalVelocity = 0;
//            //ddxdtata.maxHorizontalVelocity = 0;
//            //ddxdtata.minVerticalVelocity = 0;
//            //ddxdtata.maxVerticalVelocity = 10;
//            //ddxdtata.gravity = new Vector3(0, 5, 0);
//            //ddxdtata.minColor = new Color(255, 255, 255, 10);
//            //ddxdtata.maxColor = new Color(255, 255, 255, 255);
//            //ddxdtata.minStartSize = 5;
//            //ddxdtata.maxStartSize = 10;
//            //ddxdtata.minEndSize = 1;
//            //ddxdtata.maxEndSize = 2;
//            //ddxdtata.blendState = 1;
//            //ddxdtata.particlesPerSecond = 10;
//            //ddxdtata.Model = "Barrel";
//            //ddxdtata.Diffuse = "Barrel_diffuse";
//            //ddxdtata.Specular = "Barrel_specular";
//            //ddxdtata.Normals = "Barrel_normals";
//            //ddxdtata.DynamicRoughness = 0.7f;
//            //ddxdtata.Elasticity = 0.1f;
//            //ddxdtata.StaticRoughness = 0.6f;
//            //ddxdtata.Radius = 1.1f;
//            //ddxdtata.Lenght = 3.3f;
//            //ddxdtata.Immovable = false;
//            //ddxdtata.InstancingMode = 3;
//            //ddxdtata.Mass = 1;
//            //ddxdtata.World = Matrix.CreateTranslation(255, 77, 35);

//            //GameObjectsFactory.Create(ddxdtata);






//            FlashlightData fldata = new FlashlightData();
//            fldata.Type = (typeof(Flashlight));
//            fldata.AttenuationTexture = "FlashlightBeam";
//            fldata.Color = new Vector3(1, 1, 0.75f);
//            fldata.DynamicRoughness = 1;
//            fldata.Elasticity = 1;
//            fldata.Enabled = true;
//            fldata.FarPlane = 30;
//            fldata.Immovable = false;
//            fldata.InstancingMode = 3;
//            fldata.Intensity = 10;
//            fldata.LinearAttenuation = 1;
//            fldata.Mass = 1;
//            fldata.NearPlane = 1;
//            fldata.QuadraticAttenuation = 5;
//            fldata.Radius = 75;
//            fldata.ShadowsEnabled = true;
//            fldata.Specular = true;
//            fldata.StaticRoughness = 1;
//            fldata.World = Matrix.CreateTranslation(250, 60, 30);
//            fldata.Status = 2;
//            fldata.DepthBias = 0.05f;
//            fldata.Name = "Flashlight";
//            fldata.Icon = new Rectangle(0, 620, 50, 50);
//            fldata.SlotsIcon = new Rectangle(50, 620, 32, 32);
//            fldata.EnabledMesh = true;
//            GameObjectsFactory.Create(fldata);
//            fldata.World = Matrix.CreateTranslation(252, 60, 30);

//            GameObjectsFactory.Create(fldata);

//            MercenaryData mddd = new MercenaryData();
//            mddd.Type = typeof(Mercenary);
//            mddd.Model = "mie";
//            mddd.TimeRatio = 1.0f;
//            mddd.Diffuse = "miesniak_diff";
//            mddd.Normals = "miesniak_norm";
//            mddd.World = Matrix.CreateTranslation(245, 56, 30);

//            mddd.Mass = 90;
//            mddd.StaticRoughness = 1.0f;
//            mddd.DynamicRoughness = 1.0f;
//            mddd.Elasticity = 0.01f;

//            mddd.Length = 5;
//            mddd.Radius = 1;
//            mddd.Immovable = false;
//            mddd.SkinPitch = 90;
//            mddd.Translation = new Vector3(0, 2.2f, 0);
//            mddd.Status = 3;
//            mddd.MarkerPosition = new Vector3(0, 6.2f, 1);
//            mddd.RotationSpeed = 7.5f;
//            mddd.MovingSpeed = 1000.0f;
//            mddd.DistancePrecision = 1.0f;
//            mddd.AnglePrecision = 0.1f;
//            mddd.GripBone = "Bip001_R_Hand001";
//            mddd.Name = "Marian";
//            mddd.MaxHP = 120;
//            mddd.HP = 120;
//            mddd.InventoryIcon = new Rectangle(1260, 113, 51, 64);
//            mddd.TinySlots = 10;
//            mddd.Slots = 20;
//            mddd.Icon = new Rectangle(0, 64, 64, 64);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 100;
//            mddd.Name = "Marian 2";
//            mddd.World = Matrix.CreateTranslation(260, 56, 30);
//            GameObjectsFactory.Create(mddd);
//            mddd.TinySlots = 50;
//            mddd.Slots = 150;

//            mddd.HP = 80;
//            mddd.Name = "Marian 3";
//            mddd.World = Matrix.CreateTranslation(265, 56, 30);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 70;
//            mddd.Name = "Marian 4";
//            mddd.World = Matrix.CreateTranslation(270, 56, 30);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 60;
//            mddd.Name = "Marian 5";
//            mddd.World = Matrix.CreateTranslation(275, 56, 30);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 40;
//            mddd.Name = "Marian 6";
//            mddd.World = Matrix.CreateTranslation(260, 56, 35);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 20;
//            mddd.Name = "Marian 7";
//            mddd.World = Matrix.CreateTranslation(265, 56, 35);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 10;
//            mddd.Name = "Marian 8";
//            mddd.World = Matrix.CreateTranslation(270, 56, 35);
//            GameObjectsFactory.Create(mddd);

//            mddd.HP = 0;
//            mddd.Name = "Marian 9";
//            mddd.World = Matrix.CreateTranslation(275, 56, 35);
//            GameObjectsFactory.Create(mddd);

//            MercenariesManagerData mcmd = new MercenariesManagerData();
//            mcmd.Type = typeof(MercenariesManager);

//            GameObjectInstance mm = GameObjectsFactory.Create(mcmd);

//            LinkedCameraData lcdata = new LinkedCameraData();
//            lcdata.Type = typeof(LinkedCamera);
//            lcdata.position = new Vector3(245, 80, 10);
//            lcdata.Target = new Vector3(245, 45, 35);
//            lcdata.MovementSpeed = 0.07f;
//            lcdata.RotationSpeed = 0.005f;
//            lcdata.ZoomSpeed = 0.01f;
//            lcdata.FoV = 60;
//            lcdata.ZNear = 1f;
//            lcdata.ZFar = 200;
//            lcdata.ActiveKeyListener = true;
//            lcdata.ActiveMouseListener = true;
//            lcdata.MercenariesManager = mm.ID;

//            LinkedCamera camera = (LinkedCamera)(GameObjectsFactory.Create(lcdata));

//            (mm as MercenariesManager).LinkedCamera = camera;



            FreeCameraData fcdata = new FreeCameraData();
            fcdata.Type = typeof(FreeCamera);
            fcdata.World = Matrix.Invert(Matrix.CreateLookAt(new Vector3(240, 80, 10),
                                                             new Vector3(240, 10, 70),
                                                             new Vector3(0, 1, 0)));
            fcdata.MovementSpeed = 0.05f;
            fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
            fcdata.FoV = 60;
            fcdata.ZNear = 1;
            fcdata.ZFar = 200;
            fcdata.ActiveKeyListener = true;
            fcdata.ActiveMouseListener = true;
            GameObjectsFactory.Create(fcdata);


//            //CreatureData ssdata = new CreatureData();
//            //ssdata.Type = typeof(Creature);
//            //ssdata.Model = "FleshCreature";
//            //ssdata.TimeRatio = 1.0f;
//            //ssdata.Diffuse = "flesh_diffuse";
//            //ssdata.Normals = "flesh_normals";
//            //ssdata.World *= Matrix.CreateTranslation(245, 66, 33);



//            //ssdata.Mass = 60;
//            //ssdata.StaticRoughness = 0.7f;
//            //ssdata.DynamicRoughness = 0.7f;
//            //ssdata.Elasticity = 0.3f;

//            //ssdata.Length = 5;
//            //ssdata.Radius = 1;
//            //ssdata.Immovable = false;
//            //ssdata.SkinPitch = 90;
//            //ssdata.Translation = new Vector3(0, 2.2f, 0);
//            //GameObjectsFactory.Create(ssdata);



//            StaticMeshData data2 = new StaticMeshData();
//            data2.Model = "Barrel";
//            data2.Type = typeof(StaticMesh);
//            data2.World.Translation = new Vector3(250, 70, 30);
//            data2.EnabledMesh = true;
//            //data2.InstancingMode = 3;
//            GameObjectsFactory.Create(data2);


//>>>>>>> .r363

//            FirearmData frmdata = new FirearmData();
//            frmdata.Type = (typeof(Firearm));
//            frmdata.DynamicRoughness = 1;
//            frmdata.Elasticity = 1;
//            frmdata.Immovable = false;
            
//            frmdata.InstancingMode = 3;
//            frmdata.Mass = 1;
//            frmdata.StaticRoughness = 1;
//            frmdata.Status = 2;
//            frmdata.Name = "DesertEagle";
//            frmdata.Model = @"Firearms\DesertEagle";
//            frmdata.Diffuse = @"Firearms\desertEagle";
//            frmdata.Icon = new Rectangle(0, 620, 50, 50);
//            frmdata.SlotsIcon = new Rectangle(50, 620, 32, 32);
//            frmdata.EnabledMesh = true;
//            frmdata.Lenght = 1;
//            frmdata.Height = 1;
//            frmdata.Width = 1;

//            fldata.World = Matrix.CreateTranslation(252, 165, 31);
//            GameObjectsFactory.Create(frmdata);

        }
        /****************************************************************************/
       
    }
    /********************************************************************************/

}
/************************************************************************************/