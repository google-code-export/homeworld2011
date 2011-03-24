using System;
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

using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Resources;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input;
using PlagueEngine.Input.Components;

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

        private IntPtr gameWindowHandle;

        private string levelDirectory = @"Data\levels";
        private string levelExtension = ".lvl";
        private string currentLevelName = string.Empty;
        private Level currentLevel = null;
        private bool levelSaved = true;

        private GameObjectDefinition currentDefinition = null;
        
        
        /********************************************************************************/




        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public GameObjectEditorWindow(GameObjectsFactory factory,ContentManager contentManager,IntPtr gameWindowHandle)
        {
            InitializeComponent();
            FillClassNames();
            this.factory = factory;
            this.contentManager = contentManager;
            this.gameWindowHandle = gameWindowHandle;

            foreach (var gameObject in gameObjectClassNames)
            {
                gameObjectsName.Items.Add(gameObject.className);
            }


            this.Visible = true;



            loadLevelNames();
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Incompatibility or other black magic! :<", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
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
            bool gameWindowVisible = ((Form)(Form.FromHandle(gameWindowHandle))).Visible;

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


            ((Form)(Form.FromHandle(gameWindowHandle))).Visible = gameWindowVisible;
            
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
                    contentManager.GameObjectsDefinitions.Add(god.Name, god);
                    contentManager.SaveGameObjectsDefinitions();


                    ComboboxDefinitions.Items.Add(definitionWindow.textbox.Text);


                }


                
            }

       
        }
        /********************************************************************************/

    


    }
    /********************************************************************************/


}
/********************************************************************************/
