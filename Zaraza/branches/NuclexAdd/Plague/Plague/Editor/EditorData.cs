using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.HighLevelGameFlow;

namespace PlagueEngine.Editor
{
    class EditorData
    {
        private static string GAME_OBJECT_NAMESPACE = "PlagueEngine.LowLevelGameFlow.GameObjects";
        private static string LEVEL_DIRECTIORY = @"Data\Levels";
        private static string LEVEL_EXTENSION = ".lvl";
        private List<GameObjectClassName> _gameObjectClassNames = new List<GameObjectClassName>();
        private List<FileInfo> _levels = new List<FileInfo>();
        private Game _game;
        private ContentManager _contentManager;
        private Renderer _renderer;
        private Level _level;
        private Input.Input _input;

        public Level Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public Input.Input Input
        {
            get { return _input; }
            set { _input = value; }
        }
        public Renderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }
        public ContentManager ContentManager
        {
            get { return _contentManager; }
            set { _contentManager = value; }
        }
        public Game Game
        {
            get { return _game; }
            set { _game = value; }
        }
        public List<FileInfo> Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        public List<GameObjectClassName> GameObjectClassNames
        {
            get { return _gameObjectClassNames; }
            set { _gameObjectClassNames = value; }
        }

        public EditorData(Game game)
        {
            _game = game;
            _input = game.Input;
            _level = game.Level;
            _renderer = game.Renderer;
            _contentManager = game.ContentManager;
            FillClassNames();
            FillLevelNames();
        }
        public void FillClassNames()
        {
            _gameObjectClassNames.Clear();
            Assembly assembly = Assembly.GetExecutingAssembly();
            var gameObjectClassName = new List<Type>();
            var gameObjectDataClass = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (!String.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.Equals(GAME_OBJECT_NAMESPACE))
                {
                    if (type.Name.EndsWith("Data"))
                    {
                        gameObjectDataClass.Add(type);
                    }
                    else
                    {
                        gameObjectClassName.Add(type);
                    }
                } 
            }
            foreach (Type type in gameObjectClassName)
            {
                foreach (Type dataType in gameObjectDataClass)
                {
                    if (dataType.Name.Equals(type.Name + "Data"))
                    {
                        _gameObjectClassNames.Add(new GameObjectClassName(type.Name, type, dataType));
                        break;
                    }
                }
            }
        }

        public GameObjectClassName GetClass(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return null;
            foreach (var gameobject in _gameObjectClassNames)
            {
                if (name.Equals(gameobject.ClassName))
                {
                    return gameobject;
                }
            }
            return null;
        }

        public void FillLevelNames()
        {
            _levels.Clear();

            DirectoryInfo dir = new DirectoryInfo(LEVEL_DIRECTIORY);
            if (dir.Exists)
            {
                FileInfo[] fileNames = dir.GetFiles("*" + LEVEL_EXTENSION);
                foreach (FileInfo fileInfo in fileNames)
                {
                    _levels.Add(fileInfo);
                }
            }

        }
    }
}
