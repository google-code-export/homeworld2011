using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input;
using PlagueEngine.Input.Components;
using PlagueEngine.Physics;
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
        private PhysicsComponentFactory                  physicsComponentFactory    = null;
        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions     = null;
        
        private Dictionary<uint, GameObjectInstance>     gameObjects                = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GameObjectsFactory(RenderingComponentsFactory               renderingComponentsFactory,
                                  InputComponentsFactory                   inputComponentsFactory,
                                  Dictionary<String, GameObjectDefinition> gameObjectsDefinitions,
                                  PhysicsComponentFactory physicsComponentFactory)
        {
            this.renderingComponentsFactory = renderingComponentsFactory;
            this.inputComponentsFactory     = inputComponentsFactory;
            this.gameObjectsDefinitions     = gameObjectsDefinitions;
            this.physicsComponentFactory    = physicsComponentFactory;
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
            get
            {
                return this.gameObjects;
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
                case "Sunlight":
                    result = CreateSunlight(data);
                    break;
                case "SquareBodyMesh":
                    result = CreateSquareBodyMesh(data);
                    break;
                case "CylindricalBodyMesh":
                    result = CreateCylindricalBodyMesh(data);
                    break;
                case "SphericalBodyMesh":
                    result = CreateSphericalBodyMesh(data);
                    break;
                case "Piggy":
                    result = CreatePiggy(data);
                    break;
                case "SquareSkinMesh":
                    result = CreateSquareSkinMesh(data);
                    break;
                case "SphericalSkinMesh":
                    result = CreateSphericalSkinMesh(data);
                    break;
                case "CylindricalSkinMesh":
                    result = CreateCylindricalSkinMesh(data);
                    break;
            }

            if (result == null) return null;

            if (gameObjects != null) gameObjects.Add(result.ID, result);

            return result;
        }
        /****************************************************************************/

                
        /****************************************************************************/
        /// Create Cylindrical Body Mesh
        /****************************************************************************/
        public CylindricalBodyMesh CreateCylindricalBodyMesh(GameObjectInstanceData data)
        {
            CylindricalBodyMesh result = new CylindricalBodyMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            CylindricalBodyMeshData sbmdata = (CylindricalBodyMeshData)data;

            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(sbmdata.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       sbmdata.Model,
                                                                       sbmdata.Diffuse,
                                                                       sbmdata.Specular,
                                                                       sbmdata.Normals,
                                                                       InstancingMode),
                        physicsComponentFactory.CreateCylindricalBodyComponent(result,
                        sbmdata.Mass,
                        sbmdata.Radius,
                        sbmdata.Lenght,
                        sbmdata.Elasticity,
                        sbmdata.StaticRoughness,
                        sbmdata.DynamicRoughness,
                        sbmdata.Immovable,
                        sbmdata.World,
                        sbmdata.Translation,
                        sbmdata.SkinYaw,
                        sbmdata.SkinPitch,
                        sbmdata.SkinRoll),
                        sbmdata.World);


            return result;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Create Spherical Body Mesh
        /****************************************************************************/
        public SphericalBodyMesh CreateSphericalBodyMesh(GameObjectInstanceData data)
        {
            SphericalBodyMesh result = new SphericalBodyMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            SphericalBodyMeshData sbmdata = (SphericalBodyMeshData)data;

            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(sbmdata.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       sbmdata.Model,
                                                                       sbmdata.Diffuse,
                                                                       sbmdata.Specular,
                                                                       sbmdata.Normals,
                                                                       InstancingMode),
                        physicsComponentFactory.CreateSphericalBodyComponent(result,
                        sbmdata.Mass,
                        sbmdata.Radius,
                        sbmdata.Elasticity,
                        sbmdata.StaticRoughness,
                        sbmdata.DynamicRoughness,
                        sbmdata.Immovable,
                        sbmdata.World,
                        sbmdata.Translation,
                        sbmdata.SkinYaw,
                        sbmdata.SkinPitch,
                        sbmdata.SkinRoll),
                        sbmdata.World);


            return result;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Create Square Body Mesh
        /****************************************************************************/
        public SquareBodyMesh CreateSquareBodyMesh(GameObjectInstanceData data)
        {
            SquareBodyMesh result = new SquareBodyMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            SquareBodyMeshData sbmdata = (SquareBodyMeshData)data;

            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(sbmdata.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       sbmdata.Model,
                                                                       sbmdata.Diffuse,
                                                                       sbmdata.Specular,
                                                                       sbmdata.Normals,
                                                                       InstancingMode),
                        physicsComponentFactory.CreateSquareBodyComponent(result,
                        sbmdata.Mass,
                        sbmdata.Lenght,
                        sbmdata.Height,
                        sbmdata.Width,
                        sbmdata.Elasticity,
                        sbmdata.StaticRoughness,
                        sbmdata.DynamicRoughness,
                        sbmdata.Immovable,
                        sbmdata.World,
                        sbmdata.Translation,
                        sbmdata.SkinYaw,
                        sbmdata.SkinPitch,
                        sbmdata.SkinRoll),
                        sbmdata.World);


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
                result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                            smdata.Model,
                                                                            smdata.Diffuse,
                                                                            smdata.Specular,
                                                                            smdata.Normals,
                                                                            Renderer.UIntToInstancingMode(smdata.InstancingMode)),
                                                                            smdata.World);                    
                               
            }
            else
            {
                if(!gameObjectsDefinitions.Keys.Contains(smdata.Definition))
                {
                    Diagnostics.PushLog("No existing definition: " + smdata.Definition + ". Creating object failed.");
                    return result = null;
                }
                
                GameObjectDefinition definition = gameObjectsDefinitions[smdata.Definition];


                /**************************/
                // Model
                String Model;
                if (definition.Properties.Keys.Contains("Model")) Model = (String)definition.Properties["Model"];
                else Model = smdata.Model;
                /**************************/
                // Diffuse
                String Diffuse;
                if (definition.Properties.Keys.Contains("Diffuse")) Diffuse = (String)definition.Properties["Diffuse"];
                else Diffuse = smdata.Diffuse;
                /**************************/
                // Specular
                String Specular;
                if (definition.Properties.Keys.Contains("Specular")) Specular = (String)definition.Properties["Specular"];
                else Specular = smdata.Specular;
                /**************************/
                // Normals
                String Normals;
                if (definition.Properties.Keys.Contains("Normals")) Normals = (String)definition.Properties["Normals"];
                else Normals = smdata.Normals;
                /**************************/
                // InstancingMode
                InstancingModes InstancingMode;
                if (definition.Properties.Keys.Contains("InstancingMode")) InstancingMode = Renderer.UIntToInstancingMode((uint)definition.Properties["InstancingMode"]);
                else InstancingMode = Renderer.UIntToInstancingMode(smdata.InstancingMode);
                /**************************/



                result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                            Model,
                                                                            Diffuse,
                                                                            Specular,
                                                                            Normals,
                                                                            InstancingMode),
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
        /// Create Cylindrical Skin Mesh
        /****************************************************************************/
        private CylindricalSkinMesh CreateCylindricalSkinMesh(GameObjectInstanceData data)
        {
            CylindricalSkinMesh result = new CylindricalSkinMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            CylindricalSkinMeshData tdata = (CylindricalSkinMeshData)data;

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                            tdata.Model,
                                                                            tdata.Diffuse,
                                                                            tdata.Specular,
                                                                            tdata.Normals,
                                                                            Renderer.UIntToInstancingMode(tdata.InstancingMode)),
                        physicsComponentFactory.CreateCylindricalSkinComponent(result,
            tdata.Elasticity,
            tdata.StaticRoughness,
            tdata.DynamicRoughness,
            tdata.World,
            tdata.Lenght,
            tdata.Radius,
            tdata.Translation,
            tdata.SkinYaw,
            tdata.SkinPitch,
            tdata.SkinRoll),
                        tdata.World);

            return result;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Create Square Skin Mesh
        /****************************************************************************/
        private SquareSkinMesh CreateSquareSkinMesh(GameObjectInstanceData data)
        {
            SquareSkinMesh result = new SquareSkinMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            SquareSkinMeshData tdata = (SquareSkinMeshData)data;

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                            tdata.Model,
                                                                            tdata.Diffuse,
                                                                            tdata.Specular,
                                                                            tdata.Normals,
                                                                            Renderer.UIntToInstancingMode(tdata.InstancingMode)), 
                        physicsComponentFactory.CreateSquareSkinComponent(result,
            tdata.Elasticity,
            tdata.StaticRoughness,
            tdata.DynamicRoughness,
            tdata.World,
            tdata.Lenght,
            tdata.Height,
            tdata.Width,
            tdata.Translation,
            tdata.SkinYaw,
            tdata.SkinPitch,
            tdata.SkinRoll),
                        tdata.World);

            return result;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Create Spherical Skin Mesh
        /****************************************************************************/
        private SphericalSkinMesh CreateSphericalSkinMesh(GameObjectInstanceData data)
        {
            SphericalSkinMesh result = new SphericalSkinMesh();

            if (!DefaultObjectInit(result, data)) return result = null;

            SphericalSkinMeshData tdata = (SphericalSkinMeshData)data;

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                            tdata.Model,
                                                                            tdata.Diffuse,
                                                                            tdata.Specular,
                                                                            tdata.Normals,
                                                                            Renderer.UIntToInstancingMode(tdata.InstancingMode)),
                        physicsComponentFactory.CreateSphericalSkinComponent(result,
            tdata.Elasticity,
            tdata.StaticRoughness,
            tdata.DynamicRoughness,
            tdata.World,
            tdata.Radius,
            tdata.Translation,
            tdata.SkinYaw,
            tdata.SkinPitch,
            tdata.SkinRoll),
                        tdata.World);

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

                         physicsComponentFactory.CreateTerrainSkinComponent(result,
                                                                            tdata.HeightMap,
                                                                            tdata.Width,
                                                                            tdata.Length,
                                                                            tdata.Height,
                                                                            tdata.CellSize,
                                                                            tdata.Elasticity,
                                                                            tdata.StaticRoughness,
                                                                            tdata.DynamicRoughness),

                                                                            tdata.World);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Sun Light
        /****************************************************************************/
        public Sunlight CreateSunlight(GameObjectInstanceData data)
        {
            Sunlight result = new Sunlight();

            if (!DefaultObjectInit(result, data)) return result = null;

            SunlightData sdata = (SunlightData)data;

            result.Init(renderingComponentsFactory.CreateSunlightComponent(result,
                                                                           sdata.Diffuse,
                                                                           sdata.Specular,
                                                                           sdata.Enabled),
                                                                           sdata.World);
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Static Skinned Mesh
        /****************************************************************************/
        public Piggy CreatePiggy(GameObjectInstanceData data)
        {
            Piggy result = new Piggy();

            if (!DefaultObjectInit(result, data)) return result = null;

            PiggyData sdata = (PiggyData)data;

            result.Init(renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              sdata.Model,
                                                                              sdata.Diffuse,
                                                                              sdata.Specular,
                                                                              sdata.Normals,
                                                                              sdata.TimeRatio,
                                                                              sdata.CurrentClip,
                                                                              sdata.CurrentKeyframe,
                                                                              sdata.CurrentTime,
                                                                              sdata.Pause,
                                                                              sdata.Blend,
                                                                              sdata.BlendClip,
                                                                              sdata.BlendKeyframe,
                                                                              sdata.BlendClipTime,
                                                                              sdata.BlendDuration,
                                                                              sdata.BlendTime),                                                                              
                        inputComponentsFactory.CreateKeyboardListenerComponent(result,true),
                        physicsComponentFactory.CreateSquareBodyComponent(result,
                                                                          sdata.Mass,
                                                                          sdata.Length,
                                                                          sdata.Height,
                                                                          sdata.Width,
                                                                          sdata.Elasticity,
                                                                          sdata.StaticRoughness,
                                                                          sdata.DynamicRoughness,
                                                                          sdata.Immovable,
                                                                          sdata.World,
                                                                          sdata.Translation,
                                                                          sdata.SkinYaw,
                                                                          sdata.SkinPitch,
                                                                          sdata.SkinRoll),
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