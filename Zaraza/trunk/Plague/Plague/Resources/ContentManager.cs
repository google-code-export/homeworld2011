using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using PlagueEngine.Rendering;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.HighLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Resources
/************************************************************************************/ 

namespace PlagueEngine.Resources
{

    /********************************************************************************/
    /// Content Manager
    /********************************************************************************/
    class ContentManager : Microsoft.Xna.Framework.Content.ContentManager
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private List<String>    profiles        = new List<string>();
        private String          currentProfile  = String.Empty;

        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions = new Dictionary<string,GameObjectDefinition>();
        /****************************************************************************/
        

        /****************************************************************************/        
        /// Constants
        /****************************************************************************/        
        private const String defaultProfile     = "Default";
        private const String defaultProfileFile = "DefaultProfile.txt";
        private const String dataDirectory      = "Data";
        private const String objectsDefinitions = "ObjectsDefinitions";
        private const String levelsDirectory    = "Levels";
        private const String textures           = "Textures";
        private const String effects            = "Effects";
        private const String models             = "Models";
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ContentManager(Game game, String rootDirectory) : base(game.Services,rootDirectory)
        {
            game.Content = this;
            DetectProfiles();
            LoadDefaultProfile();
            
            LoadGameObjectsDefinitions();

            //GameObjectDefinition god = new GameObjectDefinition();
            //god.Name = "Rusty Barrel";
            //god.GameObjectClass = "StaticMesh";

            //god.Properties.Add("Model", "Barrel");
            //god.Properties.Add("Diffuse", "Barrel_diffuse");
            //god.Properties.Add("Specular", "Barrel_specular");
            //god.Properties.Add("Normals", "Barrel_normals");

            //god.Properties.Add("InstancingMode", Renderer.InstancingModeToUInt(InstancingModes.DynamicInstancing));

            //gameObjectsDefinitions.Add(god.Name, god);

            //SaveGameObjectsDefinitions();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load
        /****************************************************************************/
        public override T Load<T>(string assetName)
        {
#if DEBUG
            Diagnostics.PushLog("Requesting load " + typeof(T).ToString().Split('.')[typeof(T).ToString().Split('.').Length-1] + ": \t" + RootDirectory + "\\" + assetName);
#endif         
            return base.Load<T>(assetName);                     
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Texture 2D
        /****************************************************************************/
        public Texture2D LoadTexture2D(string textureName)
        {
            if (String.IsNullOrEmpty(textureName)) return null;

            Texture2D result = Load<Texture2D>(textures + '\\' + textureName);
            result.Name = textureName;
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Effect
        /****************************************************************************/
        public Effect LoadEffect(string effectName)
        {
            Effect result = Load<Effect>(effects + '\\' + effectName);
            result.Name = effectName;
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Model
        /****************************************************************************/
        public PlagueEngineModel LoadModel(string modelName)
        {
            PlagueEngineModel result = Load<PlagueEngineModel>(models + '\\' + modelName);
            result.Name = modelName;
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Skinned Model
        /****************************************************************************/
        public PlagueEngineSkinnedModel LoadSkinnedModel(string modelName)
        {
            PlagueEngineSkinnedModel result = Load<PlagueEngineSkinnedModel>(models + '\\' + modelName);
            result.Name = modelName;
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Configuration
        /****************************************************************************/        
        public void SaveConfiguration<T>(T configuration)
        {
#if DEBUG
            Diagnostics.PushLog("Saving Configuration: " + typeof(T).ToString() + ".");
#endif

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = null;

            textWriter = new StreamWriter( "Profiles\\" + 
                                           (currentProfile.Length > 0 ? currentProfile : defaultProfile) + 
                                           "\\" + typeof(T).ToString() + ".xml");

            serializer.Serialize(textWriter, configuration);
            textWriter.Close();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Configuration       
        /****************************************************************************/
        public T LoadConfiguration<T>()
        {
#if DEBUG
            Diagnostics.PushLog("Loading Configuration: " + typeof(T).ToString() + ".");
#endif
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader textReader = null;

            try
            {
                textReader = new StreamReader("Profiles\\" +
                               (currentProfile.Length > 0 ? currentProfile : defaultProfile) +
                               "\\" + typeof(T).ToString() + ".xml");
            }
            catch (System.IO.IOException e) 
            {
#if DEBUG
                Diagnostics.PushLog("Loading Configuration: " + typeof(T).ToString() + " failed.");
#endif
                throw e;
            }

            T configuration = (T)serializer.Deserialize(textReader);
            textReader.Close();

            return configuration;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Detect Profiles
        /****************************************************************************/
        private void DetectProfiles()
        {
            if (Directory.GetDirectories(".", "Profiles").Length == 0)
            {
                DirectoryInfo directory = Directory.CreateDirectory("Profiles");
                directory.CreateSubdirectory(defaultProfile);
            }
            else if (Directory.GetDirectories("Profiles").Length != 0)
            {
                foreach (String directory in Directory.GetDirectories("Profiles"))
                {
                    if (directory.CompareTo(defaultProfile) != 0) profiles.Add(directory.Split('\\')[1]);
                }
            }
            else
            {
                Directory.CreateDirectory("Profiles\\" + defaultProfile);
            }

            String str = String.Empty;
            foreach (String profile in profiles) str += profile + "; ";
            Diagnostics.PushLog("Decteted Profiles: " + str);        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Level
        /****************************************************************************/
        public LevelData LoadLevel(String levelName)
        {
#if DEBUG
            Diagnostics.PushLog("Loading level: " + levelName + ".");
#endif
            Stream stream = new FileStream(dataDirectory + "\\" + levelsDirectory + "\\" + levelName, System.IO.FileMode.Open);

            IFormatter formatter = new BinaryFormatter();
            LevelData levelData  = (LevelData)formatter.Deserialize(stream);

            stream.Close();

            return levelData;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Level
        /****************************************************************************/
        public void SaveLevel(String levelName,LevelData levelData)
        {
#if DEBUG
            Diagnostics.PushLog("Saving level: " + levelName + ".");
#endif
            Stream stream = new FileStream(dataDirectory + "\\" + levelsDirectory + "\\" + levelName, System.IO.FileMode.Create);

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, levelData);

            stream.Close();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// LoadGlobalGameObjectsData
        /****************************************************************************/
        public GlobalGameObjectsData LoadGlobalGameObjectsData()
        {
            Stream stream = new FileStream(dataDirectory + "\\globals.dat", System.IO.FileMode.Open);

            IFormatter formatter = new BinaryFormatter();
            GlobalGameObjectsData globalGameObjectsData = (GlobalGameObjectsData)formatter.Deserialize(stream);

            stream.Close();

            return globalGameObjectsData;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// SaveGlobalGameObjectsData
        /****************************************************************************/
        public void SaveGlobalGameObjectsData(GlobalGameObjectsData globalGameObjectsData)
        {
            Stream stream = new FileStream(dataDirectory + "\\globals.dat", System.IO.FileMode.Create);

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, globalGameObjectsData);

            stream.Close();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Profiles
        /****************************************************************************/
        public String[] Profiles
        {
            get
            {
                return profiles.ToArray();
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Profile
        /****************************************************************************/
        public bool CreateProfile(String name)
        {
            if (profiles.Contains(name) || name.CompareTo(defaultProfile) == 0) return false;
#if DEBUG
            Diagnostics.PushLog("Creating new profile: " + name + ".");
#endif
            Directory.CreateDirectory("Profiles\\" + name);
            profiles.Add(name);
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Delete Profile
        /****************************************************************************/
        public void DeleteProfile(String name)
        {
            if (!profiles.Contains(name)) return;
#if DEBUG
            Diagnostics.PushLog("Deleting profile: " + name + ".");
#endif
            Directory.Delete("Profiles\\" + name, true);
            profiles.Remove(name);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Current Profile
        /****************************************************************************/
        public String CurrentProfile
        {
            get
            {
                return currentProfile;
            }

            set
            {
                if (value.CompareTo(defaultProfile) != 0 && profiles.Contains(value)) currentProfile = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Default Profile
        /****************************************************************************/
        private void LoadDefaultProfile()
        {
#if DEBUG
            Diagnostics.PushLog("Loading default profile.");
#endif

            TextReader textReader = null;
            try
            {
                 textReader = new StreamReader(defaultProfileFile);
            }
            catch (System.IO.IOException)
            {    
#if DEBUG
                Diagnostics.PushLog("File: " + defaultProfileFile + " not found.");
#endif
                return;
            }
            
            String profile = textReader.ReadLine();
            textReader.Close();

            if (profiles.Contains(profile)) currentProfile = profile;
#if DEBUG
            Diagnostics.PushLog("Profile : " + currentProfile);    
#endif
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Default Profile
        /****************************************************************************/
        public void SaveDefaultProfile()
        {
            if (currentProfile.Length == 0) return;
#if DEBUG
            Diagnostics.PushLog("Saving default profile: " + currentProfile);
#endif
            TextWriter textWriter = new StreamWriter(defaultProfileFile);
            textWriter.WriteLine(currentProfile);
            textWriter.Close();            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Game Objects Definitions
        /****************************************************************************/
        public void LoadGameObjectsDefinitions()
        {
//            List<GameObjectDefinitionData> temp = new List<GameObjectDefinitionData>();

//            //XmlSerializer serializer = new XmlSerializer(typeof(List<GameObjectDefinitionData>));
//            //TextReader textReader = null;

//            Stream stream = new FileStream(dataDirectory + "\\" + objectsDefinitions + ".def", System.IO.FileMode.Open);

//            IFormatter formatter = new BinaryFormatter();
//            temp = (List<GameObjectDefinitionData>)formatter.Deserialize(stream);

//            stream.Close();

////            try
////            {
////                textReader = new StreamReader(dataDirectory + "\\" + objectsDefinitions + ".xml");
////            }
////            catch (System.IO.IOException e)
////            {
////#if DEBUG
////                Diagnostics.PushLog("Loading game objects definitions: " + e.Message);
////#endif
////                throw e;
////            }

////            temp = (List<GameObjectDefinitionData>)serializer.Deserialize(textReader);
////            textReader.Close();

//            gameObjectsDefinitions.Clear();

//            GameObjectDefinition godd;
//            foreach (GameObjectDefinitionData god in temp)
//            {
//                godd = new GameObjectDefinition();
//                godd.Set(god);
//                gameObjectsDefinitions.Add(godd.Name, godd);
//#if DEBUG
//                Diagnostics.PushLog("Loading object definition: " + god.Name + ".");
//#endif
//            }

//            temp.Clear();



        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Game Objects Definitions
        /****************************************************************************/
        public void SaveGameObjectsDefinitions()
        {
            List<GameObjectDefinitionData> temp = new List<GameObjectDefinitionData>();
            
            foreach (GameObjectDefinition god in gameObjectsDefinitions.Values)
            {
                temp.Add(god.GetData());
            }

            //XmlSerializer serializer = new XmlSerializer(typeof(List<GameObjectDefinitionData>));
            //TextWriter textWriter = null;
            //textWriter = new StreamWriter(dataDirectory + "\\" + objectsDefinitions + ".xml");
            //serializer.Serialize(textWriter, temp);
            //textWriter.Close();

            //temp.Clear();

            Stream stream = new FileStream(dataDirectory + "\\" + objectsDefinitions + ".def", System.IO.FileMode.Create);

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, temp);

            stream.Close();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Game Objects Definitions
        /****************************************************************************/
        public Dictionary<String, GameObjectDefinition> GameObjectsDefinitions
        {
            get
            {
                return gameObjectsDefinitions;
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/