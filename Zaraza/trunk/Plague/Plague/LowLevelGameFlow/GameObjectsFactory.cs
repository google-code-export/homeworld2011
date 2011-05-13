using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

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
        public Dictionary<uint, GameObjectInstance> GameObjects                                       { get; set; }
        public List<GameObjectInstance>             UpdatableObjects                                  { get; set; }
        public Dictionary<uint, KeyValuePair<GameObjectInstance, GameObjectInstanceData>> WaitingRoom { get; set; }
        public bool ProcessWaitingRoom = false;
        public int  ProcessedObjects   = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Create Game Object
        /****************************************************************************/
        public GameObjectInstance Create(GameObjectInstanceData data)
        {
            GameObjectInstance result = null;

            if (!ProcessWaitingRoom) result = (GameObjectInstance)Activator.CreateInstance(data.Type);
            else result = WaitingRoom[data.ID].Key;

            if (!DefaultObjectInit(result, data)) return null;

            UseDefinition(data);

            if (!(bool)this.GetType().InvokeMember("Create" + data.Type.Name,
                                        BindingFlags.InvokeMethod,
                                        null,
                                        this,
                                        new Object[] { result, data })) return null;

            GameObjects.Add(result.ID, result);

            if (result.RequiresUpdate) UpdatableObjects.Add(result);

            if (ProcessWaitingRoom) ++ProcessedObjects;
            
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Default Object Init
        /****************************************************************************/
        private bool DefaultObjectInit(GameObjectInstance gameObject, GameObjectInstanceData data)
        {
            if (data.Owner != 0)
            {
                GameObjectInstance owner = GetObject(data.Owner);
                
                if (owner == null)
                {
                    PushToWaitingRoom(gameObject, data);
                    return false;
                }

                gameObject.Init(data.ID, 
                                data.Definition,
                                data.World,
                                GameObjectInstance.UintToStatus(data.Status),
                                owner, 
                                data.OwnerBone);
                return true;
            }

            gameObject.Init(data.ID, 
                            data.Definition, 
                            data.World, 
                            GameObjectInstance.UintToStatus(data.Status));
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Object
        /****************************************************************************/
        private GameObjectInstance GetObject(uint id)
        {
            if (GameObjects.ContainsKey(id)) return GameObjects[id];

            if (WaitingRoom != null)
            {
                if (WaitingRoom.ContainsKey(id)) return WaitingRoom[id].Key;
            }

            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Push To Waiting Room
        /****************************************************************************/
        private void PushToWaitingRoom(GameObjectInstance gameObject, GameObjectInstanceData data)
        {
            if (ProcessWaitingRoom) return;

            WaitingRoom.Add(data.ID, new KeyValuePair<GameObjectInstance, GameObjectInstanceData>(gameObject, data));
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Use Definition
        /****************************************************************************/
        private void UseDefinition(GameObjectInstanceData data)
        {
            if (String.IsNullOrEmpty(data.Definition)) return;
            if (!gameObjectsDefinitions.ContainsKey(data.Definition)) return;

            GameObjectDefinition definition = gameObjectsDefinitions[data.Definition];

            foreach (KeyValuePair<String, object> value in definition.Properties)
            {
                data.GetType().InvokeMember(value.Key, 
                                            BindingFlags.SetProperty, 
                                            null, 
                                            data, 
                                            new Object[] { value.Value });
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Remove Game Object
        /****************************************************************************/
        public void RemoveGameObject(uint ID)
        {            
            if (GameObjects.ContainsKey(ID))
            {
                GameObjectInstance go = GameObjects[ID];
                
                if (go.RequiresUpdate) UpdatableObjects.Remove(go);

                go.Dispose();

                GameObjects.Remove(ID);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateBurningBarell
        /****************************************************************************/
        public bool CreateBurningBarrel(BurningBarrel result, BurningBarrelData data)
        {           
            
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);
            
 
            result.Init(particleFactory.CreateParticleEmitterComponent(
                                                                result,
                                                                data.blendState,
                                                                data.duration,
                                                                data.durationRandomnes,
                                                                data.emitterVelocitySensitivity,
                                                                data.endVelocity,
                                                                data.gravity,
                                                                data.maxColor,
                                                                data.maxEndSize,
                                                                data.maxHorizontalVelocity,
                                                                data.maxParticles,
                                                                data.maxRotateSpeed,
                                                                data.maxStartSize,
                                                                data.maxVerticalVelocity,
                                                                data.minColor,
                                                                data.minEndSize,
                                                                data.minHorizontalVelocity,
                                                                data.minRotateSpeed,
                                                                data.minStartSize,
                                                                data.minVerticalVelocity,
                                                                data.particleTexture,
                                                                data.particlesPerSecond,
                                                                data.particleTranslation,
                                                                data.World
                                                                ),
                                                                
                                                                renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode),


                        physicsComponentFactory.CreateCylindricalBodyComponent(result,
                                                                                data.Mass,
                                                                                data.Radius,
                                                                                data.Lenght,
                                                                                data.Elasticity,
                                                                                data.StaticRoughness,
                                                                                data.DynamicRoughness,
                                                                                data.Immovable,
                                                                                data.World,
                                                                                data.Translation,
                                                                                data.SkinYaw,
                                                                                data.SkinPitch,
                                                                                data.SkinRoll)
                                                                       );

            return true;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Create Cylindrical Body Mesh
        /****************************************************************************/
        public bool CreateCylindricalBodyMesh(CylindricalBodyMesh result, CylindricalBodyMeshData data)
        {            
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode),

                        physicsComponentFactory.CreateCylindricalBodyComponent( result,
                                                                                data.Mass,
                                                                                data.Radius,
                                                                                data.Lenght,
                                                                                data.Elasticity,
                                                                                data.StaticRoughness,
                                                                                data.DynamicRoughness,
                                                                                data.Immovable,
                                                                                data.World,
                                                                                data.Translation,
                                                                                data.SkinYaw,
                                                                                data.SkinPitch,
                                                                                data.SkinRoll));

            return true;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Create Cylindrical Body Mesh
        /****************************************************************************/
        public bool CreateCylindricalBodyMesh2(CylindricalBodyMesh2 result, CylindricalBodyMeshData2 data)
        {        
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode),

                        physicsComponentFactory.CreateCylindricalBodyComponent2(result,
                                                                                data.Mass,
                                                                                data.Radius,
                                                                                data.Lenght,
                                                                                data.Elasticity,
                                                                                data.StaticRoughness,
                                                                                data.DynamicRoughness,
                                                                                data.Immovable,
                                                                                data.World,
                                                                                data.Translation,
                                                                                data.SkinYaw,
                                                                                data.SkinPitch,
                                                                                data.SkinRoll));

            return true;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Create Spherical Body Mesh
        /****************************************************************************/
        public bool CreateSphericalBodyMesh(SphericalBodyMesh result, SphericalBodyMeshData data)
        {
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode),

                        physicsComponentFactory.CreateSphericalBodyComponent(result,
                                                                            data.Mass,
                                                                            data.Radius,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness,
                                                                            data.Immovable,
                                                                            data.World,
                                                                            data.Translation,
                                                                            data.SkinYaw,
                                                                            data.SkinPitch,
                                                                            data.SkinRoll));

            return true;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Create Square Body Mesh
        /****************************************************************************/
        public bool CreateSquareBodyMesh(SquareBodyMesh result, SquareBodyMeshData data)
        {
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode),

                        physicsComponentFactory.CreateSquareBodyComponent(result,
                                                                            data.Mass,
                                                                            data.Lenght,
                                                                            data.Height,
                                                                            data.Width,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness,
                                                                            data.Immovable,
                                                                            data.World,
                                                                            data.Translation,
                                                                            data.SkinYaw,
                                                                            data.SkinPitch,
                                                                            data.SkinRoll));

            return true;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// Create Static Mesh
        /****************************************************************************/
        public bool CreateStaticMesh(StaticMesh result, StaticMeshData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode)));
            return true;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Create Free Camera
        /****************************************************************************/
        public bool CreateFreeCamera(FreeCamera result, FreeCameraData data)
        {            
            result.Init(renderingComponentsFactory.CreateCameraComponent(result,
                                                                         data.FoV,
                                                                         data.ZNear,
                                                                         data.ZFar),

                         inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                data.ActiveKeyListener),

                         inputComponentsFactory.CreateMouseListenerComponent(result,
                                                                             data.ActiveMouseListener),

                         data.MovementSpeed,
                         data.RotationSpeed);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Linked Camera
        /****************************************************************************/
        public bool CreateLinkedCamera(LinkedCamera result, LinkedCameraData data)
        {            
            result.Init(renderingComponentsFactory.CreateCameraComponent(result,
                                                                         data.FoV,
                                                                         data.ZNear,
                                                                         data.ZFar),

                         inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                data.ActiveKeyListener),


                         inputComponentsFactory.CreateMouseListenerComponent(result, data.ActiveMouseListener),

                         data.MovementSpeed,
                         data.RotationSpeed,
                         data.ZoomSpeed,
                         data.position,
                         data.Target);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Cylindrical Skin Mesh
        /****************************************************************************/
        public bool CreateCylindricalSkinMesh(CylindricalSkinMesh result, CylindricalSkinMeshData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent( result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode)),

                        physicsComponentFactory.CreateCylindricalSkinComponent( result,
                                                                                data.Elasticity,
                                                                                data.StaticRoughness,
                                                                                data.DynamicRoughness,
                                                                                data.World,
                                                                                data.Lenght,
                                                                                data.Radius,
                                                                                data.Translation,
                                                                                data.SkinYaw,
                                                                                data.SkinPitch,
                                                                                data.SkinRoll));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Square Skin Mesh
        /****************************************************************************/
        public bool CreateSquareSkinMesh(SquareSkinMesh result, SquareSkinMeshData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode)),

                        physicsComponentFactory.CreateSquareSkinComponent(result,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness,
                                                                            data.World,
                                                                            data.Lenght,
                                                                            data.Height,
                                                                            data.Width,
                                                                            data.Translation,
                                                                            data.SkinYaw,
                                                                            data.SkinPitch,
                                                                            data.SkinRoll));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Spherical Skin Mesh
        /****************************************************************************/
        public bool CreateSphericalSkinMesh(SphericalSkinMesh result, SphericalSkinMeshData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent( result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode)),
                        
                        physicsComponentFactory.CreateSphericalSkinComponent(result,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness,
                                                                            data.World,
                                                                            data.Radius,
                                                                            data.Translation,
                                                                            data.SkinYaw,
                                                                            data.SkinPitch,
                                                                            data.SkinRoll));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Terrain
        /****************************************************************************/
        public bool CreateTerrain(Terrain result, TerrainData data)
        {
            result.Init(renderingComponentsFactory.CreateTerrainComponent(result,
                                                                            data.HeightMap,
                                                                            data.BaseTexture,
                                                                            data.RTexture,
                                                                            data.GTexture,
                                                                            data.BTexture,
                                                                            data.WeightMap,
                                                                            data.Width,
                                                                            data.Length,
                                                                            data.Height,
                                                                            data.CellSize,
                                                                            data.TextureTiling),

                         physicsComponentFactory.CreateTerrainSkinComponent(result,
                                                                            data.HeightMap,
                                                                            data.Width,
                                                                            data.Length,
                                                                            data.Height,
                                                                            data.CellSize,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Background Terrain
        /****************************************************************************/
        public bool CreateBackgroundTerrain(BackgroundTerrain result, BackgroundTerrainData data)
        {
            result.Init(renderingComponentsFactory.CreateTerrainComponent(result,
                                                                            data.HeightMap,
                                                                            data.BaseTexture,
                                                                            data.RTexture,
                                                                            data.GTexture,
                                                                            data.BTexture,
                                                                            data.WeightMap,
                                                                            data.Width,
                                                                            data.Length,
                                                                            data.Height,
                                                                            data.CellSize,
                                                                            data.TextureTiling));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Sun Light
        /****************************************************************************/
        public bool CreateSunlight(Sunlight result, SunlightData data)
        {
            result.Init(renderingComponentsFactory.CreateSunlightComponent(result,
                                                                           data.Diffuse,
                                                                           data.Intensity,
                                                                           data.Enabled));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Static Skinned Mesh
        /****************************************************************************/
        public bool CreateCreature(Creature result, CreatureData data)
        {
            result.Init(renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              data.Model,
                                                                              data.Diffuse,
                                                                              data.Specular,
                                                                              data.Normals,
                                                                              data.TimeRatio,
                                                                              data.CurrentClip,
                                                                              data.CurrentKeyframe,
                                                                              data.CurrentTime,
                                                                              data.Pause,
                                                                              data.Blend,
                                                                              data.BlendClip,
                                                                              data.BlendKeyframe,
                                                                              data.BlendClipTime,
                                                                              data.BlendDuration,
                                                                              data.BlendTime),

                        inputComponentsFactory.CreateKeyboardListenerComponent(result, true),

                        physicsComponentFactory.CreateCapsuleBodyComponent(result,
                                                                          data.Mass,
                                                                          data.Radius,
                                                                          data.Length,
                                                                          data.Elasticity,
                                                                          data.StaticRoughness,
                                                                          data.DynamicRoughness,
                                                                          data.Immovable,
                                                                          data.World,
                                                                          data.Translation,
                                                                          data.SkinYaw,
                                                                          data.SkinPitch,
                                                                          data.SkinRoll));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Menu Button
        /****************************************************************************/
        public bool CreateMenuButton(MenuButton result, MenuButtonData data)
        {
            result.Init(guiComponentsFactory.createButtonComponent(data.Text,
                                                                   data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size,
                                                                   null));
            //TODO: sprawdzić, czy to już wszystko dla buttona.
            return true;
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Create Water Surface
        /****************************************************************************/
        public bool CreateWaterSurface(WaterSurface result, WaterSurfaceData data)
        {
            result.Init(renderingComponentsFactory.CreateWaterSurfaceComponent(result,
                                                                            data.Width,
                                                                            data.Length,
                                                                            0,
                                                                            data.Color,
                                                                            data.ColorAmount,
                                                                            data.WaveLength,
                                                                            data.WaveHeight,
                                                                            data.WaveSpeed,
                                                                            data.NormalMap,
                                                                            data.Bias,
                                                                            data.WTextureTiling,
                                                                            data.ClipPlaneAdjustment,
                                                                            data.SpecularStength));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Point Light
        /****************************************************************************/
        public bool CreatePointLight(PointLight result, PointLightData data)
        {
            result.Init(renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             data.Enabled,
                                                                             data.Color,
                                                                             data.Specular,
                                                                             data.Intensity,
                                                                             data.LightRadius,
                                                                             data.LinearAttenuation,
                                                                             data.QuadraticAttenuation,
                                                                             Vector3.Zero),

                        physicsComponentFactory.CreateSphericalBodyComponent(result,
                                                                             data.Mass,
                                                                             data.Radius,
                                                                             data.Elasticity,
                                                                             data.StaticRoughness,
                                                                             data.DynamicRoughness,
                                                                             data.Immovable,
                                                                             data.World,
                                                                             data.Translation,
                                                                             data.SkinYaw,
                                                                             data.SkinPitch,
                                                                             data.SkinRoll));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateGlowStick
        /****************************************************************************/
        public bool CreateGlowStick(GlowStick result, GlowStickData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "GlowStick",
                                                                       data.Texture,
                                                                       "GlowStick_Specular",
                                                                       "GlowStick_Normals",
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode)),
                        
                        physicsComponentFactory.CreateCylindricalBodyComponent(result,
                                                                               data.Mass,
                                                                               0.08f,
                                                                               1.0f,
                                                                               data.Elasticity,
                                                                               data.StaticRoughness,
                                                                               data.DynamicRoughness,
                                                                               data.Immovable,
                                                                               data.World,
                                                                               Vector3.Zero,
                                                                               0,
                                                                               90,
                                                                               0),
                        
                        renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             data.Enabled,
                                                                             data.Color,
                                                                             false,
                                                                             0.5f,
                                                                             2,
                                                                             0,
                                                                             5,
                                                                             new Vector3(0,0.5f,0)),
                        
                        renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             data.Enabled,
                                                                             data.Color,
                                                                             false,
                                                                             0.5f,
                                                                             2,
                                                                             0,
                                                                             5,
                                                                             new Vector3(0, -0.5f, 0)));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateSpotLight
        /****************************************************************************/
        public bool CreateSpotLight(SpotLight result, SpotLightData data)
        {
            result.Init(renderingComponentsFactory.CreateSpotLightComponent(result,
                                                                            data.Enabled,
                                                                            data.Color,
                                                                            data.Specular,
                                                                            data.Intensity,
                                                                            data.Radius,
                                                                            data.NearPlane,
                                                                            data.FarPlane,
                                                                            data.LinearAttenuation,
                                                                            data.QuadraticAttenuation,
                                                                            data.LocalTransform,
                                                                            data.Texture,
                                                                            data.ShadowsEnabled));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateFlashlight
        /****************************************************************************/
        public bool CreateFlashlight(Flashlight result, FlashlightData data)
        {
            Matrix spotlighttrans = Matrix.CreateLookAt(new Vector3(-0.02f, -0.02f, 0.35f), new Vector3(-0.02f, -1, 0.35f), Vector3.Right);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "flashlight",
                                                                       "flashlightdiff",
                                                                       "flashlightspec",
                                                                       String.Empty,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode)),

                        null,//physicsComponentFactory.CreateCylindricalBodyComponent(result,
                                                                               //data.Mass,
                                                                               //0.06f,
                                                                               //0.4f,
                                                                               //data.Elasticity,
                                                                               //data.StaticRoughness,
                                                                               //data.DynamicRoughness,
                                                                               //data.Immovable,
                                                                               //data.World,
                                                                               //new Vector3(-0.15f,0,0),
                                                                               //0,
                                                                               //0,
                                                                               //90),
 
                        renderingComponentsFactory.CreateSpotLightComponent(result,
                                                                            data.Enabled,
                                                                            data.Color,
                                                                            data.Specular,
                                                                            data.Intensity,
                                                                            data.Radius,
                                                                            data.NearPlane,
                                                                            data.FarPlane,
                                                                            data.LinearAttenuation,
                                                                            data.QuadraticAttenuation,
                                                                            spotlighttrans,
                                                                            data.AttenuationTexture,
                                                                            data.ShadowsEnabled));
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateMercenary
        /****************************************************************************/
        public bool CreateMercenary(Mercenary result, MercenaryData data)
        {
            result.Init(renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              data.Model,
                                                                              data.Diffuse,
                                                                              data.Specular,
                                                                              data.Normals,
                                                                              data.TimeRatio,
                                                                              data.CurrentClip,
                                                                              data.CurrentKeyframe,
                                                                              data.CurrentTime,
                                                                              data.Pause,
                                                                              data.Blend,
                                                                              data.BlendClip,
                                                                              data.BlendKeyframe,
                                                                              data.BlendClipTime,
                                                                              data.BlendDuration,
                                                                              data.BlendTime),            

                        physicsComponentFactory.CreateCapsuleBodyComponent(result,
                                                                           data.Mass,
                                                                           data.Radius,
                                                                           data.Length,
                                                                           data.Elasticity,
                                                                           data.StaticRoughness,
                                                                           data.DynamicRoughness,
                                                                           data.Immovable,
                                                                           data.World,
                                                                           data.Translation,
                                                                           data.SkinYaw,
                                                                           data.SkinPitch,
                                                                           data.SkinRoll),
                        data.MarkerPosition);

            return true;
        }
        /****************************************************************************/

    }
    /********************************************************************************/    

}
/************************************************************************************/