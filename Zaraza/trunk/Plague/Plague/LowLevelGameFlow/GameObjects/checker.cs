using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Xml;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.TimeControlSystem;
using System.Threading;
using System.IO;
using PlagueEngine.Tools;
using System.Windows.Forms;
using PlagueEngine.Pathfinder;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

/************************************************************************************/
/// Plague.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Checker
    /********************************************************************************/
    class Checker : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private PathfinderManager _pm;
        private string _levelName;
        private GameObjectsFactory _factory;
        private int _frameToTest;
        private CheckerBox[,] _boxes;
        private string _path = "Data\\Levels\\";
        private string _extension = "pf";
        private string _levelExtension = "lvl";
        private string _fileName;
        /****************************************************************************/

        
        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(string levelName,float boxLength,
        float boxWidth,
        float boxHeight,
        int numberOfBoxesInLength,
        int numberOfBoxesInWidth,
        float distanceBeetwenBoxes,
        GameObjectsFactory factory,
         Vector3 boxStartPosition,
            int framesToTest)
        {
            
            if (String.IsNullOrWhiteSpace(levelName) || !File.Exists(_path + levelName + "." + _levelExtension))
            {
#if DEBUG
                if (!File.Exists(_path + levelName + "." + _levelExtension))
                {
                    Diagnostics.PushLog(LoggingLevel.ERROR, "Nie znaleziono pliku levelu dla którego ma być generwowany jest pathfinder.");
                }
                else
                {
                    Diagnostics.PushLog(LoggingLevel.ERROR, "Nie podano nazwy levelu dla którego generowny jest pathfinder.");
                }
                
                Diagnostics.PushLog(LoggingLevel.ERROR, "Niszczę utworzoną instancję obiektu Checker.");
#endif
                factory.RemoveGameObject(ID);
                this.SendEvent(new DestroyObjectEvent(ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                return;
            }
            _fileName = _path + levelName + "." + _extension;
            _pm = new PathfinderManager();
            _pm.BoxHeight = boxHeight;
            _pm.BoxLength = boxLength;
            _pm.BoxWidth = boxWidth;
            _pm.DistanceBeetwenBoxes = distanceBeetwenBoxes==0?0.01f:distanceBeetwenBoxes;
            _pm.NumberOfBoxesInLength = numberOfBoxesInLength;
            _pm.NumberOfBoxesInWidth = numberOfBoxesInWidth;
            _pm.BoxStartPosition = boxStartPosition;
            _levelName = levelName;
            _factory = factory;
            _frameToTest =framesToTest < 5? 5:framesToTest;
            _boxes = new CheckerBox[numberOfBoxesInLength, numberOfBoxesInWidth];
            if (!File.Exists(_fileName))
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, "Brak pliku pathfindera dla tego levelu, tworze nowy. Może to zająć dużo czasu");
#endif
                generate();
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, "Utworzono plik pathfindera.");
#endif
            }
            if (File.Exists(_fileName))
            {
                Stream stream = File.Open(_fileName, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                PathfinderManager.PM = (PathfinderManager)bFormatter.Deserialize(stream);
                stream.Close();
            }
            
        }
        /****************************************************************************/
        public void destroyBoxes()
        {
            for (int i = 0; i < _pm.NumberOfBoxesInLength; i++)
            {
                for (int j = 0; j < _pm.NumberOfBoxesInWidth; j++)
                {

                    if (_boxes[i, j] != null)
                    {
                        if (_boxes[i, j].isCollision)
                        {
                            _pm.BlockedNodes.Add(new Node(i, j, _boxes[i, j].nodeType));
                        }
                        _factory.RemoveGameObject(_boxes[i, j].ID);
                    }
                }
            }
            writeData();
            //_factory.RemoveGameObject(ID);
            //this.SendEvent(new DestroyObjectEvent(ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }


        private void writeData()
        {
            if (_pm != null)
            {
                Stream stream = File.Open(_fileName, FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, _pm);
                stream.Close();
            }
        }

        private void generate()
        {
            CheckerBoxData dddtata = new CheckerBoxData();
            dddtata.Type = (typeof(CheckerBox));
            dddtata.DynamicRoughness = 0.5f;
            dddtata.Elasticity = 0.5f;
            dddtata.StaticRoughness = 0.5f;
            dddtata.Mass = 1;
            dddtata.Immovable = true;
            dddtata.EnabledPhysics = true;
            dddtata.Lenght = _pm.BoxLength;
            dddtata.Width = _pm.BoxWidth;
            dddtata.Height = _pm.BoxHeight;
            Vector3 move;
            for (int i = 0; i < _pm.NumberOfBoxesInLength; i++)
            {
                for (int j = 0; j < _pm.NumberOfBoxesInWidth; j++)
                {
                    move = _pm.BoxStartPosition;
                    move.X += (_pm.BoxWidth + _pm.DistanceBeetwenBoxes) * i;
                    move.Z += (_pm.BoxWidth + _pm.DistanceBeetwenBoxes) * j;
                    dddtata.World = Matrix.CreateTranslation(move);
                    dddtata.posX = i;
                    dddtata.posY = j;
                    _boxes[i, j] = (CheckerBox)_factory.Create(dddtata);
                }
            }
            TimeControl.CreateFrameCounter(_frameToTest, 0, destroyBoxes);
        }
        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CheckerData data = new CheckerData();
            GetData(data);
            data.levelName = _levelName;
            data.boxHeight = _pm.BoxHeight;
            data.boxLength = _pm.BoxLength;
            data.boxWidth = _pm.BoxWidth;
            data.distanceBeetwenBoxes = _pm.DistanceBeetwenBoxes;
            data.numberOfBoxesInLength = _pm.NumberOfBoxesInLength;
            data.numberOfBoxesInWidth = _pm.NumberOfBoxesInWidth;
            data.boxStartPosition = _pm.BoxStartPosition;
            data.framesToTest = _frameToTest;
            return data;
        }
        /****************************************************************************/


    }
    /********************************************************************************/



    /********************************************************************************/
    /// StaticMeshData
    /********************************************************************************/
    [Serializable]
    public class CheckerData : GameObjectInstanceData
    {
        [CategoryAttribute("Level name")]
        public string levelName { get; set; }
        [CategoryAttribute("Checker properties")]
        public float boxLength { get; set; }
        [CategoryAttribute("Checker properties")]
        public float boxWidth { get; set; }
        [CategoryAttribute("Checker properties")]
        public float boxHeight { get; set; }
        [CategoryAttribute("Checker properties")]
        public int numberOfBoxesInLength { get; set; }
        [CategoryAttribute("Checker properties")]
        public int numberOfBoxesInWidth { get; set; }
        [CategoryAttribute("Checker properties")]
        public float distanceBeetwenBoxes { get; set; }
        [CategoryAttribute("Checker properties")]
        public Vector3 boxStartPosition { get; set; }
        [CategoryAttribute("Checker properties")]
        public int framesToTest { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/