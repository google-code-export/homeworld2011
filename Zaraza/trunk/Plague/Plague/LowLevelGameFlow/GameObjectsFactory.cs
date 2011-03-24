using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input;
using PlagueEngine.Input.Components;

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
        private InputComponentsFactory                   inputComponentsFactory     = null;
        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions     = null;
        
        private Dictionary<uint, GameObjectInstance>     gameObjects                = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GameObjectsFactory(RenderingComponentsFactory               renderingComponentsFactory,
                                  InputComponentsFactory                   inputComponentsFactory,
                                  Dictionary<String, GameObjectDefinition> gameObjectsDefinitions)
        {
            this.renderingComponentsFactory = renderingComponentsFactory;
            this.inputComponentsFactory     = inputComponentsFactory;
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
                case "LinkedCamera":
                    result = CreateLinkedCamera(data);
                    break;
                case "Terrain":
                    result = CreateTerrain(data);
                    break;
                case "SunLight":
                    result = CreateSunLight(data);
                    break;
            }

            if (result == null) return null;

            if (gameObjects != null) gameObjects.Add(result.ID, result);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Static Mesh
        /****************************************************************************/
        private StaticMesh CreateStaticMesh(GameObjectInstanceData data)
        {
            StaticMesh result = new StaticMesh();                    

            if (!DefaultObjectInit(result, data)) return result = null;

            StaticMeshData smdata = (StaticMeshData)data;

            if (String.IsNullOrEmpty(data.definition))
            {
                result.Init(renderingComponentsFactory.CreateBasicMeshComponent(result,
                                                                                smdata.Model),
                                                                                data.World);
            
            }
            else
            {
                result.Init(renderingComponentsFactory.CreateBasicMeshComponent(result,
                                                                               (String)gameObjectsDefinitions[smdata.Definition].Properties["Model"]),
                                                                               data.World);
            
            }
            
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Free Camera
        /****************************************************************************/
        private FreeCamera CreateFreeCamera(GameObjectInstanceData data)
        {
            FreeCamera result = new FreeCamera();

            if (!DefaultObjectInit(result, data)) return result = null; 

            FreeCameraData fcdata = (FreeCameraData)data;
            
            result.Init(renderingComponentsFactory.CreateCameraComponent(result,
                                                                         fcdata.FoV,
                                                                         fcdata.ZNear,
                                                                         fcdata.ZFar),

                         inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                fcdata.ActiveKeyListener),

                         inputComponentsFactory.CreateMouseListenerComponent(result,
                                                                             fcdata.ActiveMouseListener),

                         fcdata.World,
                         fcdata.MovementSpeed,
                         fcdata.RotationSpeed);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Linked Camera
        /****************************************************************************/
        private LinkedCamera CreateLinkedCamera(GameObjectInstanceData data)
        {            
            LinkedCamera result = new LinkedCamera();
         
            if (!DefaultObjectInit(result, data)) return result = null; 

            LinkedCameraData lcdata = (LinkedCameraData)data;

            result.Init(renderingComponentsFactory.CreateCameraComponent(result,
                                                                          lcdata.FoV,
                                                                          lcdata.ZNear,
                                                                          lcdata.ZFar),

                         inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                lcdata.ActiveKeyListener),


                         inputComponentsFactory.CreateMouseListenerComponent(result,lcdata.ActiveMouseListener),
                         lcdata.MovementSpeed,
                         lcdata.RotationSpeed,
                         lcdata.ZoomSpeed,
                         lcdata.position,
                         lcdata.Target);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Terrain
        /****************************************************************************/
        private Terrain CreateTerrain(GameObjectInstanceData data)
        {
            Terrain result = new Terrain();
            
            if (!DefaultObjectInit(result, data)) return result = null; 

            TerrainData tdata = (TerrainData)data;

            result.Init(renderingComponentsFactory.CreateTerrainComponent(result,
                                                                            tdata.HeightMap,
                                                                            tdata.BaseTexture,
                                                                            tdata.RTexture,
                                                                            tdata.GTexture,
                                                                            tdata.BTexture,
                                                                            tdata.WeightMap,
                                                                            tdata.Width,
                                                                            tdata.Length,
                                                                            tdata.Height,
                                                                            tdata.CellSize,
                                                                            tdata.TextureTiling),

                        renderingComponentsFactory.CreateWaterSurfaceComponent(result,
                                                                            tdata.Width  * tdata.CellSize,
                                                                            tdata.Length * tdata.CellSize,
                                                                            tdata.Level,
                                                                            tdata.Color,
                                                                            tdata.ColorAmount,
                                                                            tdata.WaveLength,
                                                                            tdata.WaveHeight,
                                                                            tdata.WaveSpeed,
                                                                            tdata.NormalMap,
                                                                            tdata.Bias,
                                                                            tdata.WTextureTiling,
                                                                            tdata.ClipPlaneAdjustment,
                                                                            tdata.SpecularStength),
                                                                            tdata.World);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Sun Light
        /****************************************************************************/
        public SunLight CreateSunLight(GameObjectInstanceData data)
        {
            SunLight result = new SunLight();

            if (!DefaultObjectInit(result, data)) return result = null;

            SunLightData sdata = (SunLightData)data;

            result.Init(renderingComponentsFactory.CreateSunLightComponent(result,
                                                                            sdata.Ambient,
                                                                            sdata.Diffuse,
                                                                            sdata.Specular),
                                                                            sdata.World);
            return result;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Default Object Init
        /****************************************************************************/
        private bool DefaultObjectInit(GameObjectInstance gameObject, GameObjectInstanceData data)
        {
            if (!gameObject.Init(data.ID, data.Definition))
            {
                Diagnostics.PushLog("Creating Object " + data.Type + ", id: " + data.ID +
                                    ", definition: " + data.Definition + ", failed.");
                return false;
            }
            return true;
        }
        /****************************************************************************/


    }
    /********************************************************************************/    

}
/************************************************************************************/