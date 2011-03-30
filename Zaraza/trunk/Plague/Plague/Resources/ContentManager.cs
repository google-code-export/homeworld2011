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

            //LoadGameObjectsDefinitions();


            GameObjectDefinition god = new GameObjectDefinition();
            god.Name = "Rusty Barrel";
            god.GameObjectClass = "StaticMesh";
            god.Properties.Add("Model",     "Barrel");
            god.Properties.Add("Diffuse",   "Barrel_diffuse");
            god.Properties.Add("Specular",  "Barrel_specular");
            god.Properties.Add("Normals",   "Barrel_normals");
            god.Properties.Add("InstancingMode", Renderer.InstancingModeToUInt(InstancingModes.DynamicInstancing));
            gameObjectsDefinitions.Add(god.Name, god);


            SaveGameObjectsDefinitions();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load
        /****************************************************************************/
        public override T Load<T>(string assetName)
        {
            Diagnostics.PushLog("Requesting load " + typeof(T).ToString().Split('.')[typeof(T).ToString().Split('.').Length-1] + ": \t" + RootDirectory + "\\" + assetName);                     
            return base.Load<T>(assetName);                     
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Texture 2D
        /****************************************************************************/
        public Texture2D LoadTexture2D(string textureName)
        {
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
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Configuration
        /****************************************************************************/        
        public void SaveConfiguration<T>(T configuration)
        {
            Diagnostics.PushLog("Saving Configuration: " + typeof(T).ToString() + ".");

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
            Diagnostics.PushLog("Loading Configuration: " + typeof(T).ToString() + ".");

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
                Diagnostics.PushLog("Loading Configuration: " + typeof(T).ToString() + " failed.");
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
            Diagnostics.PushLog("Loading level: " + levelName + ".");
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
            Diagnostics.PushLog("Saving level: " + levelName + ".");
            Stream stream = new FileStream(dataDirectory + "\\" + levelsDirectory + "\\" + levelName, System.IO.FileMode.Create);

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, levelData);

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
            Diagnostics.PushLog("Creating new profile: " + name + ".");
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
            Diagnostics.PushLog("Deleting profile: " + name + ".");
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
            Diagnostics.PushLog("Loading default profile.");

            TextReader textReader = null;
            try
            {
                 textReader = new StreamReader(defaultProfileFile);
            }
            catch (System.IO.IOException)
            {                
                Diagnostics.PushLog("File: " + defaultProfileFile + " not found.");
                return;
            }
            
            String profile = textReader.ReadLine();
            textReader.Close();

            if (profiles.Contains(profile)) currentProfile = profile;
            Diagnostics.PushLog("Profile : " + currentProfile);    
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Default Profile
        /****************************************************************************/
        public void SaveDefaultProfile()
        {
            if (currentProfile.Length == 0) return;

            Diagnostics.PushLog("Saving default profile: " + currentProfile);
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
            List<GameObjectDefinition> temp = new List<GameObjectDefinition>();
            
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameObjectDefinition>));
            TextReader textReader = null;

            try
            {
                textReader = new StreamReader(dataDirectory + "\\" + objectsDefinitions + ".xml");
            }
            catch (System.IO.IOException e)
            {
                Diagnostics.PushLog("Loading game objects definitions: " + e.Message);
                throw e;
            }

            temp = (List<GameObjectDefinition>)serializer.Deserialize(textReader);
            textReader.Close();

            gameObjectsDefinitions.Clear();

            foreach (GameObjectDefinition god in temp)
            {
                gameObjectsDefinitions.Add(god.Name, god);
                Diagnostics.PushLog("Loading object definition: " + god.Name + ".");
            }

            temp.Clear();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Save Game Objects Definitions
        /****************************************************************************/
        public void SaveGameObjectsDefinitions()
        {
            List<GameObjectDefinition> temp = new List<GameObjectDefinition>();
            
            foreach (GameObjectDefinition god in gameObjectsDefinitions.Values)
            {
                temp.Add(god);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<GameObjectDefinition>));
            TextWriter textWriter = null;
            textWriter = new StreamWriter(dataDirectory + "\\" + objectsDefinitions + ".xml");
            serializer.Serialize(textWriter, temp);
            textWriter.Close();

            temp.Clear();
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