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
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.ArtificialIntelligence.Controllers;

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
        private GUIComponentsFactory                     guiComponentsFactory       = null;
        private ParticleFactory                          particleFactory            = null;

        private Dictionary<String, GameObjectDefinition> gameObjectsDefinitions     = null;
        private Game game = null;
        private Level level = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Game Objects
        /****************************************************************************/
        private Dictionary<int, GameObjectInstance> GameObjects;
        private List<GameObjectInstance> UpdatableObjects;        

        public Dictionary<int, KeyValuePair<GameObjectInstance, GameObjectInstanceData>> WaitingRoom { get; set; }
        public bool ProcessWaitingRoom = false;
        public int ProcessedObjects = 0;
        /****************************************************************************/
        
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GameObjectsFactory(Dictionary<int, GameObjectInstance> gameObjects,
                                  List<GameObjectInstance> updatableObjects,
                                  Level level)
        {
            GameObjects             = gameObjects;
            UpdatableObjects        = updatableObjects;
            this.level              = level;

            GameObjectInstance.factory = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(RenderingComponentsFactory               renderingComponentsFactory,
                         InputComponentsFactory                   inputComponentsFactory,
                         GUIComponentsFactory                     guiComponentsFactory,
                         Dictionary<String, GameObjectDefinition> gameObjectsDefinitions,
                         PhysicsComponentFactory                  physicsComponentFactory,
                         ParticleFactory                          particleFactory,
                         Game                                     game)
        {
            this.renderingComponentsFactory = renderingComponentsFactory;
            this.inputComponentsFactory     = inputComponentsFactory;
            this.guiComponentsFactory       = guiComponentsFactory;
            this.gameObjectsDefinitions     = gameObjectsDefinitions;
            this.physicsComponentFactory    = physicsComponentFactory;
            this.particleFactory            = particleFactory;
            this.game                       = game;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Game Object
        /****************************************************************************/
        public GameObjectInstance Create(GameObjectInstanceData data)
        {
            //if (data.Type.Equals(typeof(Inventory))) return null;

            GameObjectInstance result = null;

            if (!ProcessWaitingRoom) result = (GameObjectInstance)Activator.CreateInstance(data.Type);
            else result = WaitingRoom[data.ID].Key;

            if (!DefaultObjectInit(result, data)) return null;

            UseDefinition(data);

            try
            {
                if (!(bool)this.GetType().InvokeMember("Create" + data.Type.Name,
                                            BindingFlags.InvokeMethod,
                                            null,
                                            this,
                                            new Object[] { result, data })) return null;
            }
            catch (Exception e)
            {
                Diagnostics.PushLog(e.InnerException.Message);
                return null;
            }

            GameObjects.Add(result.ID, result);

            if (result.RequiresUpdate) UpdatableObjects.Add(result);

            if (ProcessWaitingRoom) ++ProcessedObjects;

            if (result.ID < 0) level.UpdateGlobalGameObjectsData(data);

            result.Broadcast(new CreateEvent());

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
                                data.Name,
                                owner, 
                                data.OwnerBone);
                return true;
            }

            gameObject.Init(data.ID, 
                            data.Definition, 
                            data.World, 
                            GameObjectInstance.UintToStatus(data.Status),
                            data.Name);
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Object
        /****************************************************************************/
        private GameObjectInstance GetObject(int id)
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
        public void RemoveGameObject(int ID)
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
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        physicsComponentFactory.CreateCylindricalBodyComponent( data.EnabledPhysics, result,
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
                                                                                data.SkinRoll),
                        inputComponentsFactory.CreateKeyboardListenerComponent(result, true));

            return true;
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Create Dialog Messages Manager
        /****************************************************************************/
        public bool CreateDialogMessagesManager(DialogMessagesManager result, DialogMessagesManagerData data)
        {

            result.Init(renderingComponentsFactory.CreateFrontEndComponent(result, "MercenariesSet"),
                guiComponentsFactory.CreateLabelComponent(" ", (int)data.TextPosition.X, (int)data.TextPosition.Y),
                new Rectangle(0,128,400,64),data.WindowPosition,data.windowHeight,data.windowWidth,
                data.IconPosition);
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
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        physicsComponentFactory.CreateCylindricalBodyComponent2(data.EnabledPhysics, result,
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
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        physicsComponentFactory.CreateSphericalBodyComponent(data.EnabledPhysics, result,
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
        /// Create Checker
        /****************************************************************************/
        public bool CreateChecker(Checker result, CheckerData data)
        {
                result.Init(data.levelName,
                data.boxWidth,
                data.boxHeight,
                data.numberOfBoxesInLength,
                data.numberOfBoxesInWidth,
                data.distanceBeetwenBoxes,
                this,
                data.boxStartPosition,
                data.framesToTest);

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
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        /// Create Checker Box
        /****************************************************************************/
        public bool CreateCheckerBox(CheckerBox result, CheckerBoxData data)
        {


            result.Init(physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
                                                                            data.Mass,
                                                                            data.Width,
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
                                                                            data.SkinRoll),
                                                                            data.posX,
                                                                            data.posY);

            return true;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// CreateBulldozer
        /****************************************************************************/
        public bool CreateBulldozer(Bulldozer result, BulldozerData data)
        {
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       false),

                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                            data.SkinRoll),
                                                                            
                                                                            data.ActivationRecievers,
                                                                            data.keyId,
                                                                            data.Description,
                                                                            data.DescriptionWindowWidth,
                                                                            data.DescriptionWindowHeight,
                                                                            data.ForceForward,
                                                                            data.MoveDuration);

            return true;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Create BuildingWithRoof
        /****************************************************************************/
        public bool CreateBuildingWithRoof(BuildingWithRoof result, BuildingWithRoofData data)
        {
            InstancingModes InstancingMode1;
            InstancingMode1 = Renderer.UIntToInstancingMode(data.InstancingMode1);

            InstancingModes InstancingMode2;
            InstancingMode2 = Renderer.UIntToInstancingMode(data.InstancingMode2);
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model1,
                                                                       data.Diffuse1,
                                                                       data.Specular1,
                                                                       data.Normals1,
                                                                       InstancingMode1,
                                                                       data.EnabledMesh1,
                                                                       true),
                        renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model2,
                                                                       data.Diffuse2,
                                                                       data.Specular2,
                                                                       data.Normals2,
                                                                       InstancingMode2,
                                                                       data.EnabledMesh2,
                                                                       true),
                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics,
                        result,
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
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        true));
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
                         data.RotationSpeed,
                         data.CurrentCamera);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Linked Camera
        /****************************************************************************/
        public bool CreateLinkedCamera(LinkedCamera result, LinkedCameraData data)
        {
            GameObjectInstance mercMan=null;
           
                mercMan = GetObject(data.MercenariesManager);
           

            if (mercMan == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }
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
                         data.Target,
                         mercMan,
                         data.CurrentCamera);

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
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        physicsComponentFactory.CreateCylindricalSkinComponent(data.EnabledPhysics, result,
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
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        physicsComponentFactory.CreateSquareSkinComponent(data.EnabledPhysics, result,
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
        /// CreateSquareSkin
        /****************************************************************************/
        public bool CreateSquareSkin(SquareSkin result, SquareSkinData data)
        {
            result.Init(physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        /// Create Spherical Skin Mesh
        /****************************************************************************/
        public bool CreateSphericalSkinMesh(SphericalSkinMesh result, SphericalSkinMeshData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent( result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        physicsComponentFactory.CreateSphericalSkinComponent(data.EnabledPhysics, result,
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
                                                                            data.Segments,
                                                                            data.Height,
                                                                            data.TextureTiling),

                         physicsComponentFactory.CreateTerrainSkinComponent(result,
                                                                            data.World,
                                                                            data.HeightMap,
                                                                            data.Segments,
                                                                            data.Segments,
                                                                            data.Height,
                                                                            data.Width / (float)data.Segments,
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
                                                                            data.Segments,
                                                                            data.Height,
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
                                                                           data.Enabled,
                                                                           data.DepthBias,
                                                                           data.ShadowIntensity,
                                                                           data.FogColor,
                                                                           data.FogRange,
                                                                           data.Fog,
                                                                           data.Ambient));

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

                        physicsComponentFactory.CreateCapsuleBodyComponent(data.EnabledPhysics, result,
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
                        
                        data.RotationSpeed,
                        data.MovingSpeed,
                        data.DistancePrecision,
                        data.AnglePrecision,
                        data.AnimationMapping
                        );

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Menu Button
        /****************************************************************************
        public bool CreateMenuButton(MenuButton result, MenuButtonData data)
        {
            result.Init(guiComponentsFactory.createButtonComponent(data.Text,
                                                                   data.Tag,
                                                                   data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size,
                                                                   null));
            //TODO: sprawdzić, czy to już wszystko dla buttona.
            return true;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Create Menu Panel
        /****************************************************************************/
        public bool CreatePanel(Panel result, PanelData data)
        {
            result.Init(guiComponentsFactory.createPanelComponent( data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size));
            //TODO: sprawdzić, czy to już wszystko dla panelu.
            return true;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Create Menu Label
        /****************************************************************************
        public bool CreateLabel(Label result, LabelData data)
        {
            result.Init(guiComponentsFactory.createLabelComponent(data.Text,
                                                                   data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size));
            //TODO: sprawdzić, czy to już wszystko dla panelu.
            return true;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Create Menu Input
        /****************************************************************************/
        public bool CreateInput(GameObjects.Input result, InputData data)
        {
            result.Init(guiComponentsFactory.createInputComponent( data.Text,
                                                                   data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size));
            //TODO: sprawdzić, czy to już wszystko dla panelu.
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
                                                                             Vector3.Zero));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateGlowStick
        /****************************************************************************/
        public bool CreateGlowStick(GlowStick result, GlowStickData data)
        {
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "Misc\\GlowStick",
                                                                       "Misc\\GlowStick_Diffuse",
                                                                       "Misc\\GlowStick_Specular",
                                                                       "Misc\\GlowStick_Normals",
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),

                        physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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
                                                                             data.Intensity,
                                                                             data.Radius,
                                                                             data.LinearAttenuation,
                                                                             data.QuadraticAttenuation,
                                                                             new Vector3(0,0.5f,0)),
                        
                        renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             data.Enabled,
                                                                             data.Color,
                                                                             false,
                                                                             data.Intensity,
                                                                             data.Radius,
                                                                             data.LinearAttenuation,
                                                                             data.QuadraticAttenuation,
                                                                             new Vector3(0, -0.5f, 0)),
                        new Rectangle(0, 670, 50, 50),
                        new Rectangle(50, 670, 64, 32),
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        particleFactory.CreateParticleEmitterComponent(result,
                                                                        data.BlendState,
                                                                        data.Duration,
                                                                        data.DurationRandomnes,
                                                                        data.EmitterVelocitySensitivity,
                                                                        data.VelocityEnd,
                                                                        data.Gravity,
                                                                        data.ColorMax,
                                                                        data.EndSizeMax,
                                                                        data.VelocityHorizontalMax,
                                                                        data.ParticlesMax,
                                                                        data.RotateSpeedMax,
                                                                        data.StartSizeMax,
                                                                        data.VelocityVerticalMax,
                                                                        data.ColorMin,
                                                                        data.EndSizeMin,
                                                                        data.VelocityHorizontalMin,
                                                                        data.RotateSpeedMin,
                                                                        data.StartSizeMin,
                                                                        data.VelocityVerticalMin,
                                                                        data.ParticleTexture,
                                                                        data.ParticlesPerSecond,
                                                                        data.EmitterTranslation,
                                                                        data.World,
                                                                        data.ParticlesEnabled,
                                                                        data.Technique));                        

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
                                                                            data.ShadowsEnabled,
                                                                            data.DepthBias));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateFlashlight
        /****************************************************************************/
        public bool CreateFlashlight(Flashlight result, FlashlightData data)
        {           
            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "Misc\\flashlight",
                                                                       "Misc\\flashlightdiff",
                                                                       "Misc\\flashlightspec",
                                                                       String.Empty,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),

                      physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                        data.SkinRoll),
 
                        renderingComponentsFactory.CreateSpotLightComponent(result,
                                                                            data.Enabled,
                                                                            data.Color,
                                                                            data.SpecularEnabled,
                                                                            data.Intensity,
                                                                            data.Radius,
                                                                            data.NearPlane,
                                                                            data.FarPlane,
                                                                            data.LinearAttenuation,
                                                                            data.QuadraticAttenuation,
                                                                            Matrix.Identity,
                                                                            data.AttenuationTexture,
                                                                            data.ShadowsEnabled,
                                                                            data.DepthBias),
                        new Rectangle(0, 620, 50, 50),
                        new Rectangle(50, 620, 32, 32),
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        particleFactory.CreateParticleEmitterComponent( result,
                                                                        data.BlendState,
                                                                        data.Duration,
                                                                        data.DurationRandomnes,
                                                                        data.EmitterVelocitySensitivity,
                                                                        data.VelocityEnd,
                                                                        data.Gravity,
                                                                        data.ColorMax,
                                                                        data.EndSizeMax,
                                                                        data.VelocityHorizontalMax,
                                                                        data.ParticlesMax,
                                                                        data.RotateSpeedMax,
                                                                        data.StartSizeMax,
                                                                        data.VelocityVerticalMax,
                                                                        data.ColorMin,
                                                                        data.EndSizeMin,
                                                                        data.VelocityHorizontalMin,
                                                                        data.RotateSpeedMin,
                                                                        data.StartSizeMin,
                                                                        data.VelocityVerticalMin,
                                                                        data.ParticleTexture,
                                                                        data.ParticlesPerSecond,
                                                                        data.EmitterTranslation,
                                                                        data.World,
                                                                        data.ParticlesEnabled,
                                                                        data.Technique),
                    data.DamageModulation,
                    data.AccuracyModulation,
                    data.RangeModulation,
                    data.PenetrationModulation,
                    data.RecoilModulation,
                    data.StoppingPowerModulation,
                    data.Genre);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateMercenary
        /****************************************************************************/
        public bool CreateMercenary(Mercenary result, MercenaryData data)
        {

            Dictionary<StorableObject, ItemPosition> Items = new Dictionary<StorableObject, ItemPosition>();

            if (data.Items != null)
            {

                foreach (int[] itemData in data.Items)
                {
                    StorableObject storable = GetObject(itemData[0]) as StorableObject;

                    if (storable == null)
                    {
                        PushToWaitingRoom(result, data);
                        return false;
                    }

                    Items.Add(storable, new ItemPosition(itemData[1], itemData[2]));
                }
            }

            StorableObject currentObject = null;
            if (data.CurrentItem != 0)
            {
                currentObject = (StorableObject)GetObject(data.CurrentItem);
                if (currentObject == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
            }

            Firearm weapon = null;
            if (data.Weapon != 0)
            {
                weapon = (Firearm)GetObject(data.Weapon);
                if (weapon == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
            }

            Firearm sideArm = null;
            if (data.SideArm != 0)
            {
                sideArm = (Firearm)GetObject(data.SideArm);
                if (sideArm == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
            }    

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

                        physicsComponentFactory.CreateCapsuleBodyComponent(data.EnabledPhysics, result,
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

                        renderingComponentsFactory.CreateFrontEndComponent(result,
                                                                           "marker"),
                        data.MarkerPosition,
                        data.RotationSpeed,
                        data.MovingSpeed,
                        data.DistancePrecision,
                        data.AnglePrecision,
                        data.Grip,
                        data.SideArmGrip,
                        data.WeaponGrip,                        
                        data.MaxHP,
                        data.HP,
                        data.Icon,
                        data.InventoryIcon,
                        data.TinySlots,
                        data.Slots,
                        Items,
                        currentObject,
                        weapon,
                        sideArm,
                        data.AnimationMapping
                        );

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateMercenariesManager
        /****************************************************************************/
        public bool CreateMercenariesManager(MercenariesManager result, MercenariesManagerData data)
        {
            LinkedCamera linkedCamera = null;
            if (data.LinkedCamera != 0)
            {
                linkedCamera = GetObject(data.LinkedCamera) as LinkedCamera;
                if (linkedCamera == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
            }

            result.Init(linkedCamera,
                        inputComponentsFactory.CreateKeyboardListenerComponent(result,true),
                        inputComponentsFactory.CreateMouseListenerComponent(result,true),
                        renderingComponentsFactory.CreateFrontEndComponent(result,"MercenariesSet"));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateGameController
        /****************************************************************************/
        public bool CreateGameController(GameController result, GameControllerData data)
        {
            result.Init(game);
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateFirearm
        /****************************************************************************/
        public bool CreateFirearm(Firearm result, FirearmData data)
        {
            Firearm.AttachedAccessory[] accessories = new Firearm.AttachedAccessory[0];
            
            if(data.AvailableAccessories != null)
            {
                accessories = new Firearm.AttachedAccessory[data.AvailableAccessories.Count];

                for(int i = 0; i < accessories.Length; i++)
                {
                    if(data.AvailableAccessories[i].AccessoryID != 0)
                    {
                        Accessory accessory = (Accessory)GetObject(data.AvailableAccessories[i].AccessoryID);
                        if(accessory == null)
                        {
                            PushToWaitingRoom(result,data);
                            return false;
                        }   
                        accessories[i].Accessory = accessory;
                    }
                    else accessories[i].Accessory = null;

                    accessories[i].Genre       = data.AvailableAccessories[i].Genre;
                    accessories[i].Translation = data.AvailableAccessories[i].Translation;
                }
            }

            AmmoClip ammoClip = null;
            if (data.AmmoClip != 0)
            {
                ammoClip = (AmmoClip)GetObject(data.AmmoClip);
                if (ammoClip == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
            }

            Ammunition ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;
            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }            

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),
                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                          data.SkinRoll),
                      renderingComponentsFactory.CreatePointLightComponent(result,
                                                                           false,
                                                                           data.Color,
                                                                           false,
                                                                           data.Intensity,
                                                                           data.LightRadius,
                                                                           data.LinearAttenuation,
                                                                           data.QuadraticAttenuation,
                                                                           data.LightLocalPoistion),
                      accessories,                      
                      data.Icon,
                      data.SlotsIcon,
                      data.Description,
                      data.DescriptionWindowWidth,
                      data.DescriptionWindowHeight,
                      data.SideArm,
                      data.SelectiveFire,
                      data.SelectiveFireMode,
                      
                      particleFactory.CreateParticleEmitterComponent( result,
                                                                      data.BlendState,
                                                                      data.Duration,
                                                                      data.DurationRandomnes,
                                                                      data.EmitterVelocitySensitivity,
                                                                      data.VelocityEnd,
                                                                      data.Gravity,
                                                                      data.ColorMax,
                                                                      data.EndSizeMax,
                                                                      data.VelocityHorizontalMax,
                                                                      data.ParticlesMax,
                                                                      data.RotateSpeedMax,
                                                                      data.StartSizeMax,
                                                                      data.VelocityVerticalMax,
                                                                      data.ColorMin,
                                                                      data.EndSizeMin,
                                                                      data.VelocityHorizontalMin,
                                                                      data.RotateSpeedMin,
                                                                      data.StartSizeMin,
                                                                      data.VelocityVerticalMin,
                                                                      data.ParticleTexture,
                                                                      data.ParticlesPerSecond,
                                                                      data.EmitterTranslation,
                                                                      data.World,
                                                                      data.ParticlesEnabled,
                                                                      data.Technique),

                    data.Condition,
                    data.Reliability,
                    data.RateOfFire,
                    data.Ergonomy,
                    data.ReloadingTime,
                    data.DamageModulation,
                    data.AccuracyModulation,
                    data.RangeModulation,
                    data.PenetrationModulation,
                    data.RecoilModulation,
                    data.StoppingPowerModulation,
                    ammoClip,
                    String.IsNullOrEmpty(data.AmmunitionName) ? null : ammunition.AmmunitionData[ammunition.NameToID[data.AmmunitionName]],
                    data.AmmoClipTranslation,
                    data.On,
                    String.IsNullOrEmpty(data.AmmunitionName) ? null : ammunition.AmmunitionVersionData[ammunition.NameToID[data.AmmunitionName]],
                    data.FireOffset,
                    data.MaxDispersion);

            return true;
        }
        /****************************************************************************/










        /****************************************************************************/
        /// CreateParticleEmitter
        /****************************************************************************/
        public bool CreateParticleEmitter(ParticleEmitter result, ParticleEmitterData data)
        {


            result.Init(particleFactory.CreateParticleEmitterComponent(result,
                                                                      data.BlendState,
                                                                      data.Duration,
                                                                      data.DurationRandomnes,
                                                                      data.EmitterVelocitySensitivity,
                                                                      data.VelocityEnd,
                                                                      data.Gravity,
                                                                      data.ColorMax,
                                                                      data.EndSizeMax,
                                                                      data.VelocityHorizontalMax,
                                                                      data.ParticlesMax,
                                                                      data.RotateSpeedMax,
                                                                      data.StartSizeMax,
                                                                      data.VelocityVerticalMax,
                                                                      data.ColorMin,
                                                                      data.EndSizeMin,
                                                                      data.VelocityHorizontalMin,
                                                                      data.RotateSpeedMin,
                                                                      data.StartSizeMin,
                                                                      data.VelocityVerticalMin,
                                                                      data.ParticleTexture,
                                                                      data.ParticlesPerSecond,
                                                                      data.EmitterTranslation,
                                                                      data.World,
                                                                      data.ParticlesEnabled,
                                                                      data.Technique));

            return true;
        }
        /****************************************************************************/







        /****************************************************************************/
        /// CreateAreaParticleEmitter
        /****************************************************************************/
        public bool CreateAreaParticleEmitter(AreaParticleEmitter result, AreaParticleEmitterData data)
        {


            result.Init(particleFactory.CreateAreaParticleEmitterComponent(result,
                                                                      data.BlendState,
                                                                      data.Duration,
                                                                      data.DurationRandomnes,
                                                                      data.EmitterVelocitySensitivity,
                                                                      data.VelocityEnd,
                                                                      data.Gravity,
                                                                      data.ColorMax,
                                                                      data.EndSizeMax,
                                                                      data.VelocityHorizontalMax,
                                                                      data.ParticlesMax,
                                                                      data.RotateSpeedMax,
                                                                      data.StartSizeMax,
                                                                      data.VelocityVerticalMax,
                                                                      data.ColorMin,
                                                                      data.EndSizeMin,
                                                                      data.VelocityHorizontalMin,
                                                                      data.RotateSpeedMin,
                                                                      data.StartSizeMin,
                                                                      data.VelocityVerticalMin,
                                                                      data.ParticleTexture,
                                                                      data.ParticlesPerSecond,
                                                                      data.EmitterTranslation,
                                                                      data.World,
                                                                      data.AreaSpawnWidth,
                                                                      data.AreaSpawnLength,
                                                                      data.ParticlesEnabled));

            return true;
        }
        /****************************************************************************/





        
        /****************************************************************************/
        /// CreateActionSwitch
        /****************************************************************************/
        public bool CreateActionSwitch(ActionSwitch result, ActionSwitchData data)
        {

            GameObjectInstance feedback = GetObject(data.Feedback);
            if(feedback == null)
            {
                PushToWaitingRoom(result,data);
                return false;
            }

            result.Init(renderingComponentsFactory.CreateFrontEndComponent(result,
                                                                           "switchset"),
                        inputComponentsFactory.CreateMouseListenerComponent(result,
                                                                            true),
                        inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                               true),
                        data.Position,
                        data.Actions,
                        feedback,
                        data.ObjectName);

            return true;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// CreateMainMenu
        /****************************************************************************/
        public bool CreateMainMenu(MainMenu result, MainMenuData data)
        {
            if (data.creditswindowtext == String.Empty || data.creditswindowtext==null)
            {
                data.creditswindowtext = "<empty>";
            }

            if (data.optionswindowtext == String.Empty || data.optionswindowtext == null)
            {
                data.optionswindowtext = "<empty>";
            }

            result.Init(guiComponentsFactory.CreateButtonComponent(data.newGametext,
                                                                   data.newGametag,
                                                                   data.newGamex,
                                                                   data.newGamey,
                                                                   data.newGamewidth,
                                                                   data.newGameheight),
                        data.newGametext,
                        data.newGametag,
                        data.levelToLoad,

                        guiComponentsFactory.CreateButtonComponent(data.optionstext,
                                                                   data.optionstag,
                                                                   data.optionsx,
                                                                   data.optionsy,
                                                                   data.optionswidth,
                                                                   data.optionsheight),
                        data.optionstext,
                        data.optionstag,
                        
                        guiComponentsFactory.CreateButtonComponent(data.creditstext,
                                                                   data.creditstag,
                                                                   data.creditsx,
                                                                   data.creditsy,
                                                                   data.creditswidth,
                                                                   data.creditsheight),
                        data.creditstext,
                        data.creditstag,
                        
                        guiComponentsFactory.CreateButtonComponent(data.exittext,
                                                                   data.exittag,
                                                                   data.exitx,
                                                                   data.exity,
                                                                   data.exitwidth,
                                                                   data.exitheight),
                        data.exittext,
                        data.exittag,
                        
                        renderingComponentsFactory.CreateFrontEndComponent(result, "MainMenuWindow"),
                        data.windowx,
                        data.windowy,
                        data.windowheight,
                        data.windowwidth,
                        guiComponentsFactory.CreateLabelComponent(data.creditswindowtext,
                                                                  data.creditswindowtextx,
                                                                  data.creditswindowtexty),
                        data.creditswindowx,
                        data.creditswindowy,
                        data.creditswindowwidth,
                        data.creditswindowheight,
                        data.creditswindowtext,
                        data.creditswindowtextx,
                        data.creditswindowtexty,
                        guiComponentsFactory.CreateLabelComponent(data.optionswindowtext,
                                                                  data.optionswindowtextx,
                                                                  data.optionswindowtexty),
                        data.optionswindowx,
                        data.optionswindowy,
                        data.optionswindowwidth,
                        data.optionswindowheight,
                        data.optionswindowtext,
                        data.optionswindowtextx,
                        data.optionswindowtexty,
                        renderingComponentsFactory.CreateFrontEndComponent(result,"MainMenuFrame"),
                        
                        renderingComponentsFactory.CreateFrontEndComponent(result,"SplashScreen"));


            return true;
        }
        /****************************************************************************/





        
        /****************************************************************************/
        /// CreateDescriptionWindow
        /****************************************************************************/
        public bool CreateDescriptionWindow(DescriptionWindow result, DescriptionWindowData data)
        { 
            result.Init(guiComponentsFactory.CreateWindowComponent(data.Title,
                                                                   (game.GraphicsDevice.PresentationParameters.BackBufferWidth/2)  - (data.Width/2) ,
                                                                   (game.GraphicsDevice.PresentationParameters.BackBufferHeight/2) - (data.Height/2),
                                                                   data.Width,
                                                                   data.Height,
                                                                   true),
                        guiComponentsFactory.CreateButtonComponent("OK",
                                                                   "OK",
                                                                   2,
                                                                   data.Height - 30, 
                                                                   data.Width  - 4,
                                                                   30),
                        guiComponentsFactory.CreateLabelComponent(data.Text,
                                                                  10,
                                                                  35));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateInventory
        /****************************************************************************/
        public bool CreateInventory(Inventory result, InventoryData data)
        {
            Mercenary          merc        = (data.Mercenary          == 0 ? null : (Mercenary)         GetObject(data.Mercenary));
            Mercenary          merc2       = (data.Mercenary2         == 0 ? null : (Mercenary)         GetObject(data.Mercenary2));
            MercenariesManager mercManager = (data.MercenariesManager == 0 ? null : (MercenariesManager)GetObject(data.MercenariesManager));
            Container          container   = (data.Container          == 0 ? null : (Container)         GetObject(data.Container));

            result.Init(renderingComponentsFactory.CreateFrontEndComponent(result, "InventorySet"),
                        inputComponentsFactory.CreateKeyboardListenerComponent(result, true),
                        inputComponentsFactory.CreateMouseListenerComponent(result, true),
                        merc,
                        merc2,
                        mercManager,
                        container);

            return true;
        }
        /****************************************************************************/

        
        /****************************************************************************/
        /// CreateCompass
        /****************************************************************************/
        public bool CreateCompass(Compass result, CompassData data)
        {
            LinkedCamera camera = (LinkedCamera)GetObject(data.LinkedCamera);

            if (camera == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            result.Init(renderingComponentsFactory.CreateFrontEndComponent(result, "compass"),
                        data.Target,
                        camera,
                        data.Orientation);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateContainer
        /****************************************************************************/
        public bool CreateContainer(Container result, ContainerData data)
        {

            Dictionary<StorableObject, ItemPosition> Items = new Dictionary<StorableObject, ItemPosition>();

            if (data.Items != null)
            {

                foreach (int[] itemData in data.Items)
                {
                    StorableObject storable = GetObject(itemData[0]) as StorableObject;

                    if (storable == null)
                    {
                        PushToWaitingRoom(result, data);
                        return false;
                    }

                    Items.Add(storable, new ItemPosition(itemData[1], itemData[2]));
                }
            }

            result.Init(renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              data.Model,
                                                                              data.Diffuse,
                                                                              data.Specular,
                                                                              data.Normals),
                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics,
                                                                          result,
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
                                                                          data.Yaw,
                                                                          data.Pitch,
                                                                          data.Roll),
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        data.Slots,
                        Items);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateAmmoClip
        /****************************************************************************/
        public bool CreateAmmoClip(AmmoClip result, AmmoClipData data)
        {
            Ammunition ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;

            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            Stack<Bullet> content = new Stack<Bullet>(data.Capacity);

            for(int i = 0; i < data.Capacity  && i < data.Content.Count; i++)
            {
                content.Push(data.Content.ElementAt(i));
            }

            List<String> compability = new List<String>();
            if (data.Compability != null)
            {
                foreach (var sss in data.Compability)
                {
                    compability.Add(sss.String);
                }
            }

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),
                        physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                          data.SkinRoll),                      
                      data.Icon,
                      data.SlotsIcon,
                      data.Description,
                      data.DescriptionWindowWidth,
                      data.DescriptionWindowHeight,                      

                      particleFactory.CreateParticleEmitterComponent(result,
                                                                      data.BlendState,
                                                                      data.Duration,
                                                                      data.DurationRandomnes,
                                                                      data.EmitterVelocitySensitivity,
                                                                      data.VelocityEnd,
                                                                      data.Gravity,
                                                                      data.ColorMax,
                                                                      data.EndSizeMax,
                                                                      data.VelocityHorizontalMax,
                                                                      data.ParticlesMax,
                                                                      data.RotateSpeedMax,
                                                                      data.StartSizeMax,
                                                                      data.VelocityVerticalMax,
                                                                      data.ColorMin,
                                                                      data.EndSizeMin,
                                                                      data.VelocityHorizontalMin,
                                                                      data.RotateSpeedMin,
                                                                      data.StartSizeMin,
                                                                      data.VelocityVerticalMin,
                                                                      data.ParticleTexture,
                                                                      data.ParticlesPerSecond,
                                                                      data.EmitterTranslation,
                                                                      data.World,
                                                                      data.ParticlesEnabled,
                                                                      data.Technique),
                        ammunition.AmmunitionData[ammunition.NameToID[data.Ammunition]],
                        ammunition.AmmunitionVersionData[ammunition.NameToID[data.Ammunition]],
                        content,
                        data.Capacity,
                        compability);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateAmmunition
        /****************************************************************************/
        public bool CreateAmmunition(Ammunition result, AmmunitionData data)
        {
            result.Init(data.AmmunitionInfo);
            
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateAmmoBox
        /****************************************************************************/
        public bool CreateAmmoBox(AmmoBox result, AmmoBoxData data)
        {
            Ammunition ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;

            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            uint ammunitionInfoID;
            AmmunitionInfo info;
            AmmunitionVersionInfo versionInfo;
  
            try
            {
                 ammunitionInfoID = ammunition.NameToID[data.AmmunitionName];
                 info = ammunition.AmmunitionData[ammunitionInfoID];
                 versionInfo = ammunition.AmmunitionVersionData[ammunitionInfoID][data.Version];
            }
            catch 
            {
                return false;
            }

            String Diffuse = "Firearms\\Jackal";

            switch (info.Genre)
            {
                case 1: Diffuse += "Pistol"; break;
                case 2: Diffuse += "Intermediate"; break;
                case 3: Diffuse += "Rifle"; break;
                case 4: Diffuse += "Shotgun"; break;
            }

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                            "Firearms\\JackalAmmo",
                                                            Diffuse,
                                                            String.Empty,
                                                            String.Empty,
                                                            InstancingModes.DynamicInstancing,
                                                            data.EnabledMesh,
                                                            false),
            physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
                                                                2,
                                                                1,
                                                                0.25f,
                                                                0.55f,
                                                                0.1f,
                                                                0.1f,
                                                                0.1f,
                                                                data.Immovable,
                                                                data.World,
                                                                new Vector3(0, 0.175f, 0),
                                                                0,
                                                                0,
                                                                0),
            particleFactory.CreateParticleEmitterComponent(result,
                                                            data.BlendState,
                                                            data.Duration,
                                                            data.DurationRandomnes,
                                                            data.EmitterVelocitySensitivity,
                                                            data.VelocityEnd,
                                                            data.Gravity,
                                                            data.ColorMax,
                                                            data.EndSizeMax,
                                                            data.VelocityHorizontalMax,
                                                            data.ParticlesMax,
                                                            data.RotateSpeedMax,
                                                            data.StartSizeMax,
                                                            data.VelocityVerticalMax,
                                                            data.ColorMin,
                                                            data.EndSizeMin,
                                                            data.VelocityHorizontalMin,
                                                            data.RotateSpeedMin,
                                                            data.StartSizeMin,
                                                            data.VelocityVerticalMin,
                                                            data.ParticleTexture,
                                                            data.ParticlesPerSecond,
                                                            data.EmitterTranslation,
                                                            data.World,
                                                            data.ParticlesEnabled,
                                                            data.Technique),
            info,
            data.PPP,
            versionInfo,
            data.Amount,
            data.Description,
            data.DescriptionWindowWidth,
            data.DescriptionWindowHeight);
                        
            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Accessory
        /****************************************************************************/
        public bool CreateAccessory(Accessory result, AccessoryData data)
        {

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                     data.Model,
                                                                     data.Diffuse,
                                                                     data.Specular,
                                                                     data.Normals,
                                                                     Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                     data.EnabledMesh,
                                                                     false),
                      physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                        data.SkinRoll),
                    data.Icon,
                    data.SlotsIcon,
                    data.Description,
                    data.DescriptionWindowWidth,
                    data.DescriptionWindowHeight,

                    particleFactory.CreateParticleEmitterComponent(result,
                                                                    data.BlendState,
                                                                    data.Duration,
                                                                    data.DurationRandomnes,
                                                                    data.EmitterVelocitySensitivity,
                                                                    data.VelocityEnd,
                                                                    data.Gravity,
                                                                    data.ColorMax,
                                                                    data.EndSizeMax,
                                                                    data.VelocityHorizontalMax,
                                                                    data.ParticlesMax,
                                                                    data.RotateSpeedMax,
                                                                    data.StartSizeMax,
                                                                    data.VelocityVerticalMax,
                                                                    data.ColorMin,
                                                                    data.EndSizeMin,
                                                                    data.VelocityHorizontalMin,
                                                                    data.RotateSpeedMin,
                                                                    data.StartSizeMin,
                                                                    data.VelocityVerticalMin,
                                                                    data.ParticleTexture,
                                                                    data.ParticlesPerSecond,
                                                                    data.EmitterTranslation,
                                                                    data.World,
                                                                    data.ParticlesEnabled,
                                                                    data.Technique),
                  data.DamageModulation,
                  data.AccuracyModulation,
                  data.RangeModulation,
                  data.PenetrationModulation,
                  data.RecoilModulation,
                  data.StoppingPowerModulation,
                  data.Genre);

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create FlammableBarrel
        /****************************************************************************/
        public bool CreateFlammableBarrel(FlammableBarrel result, FlammableBarrelData data)
        {
            InstancingModes InstancingMode;
            InstancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       InstancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),
             physicsComponentFactory.CreateCylindricalBodyComponent( data.EnabledPhysics, result,
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
                                                                                data.SkinRoll),
                        particleFactory.CreateParticleEmitterComponent(result,
                                                            data.BlendState,
                                                            data.Duration,
                                                            data.DurationRandomnes,
                                                            data.EmitterVelocitySensitivity,
                                                            data.VelocityEnd,
                                                            data.Gravity,
                                                            data.ColorMax,
                                                            data.EndSizeMax,
                                                            data.VelocityHorizontalMax,
                                                            data.ParticlesMax,
                                                            data.RotateSpeedMax,
                                                            data.StartSizeMax,
                                                            data.VelocityVerticalMax,
                                                            data.ColorMin,
                                                            data.EndSizeMin,
                                                            data.VelocityHorizontalMin,
                                                            data.RotateSpeedMin,
                                                            data.StartSizeMin,
                                                            data.VelocityVerticalMin,
                                                            data.ParticleTexture,
                                                            data.ParticlesPerSecond,
                                                            data.EmitterTranslation,
                                                            data.World,
                                                            data.ParticlesEnabled,
                                                            data.Technique),
                     renderingComponentsFactory.CreatePointLightComponent(result,
                                                                           data.LightEnabled,
                                                                           data.Color,
                                                                           false,
                                                                           0,
                                                                           data.LightRadius,
                                                                           data.LinearAttenuation,
                                                                           data.QuadraticAttenuation,
                                                                           data.LightLocalPoistion));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Fire
        /****************************************************************************/
        public bool CreateFire(Fire result, FireData data)
        {


            result.Init(
                        particleFactory.CreateParticleEmitterComponent(result,
                                                            data.BlendState,
                                                            data.Duration,
                                                            data.DurationRandomnes,
                                                            data.EmitterVelocitySensitivity,
                                                            data.VelocityEnd,
                                                            data.Gravity,
                                                            data.ColorMax,
                                                            data.EndSizeMax,
                                                            data.VelocityHorizontalMax,
                                                            data.ParticlesMax,
                                                            data.RotateSpeedMax,
                                                            data.StartSizeMax,
                                                            data.VelocityVerticalMax,
                                                            data.ColorMin,
                                                            data.EndSizeMin,
                                                            data.VelocityHorizontalMin,
                                                            data.RotateSpeedMin,
                                                            data.StartSizeMin,
                                                            data.VelocityVerticalMin,
                                                            data.ParticleTexture,
                                                            data.ParticlesPerSecond,
                                                            data.EmitterTranslation,
                                                            data.World,
                                                            data.ParticlesEnabled,
                                                            data.Technique),
                     renderingComponentsFactory.CreatePointLightComponent(result,
                                                                           data.LightEnabled,
                                                                           data.Color,
                                                                           false,
                                                                           0,
                                                                           data.LightRadius,
                                                                           data.LinearAttenuation,
                                                                           data.QuadraticAttenuation,
                                                                           data.LightLocalPoistion),
                                                                           data.Frequency,
                                                                           data.IntensityRange
                                                                           );

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateFireSelector
        /****************************************************************************/
        public bool CreateFireSelector(FireSelector result, FireSelectorData data)
        {

            result.Init(renderingComponentsFactory.CreateFrontEndComponent(result, "selector"),
                        inputComponentsFactory.CreateMouseListenerComponent(result, true),
                        (Firearm)GetObject(data.Firearm));

            return true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateBloom
        /****************************************************************************/
        public bool CreateBloom(Bloom result, BloomData data)
        {

            result.Init(data.BloomIntensity, data.BaseIntensity, data.BloomSaturation, data.BaseSaturation, data.BloomThreshold);

            return true;
        }
        /****************************************************************************/

    }
    /********************************************************************************/    

}
/************************************************************************************/