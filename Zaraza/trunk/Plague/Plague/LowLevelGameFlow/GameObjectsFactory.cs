using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.LowLevelGameFlow.GameObjects;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Game Objects Factory
    /********************************************************************************/    
    class GameObjectsFactory
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private RenderingComponentsFactory               renderingComponentsFactory = null;
        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions     = null;
        private Dictionary<uint, GameObjectInstance>     gameObjects                = null;
        private List<GameObjectInstance>                 gameObjectsRequiredUpdate  = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GameObjectsFactory(RenderingComponentsFactory               renderingComponentsFactory,
                                  Dictionary<String, GameObjectDefinition> gameObjectsDefinitions)
        {
            this.renderingComponentsFactory = renderingComponentsFactory;
            this.gameObjectsDefinitions     = gameObjectsDefinitions;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Game Objects
        /****************************************************************************/
        public Dictionary<uint, GameObjectInstance> GameObjects
        {
            set
            {
                gameObjects = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Game Objects Required Update
        /****************************************************************************/
        public List<GameObjectInstance> GameObjectsRequiredUpdate
        {
            set
            {
                gameObjectsRequiredUpdate = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Game Object
        /****************************************************************************/
        public GameObjectInstance Create(GameObjectInstanceData data)
        {
            GameObjectInstance result = null;
            
            switch (data.Type.Name)
            {
                case "StaticMesh": 
                    result = CreateStaticMesh(data);
                    break;
                case "FreeCamera":
                    result = CreateFreeCamera(data);
                    break;
            }

            if (result == null) return null;

            if (gameObjects != null)                gameObjects.Add(result.ID, result);
            if (gameObjectsRequiredUpdate != null)  gameObjectsRequiredUpdate.Add(result);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Static Mesh
        /****************************************************************************/
        private StaticMesh CreateStaticMesh(GameObjectInstanceData data)
        {
            StaticMesh result = new StaticMesh(data.ID,data.Definition);

            result.Init( renderingComponentsFactory.CreateBasicMeshComponent ( result, 
                                                                               (String)gameObjectsDefinitions[data.Definition].Properties["Model"]), 
                         data.World);
            
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Free Camera
        /****************************************************************************/
        private FreeCamera CreateFreeCamera(GameObjectInstanceData data)
        {
            FreeCamera result = new FreeCamera(data.ID, data.Definition);

            FreeCameraData trueData = (FreeCameraData)data;

            result.Init( renderingComponentsFactory.CreateCameraComponent(result,
                                                                          trueData.FoV,
                                                                          trueData.ZNear,
                                                                          trueData.ZFar),
                         trueData.World,
                         trueData.MovementSpeed,
                         trueData.RotationSpeed);

            return result;
        }
        /****************************************************************************/

    }
    /********************************************************************************/    

}
/************************************************************************************/