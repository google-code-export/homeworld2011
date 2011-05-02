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
using System.Reflection;

using System.Drawing.Imaging;

using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input;
using PlagueEngine.Input.Components;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering;
using PlagueEngine.EventsSystem;
using PlagueEngine;
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
        /// DummySniffer
        /********************************************************************************/
        class DummySniffer : EventsSniffer
        {
            GameObjectEditorWindow editor = null;

            public DummySniffer(GameObjectEditorWindow editor)
            {
                this.editor = editor;
                SubscribeAll();
                SubscribeEvents(typeof(LowLevelGameFlow.GameObjectReleased), typeof(LowLevelGameFlow.GameObjectClicked));
            }

            public override void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
            {
                if (e.GetType().Equals(typeof(LowLevelGameFlow.GameObjectClicked)))
                {
                    editor.ShowGameObjectProperties(  ((LowLevelGameFlow.GameObjectClicked)e).gameObjectID);
                    
                    editor.renderer.debugDrawer.StartSelectiveDrawing(((LowLevelGameFlow.GameObjectClicked)e).gameObjectID);

       
                }

                if (e.GetType().Equals(typeof(LowLevelGameFlow.GameObjectReleased)))
                {
                    editor.renderer.debugDrawer.StopSelectiveDrawing();
                }

            }

        }

        /********************************************************************************/




        /********************************************************************************/
        /// Definition Counter
        /********************************************************************************/
        class DefinitionCounter
        {
            public int count = 0;
            public string levelName = string.Empty;
        }
        /********************************************************************************/


        
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
        private Renderer renderer = null;
        private Input.Input input=null;
        private Game game = null;

        private gameObjectsClassName currentClassName = null;
        private GameObjectInstanceData currentObject = null;

    

        private string levelDirectory = @"Data\levels";
        private string levelExtension = ".lvl";
        private string currentLevelName = string.Empty;
        private Level currentLevel = null;
        private bool levelSaved = true;

        private GameObjectDefinition currentDefinition = null;
        

        //pola do zakladki edytuj
        private GameObjectInstanceData currentEditGameObject = null;


        private DummySniffer sniffer = null;
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public GameObjectEditorWindow(GameObjectsFactory factory,ContentManager contentManager,Renderer renderer,Input.Input input,Game game)
        {
            InitializeComponent();
            FillClassNames();
            this.factory = factory;
            this.contentManager = contentManager;
            this.renderer = renderer;
            this.input = input;
            this.sniffer = new DummySniffer(this);
            this.game = game;


            foreach (var gameObject in gameObjectClassNames)
            {
                gameObjectsName.Items.Add(gameObject.className);
            }


            this.Visible = true;
            this.MaximizeBox = false;


            loadLevelNames();
            LoadAllObjectsId();
            LoadFilters();
            checkBoxShowCollisionSkin.Checked = renderer.debugDrawer.IsEnabled;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// ShowGameObjectProperties
        /********************************************************************************/
        public void ShowGameObjectProperties(uint gameObjectID)
        {
            if (factory.GameObjects.ContainsKey(gameObjectID))
            {

                currentEditGameObject = factory.GameObjects[gameObjectID].GetData();
                currentEditGameObject.Position = currentEditGameObject.World.Translation;
                propertyGrid2.SelectedObject = currentEditGameObject;
                comboBoxFilterId.SelectedItem = "Show all";
                comboboxGameObjectId.SelectedIndex=(int)(gameObjectID-1);
            }

            
        }
        /********************************************************************************/






        /********************************************************************************/
        /// Load Definition For Class
        /********************************************************************************/
        private void LoadDefinitionForClass(string gameObjectClass)
        {
            
           
            ComboboxDefinitions.SelectedIndex = -1;
            ComboboxDefinitions.SelectedIndex = -1;
            ComboboxDefinitions.SelectedText = "";
            ComboboxDefinitions.Items.Clear();
            currentDefinition = null;

            foreach (GameObjectDefinition definition in contentManager.GameObjectsDefinitions.Values)
            {

                if (definition.GameObjectClass == gameObjectClass)
                {

                    ComboboxDefinitions.Items.Add(definition.Name);
                    
                }
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Load Filters
        /********************************************************************************/
        private void LoadFilters()
        {
            comboBoxFilterId.Items.Clear();

            comboBoxFilterId.Items.Add("Show all");

            foreach (gameObjectsClassName className in gameObjectClassNames)
            {
                comboBoxFilterId.Items.Add(className.className);
            }

            comboBoxFilterId.SelectedItem = "Show all";
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
                
                LoadDefinitionForClass(objectname);
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


            gameObjectsClassName Sunlight = new gameObjectsClassName();
            Sunlight.className = "Sunlight";
            Sunlight.ClassType = typeof(Sunlight);
            Sunlight.dataClassType = typeof(SunlightData);
            gameObjectClassNames.Add(Sunlight);

            gameObjectsClassName CylindricalBodyMesh = new gameObjectsClassName();
            CylindricalBodyMesh.className = "CylindricalBodyMesh";
            CylindricalBodyMesh.ClassType = typeof(CylindricalBodyMesh);
            CylindricalBodyMesh.dataClassType = typeof(CylindricalBodyMeshData);
            gameObjectClassNames.Add(CylindricalBodyMesh);


            gameObjectsClassName SquareBodyMesh = new gameObjectsClassName();
            SquareBodyMesh.className = "SquareBodyMesh";
            SquareBodyMesh.ClassType = typeof(SquareBodyMesh);
            SquareBodyMesh.dataClassType = typeof(SquareBodyMeshData);
            gameObjectClassNames.Add(SquareBodyMesh);


            gameObjectsClassName Creature = new gameObjectsClassName();
            Creature.className = "Creature";
            Creature.ClassType = typeof(Creature);
            Creature.dataClassType = typeof(CreatureData);
            gameObjectClassNames.Add(Creature);

            gameObjectsClassName MenuButton = new gameObjectsClassName();
            MenuButton.className = "MenuButton";
            MenuButton.ClassType = typeof(MenuButton);
            MenuButton.dataClassType = typeof(MenuButtonData);
            gameObjectClassNames.Add(MenuButton);

            gameObjectsClassName BackgroundTerrain = new gameObjectsClassName();
            BackgroundTerrain.className = "BackgroundTerrain";
            BackgroundTerrain.ClassType = typeof(BackgroundTerrain);
            BackgroundTerrain.dataClassType = typeof(BackgroundTerrainData);
            gameObjectClassNames.Add(BackgroundTerrain);

            gameObjectsClassName WaterSurface = new gameObjectsClassName();
            WaterSurface.className = "WaterSurface";
            WaterSurface.ClassType = typeof(WaterSurface);
            WaterSurface.dataClassType = typeof(WaterSurfaceData);
            gameObjectClassNames.Add(WaterSurface);

            gameObjectsClassName PointLight = new gameObjectsClassName();
            PointLight.className = "PointLight";
            PointLight.ClassType = typeof(PointLight);
            PointLight.dataClassType = typeof(PointLightData);
            gameObjectClassNames.Add(PointLight);

            gameObjectsClassName GlowStick = new gameObjectsClassName();
            GlowStick.className = "GlowStick";
            GlowStick.ClassType = typeof(GlowStick);
            GlowStick.dataClassType = typeof(GlowStickData);
            gameObjectClassNames.Add(GlowStick);

            gameObjectsClassName SpotLight = new gameObjectsClassName();
            SpotLight.className = "SpotLight";
            SpotLight.ClassType = typeof(SpotLight);
            SpotLight.dataClassType = typeof(SpotLightData);
            gameObjectClassNames.Add(SpotLight);
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
                currentEditGameObject=this.factory.Create(currentObject).GetData();
                propertyGrid2.SelectedObject = currentEditGameObject;
             
                

                levelSaved = false;

              
                comboBoxFilterId.SelectedItem = "Show all";
                LoadFilteredID(null, null);
                comboboxGameObjectId.SelectedIndex = comboboxGameObjectId.Items.Count - 1;
                
               
            }
            catch(Exception execption)
            {
                MessageBox.Show("That makes 100 errors \nPlease try again.\n\n"+execption.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (currentDefinition != null)
                {
                    PropertyInfo[] propINFO = currentClassName.dataClassType.GetProperties();

                    foreach (PropertyInfo pI in propINFO)
                    {
                        if (currentDefinition.Properties.ContainsKey(pI.Name))
                        {
                            
                            pI.SetValue(this.currentObject,null,null);
                        }
                    }
                }

                




                currentDefinition = contentManager.GameObjectsDefinitions[ComboboxDefinitions.SelectedItem.ToString()];
                currentObject.definition = ComboboxDefinitions.SelectedItem.ToString();

                PropertyInfo[] propInfo = currentClassName.dataClassType.GetProperties();

                foreach (PropertyInfo pf in propInfo)
                {
                    if (currentDefinition.Properties.ContainsKey(pf.Name))
                    {
                        pf.SetValue(this.currentObject, currentDefinition.Properties[pf.Name], null);
                    }
                }

                propertyGrid1.Refresh();
            }
        }
        /********************************************************************************/


        /********************************************************************************/
        /// Load Level Names
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
        /********************************************************************************/




        /********************************************************************************/
        /// Load objects id
        /********************************************************************************/
        private void LoadAllObjectsId()
        {
            comboboxGameObjectId.SelectedIndex = -1;
            comboboxGameObjectId.SelectedIndex = -1;
            comboboxGameObjectId.Items.Clear();

            foreach (GameObjectInstance gameObject in factory.GameObjects.Values)
            {
                comboboxGameObjectId.Items.Add(gameObject.ID.ToString());
            }


        }
        /********************************************************************************/





        /********************************************************************************/
        /// Set Level
        /********************************************************************************/
        public void setLevel(Level level,string levelName)
        {
            this.currentLevel = level;
            this.currentLevelName = levelName;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Load Click
        /********************************************************************************/
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (listBoxLevelNames.SelectedIndex != -1)
            {
                if (this.currentLevelName != listBoxLevelNames.SelectedItem.ToString())
                {
                    this.currentLevelName = listBoxLevelNames.SelectedItem.ToString();

                    try
                    {
                        
                        this.currentLevel.LoadLevel(contentManager.LoadLevel(this.currentLevelName));
                        LoadFilteredID(null,null);
                        renderer.debugDrawer.DisableHeightmapDrawing();
                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Incompatibility or other black magic! :<\n\n"+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }
                

            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button New Click
        /********************************************************************************/
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
                renderer.debugDrawer.DisableHeightmapDrawing();

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

                } while ((newName == false) && (box2.canceled == false));

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

            LoadFilteredID(null,null);

         }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Save Click
        /********************************************************************************/
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (currentLevelName != string.Empty && currentLevel != null)
            {
                contentManager.SaveLevel(currentLevelName, currentLevel.SaveLevel());
                levelSaved = true;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Delete Click
        /********************************************************************************/
        private void buttonDelete_Click(object sender, EventArgs e)
        {


            if (listBoxLevelNames.SelectedIndex == -1)
            {
                if (currentLevelName == string.Empty || currentLevel == null) return;

                DialogResult result = MessageBox.Show("Delete currrent level: " + currentLevelName + " ?", "Deleting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\" + levelDirectory + "\\" + currentLevelName);
                    currentLevel.Clear();
                    currentLevelName = string.Empty;
                }
            }
            else
            {
                string filename = listBoxLevelNames.SelectedItem.ToString();

                DialogResult result = MessageBox.Show("Delete level: " + filename + " ?", "Deleting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    File.Delete(Directory.GetCurrentDirectory()+ "\\" + levelDirectory +"\\"+ filename);
                    if (filename == currentLevelName)
                        currentLevel.Clear();

                    listBoxLevelNames.Items.Remove(filename);

                }
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Form Closing
        /********************************************************************************/
        private void GameObjectEditorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!levelSaved)
            {
                if (currentLevelName != string.Empty && currentLevel != null)
                {
                    DialogResult result = MessageBox.Show("Save current level?", "Level is not saved!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        contentManager.SaveLevel(currentLevelName, currentLevel.SaveLevel());
                        levelSaved = true;
                    }
                }
            }

        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Create Definitin Click
        /********************************************************************************/
        private void buttonCreateDefinition_Click(object sender, EventArgs e)
        {
            if (gameObjectsName.SelectedIndex != -1)
            {


                PropertyInfo[] PropertyInfo = currentClassName.dataClassType.GetProperties();
                List<PropertyInfo> list = PropertyInfo.ToList<PropertyInfo>();
               
                for (int i = 0; i < list.Count; i++)//zagniezdzanie definicji nam chyba nie jest potrzebne
                {

                    if (list[i].Name == "definition" || list[i].Name == "Definition" || list[i].Name == "Yaw" || list[i].Name == "Roll" || list[i].Name == "Pitch" || list[i].Name == "Position" || list[i].Name == "Scale")
                    {
                        list.RemoveAt(i);
                        --i;
                    }
                }

                DefinitionWindow definitionWindow = new DefinitionWindow(list,this.currentObject);
                definitionWindow.ShowDialog();

                if (!definitionWindow.canceled)
                {
                    GameObjectDefinition god = new GameObjectDefinition();
                    god.Name = definitionWindow.textbox.Text;
                    god.GameObjectClass = this.currentClassName.className;

                    foreach(DefinitionWindow.Field field in definitionWindow.fields )
                    {
                        if (field.checkbox.Checked)
                        {
                            god.Properties.Add(field.label.Text, currentClassName.dataClassType.GetProperty(field.label.Text).GetValue(currentObject, null));
                        }
                    }

                    if(contentManager.GameObjectsDefinitions.ContainsKey(god.Name))
                    {
                        DialogResult dr=MessageBox.Show("Definition exists. Override?","",MessageBoxButtons.YesNo);
                        if(dr==DialogResult.Yes)
                        {
                            contentManager.GameObjectsDefinitions.Remove(god.Name);
                            contentManager.GameObjectsDefinitions.Add(god.Name, god);
                            contentManager.SaveGameObjectsDefinitions();
                        }
                        
                    }
                    else
                    {
                    contentManager.GameObjectsDefinitions.Add(god.Name, god);
                    contentManager.SaveGameObjectsDefinitions();


                    ComboboxDefinitions.Items.Add(definitionWindow.textbox.Text);

                    }


                }


                
            }

       
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button Delete Definitin Click
        /********************************************************************************/
        private void buttonDeleteDefinition_Click(object sender, EventArgs e)
        {
            if (ComboboxDefinitions.SelectedIndex != -1 && currentDefinition != null)
            {

                //zliczanie ile obiektow w levelach korzysta z definicji 
                List<DefinitionCounter> definitionCounter = new List<DefinitionCounter>();
                int allDefinitions = 0;


                foreach (string levelName in listBoxLevelNames.Items)
                {
                    LevelData levelData = contentManager.LoadLevel(levelName);

                    DefinitionCounter dc = new DefinitionCounter();
                    dc.levelName = levelName;

                    if (levelName == currentLevelName)
                    {
                        foreach (GameObjectInstance gameObject in factory.GameObjects.Values)
                        {
                            if (gameObject.Definition == ComboboxDefinitions.SelectedItem.ToString())
                            {
                                dc.count++;
                                allDefinitions++;
                            }
                        }
                    }
                    else
                    {
                        foreach (GameObjectInstanceData gameObjectdata in levelData.gameObjects)
                        {
                            if (gameObjectdata.definition == ComboboxDefinitions.SelectedItem.ToString())
                            {
                                dc.count++;
                                allDefinitions++;
                            }
                        }
                    }

                    definitionCounter.Add(dc);

                }


                //wyswietlanie ilosci definicji w levelach
                string messageBoxText = string.Empty;

                for(int i=0;i<definitionCounter.Count;i++)
                {
                    if (definitionCounter[i].count == 0)
                    {
                        definitionCounter.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        messageBoxText += definitionCounter[i].count.ToString() + " objects in " + definitionCounter[i].levelName+"\n";
                    }
                }


                DialogResult dialogResult = new DialogResult();
                if (allDefinitions != 0)
                {
                    dialogResult = MessageBox.Show("Objects using this definition:\n\n" + messageBoxText + "\n\nDelete this definition?\n(Clicking yes can leads to error while loading level data)", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                //usuwanie definicji z comboboxa
                if (dialogResult == DialogResult.Yes || allDefinitions==0)
                {


                    contentManager.GameObjectsDefinitions.Remove(currentDefinition.Name);
                    contentManager.SaveGameObjectsDefinitions();

                    ComboboxDefinitions.Items.Remove(ComboboxDefinitions.SelectedItem);

                    ComboboxDefinitions.SelectedIndex = -1;
                    ComboboxDefinitions.SelectedIndex = -1;
                    ComboboxDefinitions.SelectedText = "";
                }
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// PropertyGid Property Value Changed
        /********************************************************************************/
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if(ComboboxDefinitions.SelectedIndex!=-1)
            {
            
                bool cancelDefinition = false;

                foreach (string propertyName in currentDefinition.Properties.Keys)
                {
                    if (e.ChangedItem.Label == propertyName && currentDefinition.Properties[propertyName] != e.ChangedItem.Value)
                    {
                        cancelDefinition = true;
                    }
                }


                if (cancelDefinition)
                {
                    PropertyInfo propINFO = currentClassName.dataClassType.GetProperty("definition");
                    propINFO.SetValue(this.currentObject, null, null);

                    this.ComboboxDefinitions.SelectedIndex = -1;//2x, tak musi byc
                    this.ComboboxDefinitions.SelectedIndex = -1;
                    currentDefinition = null;
                    propertyGrid1.Refresh();
                    
                }



            }
        }

        /********************************************************************************/




        /********************************************************************************/
        /// combobox GameObjectId Selected Index Changed
        /********************************************************************************/
        private void comboboxGameObjectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboboxGameObjectId.SelectedIndex != -1)
            {
                uint id;
                uint.TryParse(comboboxGameObjectId.SelectedItem.ToString(), out id);

                currentEditGameObject = factory.GameObjects[id].GetData();
                currentEditGameObject.Position = currentEditGameObject.World.Translation;

           
                propertyGrid2.SelectedObject = currentEditGameObject;
            }
            else
            {
                propertyGrid2.SelectedObject = null;
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// propertyGrid2 Property Value Changed
        /********************************************************************************/
        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!checkBoxDisableEditing.Checked)
            {
                


                factory.GameObjects[currentEditGameObject.ID].Dispose();
                factory.GameObjects.Remove(currentEditGameObject.ID);
                factory.Create(currentEditGameObject);
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// button Delete Object Click
        /********************************************************************************/
        private void buttonDeleteObject_Click(object sender, EventArgs e)
        {
            if (comboboxGameObjectId.SelectedIndex != -1)
            {
                factory.GameObjects[currentEditGameObject.ID].Dispose();
                comboboxGameObjectId.SelectedIndex = -1;
                comboboxGameObjectId.SelectedIndex = -1;

                factory.GameObjects.Remove(currentEditGameObject.ID);
                currentEditGameObject = null;
                propertyGrid2.SelectedObject = null;
                
                LoadFilteredID(null,null);
            }

        }

        private void LoadFilteredID(object sender, EventArgs e)
        {
            if (comboBoxFilterId.SelectedItem.ToString() == "Show all")
            {
                LoadAllObjectsId();
            }
            else
            {

                foreach (gameObjectsClassName gameObjectclassName in gameObjectClassNames)
                {
                    if (comboBoxFilterId.SelectedItem.ToString() == gameObjectclassName.className)
                    {
                        comboboxGameObjectId.SelectedIndex = -1;
                        comboboxGameObjectId.SelectedIndex = -1;
                        comboboxGameObjectId.Items.Clear();

                        foreach (GameObjectInstance gameObject in factory.GameObjects.Values)
                        {
                            if (gameObject.GetType() == gameObjectclassName.ClassType)
                            {
                                comboboxGameObjectId.Items.Add(gameObject.ID);
                            }
                        }
                    }
                }
            }
        }

        private void checkBoxDisableEditing_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxDisableEditing.Checked)
            {
              
                factory.GameObjects[currentEditGameObject.ID].Dispose();
                factory.GameObjects.Remove(currentEditGameObject.ID);
                factory.Create(currentEditGameObject);
            }
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e)
        {
           
            factory.GameObjects[currentEditGameObject.ID].Dispose();
            factory.GameObjects.Remove(currentEditGameObject.ID);
            factory.Create(currentEditGameObject);
        }

        private void buttonCreateDefinitionEdit_Click(object sender, EventArgs e)
        {
            if (currentEditGameObject != null)
            {
                PropertyInfo[] PropertyInfo = currentEditGameObject.GetType().GetProperties();
                List<PropertyInfo> list = PropertyInfo.ToList<PropertyInfo>();

                for (int i = 0; i < list.Count; i++)//zagniezdzanie definicji nam chyba nie jest potrzebne
                {

                    if (list[i].Name == "definition" || list[i].Name == "Definition" || list[i].Name == "Yaw" || list[i].Name == "Roll" || list[i].Name == "Pitch" || list[i].Name == "Position" || list[i].Name == "Scale")
                    {
                        list.RemoveAt(i);
                        --i;
                    }
                }

                DefinitionWindow definitionWindow = new DefinitionWindow(list, this.currentEditGameObject);
                definitionWindow.ShowDialog();

                if (!definitionWindow.canceled)
                {
                    GameObjectDefinition god = new GameObjectDefinition();
                    god.Name = definitionWindow.textbox.Text;
                    god.GameObjectClass = this.currentClassName.className;

                    foreach (DefinitionWindow.Field field in definitionWindow.fields)
                    {
                        if (field.checkbox.Checked)
                        {
                            god.Properties.Add(field.label.Text, currentClassName.dataClassType.GetProperty(field.label.Text).GetValue(currentObject, null));
                        }
                    }
                    contentManager.GameObjectsDefinitions.Add(god.Name, god);
                    contentManager.SaveGameObjectsDefinitions();


                    ComboboxDefinitions.Items.Add(definitionWindow.textbox.Text);


                }

            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {


                        LevelNameMessageBox box = new LevelNameMessageBox("Level name:");
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

        private void propertyGrid1_PropertyValueChanged_1(object s, PropertyValueChangedEventArgs e)
        {
            if (currentDefinition != null)
            {
                bool changed = false;
                foreach (String name in contentManager.GameObjectsDefinitions.Keys)
                {
                    if (name == currentDefinition.Name)
                    {

                        PropertyInfo[] pf = currentObject.GetType().GetProperties();
                        foreach (PropertyInfo Property in pf)
                        {
                            if (contentManager.GameObjectsDefinitions[name].Properties.ContainsKey(Property.Name))
                            {
                                if (currentObject.GetType().GetProperty(Property.Name).GetValue(currentObject, null).ToString() != contentManager.GameObjectsDefinitions[name].Properties[Property.Name].ToString())
                                {
                                    changed = true;

                                }
                            }
                        }
                    }
                }
                if (changed)
                {
                    currentObject.GetType().GetProperty("definition").SetValue(currentObject, null, null);
                    propertyGrid1.Refresh();
                }
            }
        }




        private void buttonCommitMeshTransforms_Click(object sender, EventArgs e)
        {
            renderer.batchedMeshes.CommitMeshTransforms();
        }





        private void checkBoxShowCollisionSkin_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowCollisionSkin.Checked)
            {
                renderer.debugDrawer.Enable();
            }
            else
            {
                renderer.debugDrawer.Disable();
            }
        }




        private void GameObjectEditorWindow_Activated(object sender, EventArgs e)
        {
            input.enabled = false;
        }

        private void GameObjectEditorWindow_Deactivate(object sender, EventArgs e)
        {
            input.enabled = true;
        }

        private void checkBoxGamePaused_CheckedChanged(object sender, EventArgs e)
        {
            this.game.gameStopped = checkBoxGamePaused.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            

            if (checkBox1.Checked)
            {
                foreach (GameObjectInstance gameObject in factory.GameObjects.Values)
                {
                    if (gameObject.GetType().Equals(typeof(Terrain)))
                    {
                        renderer.debugDrawer.EnableHeightmapDrawing();
                    }
                }
            }
            else
            {
                renderer.debugDrawer.DisableHeightmapDrawing();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            renderer.ssaoEnabled = checkBox2.Checked;
        }






        /********************************************************************************/

    


    }
    /********************************************************************************/


}
/********************************************************************************/
