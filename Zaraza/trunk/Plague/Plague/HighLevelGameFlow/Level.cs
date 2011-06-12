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

        private bool spawningGlobals = false;
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
            GameObjectsFactory = new GameObjectsFactory(GameObjects, 
                                                        updatableObjects, 
                                                        this);


            // TODO: usunąc linię 66 i 67 
            //RegisterGlobalObject(new GameControllerData(),false,false);
            //RegisterGlobalObject(new AmmunitionData(), false, false);
            //SaveGlobalGameObjectsData();
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
            spawningGlobals = true;

            if (GlobalGameObjectsData.ContainsKey(levelName))
            {
                foreach (GameObjectInstanceData goid in GlobalGameObjectsData[levelName])
                {
                    if(!GameObjects.ContainsKey(goid.ID)) GameObjectsFactory.Create(goid);
                }
            }

            spawningGlobals = false;
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
            spawningGlobals = true;

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

            spawningGlobals = false;

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
        /// Update Global Game Objects Data
        /****************************************************************************/
        public void UpdateGlobalGameObjectsData(GameObjectInstanceData data)
        {
            if (spawningGlobals) return;

            bool replace = false;
            GameObjectInstanceData toReplace = null;
            
            foreach (var ggod in GlobalGameObjectsData)
            {
                foreach (var god in ggod.Value)
                {
                    if (god.ID == data.ID)
                    {
                        replace   = true;
                        toReplace = god;
                        break;
                    }
                }

                if (replace)
                {
                    ggod.Value.Remove(toReplace);
                    ggod.Value.Add(data);
                    break;
                }
            }
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

            GlobalGameObjectsData.Clear();

            foreach (StringListPair pair in globalGameObjectsData.GameObjectsData)
            {
                GlobalGameObjectsData.Add(pair.String, pair.GameObjects);
            }
        }
        /****************************************************************************/
       
    }
    /********************************************************************************/

}
/************************************************************************************/