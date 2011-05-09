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
using PlagueEngine.GUI;
using PlagueEngine.Particles;

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
        private GUIComponentsFactory                     guiComponentsFactory        = null;
        private ParticleFactory                          particleFactory = null;
        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions     = null;
        
        private Dictionary<uint, GameObjectInstance>     gameObjects                = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GameObjectsFactory(RenderingComponentsFactory               renderingComponentsFactory,
                                  InputComponentsFactory                   inputComponentsFactory,
                                  GUIComponentsFactory                     guiComponentsFactory,
                                  Dictionary<String, GameObjectDefinition> gameObjectsDefinitions,
                                  PhysicsComponentFactory                  physicsComponentFactory,
                                  ParticleFactory                          particleFactory)
        {
            this.renderingComponentsFactory = renderingComponentsFactory;
            this.inputComponentsFactory     = inputComponentsFactory;
            this.guiComponentsFactory       = guiComponentsFactory;
            this.gameObjectsDefinitions     = gameObjectsDefinitions;
            this.physicsComponentFactory    = physicsComponentFactory;
            this.particleFactory            = particleFactory;
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
                case "CylindricalBodyMesh2":
                    result = CreateCylindricalBodyMesh2(data);
                    break;
                case "SphericalBodyMesh":
                    result = CreateSphericalBodyMesh(data);
                    break;
                case "Creature":
                    result = CreateCreature(data);
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
                case "MovingBarrel":
                    result = CreateMovingBarrel(data);
                    break;
                case "MenuButton":
                    result = CreateMenuButton(data);
                    break;
                case "BackgroundTerrain":
                    result = CreateBackgroundTerrain(data);
                    break;
                case "WaterSurface":
                    result = CreateWaterSurface(data);
                    break;
                case "PointLight":
                    result = CreatePointLight(data);
                    break;
                case "SpotLight":
                    result = CreateSpotLight(data);
                    break;
                case "GlowStick":
                    result = CreateGlowStick(data);
                    break;
                case "ParticleEmitterGO":
                    result = CreateParticleEmitterGO(data);
                    break;
                case "Flashlight":
                    result = CreateFlashLight(data);
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
        public ParticleEmitterGO CreateParticleEmitterGO(GameObjectInstanceData data)
        {
            ParticleEmitterGO result = new ParticleEmitterGO();

            if (!DefaultObjectInit(result, data)) return result = null;

            ParticleEmitterGOData sbmdata = (ParticleEmitterGOData)data;


            result.Init(particleFactory.CreateParticleEmitter(
            sbmdata.blendState,
            sbmdata.duration,
            sbmdata.durationRandomnes,
            sbmdata.emitterVelocitySensitivity,
            sbmdata.endVelocity,
            sbmdata.gravity,
            sbmdata.maxColor,
            sbmdata.maxEndSize,
            sbmdata.maxHorizontalVelocity,
            sbmdata.maxParticles,
            sbmdata.maxRotateSpeed,
            sbmdata.maxStartSize,
            sbmdata.maxVerticalVelocity,
            sbmdata.minColor,
            sbmdata.minEndSize,
            sbmdata.minHorizontalVelocity,
            sbmdata.minRotateSpeed,
            sbmdata.minStartSize,
            sbmdata.minVerticalVelocity,
            sbmdata.particleTexture,
            sbmdata.particlesPerSecond,
            sbmdata.initialPosition
                ));


            return result;
        }
        /****************************************************************************/






        /****************************************************************************/
        /// Create Cylindrical Body Mesh
        /****************************************************************************/
        public MovingBarrel CreateMovingBarrel(GameObjectInstanceData data)
        {
            MovingBarrel result = new MovingBarrel();

            if (!DefaultObjectInit(result, data)) return result = null;

            MovingBarrelData sbmdata = (MovingBarrelData)data;

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
                        sbmdata.Width,
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
        /// Create Cylindrical Body Mesh
        /****************************************************************************/
        public CylindricalBodyMesh2 CreateCylindricalBodyMesh2(GameObjectInstanceData data)
        {
            CylindricalBodyMesh2 result = new CylindricalBodyMesh2();

            if (!DefaultObjectInit(result, data)) return result = null;

            CylindricalBodyMeshData2 sbmdata = (CylindricalBodyMeshData2)data;

            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(sbmdata.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       sbmdata.Model,
                                                                       sbmdata.Diffuse,
                                                                       sbmdata.Specular,
                                                                       sbmdata.Normals,
                                                                       InstancingMode),
                        physicsComponentFactory.CreateCylindricalBodyComponent2(result,
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
#if DEBUG
                    Diagnostics.PushLog("No existing definition: " + smdata.Definition + ". Creating object failed.");
#endif
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
        /// Create Background Terrain
        /****************************************************************************/
        private BackgroundTerrain CreateBackgroundTerrain(GameObjectInstanceData data)
        {
            BackgroundTerrain result = new BackgroundTerrain();

            if (!DefaultObjectInit(result, data)) return result = null;

            BackgroundTerrainData tdata = (BackgroundTerrainData)data;

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
                                                                           sdata.Intensity,
                                                                           sdata.Enabled),
                                                                           sdata.World);
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Static Skinned Mesh
        /****************************************************************************/
        public Creature CreateCreature(GameObjectInstanceData data)
        {
            Creature result = new Creature();

            if (!DefaultObjectInit(result, data)) return result = null;

            CreatureData sdata = (CreatureData)data;

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
                        inputComponentsFactory.CreateKeyboardListenerComponent(result, true),
                        physicsComponentFactory.CreateCapsuleBodyComponent(result,
                                                                          sdata.Mass,
                                                                          sdata.Radius,
                                                                          sdata.Length,
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
        /// Create Menu Button
        /****************************************************************************/
        private MenuButton CreateMenuButton(GameObjectInstanceData data)
        {
            MenuButton result = new MenuButton();

            if (!DefaultObjectInit(result, data)) return result = null;

            MenuButtonData sdata = (MenuButtonData) data;

            result.Init(guiComponentsFactory.createButtonComponent(sdata.Text,
                                                                   sdata.Vertical,
                                                                   sdata.Horizontal,
                                                                   sdata.Size,
                                                                   null),
                                                                        sdata.World);
            //TODO: sprawdzić, czy to już wszystko dla buttona.
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
#if DEBUG
                Diagnostics.PushLog("Creating Object " + data.Type + ", id: " + data.ID +
                                    ", definition: " + data.Definition + ", failed.");
#endif
                return false;
            }
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Water Surface
        /****************************************************************************/
        private WaterSurface CreateWaterSurface(GameObjectInstanceData data)
        {
            WaterSurface result = new WaterSurface();

            if (!DefaultObjectInit(result, data)) return result = null;

            WaterSurfaceData tdata = (WaterSurfaceData)data;

            result.World = tdata.World;
            result.Init(renderingComponentsFactory.CreateWaterSurfaceComponent(result,
                                                                            tdata.Width,
                                                                            tdata.Length,
                                                                            0,
                                                                            tdata.Color,
                                                                            tdata.ColorAmount,
                                                                            tdata.WaveLength,
                                                                            tdata.WaveHeight,
                                                                            tdata.WaveSpeed,
                                                                            tdata.NormalMap,
                                                                            tdata.Bias,
                                                                            tdata.WTextureTiling,
                                                                            tdata.ClipPlaneAdjustment,
                                                                            tdata.SpecularStength));
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Point Light
        /****************************************************************************/
        private PointLight CreatePointLight(GameObjectInstanceData data)
        {
            PointLight result = new PointLight();

            if (!DefaultObjectInit(result, data)) return result = null;

            PointLightData pdata = (PointLightData)data;

            result.Init(renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             pdata.Enabled,
                                                                             pdata.Color,
                                                                             pdata.Specular,
                                                                             pdata.Intensity,
                                                                             pdata.LightRadius,
                                                                             pdata.LinearAttenuation,
                                                                             pdata.QuadraticAttenuation,
                                                                             Vector3.Zero),
                        physicsComponentFactory.CreateSphericalBodyComponent(result,
                                                                             pdata.Mass,
                                                                             pdata.Radius,
                                                                             pdata.Elasticity,
                                                                             pdata.StaticRoughness,
                                                                             pdata.DynamicRoughness,
                                                                             pdata.Immovable,
                                                                             pdata.World,
                                                                             pdata.Translation,
                                                                             pdata.SkinYaw,
                                                                             pdata.SkinPitch,
                                                                             pdata.SkinRoll),
                                                                             pdata.World);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateGlowStick
        /****************************************************************************/
        public GlowStick CreateGlowStick(GameObjectInstanceData data)
        {
            GlowStick result = new GlowStick();

            if (!DefaultObjectInit(result, data)) return result = null;

            GlowStickData gdata = (GlowStickData)data;

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "GlowStick",
                                                                       gdata.Texture,
                                                                       "GlowStick_Specular",
                                                                       "GlowStick_Normals",
                                                                       Renderer.UIntToInstancingMode(gdata.InstancingMode)),
                        
                        physicsComponentFactory.CreateCylindricalBodyComponent(result,
                                                                               gdata.Mass,
                                                                               0.08f,
                                                                               1.0f,
                                                                               gdata.Elasticity,
                                                                               gdata.StaticRoughness,
                                                                               gdata.DynamicRoughness,
                                                                               gdata.Immovable,
                                                                               gdata.World,
                                                                               Vector3.Zero,
                                                                               0,
                                                                               90,
                                                                               0),
                        
                        renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             gdata.Enabled,
                                                                             gdata.Color,
                                                                             false,
                                                                             1,
                                                                             2,
                                                                             0,
                                                                             10,
                                                                             new Vector3(0,0.5f,0)),
                        
                        renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             gdata.Enabled,
                                                                             gdata.Color,
                                                                             false,
                                                                             1,
                                                                             2,
                                                                             0,
                                                                             10,
                                                                             new Vector3(0, -0.5f, 0)),
                                                                             gdata.World);
            
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateSpotLight
        /****************************************************************************/
        public SpotLight CreateSpotLight(GameObjectInstanceData data)
        {
            SpotLight result = new SpotLight();

            if (!DefaultObjectInit(result, data)) return result = null;

            SpotLightData sdata = (SpotLightData)data;

            result.Init(renderingComponentsFactory.CreateSpotLightComponent(result,
                                                                            sdata.Enabled,
                                                                            sdata.Color,
                                                                            sdata.Specular,
                                                                            sdata.Intensity,
                                                                            sdata.Radius,
                                                                            sdata.NearPlane,
                                                                            sdata.FarPlane,
                                                                            sdata.LinearAttenuation,
                                                                            sdata.QuadraticAttenuation,
                                                                            sdata.LocalTransform,
                                                                            sdata.Texture,
                                                                            sdata.ShadowsEnabled),
                                                                            sdata.World);

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateFlashlight
        /****************************************************************************/
        public Flashlight CreateFlashLight(GameObjectInstanceData data)
        {
            Flashlight result = new Flashlight();

            if (!DefaultObjectInit(result, data)) return result = null;

            FlashlightData sdata = (FlashlightData)data;

            Matrix spotlighttrans = Matrix.CreateLookAt(new Vector3(-0.02f, -0.02f, 0.35f), new Vector3(-0.02f, -1, 0.35f), Vector3.Right);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "flashlight",
                                                                       "flashlightdiff",
                                                                       "flashlightspec",
                                                                       String.Empty,
                                                                       Renderer.UIntToInstancingMode(sdata.InstancingMode)),

                        physicsComponentFactory.CreateCylindricalBodyComponent(result,
                                                                               sdata.Mass,
                                                                               0.06f,
                                                                               0.4f,
                                                                               sdata.Elasticity,
                                                                               sdata.StaticRoughness,
                                                                               sdata.DynamicRoughness,
                                                                               sdata.Immovable,
                                                                               sdata.World,
                                                                               new Vector3(-0.15f,0,0),
                                                                               0,
                                                                               0,
                                                                               90),
 
                        renderingComponentsFactory.CreateSpotLightComponent(result,
                                                                            sdata.Enabled,
                                                                            sdata.Color,
                                                                            sdata.Specular,
                                                                            sdata.Intensity,
                                                                            sdata.Radius,
                                                                            sdata.NearPlane,
                                                                            sdata.FarPlane,
                                                                            sdata.LinearAttenuation,
                                                                            sdata.QuadraticAttenuation,
                                                                            spotlighttrans,
                                                                            sdata.AttenuationTexture,
                                                                            sdata.ShadowsEnabled),
                                                                            sdata.World);

            return result;
        }
        /****************************************************************************/

    }
    /********************************************************************************/    

}
/************************************************************************************/