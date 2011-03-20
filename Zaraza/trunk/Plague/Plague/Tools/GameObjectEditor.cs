using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Tools
{
    public class GameObjectEditor
    {

        private struct gameObjectsClassName
        {
            public string className;
            public string dataClassName;
        }




        List<gameObjectsClassName> gameObjectClassNames = new List<gameObjectsClassName>();
        private string GameObjectNameSpace = "PlagueEngine.LowLevelGameFlow.GameObjects.";
        private GameObjectEditorWindow window = null;
       

        public GameObjectEditor()
        {
            
            window = new GameObjectEditorWindow(this);

            FillClassNames();


            foreach (var gameObject in gameObjectClassNames)
            {
                window.addGameObjectName(gameObject.className);
            }

 
        }



        public void FillClassNames()
        {
            //moze potem zrobie wczytywanie z pliku; jesli ktos chce to zrobic to droga wolna;)

            gameObjectsClassName linkedCamera;
            linkedCamera.className = "LinkedCamera";
            linkedCamera.dataClassName = "LinkedCameraData";
            gameObjectClassNames.Add(linkedCamera);

            gameObjectsClassName freeCamera;
            freeCamera.className = "FreeCamera";
            freeCamera.dataClassName = "FreeCameraData";
            gameObjectClassNames.Add(freeCamera);

            gameObjectsClassName staticMesh;
            staticMesh.className = "StaticMesh";
            staticMesh.dataClassName = "StaticMeshData";
            gameObjectClassNames.Add(staticMesh);
        }
        
        public List<string> getClassFieldsName(string className)
        {
            List<string> fieldNames = new List<string>();

            foreach (var gameobject in gameObjectClassNames)
            {
                if (className == gameobject.className)
                {
                    Type type = Type.GetType(GameObjectNameSpace + gameobject.dataClassName);

                    System.Reflection.FieldInfo[] fieldInfo = type.GetFields();

                    foreach (var info in fieldInfo)
                    {
                        fieldNames.Add(info.Name);


                    }
                }
            }


            return fieldNames;
        }


    }
}
