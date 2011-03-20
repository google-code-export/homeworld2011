using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Tools
{
    partial class GameObjectEditorWindow : Form
    {



        public class gameObjectsClassName
        {
            public string className;
            public Type ClassType;
            public Type dataClassType;
        }




        private List<gameObjectsClassName> gameObjectClassNames = new List<gameObjectsClassName>();

        private GameObjectsFactory factory = null;
        private gameObjectsClassName currentClassName = null;
        private GameObjectInstanceData currentObject = null;


        public GameObjectEditorWindow(GameObjectsFactory factory)
        {
            InitializeComponent();
            FillClassNames();
            this.factory = factory;
            foreach (var gameObject in gameObjectClassNames)
            {
                addGameObjectName(gameObject.className);
            }


            this.Visible = true;
        }

        public void addGameObjectName(string name)
        {
            gameObjectsName.Items.Add(name);
        }


        private void FillNames(object sender, EventArgs e)
        {
            string objectname = gameObjectsName.Items[gameObjectsName.SelectedIndex].ToString();

            currentClassName = getClass(objectname);
            currentObject = (GameObjectInstanceData)(Activator.CreateInstance(currentClassName.dataClassType));
            
            propertyGrid1.SelectedObject = currentObject;
        }


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
        }


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


        private void button1_Click(object sender, EventArgs e)
        {
            currentObject.Type = currentClassName.ClassType;

            factory.Create(currentObject);
        }

    




    }
}

