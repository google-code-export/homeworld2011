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
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

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
            bool reload = false;

            public DummySniffer(GameObjectEditorWindow editor)
            {
                this.editor = editor;
                SubscribeAll();
                SubscribeEvents(typeof(LowLevelGameFlow.GameObjectReleased), typeof(LowLevelGameFlow.GameObjectClicked),typeof(LowLevelGameFlow.CreateEvent),typeof(LowLevelGameFlow.DestroyEvent));
                TimeControlSystem.TimeControl.CreateTimer(TimeSpan.FromSeconds(1),-1,CheckToReload);
            }


            public void CheckToReload()
            {
                if (reload)
                {
                    editor.LoadAllObjectsId();
                    reload = false;
                }
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
                if (e.GetType().Equals(typeof(LowLevelGameFlow.CreateEvent)) || e.GetType().Equals(typeof(LowLevelGameFlow.DestroyEvent))) 
                {
                    reload = true;
                    
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
        private Input.Input input = null;
        private Game game = null;

        private gameObjectsClassName currentClassNameNew = null;
        private GameObjectInstanceData currentObject = null;



        private string levelDirectory = @"Data\levels";
        private string levelExtension = ".lvl";
        public Level level = null;
        private bool levelSaved = true;

        private GameObjectDefinition currentDefinition = null;


        //pola do zakladki edytuj

        private gameObjectsClassName currentClassNameEdit = null;
        private GameObjectInstanceData currentEditGameObject = null;


        private DummySniffer sniffer = null;
        private bool releaseInput = true;

        LinkedCamera linkedCamera = null;
        FreeCamera freeCamera = null;
        Type cameraType;
        bool linkedFirst = true;
        public bool jiglibxSelection = false;
        public bool jiglibxDrag = false;
        //rysowanie ikonek
        List<GameObjectInstance> icons = new List<GameObjectInstance>();
        public List<iconInfo> iconInfo = new List<iconInfo>();
        Texture2D sunLightIcon;
        Texture2D spotLightIcon;
        Texture2D pointLightIcon;
        Texture2D particleEmitterIcon;
        Texture2D AreaparticleEmitterIcon;
        Texture2D moveIcon;
        Texture2D rotateIcon;
        public bool moveObject = true;
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public GameObjectEditorWindow(Level level, ContentManager contentManager, Renderer renderer, Input.Input input, Game game)
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

            Renderer.editor = this;
            FreeCamera.editor = this;
            setUpCameraButton();
            LoadIconsInfo();

            SSAOSampleRadius.Value = (decimal)renderer.sampleRadius;
            numericUpDown1.Value = (decimal)renderer.distanceScale;
            numericUpDown2.Value = (decimal)renderer.Brightness;
            numericUpDown3.Value = (decimal)renderer.Contrast;

            numericUpDown4.Value = (decimal)renderer.BloomThreshold;

             numericUpDown5.Value = (decimal)renderer.BaseSaturation;

            numericUpDown6.Value = (decimal)renderer.BaseIntensity;
            numericUpDown7.Value = (decimal)renderer.BloomSaturation;
            numericUpDown8.Value = (decimal)renderer.BloomIntensity;
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
                currentClassNameEdit = new gameObjectsClassName();
                currentClassNameEdit.dataClassType = currentEditGameObject.GetType();

                foreach (gameObjectsClassName name in gameObjectClassNames)
                {
                    if (name.dataClassType == currentClassNameEdit.dataClassType)
                    {
                        currentClassNameEdit.className = name.className;
                        currentClassNameEdit.ClassType = name.ClassType;
                    }
                }


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

                currentClassNameNew = getClass(objectname);
                currentObject = (GameObjectInstanceData)(Activator.CreateInstance(currentClassNameNew.dataClassType));


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


            gameObjectsClassName BuildingWithRoof = new gameObjectsClassName();
            BuildingWithRoof.className = "BuildingWithRoof";
            BuildingWithRoof.ClassType = typeof(BuildingWithRoof);
            BuildingWithRoof.dataClassType = typeof(BuildingWithRoofData);
            gameObjectClassNames.Add(BuildingWithRoof);


            gameObjectsClassName Compass = new gameObjectsClassName();
            Compass.className = "Compass";
            Compass.ClassType = typeof(Compass);
            Compass.dataClassType = typeof(CompassData);
            gameObjectClassNames.Add(Compass);


            gameObjectsClassName SquareSkin = new gameObjectsClassName();
            SquareSkin.className = "SquareSkin";
            SquareSkin.ClassType = typeof(SquareSkin);
            SquareSkin.dataClassType = typeof(SquareSkinData);
            gameObjectClassNames.Add(SquareSkin);

            gameObjectsClassName Bulldozer = new gameObjectsClassName();
            Bulldozer.className = "Bulldozer";
            Bulldozer.ClassType = typeof(Bulldozer);
            Bulldozer.dataClassType = typeof(BulldozerData);
            gameObjectClassNames.Add(Bulldozer);


            gameObjectsClassName ParticleEmitter = new gameObjectsClassName();
            ParticleEmitter.className = "ParticleEmitter";
            ParticleEmitter.ClassType = typeof(ParticleEmitter);
            ParticleEmitter.dataClassType = typeof(ParticleEmitterData);
            gameObjectClassNames.Add(ParticleEmitter);

            gameObjectsClassName AreaParticleEmitter = new gameObjectsClassName();
            AreaParticleEmitter.className = "AreaParticleEmitter";
            AreaParticleEmitter.ClassType = typeof(AreaParticleEmitter);
            AreaParticleEmitter.dataClassType = typeof(AreaParticleEmitterData);
            gameObjectClassNames.Add(AreaParticleEmitter);

            gameObjectsClassName Container = new gameObjectsClassName();
            Container.className = "Container";
            Container.ClassType = typeof(Container);
            Container.dataClassType = typeof(ContainerData);
            gameObjectClassNames.Add(Container);

            gameObjectsClassName AmmoClip = new gameObjectsClassName();
            AmmoClip.className = "AmmoClip";
            AmmoClip.ClassType = typeof(AmmoClip);
            AmmoClip.dataClassType = typeof(AmmoClipData);
            gameObjectClassNames.Add(AmmoClip);

            gameObjectsClassName AmmoBox = new gameObjectsClassName();
            AmmoBox.className = "AmmoBox";
            AmmoBox.ClassType = typeof(AmmoBox);
            AmmoBox.dataClassType = typeof(AmmoBoxData);
            gameObjectClassNames.Add(AmmoBox);

            gameObjectsClassName Ammunition = new gameObjectsClassName();
            Ammunition.className = "Ammunition";
            Ammunition.ClassType = typeof(Ammunition);
            Ammunition.dataClassType = typeof(AmmunitionData);
            gameObjectClassNames.Add(Ammunition);

            gameObjectsClassName Accessory = new gameObjectsClassName();
            Accessory.className = "Accessory";
            Accessory.ClassType = typeof(Accessory);
            Accessory.dataClassType = typeof(AccessoryData);
            gameObjectClassNames.Add(Accessory);

            gameObjectsClassName Checker = new gameObjectsClassName();
            Checker.className = "Checker";
            Checker.ClassType = typeof(Checker);
            Checker.dataClassType = typeof(CheckerData);
            gameObjectClassNames.Add(Checker);


            gameObjectsClassName MainMenu = new gameObjectsClassName();
            MainMenu.className = "MainMenu";
            MainMenu.ClassType = typeof(PlagueEngine.LowLevelGameFlow.GameObjects.MainMenu);
            MainMenu.dataClassType = typeof(MainMenuData);
            gameObjectClassNames.Add(MainMenu);

            gameObjectsClassName FlammableBarrel = new gameObjectsClassName();
            FlammableBarrel.className = "FlammableBarrel";
            FlammableBarrel.ClassType = typeof(FlammableBarrel);
            FlammableBarrel.dataClassType = typeof(FlammableBarrelData);
            gameObjectClassNames.Add(FlammableBarrel);


            gameObjectsClassName Fire = new gameObjectsClassName();
            Fire.className = "Fire";
            Fire.ClassType = typeof(Fire);
            Fire.dataClassType = typeof(FireData);
            gameObjectClassNames.Add(Fire);



            gameObjectsClassName DialogMessagesManager = new gameObjectsClassName();
            DialogMessagesManager.className = "DialogMessagesManager";
            DialogMessagesManager.ClassType = typeof(DialogMessagesManager);
            DialogMessagesManager.dataClassType = typeof(DialogMessagesManagerData);
            gameObjectClassNames.Add(DialogMessagesManager);



            gameObjectsClassName Bloom = new gameObjectsClassName();
            Bloom.className = "Bloom";
            Bloom.ClassType = typeof(Bloom);
            Bloom.dataClassType = typeof(BloomData);
            gameObjectClassNames.Add(Bloom);

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
            // Kto to kurwa cały czas zakomentowywuje !!!
            try
            {
                this.currentObject.Type = currentClassNameNew.ClassType;
                currentEditGameObject = this.level.GameObjectsFactory.Create(currentObject).GetData();
                propertyGrid2.SelectedObject = currentEditGameObject;


                TreeNode[] tn = treeView1.Nodes.Find(currentEditGameObject.Type.Name, false);
                if (tn.GetLength(0) != 0)
                {
                    tn[0].Nodes.Add(currentEditGameObject.ID.ToString());
                }



                levelSaved = false;




            }
            catch (Exception execption)
            {
                releaseInput = false;
                MessageBox.Show("That makes 100 errors \nPlease try again.\n\n" + execption.Message + "\n\n\n" + execption.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                releaseInput = true;
            }

            LoadIconsInfo();
            setUpCameraButton();
        }
        /********************************************************************************/



        /********************************************************************************/
        /// ComboboxDefinitions_SelectedIndexChanged //wybranie definicji z comboboxa
        /********************************************************************************/
        private void ComboboxDefinitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (ComboboxDefinitions.SelectedIndex != -1 && currentObject!=null && propertyGrid1.SelectedObject!=null)
            {
                if (currentDefinition != null)
                {
                    PropertyInfo[] propINFO = currentClassNameNew.dataClassType.GetProperties();

                    foreach (PropertyInfo pI in propINFO)
                    {
                        if (currentDefinition.Properties.ContainsKey(pI.Name))
                        {

                            pI.SetValue(this.currentObject, null, null);
                        }
                    }
                }






                currentDefinition = contentManager.GameObjectsDefinitions[ComboboxDefinitions.SelectedItem.ToString()];
                currentObject.Definition = ComboboxDefinitions.SelectedItem.ToString();

                PropertyInfo[] propInfo = currentClassNameNew.dataClassType.GetProperties();

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
            FileInfo[] fileNames = di.GetFiles("*" + levelExtension);

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

            treeView1.Sort();
        }
        /********************************************************************************/







        /********************************************************************************/
        /// Button Load Click
        /********************************************************************************/
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            RestoreCamera();

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

            LoadIconsInfo();
            LoadAllObjectsId();

            setUpCameraButton();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Button New Click
        /********************************************************************************/
        private void buttonNew_Click(object sender, EventArgs e)
        {
            RestoreCamera();

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

                            Regex reg = new Regex(@"" + levelExtension + "$");
                            if (!reg.IsMatch(box.levelName))
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
                    if (reg2.IsMatch(box2.levelName))
                    {
                        lvlName = box2.levelName;

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

            LoadAllObjectsId();
            LoadIconsInfo();
        }

        /********************************************************************************/


        /********************************************************************************/
        /// Button Save Click
        /********************************************************************************/
        private void buttonSave_Click(object sender, EventArgs e)
        {
            RestoreCamera();

            level.SaveLevel();
            levelSaved = true;

            setUpCameraButton();

        }
        /********************************************************************************/


        /********************************************************************************/
        /// Button Delete Click
        /********************************************************************************/
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            RestoreCamera();

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
                    File.Delete(Directory.GetCurrentDirectory() + "\\" + levelDirectory + "\\" + filename);
                    if (filename == level.CurrentLevel)
                        level.Clear(true);

                    listBoxLevelNames.Items.Remove(filename);

                }
            }


            LoadIconsInfo();
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


                PropertyInfo[] PropertyInfo = currentClassNameNew.dataClassType.GetProperties();
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
                DefinitionWindow definitionWindow = new DefinitionWindow(list, this.currentObject);
                definitionWindow.ShowDialog();
                releaseInput = true;
                if (!definitionWindow.canceled)
                {
                    GameObjectDefinition god = new GameObjectDefinition();
                    god.Name = definitionWindow.textbox.Text;
                    god.GameObjectClass = this.currentClassNameNew.className;

                    foreach (DefinitionWindow.Field field in definitionWindow.fields)
                    {
                        if (field.checkbox.Checked)
                        {
                            god.Properties.Add(field.label.Text, currentClassNameNew.dataClassType.GetProperty(field.label.Text).GetValue(currentObject, null));
                        }
                    }

                    if (contentManager.GameObjectsDefinitions.ContainsKey(god.Name))
                    {
                        releaseInput = false;
                        DialogResult dr = MessageBox.Show("Definition exists. Override?", "", MessageBoxButtons.YesNo);
                        releaseInput = true;
                        if (dr == DialogResult.Yes)
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

                for (int i = 0; i < definitionCounter.Count; i++)
                {
                    if (definitionCounter[i].count == 0)
                    {
                        definitionCounter.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        messageBoxText += definitionCounter[i].count.ToString() + " objects in " + definitionCounter[i].levelName + "\n";
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
                if (dialogResult == DialogResult.Yes || allDefinitions == 0)
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
            if (ComboboxDefinitions.SelectedIndex != -1)
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
                    PropertyInfo propINFO = currentClassNameNew.dataClassType.GetProperty("definition");
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
        /// propertyGrid2 Property Value Changed
        /********************************************************************************/
        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!checkBoxDisableEditing.Checked)
            {
                level.GameObjectsFactory.RemoveGameObject(currentEditGameObject.ID);
                level.GameObjectsFactory.Create(currentEditGameObject);
                LoadIconsInfo();
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// button Delete Object Click
        /********************************************************************************/
        private void buttonDeleteObject_Click(object sender, EventArgs e)
        {

            if (propertyGrid2.SelectedObject != null)
            {


                //foreach (TreeNode node1 in treeView1.Nodes)
                //{

                //    foreach (TreeNode node2 in node1.Nodes)
                //    {
                //        if (node2 != null)
                //        {
                //            if (node2.Text.Equals(((GameObjectInstanceData)(propertyGrid2.SelectedObject)).ID.ToString()))
                //            {
                //                node2.Remove();

                //            }
                //        }
                //    }
                //}


                level.GameObjectsFactory.RemoveGameObject(((GameObjectInstanceData)(propertyGrid2.SelectedObject)).ID);
                currentEditGameObject = null;
                propertyGrid2.SelectedObject = null;
                LoadAllObjectsId();
            }

            LoadIconsInfo();
            setUpCameraButton();
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

            level.GameObjectsFactory.RemoveGameObject(currentEditGameObject.ID);
            level.GameObjectsFactory.Create(currentEditGameObject);
        }

        private void buttonCreateDefinitionEdit_Click(object sender, EventArgs e)
        {
            if (currentEditGameObject != null && currentClassNameEdit != null)
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
                    god.GameObjectClass = this.currentClassNameEdit.className;

                    foreach (DefinitionWindow.Field field in definitionWindow.fields)
                    {
                        if (field.checkbox.Checked)
                        {
                            god.Properties.Add(field.label.Text, currentClassNameEdit.dataClassType.GetProperty(field.label.Text).GetValue(currentEditGameObject, null));
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

            RestoreCamera();

            releaseInput = false;
            LevelNameMessageBox box = new LevelNameMessageBox("Level name:");
            box.ShowDialog();
            releaseInput = true;

            string lvlName;
            if (!box.canceled)
            {

                Regex reg = new Regex(@"" + levelExtension + "$");
                if (!reg.IsMatch(box.levelName))
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
                    //currentObject.GetType().GetProperty("definition").SetValue(currentObject, null, null);
                    propertyGrid1.Refresh();
                }
            }






        }

        private void buttonCommitMeshTransforms_Click(object sender, EventArgs e)
        {
            level.SaveGlobalGameObjectsData();
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
            input.Enabled = false;
            inputEnable.Checked = input.Enabled;
        }

        private void GameObjectEditorWindow_Deactivate(object sender, EventArgs e)
        {
            //input.Enabled = true;
            //inputEnable.Checked = input.Enabled;
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
                currentClassNameEdit = new gameObjectsClassName();
                currentClassNameEdit.dataClassType = currentEditGameObject.GetType();

                

                foreach (gameObjectsClassName name in gameObjectClassNames)
                {
                    if (name.dataClassType == currentClassNameEdit.dataClassType)
                    {
                        currentClassNameEdit.className = name.className;
                        currentClassNameEdit.ClassType = name.ClassType;
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


        private void RestoreCamera()
        {
            //if (linkedFirst)
            //{
            //    renderer.CurrentCamera = linkedCamera.cameraComponent;
            //    cameraType = typeof(LinkedCamera);

            //    if (freeCamera != null)
            //    {
            //        level.GameObjectsFactory.RemoveGameObject(freeCamera.ID);
            //        freeCamera = null;
            //    }

            //}
            //else
            //{
            //    renderer.CurrentCamera = freeCamera.cameraComponent;
            //    cameraType = typeof(FreeCamera);

            //    if (linkedCamera != null)
            //    {
            //        level.GameObjectsFactory.RemoveGameObject(linkedCamera.ID);
            //        linkedCamera = null;
            //    }

            //}

        }
        private void setUpCameraButton()
        {

            TreeNode[] nodes = treeView1.Nodes.Find("LinkedCamera", false);
            if (nodes.GetLength(0) != 0)
            {
                if (nodes[0].Nodes.Count != 0)
                {
                    int id = int.Parse(nodes[0].Nodes[0].Text);

                    foreach (GameObjectInstance go in level.GameObjects.Values)
                    {
                        if (go.ID == id)
                        {
                            linkedCamera = (LinkedCamera)go;
                        }
                    }
                }
            }


            TreeNode[] nodes2 = treeView1.Nodes.Find("FreeCamera", false);
            if (nodes2.GetLength(0) != 0)
            {
                if (nodes2[0].Nodes.Count != 0)
                {
                    int id = int.Parse(nodes2[0].Nodes[0].Text);
                    foreach (GameObjectInstance go in level.GameObjects.Values)
                    {
                        if (go.ID == id)
                        {
                            freeCamera = (FreeCamera)go;
                        }
                    }
                }
            }
            if (freeCamera == null && linkedCamera == null)
            {
                cameraType = null;
                button3.Text = "Cant switch camera types";
            }
            else
            {
                if (linkedCamera != null && renderer.CurrentCamera != null && renderer.CurrentCamera.Equals(linkedCamera.CameraComponent))
                {
                    button3.Text = "Switch to freeCamera";
                    cameraType = typeof(LinkedCamera);
                }
                else if (freeCamera != null && renderer.CurrentCamera != null && renderer.CurrentCamera.Equals(freeCamera.cameraComponent))
                {
                    button3.Text = "Switch to linkedCamera";
                    cameraType = typeof(FreeCamera);
                    linkedFirst = false;
                }
            }
        }


        private void switchToFreeCamera()
        {
            if (freeCamera == null)
            {
                Vector3 pos = renderer.CurrentCamera.Position;
                FreeCameraData fcdata = new FreeCameraData();
                fcdata.Type = typeof(FreeCamera);
                fcdata.World = Matrix.Invert(Matrix.CreateLookAt(pos,
                                                                 new Vector3(pos.X, pos.Y - 70, pos.Z + 60),
                                                                 new Vector3(0, 1, 0)));
                fcdata.MovementSpeed = 0.05f;
                fcdata.RotationSpeed = MathHelper.PiOver4 / 500;
                fcdata.FoV = 60;
                fcdata.ZNear = 1;
                fcdata.ZFar = 200;
                fcdata.ActiveKeyListener = true;
                fcdata.ActiveMouseListener = true;
                freeCamera = (FreeCamera)level.GameObjectsFactory.Create(fcdata);




            }

            renderer.CurrentCamera = freeCamera.cameraComponent;
            cameraType = typeof(FreeCamera);
            freeCamera.keyboardListenerComponent.Active = true;
            freeCamera.mouseListenerComponent.Active = true;
            linkedCamera.KeyboardListenerComponent.Active = false;
            linkedCamera.MouseListenerComponent.Active = false;
            //level.GameObjectsFactory.RemoveGameObject(linkedCamera.ID);

        }

        private void switchToLinkedCamera()
        {
            int id = 0;
            TreeNode[] nodes;
            MercenariesManager mercManager;
            if (linkedCamera == null)
            {
                Vector3 pos = renderer.CurrentCamera.Position;


                id = 0;
                nodes = treeView1.Nodes.Find("MercenariesManager", false);
                if (nodes.GetLength(0) != 0)
                {
                    id = int.Parse(nodes[0].Nodes[0].Text);

                }
                mercManager = (MercenariesManager)level.GameObjects[id];

                LinkedCameraData lcdata = new LinkedCameraData();
                lcdata.Type = typeof(LinkedCamera);
                lcdata.position = pos;
                lcdata.Target = new Vector3(pos.X, pos.Y - 35, pos.Z + 25);
                lcdata.MovementSpeed = 0.07f;
                lcdata.RotationSpeed = 0.005f;
                lcdata.ZoomSpeed = 0.01f;
                lcdata.FoV = 60;
                lcdata.ZNear = 1f;
                lcdata.ZFar = 201;
                lcdata.ActiveKeyListener = true;
                lcdata.ActiveMouseListener = true;
                lcdata.MercenariesManager = id;

                linkedCamera = (LinkedCamera)(level.GameObjectsFactory.Create(lcdata));
                if (mercManager != null)
                {
                    mercManager.LinkedCamera = linkedCamera;
                }

            }

            renderer.CurrentCamera = linkedCamera.CameraComponent;
            cameraType = typeof(LinkedCamera);

            linkedCamera.KeyboardListenerComponent.Active = true;
            linkedCamera.MouseListenerComponent.Active = true;
            freeCamera.keyboardListenerComponent.Active = false;
            freeCamera.mouseListenerComponent.Active = false;


            nodes = treeView1.Nodes.Find("MercenariesManager", false);
            if (nodes.GetLength(0) != 0)
            {
                id = int.Parse(nodes[0].Nodes[0].Text);

            }
            mercManager = (MercenariesManager)level.GameObjects[id];

            linkedCamera.MercenariesManager = mercManager;
            if (mercManager != null)
            {
                mercManager.LinkedCamera = linkedCamera;
            }
            //level.GameObjectsFactory.RemoveGameObject(freeCamera.ID);
        }




        private void button3_Click(object sender, EventArgs e)
        {
            if (cameraType != null)
            {
                if (cameraType == typeof(LinkedCamera))
                {
                    switchToFreeCamera();

                    button3.Text = "Switch to linkedCamera";
                }
                else if (cameraType == typeof(FreeCamera))
                {
                    switchToLinkedCamera();
                    button3.Text = "Switch to freeCamera";
                }
            }

            LoadAllObjectsId();

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {

                LoadIconsInfo();

            }
            else
            {
                icons.Clear();
            }
        }



        public void DrawIcons(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {

            if (renderer.CurrentCamera != null && renderer.CurrentCamera.GameObject.GetType() == typeof(FreeCamera))
            {
                if (icons.Count != 0)
                {
                    iconInfo.Clear();



                    foreach (GameObjectInstance gameobject in icons)
                    {
                        Vector4 position;
                        Vector2 pos2;
                        int textureWidth = 0;
                        int textureHeight = 0;
                        Texture2D texture = null;


                        if (gameobject.GetType().Name.Equals("Sunlight"))
                        {
                            textureWidth = sunLightIcon.Width;
                            textureHeight = sunLightIcon.Height;
                            texture = sunLightIcon;

                        }
                        else if (gameobject.GetType().Name.Equals("SpotLight"))
                        {
                            textureWidth = spotLightIcon.Width;
                            textureHeight = spotLightIcon.Height;
                            texture = spotLightIcon;
                        }
                        else if (gameobject.GetType().Name.Equals("PointLight"))
                        {
                            textureWidth = pointLightIcon.Width;
                            textureHeight = pointLightIcon.Height;
                            texture = pointLightIcon;
                        }
                        else if (gameobject.GetType().Name.Equals("ParticleEmitter"))
                        {
                            textureWidth = particleEmitterIcon.Width;
                            textureHeight = particleEmitterIcon.Height;
                            texture = particleEmitterIcon;
                        }
                        else if (gameobject.GetType().Name.Equals("AreaParticleEmitter"))
                        {
                            textureWidth = AreaparticleEmitterIcon.Width;
                            textureHeight = AreaparticleEmitterIcon.Height;
                            texture = AreaparticleEmitterIcon;
                        }

                        position = Vector4.Transform(Vector3.Transform(Vector3.Zero, gameobject.World), ViewProjection);

                        pos2.X = MathHelper.Clamp(0.5f * ((position.X / Math.Abs(position.W)) + 1.0f), 0.01f, 0.99f);
                        pos2.X *= screenWidth;


                        pos2.Y = MathHelper.Clamp(1.0f - (0.5f * ((position.Y / Math.Abs(position.W)) + 1.0f)), 0.01f, 0.99f);
                        pos2.Y *= screenHeight;
                        pos2.Y -= textureHeight / 2;
                        pos2.X -= textureWidth / 2;
                        spriteBatch.Draw(texture, pos2, Color.White);


                        iconInfo.Add(new iconInfo(pos2, textureWidth, textureHeight, gameobject.ID));



                    }

                }
                if (moveObject)
                {
                    Vector2 pos = new Vector2(screenWidth - moveIcon.Width, screenHeight - moveIcon.Height);
                    spriteBatch.Draw(moveIcon, pos, Color.White);
                }
                else
                {
                    Vector2 pos = new Vector2(screenWidth - rotateIcon.Width, screenHeight - rotateIcon.Height);
                    spriteBatch.Draw(rotateIcon, pos, Color.White);
                }
            }
        }



        private void LoadIconsInfo()
        {

            icons.Clear();
            foreach (GameObjectInstance gameobject in level.GameObjects.Values)
            {
                if (gameobject.GetType().Name.Equals("Sunlight"))
                {
                    icons.Add(gameobject);
                }
                else if (gameobject.GetType().Name.Equals("SpotLight"))
                {
                    icons.Add(gameobject);
                }
                else if (gameobject.GetType().Name.Equals("PointLight"))
                {
                    icons.Add(gameobject);
                }
                else if (gameobject.GetType().Name.Equals("ParticleEmitter"))
                {
                    icons.Add(gameobject);
                }
                else if (gameobject.GetType().Name.Equals("AreaParticleEmitter"))
                {
                    icons.Add(gameobject);
                }
            }
        }




        public void LoadIconTextures()
        {
            if (sunLightIcon == null || pointLightIcon == null || spotLightIcon == null)
            {
                sunLightIcon = contentManager.LoadTexture2D("sunLightIcon");
                spotLightIcon = contentManager.LoadTexture2D("spotLightIcon");
                pointLightIcon = contentManager.LoadTexture2D("pointLightIcon");
                particleEmitterIcon = contentManager.LoadTexture2D("particleEmitterIcon");
                AreaparticleEmitterIcon = contentManager.LoadTexture2D("AreparticleEmitterIcon");
                moveIcon = contentManager.LoadTexture2D("moveIcon");
                rotateIcon = contentManager.LoadTexture2D("rotateIcon");
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            jiglibxSelection = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            jiglibxDrag = checkBox5.Checked;
        }


        private void fillInfoInLabel()
        {
            if (propertyGrid2.SelectedObject != null)
            {
                label5.Text = ((GameObjectInstanceData)(propertyGrid2.SelectedObject)).ID.ToString();
                label6.Text = propertyGrid2.SelectedObject.GetType().Name.ToString();
            }
            else
            {
                label5.Text = "";
                label6.Text = "";
            }
        }

        private void propertyGrid2_SelectedObjectsChanged(object sender, EventArgs e)
        {
            fillInfoInLabel();

        }

        private void SSAOSampleRadius_ValueChanged(object sender, EventArgs e)
        {
            renderer.sampleRadius = (float)SSAOSampleRadius.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            renderer.distanceScale = (float)numericUpDown1.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RestoreCamera();

            string name = level.CurrentLevel;
            level.Clear(true);
            level.LoadLevel(name);
            LoadAllObjectsId();
            LoadIconsInfo();
            LoadAllObjectsId();

            setUpCameraButton();

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            renderer.Brightness = (float)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            renderer.Contrast = (float)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            renderer.BloomThreshold = (float)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            renderer.BaseSaturation = (float)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            renderer.BaseIntensity = (float)numericUpDown6.Value;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            renderer.BloomSaturation = (float)numericUpDown7.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            renderer.BloomIntensity = (float)numericUpDown8.Value;
        }







        /********************************************************************************/




    }
    /********************************************************************************/





    public class iconInfo
    {
        public Vector2 pos;
        public int width, height;
        public int goID;

        public iconInfo(Vector2 pos, int w, int h, int id)
        {
            this.pos = pos;
            width = w;
            height = h;
            goID = id;
        }
    }



}
/********************************************************************************/