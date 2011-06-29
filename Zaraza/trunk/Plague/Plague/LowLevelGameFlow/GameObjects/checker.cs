using System;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using PlagueEngine.TimeControlSystem;
using System.IO;
using PlagueEngine.Pathfinder;
using System.Runtime.Serialization.Formatters.Binary;

/************************************************************************************/
// Plague.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    // Checker
    /********************************************************************************/
    class Checker : GameObjectInstance
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        private PathfinderManager _pm;
        private string _levelName;
        private int _frameToTest;
        private CheckerBox[,] _boxes;
        private const string Path = "Data\\Levels\\";
        private const string Extension = "pf";
        private const string LevelExtension = "lvl";
        private string _fileName;
        /****************************************************************************/

        
        /****************************************************************************/
        // Initialization
        /****************************************************************************/
        public void Init(string levelName,
        float boxWidth,
        float boxHeight,
        int numberOfBoxesInLength,
        int numberOfBoxesInWidth,
        float distanceBeetwenBoxes,
        GameObjectsFactory factory,
         Vector3 boxStartPosition,
            int framesToTest)
        {

            if (String.IsNullOrWhiteSpace(levelName) || !File.Exists(Path + levelName + "." + LevelExtension))
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.ERROR,
                                    !File.Exists(Path + levelName + "." + LevelExtension)
                                        ? "There is not such level Name as given: "+levelName
                                        : "The name of level was not given.");

                Diagnostics.PushLog(LoggingLevel.ERROR, "Checker will be destroyed.");
#endif
                factory.RemoveGameObject(ID);
                SendEvent(new DestroyObjectEvent(ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                return;
            }
            _fileName = Path + levelName + "." + Extension;
            _pm = new PathfinderManager
                      {
                          BoxHeight = boxHeight,
                          BoxWidth = boxWidth,
                          DistanceBeetwenBoxes = (Math.Abs(distanceBeetwenBoxes - 0) < Math.E ? 0.01f : distanceBeetwenBoxes),
                          NumberOfBoxesInLength = numberOfBoxesInLength,
                          NumberOfBoxesInWidth = numberOfBoxesInWidth,
                          BoxStartPosition = boxStartPosition
                      };
            _levelName = levelName;
            Factory = factory;
            _frameToTest =framesToTest < 5? 5:framesToTest;
            _boxes = new CheckerBox[numberOfBoxesInLength, numberOfBoxesInWidth];
            if (!File.Exists(_fileName))
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, "There is no PathFinder file for this Level. It will be generated right now. This could take a while.");
#endif
                Generate();
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, "PathFinder file was successfully created.");
#endif
                PathfinderManager.Pm = _pm;
                _pm.Factory = Factory;
                return;
            }
            Stream stream = File.Open(_fileName, FileMode.Open);
            var bFormatter = new BinaryFormatter();
            PathfinderManager.Pm = (PathfinderManager)bFormatter.Deserialize(stream);
            stream.Close();
            PathfinderManager.Pm.Factory = Factory;
            //GenerateBoxes();
            TimeControl.CreateFrameCounter(10, 0, GenerateFild);

        }
        /****************************************************************************/

        public void GenerateFild()
        {
            
            PathfinderManager.Pm.GenerateBox(PathfinderManager.Pm.GetNode(PathfinderManager.Pm.BoxStartPosition));
            PathfinderManager.Pm.GenerateBox(PathfinderManager.Pm.GetNode(PathfinderManager.Pm.BoxStartPosition + new Vector3(PathfinderManager.Pm.BoxWidth * PathfinderManager.Pm.NumberOfBoxesInWidth,0,0)));
            PathfinderManager.Pm.GenerateBox(PathfinderManager.Pm.GetNode(PathfinderManager.Pm.BoxStartPosition + new Vector3(0,0,PathfinderManager.Pm.BoxWidth * PathfinderManager.Pm.NumberOfBoxesInLength)));
            PathfinderManager.Pm.GenerateBox(PathfinderManager.Pm.GetNode(PathfinderManager.Pm.BoxStartPosition + new Vector3(PathfinderManager.Pm.BoxWidth * PathfinderManager.Pm.NumberOfBoxesInWidth, 0, PathfinderManager.Pm.BoxWidth * PathfinderManager.Pm.NumberOfBoxesInLength)));
        }
        public void GenerateBoxes()
        {
            foreach (var n in PathfinderManager.Pm.BlockedNodes)
            {
                PathfinderManager.Pm.GenerateBox(n);
            }
        }
        public void DestroyBoxes()
        {
            for (var i = 0; i < _pm.NumberOfBoxesInLength; i++)
            {
                for (var j = 0; j < _pm.NumberOfBoxesInWidth; j++)
                {
                    if (_boxes[i, j] == null) continue;
                    if (_boxes[i, j].isCollision)
                    {
                        _pm.BlockedNodes.Add(new Node(i, j, _boxes[i, j].nodeType));
                    }
                    Factory.RemoveGameObject(_boxes[i, j].ID);
                }
            }
            WriteData();
            //_factory.RemoveGameObject(ID);
            //this.SendEvent(new DestroyObjectEvent(ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }


        private void WriteData()
        {
            if (_pm == null) return;
            Stream stream = File.Open(_fileName, FileMode.Create);
            var bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, _pm);
            stream.Close();
        }

        private void Generate()
        {
            var dddtata = new CheckerBoxData
                              {
                                  Type = (typeof (CheckerBox)),
                                  DynamicRoughness = 0.5f,
                                  Elasticity = 0.5f,
                                  StaticRoughness = 0.5f,
                                  Mass = 1,
                                  Immovable = true,
                                  EnabledPhysics = true,
                                  Width = _pm.BoxWidth,
                                  Height = _pm.BoxHeight
                              };
            for (var i = 0; i < _pm.NumberOfBoxesInLength; i++)
            {
                for (var j = 0; j < _pm.NumberOfBoxesInWidth; j++)
                {
                    var move = _pm.BoxStartPosition;
                    move.X += (_pm.BoxSpace) * i;
                    move.Z += (_pm.BoxSpace) * j;
                    dddtata.World = Matrix.CreateTranslation(move);
                    dddtata.posX = i;
                    dddtata.posY = j;
                    _boxes[i, j] = (CheckerBox)Factory.Create(dddtata);
                }
            }
            TimeControl.CreateFrameCounter(_frameToTest, 0, DestroyBoxes);
        }
        /****************************************************************************/
        // Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {

        }
        /****************************************************************************/


        /****************************************************************************/
        // Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new CheckerData();
            GetData(data);
            data.LevelName = _levelName;
            data.BoxHeight = _pm.BoxHeight;
            data.BoxWidth = _pm.BoxWidth;
            data.DistanceBeetwenBoxes = _pm.DistanceBeetwenBoxes;
            data.NumberOfBoxesInLength = _pm.NumberOfBoxesInLength;
            data.NumberOfBoxesInWidth = _pm.NumberOfBoxesInWidth;
            data.BoxStartPosition = _pm.BoxStartPosition;
            data.FramesToTest = _frameToTest;
            return data;
        }
        /****************************************************************************/


    }
    /********************************************************************************/



    /********************************************************************************/
    // StaticMeshData
    /********************************************************************************/
    [Serializable]
    public class CheckerData : GameObjectInstanceData
    {
        [CategoryAttribute("Checker properties")]
        public string LevelName { get; set; }
        [CategoryAttribute("Checker properties")]
        public float BoxWidth { get; set; }
        [CategoryAttribute("Checker properties")]
        public float BoxHeight { get; set; }
        [CategoryAttribute("Checker properties")]
        public int NumberOfBoxesInLength { get; set; }
        [CategoryAttribute("Checker properties")]
        public int NumberOfBoxesInWidth { get; set; }
        [CategoryAttribute("Checker properties")]
        public float DistanceBeetwenBoxes { get; set; }
        [CategoryAttribute("Checker properties")]
        public Vector3 BoxStartPosition { get; set; }
        [CategoryAttribute("Checker properties")]
        public int FramesToTest { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/