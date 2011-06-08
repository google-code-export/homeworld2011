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
        float boxLength;
        float boxWidth;
        float boxHeight;
        int numberOfBoxesInLength;
        int numberOfBoxesInWidth;
        float distanceBeetwenBoxes = 0.01f;
        Vector3 boxStartPosition;
        GameObjectsFactory factory;
        int frameToTest;
        public bool[,] collisionValues;
        GameObjectInstance[,] boxes;
        /****************************************************************************/

        
        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(float boxLength,
        float boxWidth,
        float boxHeight,
        int numberOfBoxesInLength,
        int numberOfBoxesInWidth,
        float distanceBeetwenBoxes,
        GameObjectsFactory factory,
         Vector3 boxStartPosition,
            int framesToTest)
        {
            this.boxLength =boxLength;
            this.boxWidth= boxWidth;
            this.boxHeight =boxHeight;
            this.numberOfBoxesInLength =numberOfBoxesInLength;
            this.numberOfBoxesInWidth= numberOfBoxesInWidth;
            this.distanceBeetwenBoxes = distanceBeetwenBoxes;
            this.factory = factory;
            this.boxStartPosition = boxStartPosition;
            if (framesToTest < 3)
            {
                this.frameToTest = 3;
            }
            else
            {
                this.frameToTest = framesToTest;
            }
            CheckerBox.checker = this;

            collisionValues = new bool[numberOfBoxesInLength, numberOfBoxesInWidth];
            boxes = new GameObjectInstance[numberOfBoxesInLength, numberOfBoxesInWidth];

            for (int i = 0; i < numberOfBoxesInLength; i++)
            {
                for (int j = 0; j < numberOfBoxesInWidth; j++)
                {
                    CheckerBoxData dddtata = new CheckerBoxData();
                    dddtata.Type = (typeof(CheckerBox));
                    dddtata.DynamicRoughness = 0.5f;
                    dddtata.Elasticity = 0.5f;
                    dddtata.StaticRoughness = 0.5f;
                    dddtata.Mass = 1;
                    dddtata.Immovable = true;
                    dddtata.EnabledPhysics = true;

                    dddtata.Lenght = boxLength;
                    dddtata.Width = boxWidth;
                    dddtata.Height = boxHeight;
                    Vector3 move=boxStartPosition;
                    move.X+=(boxWidth+distanceBeetwenBoxes)*i;
                    move.Z+=(boxWidth+distanceBeetwenBoxes)*j;
                    dddtata.World = Matrix.CreateTranslation(move);
                    dddtata.posX = i;
                    dddtata.posY = j;
                    boxes[i,j]=factory.Create(dddtata);
                }
            }

            TimeControl.CreateFrameCounter(this.frameToTest, 0, destroyBoxes);
        }
        /****************************************************************************/

        public void destroyBoxes()
        {
            writeData();

            for (int i = 0; i < numberOfBoxesInLength; i++)
            {
                for (int j = 0; j < numberOfBoxesInWidth; j++)
                {

                    if (boxes[i, j] != null)
                    {
                        factory.RemoveGameObject(boxes[i, j].ID);
                    }
                }
            }

            this.SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }


        private void writeData()
        {
            using (XmlWriter writer = XmlWriter.Create("pathFinderData.xml"))
            {
                writer.WriteStartDocument();



                writer.WriteStartElement("PathfinderData");

                writer.WriteStartElement("Info");
                writer.WriteElementString("NodesCount", (numberOfBoxesInLength * numberOfBoxesInWidth).ToString());
                writer.WriteElementString("BoxLength", boxLength.ToString());
                writer.WriteElementString("BoxWidth", boxWidth.ToString());
                writer.WriteElementString("BoxHeight", boxHeight.ToString());
                writer.WriteElementString("DistanceBeetwenBoxes", distanceBeetwenBoxes.ToString());
                writer.WriteElementString("StartPositionXYZ", boxStartPosition.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Boxes");
                for (int i = 0; i < numberOfBoxesInLength; i++)
                {
                    for (int j = 0; j < numberOfBoxesInWidth; j++)
                    {

                        writer.WriteStartElement("Box");

                        writer.WriteElementString("i", i.ToString());
                        writer.WriteElementString("j", j.ToString());
                        writer.WriteElementString("XYZ", boxes[i, j].World.Translation.ToString());
                        writer.WriteElementString("Collision", collisionValues[i,j].ToString());
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
             


                writer.WriteEndDocument();

            }

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

            data.boxHeight = boxHeight;
            data.boxLength = boxLength;
            data.boxWidth = boxWidth;
            data.distanceBeetwenBoxes = distanceBeetwenBoxes;
            data.numberOfBoxesInLength = numberOfBoxesInLength;
            data.numberOfBoxesInWidth = numberOfBoxesInWidth;
            data.boxStartPosition = boxStartPosition;
            data.framesToTest = frameToTest;
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