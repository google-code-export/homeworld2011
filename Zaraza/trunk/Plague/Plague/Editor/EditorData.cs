using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.HighLevelGameFlow;

namespace PlagueEngine.Editor
{
    class EditorData
    {
        private const string GameObjectNamespace = "PlagueEngine.LowLevelGameFlow.GameObjects";
        private const string GameObjectFactory = "GameObjectsFactory";
        private const string LevelDirectiory = @"Data\Levels";
        private const string LevelExtension = ".lvl";

        public Level Level { get; set; }

        public Input.Input Input { get; set; }

        public Renderer Renderer { get; set; }

        public ContentManager ContentManager { get; set; }

        public Game Game { get; set; }

        public List<FileInfo> Levels { get; set; }

        public List<GameObjectClassName> GameObjectClassNames { get; set; }

        public EditorData(Game game)
        {
            GameObjectClassNames = new List<GameObjectClassName>();
            Levels = new List<FileInfo>();
            Game = game;
            Input = game.Input;
            Level = game.Level;
            Renderer = game.Renderer;
            ContentManager = game.ContentManager;
            FillClassNames();
            FillLevelNames();
        }
        public void FillClassNames()
        {
            GameObjectClassNames.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            var gameObjectClassName = new List<Type>();
            var gameObjectDataClass = new List<Type>();
            Type factory = null;
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.Equals(GameObjectFactory))
                {
                    factory = type;
                    continue;
                }
                if (String.IsNullOrWhiteSpace(type.Namespace) || !type.Namespace.Equals(GameObjectNamespace)) continue;
                
                if (type.Name.EndsWith("Data"))
                {
                    gameObjectDataClass.Add(type);
                }
                else
                {
                    gameObjectClassName.Add(type);
                }
            }
            if (factory == null)
            {
#if DEBUG
                Diagnostics.Fatal("There is no class " + GameObjectFactory +" !!!");
#endif
                return;
            }
            foreach (var type in gameObjectClassName)
            {
                foreach (var dataType in gameObjectDataClass)
                {
                    if (!dataType.Name.Equals(type.Name + "Data")) continue;
                    GameObjectClassNames.Add(new GameObjectClassName(type.Name, type, dataType, factory.GetMethod("Create" + type.Name) != null));
                    break;
                }
            }
        }

        public GameObjectClassName GetClass(string name)
        {
            return String.IsNullOrWhiteSpace(name) ? null : GameObjectClassNames.FirstOrDefault(gameobject => name.Equals(gameobject.ClassName));
        }

        public void FillLevelNames()
        {
            Levels.Clear();
            var dir = new DirectoryInfo(LevelDirectiory);
            if (!dir.Exists) return;
            var fileNames = dir.GetFiles("*" + LevelExtension);
            foreach (var fileInfo in fileNames)
            {
                Levels.Add(fileInfo);
            }
        }
    }
}
