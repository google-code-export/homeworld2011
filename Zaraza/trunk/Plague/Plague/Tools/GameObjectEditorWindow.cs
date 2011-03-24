﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;
using PlagueEngine.HighLevelGameFlow;


/********************************************************************************/
/// PlagueEngine.Tools
/********************************************************************************/
namespace PlagueEngine.Tools
{

    /********************************************************************************/
    /// Game Object Editor Window
    /********************************************************************************/
    partial class GameObjectEditorWindow : Form
    {



        /********************************************************************************/
        /// game Objects Class Name
        /********************************************************************************/
        public class gameObjectsClassName
        {
            public string className;
            public Type ClassType;
            public Type dataClassType;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        private List<gameObjectsClassName> gameObjectClassNames = new List<gameObjectsClassName>();
        private ContentManager contentManager = null;
        private GameObjectsFactory factory = null;
        private gameObjectsClassName currentClassName = null;
        private GameObjectInstanceData currentObject = null;

        private string levelDirectory = @"Data\levels";
        private string levelExtension = ".lvl";
        private string currentLevelName = string.Empty;
        private Level currentLevel = null;
        private bool levelSaved = true;
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public GameObjectEditorWindow(GameObjectsFactory factory,ContentManager contentManager)
        {
            InitializeComponent();
            FillClassNames();
            this.factory = factory;
            this.contentManager = contentManager;


            foreach (var gameObject in gameObjectClassNames)
            {
                gameObjectsName.Items.Add(gameObject.className);
            }

            foreach(var definition in contentManager.GameObjectsDefinitions.Keys)
            {
                ComboboxDefinitions.Items.Add(definition);
            }
            this.Visible = true;


            loadLevelNames();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Fill Names
        /********************************************************************************/
        private void FillNames(object sender, EventArgs e)
        {
            if (gameObjectsName.SelectedIndex != -1)
            {
                string objectname = gameObjectsName.Items[gameObjectsName.SelectedIndex].ToString();

                currentClassName = getClass(objectname);
                currentObject = (GameObjectInstanceData)(Activator.CreateInstance(currentClassName.dataClassType));

                propertyGrid1.SelectedObject = currentObject;
            }
        }
        /********************************************************************************/



        /********************************************************************************/
        /// Fill Class Names
        /********************************************************************************/
        public void FillClassNames()
        {

            gameObjectsClassName linkedCamera = new gameObjectsClassName();
            linkedCamera.className = "LinkedCamera";
            linkedCamera.ClassType = typeof(LinkedCamera);
            linkedCamera.dataClassType = typeof(LinkedCameraData);
            gameObjectClassNames.Add(linkedCamera);

            gameObjectsClassName freeCamera = new gameObjectsClassName();
            freeCamera.className = "FreeCamera";
            freeCamera.ClassType = typeof(FreeCamera);
            freeCamera.dataClassType = typeof(FreeCameraData);
            gameObjectClassNames.Add(freeCamera);

            gameObjectsClassName staticMesh = new gameObjectsClassName();
            staticMesh.className = "StaticMesh";
            staticMesh.ClassType = typeof(StaticMesh);
            staticMesh.dataClassType = typeof(StaticMeshData);
            gameObjectClassNames.Add(staticMesh);

            gameObjectsClassName terrain = new gameObjectsClassName();
            terrain.className = "Terrain";
            terrain.ClassType = typeof(Terrain);
            terrain.dataClassType = typeof(TerrainData);
            gameObjectClassNames.Add(terrain);
        }
        /********************************************************************************/




        /********************************************************************************/
        /// get Class
        /********************************************************************************/
        public gameObjectsClassName getClass(string name)
        {


            foreach (var gameobject in gameObjectClassNames)
            {
                if (name == gameobject.className)
                {
                    return gameobject;
                }
            }
            return null;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// button1_Click //klikniecie create
        /********************************************************************************/
        private void button1_Click(object sender, EventArgs e)
        {
            

            try
            {
                this.currentObject.Type = currentClassName.ClassType;
                this.factory.Create(currentObject);
                this.propertyGrid1.SelectedObject = null;

                this.ComboboxDefinitions.SelectedIndex = -1;//2x, tak musi byc
                this.ComboboxDefinitions.SelectedIndex = -1;
                this.gameObjectsName.SelectedIndex = -1;
                this.gameObjectsName.SelectedIndex = -1;

                levelSaved = false;

            }
            catch(Exception execption)
            {
                MessageBox.Show("That makes 100 errors \nPlease try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        /********************************************************************************/



        /********************************************************************************/
        /// ComboboxDefinitions_SelectedIndexChanged //wybranie definicji z comboboxa
        /********************************************************************************/
        private void ComboboxDefinitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxDefinitions.SelectedIndex != -1)
            {
                currentObject.definition = ComboboxDefinitions.Items[ComboboxDefinitions.SelectedIndex].ToString();
                propertyGrid1.Refresh();
            }
        }
        /********************************************************************************/

        private void loadLevelNames()
        {
            listBoxLevelNames.Items.Clear();
           
            DirectoryInfo di = new DirectoryInfo(levelDirectory);
            FileInfo[] fileNames = di.GetFiles("*"+levelExtension);

            foreach (FileInfo fileInfo in fileNames)
            {
                listBoxLevelNames.Items.Add(fileInfo.Name);
            }

        }


        public void setLevel(Level level,string levelName)
        {
            this.currentLevel = level;
            this.currentLevelName = levelName;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (listBoxLevelNames.SelectedIndex != -1)
            {
                if (this.currentLevelName != listBoxLevelNames.SelectedItem.ToString())
                {
                    this.currentLevelName = listBoxLevelNames.SelectedItem.ToString();


                    this.currentLevel.LoadLevel(contentManager.LoadLevel(this.currentLevelName));
                }
                

            }
        }

           

        private void buttonNew_Click(object sender, EventArgs e)
        {
            bool NewCanceled = false;

            if (!levelSaved)
            {
                DialogResult result = MessageBox.Show("Save level?", "Notification", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (currentLevelName == string.Empty)
                    {
                        LevelNameMessageBox box = new LevelNameMessageBox("Old level name:");
                        box.ShowDialog();

                        if (!box.canceled)
                        {

                            Regex reg = new Regex(@""+levelExtension+"$");
                            if(!reg.IsMatch(box.levelName))
                            {
                                currentLevelName = box.levelName + levelExtension;
                            }
                            else
                            {
                                currentLevelName = box.levelName;
                            }
                            
                            contentManager.SaveLevel( currentLevelName, currentLevel.SaveLevel());
                            listBoxLevelNames.Items.Add(currentLevelName);
                        }
                    }
                    else
                    {
                        contentManager.SaveLevel(currentLevelName, currentLevel.SaveLevel());
                    }
                }

                if (result == DialogResult.No)
                {
                    currentLevelName = string.Empty;
                }

                if (result == DialogResult.Cancel)
                {
                    NewCanceled = true;
                }
            }



            if (!NewCanceled)
            {
                
                LevelNameMessageBox box2 = new LevelNameMessageBox("New level name:");
                bool newName;
                do
                {
                    newName = true;
                    box2.ShowDialog();

                    foreach (string name in listBoxLevelNames.Items)
                    {
                        if ((name == box2.levelName) || (name == (box2.levelName + levelExtension)))
                        {
                            newName = false;
                            MessageBox.Show("Name already exists!", "Error", MessageBoxButtons.OK);
                        }
                    }

                } while ((newName == false) || (box2.canceled == true));

                if (!box2.canceled)
                {

                    Regex reg2 = new Regex(@"" + levelExtension + "$");
                    if(reg2.IsMatch(box2.levelName))
                    {
                        currentLevelName=box2.levelName;
                    
                    }
                    else
                    {
                        currentLevelName = box2.levelName + levelExtension;
                    }

                    listBoxLevelNames.Items.Add(currentLevelName);
                    currentLevel.Clear();
                    contentManager.SaveLevel(currentLevelName, currentLevel.SaveLevel());
                    levelSaved = true;

                }
            }

         }
    


    }
    /********************************************************************************/


}
/********************************************************************************/
