using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.EventsSystem;

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
                if (this.editor.Visible == true)
                {
                    if (e.GetType().Equals(typeof(LowLevelGameFlow.GameObjectClicked)))
                    {
                        editor.ShowGameObjectProperties(((LowLevelGameFlow.GameObjectClicked)e).gameObjectID);

                        editor.renderer.debugDrawer.StartSelectiveDrawing(((LowLevelGameFlow.GameObjectClicked)e).gameObjectID);


                    }
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
        private Renderer renderer = null;
        private Input.Input input=null;
        private Game game = null;

        private gameObjectsClassName currentClassName = null;
        private GameObjectInstanceData currentObject = null;

    

        private string levelDirectory = @"Data\levels";
        private string levelExtension = ".lvl";
        private Level  level = null;
        private bool levelSaved = true;

        private GameObjectDefinition currentDefinition = null;
        

        //pola do zakladki edytuj
        private GameObjectInstanceData currentEditGameObject = null;


        private DummySniffer sniffer = null;
        private bool releaseInput = true;
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public GameObjectEditorWindow(Level level,ContentManager contentManager,Renderer renderer,Input.Input input, Game game)
        {
            InitializeComponent();
            FillClassNames();
            this.level = level;
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
            checkBoxShowCollisionSkin.Checked = renderer.debugDrawer.IsEnabled;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// ShowGameObjectProperties
        /********************************************************************************/
        public void ShowGameObjectProperties(int gameObjectID)
        {
            if (level.GameObjects.ContainsKey(gameObjectID))
            {
                currentEditGameObject = level.GameObjects[gameObjectID].GetData();
                currentEditGameObject.Position = currentEditGameObject.World.Translation;
                propertyGrid2.SelectedObject = currentEditGameObject;
                TreeNode[] tn = treeView1.Nodes.Find(gameObjectID.ToString(), true);
                if (tn.GetLength(0) != 0)
                {
                    treeView1.SelectedNode = tn[0];
                }
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

            gameObjectsClassName CylindricalBodyMesh2 = new gameObjectsClassName();
            CylindricalBodyMesh2.className = "CylindricalBodyMesh2";
            CylindricalBodyMesh2.ClassType = typeof(CylindricalBodyMesh2);
            CylindricalBodyMesh2.dataClassType = typeof(CylindricalBodyMeshData2);
            gameObjectClassNames.Add(CylindricalBodyMesh2);


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

            gameObjectsClassName BurningBarrel = new gameObjectsClassName();
            BurningBarrel.className = "BurningBarrel";
            BurningBarrel.ClassType = typeof(BurningBarrel);
            BurningBarrel.dataClassType = typeof(BurningBarrelData);
            gameObjectClassNames.Add(BurningBarrel);

            gameObjectsClassName Flashlight = new gameObjectsClassName();
            Flashlight.className = "Flashlight";
            Flashlight.ClassType = typeof(Flashlight);
            Flashlight.dataClassType = typeof(FlashlightData);
            gameObjectClassNames.Add(Flashlight);
            
            gameObjectsClassName Mercenary = new gameObjectsClassName();
            Mercenary.className = "Mercenary";
            Mercenary.ClassType = typeof(Mercenary);
            Mercenary.dataClassType = typeof(MercenaryData);
            gameObjectClassNames.Add(Mercenary);

            gameObjectsClassName Label = new gameObjectsClassName();
            Label.className = "Label";
            Label.ClassType = typeof(PlagueEngine.LowLevelGameFlow.GameObjects.Label);
            Label.dataClassType = typeof(LabelData);
            gameObjectClassNames.Add(Label);

            gameObjectsClassName Input = new gameObjectsClassName();
            Input.className = "Input";
            Input.ClassType = typeof(PlagueEngine.LowLevelGameFlow.GameObjects.Input);
            Input.dataClassType = typeof(InputData);
            gameObjectClassNames.Add(Input);

            gameObjectsClassName Panel = new gameObjectsClassName();
            Panel.className = "Panel";
            Panel.ClassType = typeof(PlagueEngine.LowLevelGameFlow.GameObjects.Panel);
            Panel.dataClassType = typeof(PanelData);
            gameObjectClassNames.Add(Panel);

            gameObjectsClassName Firearm = new gameObjectsClassName();
            Firearm.className = "Firearm";
            Firearm.ClassType = typeof(Firearm);
            Firearm.dataClassType = typeof(FirearmData);
            gameObjectClassNames.Add(Firearm);


            gameObjectsClassName CylindricalSkinMesh = new gameObjectsClassName();
            CylindricalSkinMesh.className = "CylindricalSkinMesh";
            CylindricalSkinMesh.ClassType = typeof(CylindricalSkinMesh);
            CylindricalSkinMesh.dataClassType = typeof(CylindricalSkinMeshData);
            gameObjectClassNames.Add(CylindricalSkinMesh);

            gameObjectsClassName GameController = new gameObjectsClassName();
            GameController.className = "GameController";
            GameController.ClassType = typeof(GameController);
            GameController.dataClassType = typeof(GameControllerData);
            gameObjectClassNames.Add(GameController);



            gameObjectsClassName MercenariesManager = new gameObjectsClassName();
            MercenariesManager.className = "MercenariesManager";
            MercenariesManager.ClassType = typeof(MercenariesManager);
            MercenariesManager.dataClassType = typeof(MercenariesManagerData);
            gameObjectClassNames.Add(MercenariesManager);



            gameObjectsClassName SphericalBodyMesh = new gameObjectsClassName();
            SphericalBodyMesh.className = "SphericalBodyMesh";
            SphericalBodyMesh.ClassType = typeof(SphericalBodyMesh);
            SphericalBodyMesh.dataClassType = typeof(SphericalBodyMeshData);
            gameObjectClassNames.Add(SphericalBodyMesh);


            gameObjectsClassName SphericalSkinMesh = new gameObjectsClassName();
            SphericalSkinMesh.className = "SphericalSkinMesh";
            SphericalSkinMesh.ClassType = typeof(SphericalSkinMesh);
            SphericalSkinMesh.dataClassType = typeof(SphericalSkinMeshData);
            gameObjectClassNames.Add(SphericalSkinMesh);


            gameObjectsClassName SquareSkinMesh = new gameObjectsClassName();
            SquareSkinMesh.className = "SquareSkinMesh";
            SquareSkinMesh.ClassType = typeof(SquareSkinMesh);
            SquareSkinMesh.dataClassType = typeof(SquareSkinMeshData);
            gameObjectClassNames.Add(SquareSkinMesh);



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
                currentEditGameObject=this.level.GameObjectsFactory.Create(currentObject).GetData();
                propertyGrid2.SelectedObject = currentEditGameObject;


                TreeNode[] tn = treeView1.Nodes.Find(currentEditGameObject.Type.Name, false);
                if (tn.GetLength(0) != 0)
                {
                    tn[0].Nodes.Add(currentEditGameObject.ID.ToString());
                }
                


                levelSaved = false;

              
                
               
            }
            catch(Exception execption)
            {
                releaseInput = false;
                MessageBox.Show("That makes 100 errors \nPlease try again.\n\n"+execption.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                releaseInput = true;
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
                currentObject.Definition = ComboboxDefinitions.SelectedItem.ToString();

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
            treeView1.Nodes.Clear();

            foreach (gameObjectsClassName name in gameObjectClassNames)
            {
                treeView1.Nodes.Add(name.className, name.className);
            }





            foreach (GameObjectInstance gameObject in level.GameObjects.Values)
            {
                TreeNode[] tn = treeView1.Nodes.Find(gameObject.GetType().Name, false);
                if (tn.GetLength(0) != 0)
                {
                    tn[0].Nodes.Add(gameObject.ID.ToString());
                }
                


            }


        }
        /********************************************************************************/







        /********************************************************************************/
        /// Button Load Click
        /********************************************************************************/
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (listBoxLevelNames.SelectedIndex != -1)
            {
                if (level.CurrentLevel != listBoxLevelNames.SelectedItem.ToString())
                {
                    String currentLevelName = listBoxLevelNames.SelectedItem.ToString();

                    //try
                    //{
                        
                        level.LoadLevel(currentLevelName);
                        LoadAllObjectsId();
                        renderer.debugDrawer.DisableHeightmapDrawing();
                       
                    //}
                    //catch (Exception ex)
                    //{
                    //    releaseInput = false;
                    //    MessageBox.Show("Incompatibility or other black magic! :<\n\n"+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    releaseInput = true;
                    //}
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
            string lvlName;

            if (!levelSaved)
            {
                releaseInput = false;
                DialogResult result = MessageBox.Show("Save level?", "Notification", MessageBoxButtons.YesNoCancel);
                releaseInput = true;
                if (result == DialogResult.Yes)
                {
                    if (level.CurrentLevel == string.Empty)
                    {
                        releaseInput = false;
                        var box = new LevelNameMessageBox("Old level name:");
                        box.Activated += new EventHandler(GameObjectEditorWindow_Activated);
                        box.ShowDialog();
                        releaseInput = true;
                        if (!box.canceled)
                        {

                            Regex reg = new Regex(@""+levelExtension+"$");
                            if(!reg.IsMatch(box.levelName))
                            {
                                lvlName = box.levelName + levelExtension;
                            }
                            else
                            {
                                lvlName = box.levelName;
                            }

                            level.SaveLevel(lvlName);
                            listBoxLevelNames.Items.Add(lvlName);
                        }
                    }
                    else
                    {
                        level.SaveLevel();
                    }
                }

                //if (result == DialogResult.No)
                //{
                //    currentLevelName = string.Empty;
                //}

                if (result == DialogResult.Cancel)
                {
                    NewCanceled = true;
                }
            }



            if (!NewCanceled)
            {
                renderer.debugDrawer.DisableHeightmapDrawing();
                
                var box2 = new LevelNameMessageBox("New level name:");
                box2.Activated += new EventHandler(GameObjectEditorWindow_Activated);
                bool newName;
                do
                {
                    newName = true;
                    releaseInput = false;
                    box2.ShowDialog();
                    releaseInput = true;
                    foreach (string name in listBoxLevelNames.Items)
                    {
                        if ((name == box2.levelName) || (name == (box2.levelName + levelExtension)))
                        {
                            newName = false;
                            releaseInput = false;
                            MessageBox.Show("Name already exists!", "Error", MessageBoxButtons.OK);
                            releaseInput = true;
                        }
                    }

                } while ((newName == false) && (box2.canceled == false));

                if (!box2.canceled)
                {

                    Regex reg2 = new Regex(@"" + levelExtension + "$");
                    if(reg2.IsMatch(box2.levelName))
                    {
                        lvlName=box2.levelName;
                    
                    }
                    else
                    {
                        lvlName = box2.levelName + levelExtension;
                    }

                    listBoxLevelNames.Items.Add(lvlName);
                    level.Clear(true);
                    level.SaveLevel(lvlName);
                    levelSaved = true;

                }
            }


         }

        /********************************************************************************/


        /********************************************************************************/
        /// Button Save Click
        /********************************************************************************/
        private void buttonSave_Click(object sender, EventArgs e)
        {

                level.SaveLevel();
                levelSaved = true;

        }
        /********************************************************************************/

        
        /********************************************************************************/
        /// Button Delete Click
        /********************************************************************************/
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            

            if (listBoxLevelNames.SelectedIndex == -1)
            {
                if (level.CurrentLevel == string.Empty || level.CurrentLevel == null) return;
                releaseInput = false;
                DialogResult result = MessageBox.Show("Delete currrent level: " + level.CurrentLevel + " ?", "Deleting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                releaseInput = true;
                if (result == DialogResult.Yes)
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\" + levelDirectory + "\\" + level.CurrentLevel);
                    level.Clear(true);
                }
            }
            else
            {
                string filename = listBoxLevelNames.SelectedItem.ToString();
                releaseInput = false;
                DialogResult result = MessageBox.Show("Delete level: " + filename + " ?", "Deleting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                releaseInput = true;
                if (result == DialogResult.Yes)
                {
                    File.Delete(Directory.GetCurrentDirectory()+ "\\" + levelDirectory +"\\"+ filename);
                    if (filename == level.CurrentLevel)
                        level.Clear(true);

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
                releaseInput = false;
                DialogResult result = MessageBox.Show("Save current level?", "Level is not saved!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                releaseInput = true;
                if (result == DialogResult.Yes)
                {
                    level.SaveLevel();
                    levelSaved = true;
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
                releaseInput = false;
                DefinitionWindow definitionWindow = new DefinitionWindow(list,this.currentObject);
                definitionWindow.ShowDialog();
                releaseInput = true;
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
                        releaseInput = false;
                        DialogResult dr=MessageBox.Show("Definition exists. Override?","",MessageBoxButtons.YesNo);
                        releaseInput = true;
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

                    if (levelName == level.CurrentLevel)
                    {
                        foreach (GameObjectInstance gameObject in level.GameObjects.Values)
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
                            if (gameObjectdata.Definition == ComboboxDefinitions.SelectedItem.ToString())
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
                    releaseInput = false;
                    dialogResult = MessageBox.Show("Objects using this definition:\n\n" + messageBoxText + "\n\nDelete this definition?\n(Clicking yes can leads to error while loading level data)", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    releaseInput = true;
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

                    // TODO: wtf ?
                    this.ComboboxDefinitions.SelectedIndex = -1;//2x, tak musi byc
                    this.ComboboxDefinitions.SelectedIndex = -1;
                    currentDefinition = null;
                    propertyGrid1.Refresh();
                    
                }



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



                level.GameObjects[currentEditGameObject.ID].Dispose();

                level.GameObjectsFactory.RemoveGameObject(currentEditGameObject.ID);
                level.GameObjectsFactory.Create(currentEditGameObject);
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// button Delete Object Click
        /********************************************************************************/
        private void buttonDeleteObject_Click(object sender, EventArgs e)
        {

            if (treeView1.SelectedNode.Parent != null)
            {
                int res;
                if (int.TryParse(treeView1.SelectedNode.Text, out res))
                {
                    treeView1.SelectedNode.Remove();
                    level.GameObjectsFactory.RemoveGameObject(res);
                    currentEditGameObject = null;
                    propertyGrid2.SelectedObject = null;
                }

            }
            


        }


        private void checkBoxDisableEditing_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxDisableEditing.Checked)
            {
              
                level.GameObjectsFactory.RemoveGameObject(currentEditGameObject.ID);
                level.GameObjectsFactory.Create(currentEditGameObject);
            }
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e)
        {

            level.GameObjects[currentEditGameObject.ID].Dispose();
            level.GameObjectsFactory.RemoveGameObject(currentEditGameObject.ID);
            level.GameObjectsFactory.Create(currentEditGameObject);
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
                releaseInput = false;
                DefinitionWindow definitionWindow = new DefinitionWindow(list, this.currentEditGameObject);
                definitionWindow.ShowDialog();
                releaseInput = true;
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
                    
                    if (contentManager.GameObjectsDefinitions.ContainsKey(god.Name))
                    {
                        contentManager.GameObjectsDefinitions[god.Name] = god;
                    }                    
                    else
                    {
                        contentManager.GameObjectsDefinitions.Add(god.Name, god);
                        ComboboxDefinitions.Items.Add(definitionWindow.textbox.Text);
                    }

                    contentManager.SaveGameObjectsDefinitions();



                }

            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
                        releaseInput = false;
                        LevelNameMessageBox box = new LevelNameMessageBox("Level name:");
                        box.ShowDialog();
                        releaseInput = true;

                        string lvlName;
                        if (!box.canceled)
                        {

                            Regex reg = new Regex(@""+levelExtension+"$");
                            if(!reg.IsMatch(box.levelName))
                            {
                                lvlName = box.levelName + levelExtension;
                            }
                            else
                            {
                                lvlName = box.levelName;
                            }

                            level.SaveLevel(lvlName);
                            listBoxLevelNames.Items.Add(lvlName);
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
            inputEnable.Checked = input.Enabled;
            input.Enabled = false;
        }

        private void GameObjectEditorWindow_Deactivate(object sender, EventArgs e)
        {
            if (releaseInput)
            {
                input.Enabled = true;
            }
        }

        private void checkBoxGamePaused_CheckedChanged(object sender, EventArgs e)
        {
            this.game.GameStopped = checkBoxGamePaused.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            

            if (checkBox1.Checked)
            {
                foreach (GameObjectInstance gameObject in level.GameObjects.Values)
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

        private void inputEnable_CheckedChanged(object sender, EventArgs e)
        {
            input.Enabled = inputEnable.Checked;
        }



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {

                    int id;
                    int.TryParse(e.Node.Text, out id);

                    currentEditGameObject = level.GameObjects[id].GetData();
                    currentEditGameObject.Position = currentEditGameObject.World.Translation;
                    currentClassName = new gameObjectsClassName();
                    currentClassName.dataClassType = currentEditGameObject.GetType();
                    currentObject = currentEditGameObject;

                    foreach (gameObjectsClassName name in gameObjectClassNames)
                    {
                        if (name.dataClassType == currentClassName.dataClassType)
                        {
                            currentClassName.className = name.className;
                            currentClassName.ClassType = name.ClassType;
                        }
                    }


                    propertyGrid2.SelectedObject = currentEditGameObject;
     

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (propertyGrid2.SelectedObject != null)
            {
                currentEditGameObject = level.GameObjects[currentEditGameObject.ID].GetData();
                currentEditGameObject.Position = currentEditGameObject.World.Translation;
                currentObject = currentEditGameObject;

                propertyGrid2.SelectedObject = currentEditGameObject;
            }

        }

  









        /********************************************************************************/

    


    }
    /********************************************************************************/


}
/********************************************************************************/
