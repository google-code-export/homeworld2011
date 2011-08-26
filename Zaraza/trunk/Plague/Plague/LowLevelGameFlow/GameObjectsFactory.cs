using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using PlagueEngine.GUI;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.Particles;
using PlagueEngine.Physics;
using PlagueEngine.Rendering;

/************************************************************************************/
// PlagueEngine.LowLevelGameFlow
/************************************************************************************/

namespace PlagueEngine.LowLevelGameFlow
{
    /********************************************************************************/
    // Game Objects Factory
    /********************************************************************************/

    internal class GameObjectsFactory
    {
        /****************************************************************************/
        // Fields
        /****************************************************************************/
        private RenderingComponentsFactory _renderingComponentsFactory;
        private InputComponentsFactory _inputComponentsFactory;
        private PhysicsComponentFactory _physicsComponentFactory;
        private GUIComponentsFactory _guiComponentsFactory;
        private ParticleFactory _particleFactory;

        private Dictionary<String, GameObjectDefinition> _gameObjectsDefinitions;
        private Game _game;
        private readonly Level _level;
        /****************************************************************************/

        /****************************************************************************/
        // Game Objects
        /****************************************************************************/
        private readonly Dictionary<int, GameObjectInstance> _gameObjects;
        private readonly List<GameObjectInstance> _updatableObjects;

        public Dictionary<int, KeyValuePair<GameObjectInstance, GameObjectInstanceData>> WaitingRoom { get; set; }

        public bool ProcessWaitingRoom;
        public int ProcessedObjects;
        /****************************************************************************/

        /****************************************************************************/
        // Constructor
        /****************************************************************************/

        public GameObjectsFactory(Dictionary<int, GameObjectInstance> gameObjects,
                                  List<GameObjectInstance> updatableObjects,
                                  Level level)
        {
            _gameObjects = gameObjects;
            _updatableObjects = updatableObjects;
            _level = level;

            GameObjectInstance.Factory = this;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Init
        /****************************************************************************/

        public void Init(RenderingComponentsFactory renderingComponentsFactory,
                         InputComponentsFactory inputComponentsFactory,
                         GUIComponentsFactory guiComponentsFactory,
                         Dictionary<String, GameObjectDefinition> gameObjectsDefinitions,
                         PhysicsComponentFactory physicsComponentFactory,
                         ParticleFactory particleFactory,
                         Game game)
        {
            _renderingComponentsFactory = renderingComponentsFactory;
            _inputComponentsFactory = inputComponentsFactory;
            _guiComponentsFactory = guiComponentsFactory;
            _gameObjectsDefinitions = gameObjectsDefinitions;
            _physicsComponentFactory = physicsComponentFactory;
            _particleFactory = particleFactory;
            _game = game;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Game Object
        /****************************************************************************/

        public GameObjectInstance Create(GameObjectInstanceData data)
        {
            //if (data.Type.Equals(typeof(Inventory))) return null;

            var result = !ProcessWaitingRoom ? (GameObjectInstance)Activator.CreateInstance(data.Type) : WaitingRoom[data.ID].Key;

            if (!DefaultObjectInit(result, data)) return null;

            UseDefinition(data);

            try
            {
                if (!(bool)GetType().InvokeMember("Create" + data.Type.Name,
                                            BindingFlags.InvokeMethod,
                                            null,
                                            this,
                                            new Object[] { result, data })) return null;
            }
            catch (Exception e)
            {
#if DEBUG
                Diagnostics.Fatal("There was an error when object " + data.ID + " was creting: " + e.InnerException.Message);
#endif
                return null;
            }

            _gameObjects.Add(result.ID, result);

            if (result.RequiresUpdate) _updatableObjects.Add(result);

            if (ProcessWaitingRoom) ++ProcessedObjects;

            if (result.ID < 0) _level.UpdateGlobalGameObjectsData(data);

            result.Broadcast(new CreateEvent());

            return result;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Default Object Init
        /****************************************************************************/

        private bool DefaultObjectInit(GameObjectInstance gameObject, GameObjectInstanceData data)
        {
            if (data.Owner != 0)
            {
                var owner = GetObject(data.Owner);

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
        // Get Object
        /****************************************************************************/

        private GameObjectInstance GetObject(int id)
        {
            if (_gameObjects.ContainsKey(id)) return _gameObjects[id];

            if (WaitingRoom != null && WaitingRoom.ContainsKey(id)) return WaitingRoom[id].Key;

            return null;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Push To Waiting Room
        /****************************************************************************/

        private void PushToWaitingRoom(GameObjectInstance gameObject, GameObjectInstanceData data)
        {
            if (ProcessWaitingRoom) return;
#if DEBUG
            Diagnostics.Info("Object " + data.ID + " type of " + gameObject.GetType().Name + " has been moved to WaitingRoom.");
#endif
            WaitingRoom.Add(data.ID, new KeyValuePair<GameObjectInstance, GameObjectInstanceData>(gameObject, data));
        }

        /****************************************************************************/

        /****************************************************************************/
        // Use Definition
        /****************************************************************************/

        private void UseDefinition(GameObjectInstanceData data)
        {
            if (String.IsNullOrEmpty(data.Definition)) return;
            if (!_gameObjectsDefinitions.ContainsKey(data.Definition)) return;

            var definition = _gameObjectsDefinitions[data.Definition];

            foreach (var value in definition.Properties)
            {
                data.GetType().InvokeMember(value.Key,
                                            BindingFlags.SetProperty,
                                            null,
                                            data,
                                            new[] { value.Value });
            }
        }

        /****************************************************************************/

        /****************************************************************************/
        // Remove Game Object
        /****************************************************************************/

        public void RemoveGameObject(int id)
        {
            if (!_gameObjects.ContainsKey(id)) return;
            var go = _gameObjects[id];

            if (go.RequiresUpdate) _updatableObjects.Remove(go);

            go.Dispose();

            _gameObjects.Remove(id);
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Cylindrical Body Mesh
        /****************************************************************************/

        public bool CreateCylindricalBodyMesh(CylindricalBodyMesh result, CylindricalBodyMeshData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        instancingMode,
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        _physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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
        // Create Dialog Messages Manager
        /****************************************************************************/

        public bool CreateDialogMessagesManager(DialogMessagesManager result, DialogMessagesManagerData data)
        {
            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "MercenariesSet"),
                _guiComponentsFactory.CreateLabelComponent(" ", (int)data.TextPosition.X, (int)data.TextPosition.Y),
                new Rectangle(0, 128, 400, 64), data.WindowPosition, data.windowHeight, data.windowWidth,
                data.IconPosition);
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create InGameMenu
        /****************************************************************************/

        public bool CreateInGameMenu(InGameMenu result, InGameMenuData data)
        {
            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "MainMenuFrame"));
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create MovieClip
        /****************************************************************************/

        public bool CreateMovieClip(MovieClip result, MovieClipData data)
        {
            result.Init(_game.ContentManager.Load<Video>(data.videoName), data.videoName, _renderingComponentsFactory.CreateFrontEndComponent(result, null),
                  _renderingComponentsFactory.CreateFrontEndComponent(result, "SplashScreen"));
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Cylindrical Body Mesh
        /****************************************************************************/

        public bool CreateCylindricalBodyMesh2(CylindricalBodyMesh2 result, CylindricalBodyMeshData2 data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        _physicsComponentFactory.CreateCylindricalBodyComponent2(data.EnabledPhysics, result,
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
        // Create Spherical Body Mesh
        /****************************************************************************/

        public bool CreateSphericalBodyMesh(SphericalBodyMesh result, SphericalBodyMeshData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        _physicsComponentFactory.CreateSphericalBodyComponent(data.EnabledPhysics, result,
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
        // Create Checker
        /****************************************************************************/

        public bool CreateChecker(Checker result, CheckerData data)
        {
            result.Init(data.LevelName,
            data.BoxWidth,
            data.BoxHeight,
            data.NumberOfBoxesInLength,
            data.NumberOfBoxesInWidth,
            data.DistanceBeetwenBoxes,
            this,
            data.BoxStartPosition,
            data.FramesToTest);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateEndGameTrigerr
        /****************************************************************************/

        public bool CreateEndGameTrigerr(EndGameTrigerr result, EndGameTrigerrData data)
        {
            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        // Create Square Body Mesh
        /****************************************************************************/

        public bool CreateSquareBodyMesh(SquareBodyMesh result, SquareBodyMeshData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        // Create Checker Box
        /****************************************************************************/

        public bool CreateCheckerBox(CheckerBox result, CheckerBoxData data)
        {
            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        // CreateBulldozer
        /****************************************************************************/

        public bool CreateBulldozer(Bulldozer result, BulldozerData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       false),

                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        // CreateDoors
        /****************************************************************************/

        public bool CreateDoors(Doors result, DoorsData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                            data.keyId,
                                                                            data.Description,
                                                                            data.DescriptionWindowWidth,
                                                                            data.DescriptionWindowHeight,

                                             _renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       false));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateRadioBox
        /****************************************************************************/

        public bool CreateRadioBox(RadioBox result, RadioBoxData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                                                                            data.keyId,
                                                                            data.Description,
                                                                            data.DescriptionWindowWidth,
                                                                            data.DescriptionWindowHeight,
                                               _renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       false));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateBoomTriger
        /****************************************************************************/

        public bool CreateBoomTriger(BoomTriger result, BoomTrigerData data)
        {
            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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

                                                                            data.keyId,
                                                                            data.Description,
                                                                            data.DescriptionWindowWidth,
                                                                            data.DescriptionWindowHeight,
                                                                            data.Stones,
                                                                            _level,
                                                                            data.timer,
                                                                            data.explosionForce,
                                                                            data.explosionRadius,
                                                                            data.explosionPosition,
                            _particleFactory.CreateParticleEmitterComponent(result,
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
        // Create BuildingWithRoof
        /****************************************************************************/

        public bool CreateBuildingWithRoof(BuildingWithRoof result, BuildingWithRoofData data)
        {
            var instancingMode1 = Renderer.UIntToInstancingMode(data.InstancingMode1);

            var instancingMode2 = Renderer.UIntToInstancingMode(data.InstancingMode2);
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model1,
                                                                       data.Diffuse1,
                                                                       data.Specular1,
                                                                       data.Normals1,
                                                                       instancingMode1,
                                                                       data.EnabledMesh1,
                                                                       true),
                        _renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model2,
                                                                       data.Diffuse2,
                                                                       data.Specular2,
                                                                       data.Normals2,
                                                                       instancingMode2,
                                                                       data.EnabledMesh2,
                                                                       true),
                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics,
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
        // Create Static Mesh
        /****************************************************************************/

        public bool CreateStaticMesh(StaticMesh result, StaticMeshData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
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
        // Create Free Camera
        /****************************************************************************/

        public bool CreateFreeCamera(FreeCamera result, FreeCameraData data)
        {
            result.Init(_renderingComponentsFactory.CreateCameraComponent(result,
                                                                         data.FoV,
                                                                         data.ZNear,
                                                                         data.ZFar),

                         _inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                data.ActiveKeyListener),

                         _inputComponentsFactory.CreateMouseListenerComponent(result,
                                                                             data.ActiveMouseListener),

                         data.MovementSpeed,
                         data.RotationSpeed,
                         data.CurrentCamera);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Linked Camera
        /****************************************************************************/

        public bool CreateLinkedCamera(LinkedCamera result, LinkedCameraData data)
        {
            var mercMan = GetObject(data.MercenariesManager);

            if (mercMan == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }
            result.Init(_renderingComponentsFactory.CreateCameraComponent(result,
                                                                         data.FoV,
                                                                         data.ZNear,
                                                                         data.ZFar),

                         _inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                                data.ActiveKeyListener),

                         _inputComponentsFactory.CreateMouseListenerComponent(result, data.ActiveMouseListener),

                         data.MovementSpeed,
                         data.RotationSpeed,
                         data.ZoomSpeed,
                         data.position,
                         data.Target,
                         mercMan,
                         data.CurrentCamera,
                         data.HeightRange,
                         data.Xmax,
                         data.Xmin,
                         data.Ymax,
                         data.Ymin,
                         data.Zmax,
                         data.Zmin,
                         data.AngleMin);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Cylindrical Skin Mesh
        /****************************************************************************/

        public bool CreateCylindricalSkinMesh(CylindricalSkinMesh result, CylindricalSkinMeshData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        _physicsComponentFactory.CreateCylindricalSkinComponent(data.EnabledPhysics, result,
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
        // Create Square Skin Mesh
        /****************************************************************************/

        public bool CreateSquareSkinMesh(SquareSkinMesh result, SquareSkinMeshData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        _physicsComponentFactory.CreateSquareSkinComponent(data.EnabledPhysics, result,
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
        // CreateSquareSkin
        /****************************************************************************/

        public bool CreateSquareSkin(SquareSkin result, SquareSkinData data)
        {
            result.Init(_physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
        // Create Spherical Skin Mesh
        /****************************************************************************/

        public bool CreateSphericalSkinMesh(SphericalSkinMesh result, SphericalSkinMeshData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                        data.Model,
                                                                        data.Diffuse,
                                                                        data.Specular,
                                                                        data.Normals,
                                                                        Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                        data.EnabledMesh,
                                                                        data.Static),

                        _physicsComponentFactory.CreateSphericalSkinComponent(data.EnabledPhysics, result,
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
        // Create Terrain
        /****************************************************************************/

        public bool CreateTerrain(Terrain result, TerrainData data)
        {
            result.Init(_renderingComponentsFactory.CreateTerrainComponent(result,
                                                                            data.HeightMap,
                                                                            data.BaseTexture,
                                                                            data.RTexture,
                                                                            data.GTexture,
                                                                            data.BTexture,
                                                                            data.WeightMap,
                                                                            data.NormalMap,
                                                                            data.Width,
                                                                            data.Segments,
                                                                            data.Height,
                                                                            data.TextureTiling),

                         _physicsComponentFactory.CreateTerrainSkinComponent(result,
                                                                            data.World,
                                                                            data.HeightMap,
                                                                            data.Segments,
                                                                            data.Segments,
                                                                            data.Height,
                                                                            data.Width / data.Segments,
                                                                            data.Elasticity,
                                                                            data.StaticRoughness,
                                                                            data.DynamicRoughness));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Background Terrain
        /****************************************************************************/

        public bool CreateBackgroundTerrain(BackgroundTerrain result, BackgroundTerrainData data)
        {
            result.Init(_renderingComponentsFactory.CreateTerrainComponent(result,
                                                                            data.HeightMap,
                                                                            data.BaseTexture,
                                                                            data.RTexture,
                                                                            data.GTexture,
                                                                            data.BTexture,
                                                                            data.WeightMap,
                                                                            data.NormalMap,
                                                                            data.Width,
                                                                            data.Segments,
                                                                            data.Height,
                                                                            data.TextureTiling));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Sun Light
        /****************************************************************************/

        public bool CreateSunlight(Sunlight result, SunlightData data)
        {
            result.Init(_renderingComponentsFactory.CreateSunlightComponent(result,
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
        // Create Static Skinned Mesh
        /****************************************************************************/

        public bool CreateCreature(Creature result, CreatureData data)
        {
            result.Init(_renderingComponentsFactory.CreateSkinnedMeshComponent(result,
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

                        _inputComponentsFactory.CreateKeyboardListenerComponent(result, true),

                        _physicsComponentFactory.CreateCapsuleBodyComponent(data.EnabledPhysics, result,
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
                        data.GetAnimationMapping()
                        );

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Menu Button
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
        // Create Menu Panel
        /****************************************************************************/

        public bool CreatePanel(Panel result, PanelData data)
        {
            result.Init(_guiComponentsFactory.createPanelComponent(data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size));
            //TODO: sprawdzić, czy to już wszystko dla panelu.
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Menu Label
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
        // Create Menu Input
        /****************************************************************************/

        public bool CreateInput(GameObjects.Input result, InputData data)
        {
            result.Init(_guiComponentsFactory.createInputComponent(data.Text,
                                                                   data.Vertical,
                                                                   data.Horizontal,
                                                                   data.Size));
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // Create Water Surface
        /****************************************************************************/

        public bool CreateWaterSurface(WaterSurface result, WaterSurfaceData data)
        {
            result.Init(_renderingComponentsFactory.CreateWaterSurfaceComponent(result,
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
        // Create Point Light
        /****************************************************************************/

        public bool CreatePointLight(PointLight result, PointLightData data)
        {
            result.Init(_renderingComponentsFactory.CreatePointLightComponent(result,
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
        // CreateGlowStick
        /****************************************************************************/

        public bool CreateGlowStick(GlowStick result, GlowStickData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "Misc\\GlowStick",
                                                                       "Misc\\GlowStick_Diffuse",
                                                                       "Misc\\GlowStick_Specular",
                                                                       "Misc\\GlowStick_Normals",
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),

                        _physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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

                        _renderingComponentsFactory.CreatePointLightComponent(result,
                                                                             data.Enabled,
                                                                             data.Color,
                                                                             false,
                                                                             data.Intensity,
                                                                             data.Radius,
                                                                             data.LinearAttenuation,
                                                                             data.QuadraticAttenuation,
                                                                             new Vector3(0, 0.5f, 0)),

                        _renderingComponentsFactory.CreatePointLightComponent(result,
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
                        _particleFactory.CreateParticleEmitterComponent(result,
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
        // CreateSpotLight
        /****************************************************************************/

        public bool CreateSpotLight(SpotLight result, SpotLightData data)
        {
            result.Init(_renderingComponentsFactory.CreateSpotLightComponent(result,
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
        // CreateFlashlight
        /****************************************************************************/

        public bool CreateFlashlight(Flashlight result, FlashlightData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       "Misc\\flashlight",
                                                                       "Misc\\flashlightdiff",
                                                                       "Misc\\flashlightspec",
                                                                       String.Empty,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),

                      _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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

                        _renderingComponentsFactory.CreateSpotLightComponent(result,
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
                        _particleFactory.CreateParticleEmitterComponent(result,
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
        // CreateMercenary
        /****************************************************************************/

        public bool CreateMercenary(Mercenary result, MercenaryData data)
        {
            var items = new Dictionary<StorableObject, ItemPosition>();

            if (data.Items != null)
            {
                foreach (var itemData in data.Items)
                {
                    var storable = GetObject(itemData[0]) as StorableObject;

                    if (storable == null)
                    {
                        PushToWaitingRoom(result, data);
                        return false;
                    }

                    items.Add(storable, new ItemPosition(itemData[1], itemData[2]));
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

            result.Init(_renderingComponentsFactory.CreateSkinnedMeshComponent(result,
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

                        _physicsComponentFactory.CreateCapsuleBodyComponent(data.EnabledPhysics, result,
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

                        _renderingComponentsFactory.CreateFrontEndComponent(result, "marker"),
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
                        items,
                        currentObject,
                        weapon,
                        sideArm,
                        data.GetAnimationMapping()
                        );

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateMercenariesManager
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
                        _inputComponentsFactory.CreateKeyboardListenerComponent(result, true),
                        _inputComponentsFactory.CreateMouseListenerComponent(result, true),
                        _renderingComponentsFactory.CreateFrontEndComponent(result, "MercenariesSet"),
                        _renderingComponentsFactory.CreateFogOfWarComponent(result,
                                                                            "spot",
                                                                            data.SpotSize,
                                                                            data.FogScale,
                                                                            data.FogSize,
                                                                            data.Enabled),
                                                                            data.IgnoreMercenaries);
            MercenaryActivationTrigger.MercenariesManager = result;
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateMercenaryActivationTrigger
        /****************************************************************************/

        public bool CreateMercenaryActivationTrigger(MercenaryActivationTrigger result, MercenaryActivationTriggerData data)
        {
            List<Mercenary> tmp = new List<Mercenary>();
            Mercenary hlp = null;
            foreach (int id in data.MercIDs)
            {
                hlp = this.GetObject(id) as Mercenary;
                if (hlp == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
                tmp.Add(hlp);
            }
            result.Init(tmp.ToArray(),
                _physicsComponentFactory.CreateSphericalBodyComponent(true,
                                                                      result,
                                                                      0,
                                                                      data.Radius,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      true,
                                                                      data.World,
                                                                      Vector3.Zero,
                                                                      0,
                                                                      0,
                                                                      0));

            return true;
        }

        /****************************************************************************/

        // CreateFadeInOut
        /****************************************************************************/

        public bool CreateFadeInOut(FadeInOut result, FadeInOutData data)
        {
            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "black"), data.Time,
                _renderingComponentsFactory.CreateFrontEndComponent(result, "gameover"));
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/

        /****************************************************************************/
        // CreateMonologueTrigger
        /****************************************************************************/

        public bool CreateMonologueTrigger(MonologueTrigger result, MonologueTriggerData data)
        {
            result.Init(_physicsComponentFactory.CreateSphericalBodyComponent(true,
                                                                      result,
                                                                      0,
                                                                      data.Radius,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      true,
                                                                      data.World,
                                                                      Vector3.Zero,
                                                                      0,
                                                                      0,
                                                                      0),
                        data.GetMessages(),
                        data.WaitTimes);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateDialogueTrigger
        /****************************************************************************/

        public bool CreateDialogueTrigger(DialogueTrigger result, DialogueTriggerData data)
        {
            List<Mercenary> tmp = new List<Mercenary>();
            List<Mercenary> tmpIgnored = new List<Mercenary>();
            Mercenary hlp = null;
            foreach (int id in data.MercIDs)
            {
                hlp = this.GetObject(id) as Mercenary;
                if (hlp == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
                tmp.Add(hlp);
            }

            foreach (int id in data.IgnoredMercIDs)
            {
                hlp = this.GetObject(id) as Mercenary;
                if (hlp == null)
                {
                    PushToWaitingRoom(result, data);
                    return false;
                }
                tmpIgnored.Add(hlp);
            }
            result.Init(_physicsComponentFactory.CreateSphericalBodyComponent(true,
                                                                      result,
                                                                      0,
                                                                      data.Radius,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      true,
                                                                      data.World,
                                                                      Vector3.Zero,
                                                                      0,
                                                                      0,
                                                                      0),
                        data.GetMessages(),
                        data.WaitTimes,
                        tmp,
                        tmpIgnored);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/

        // CreateGameController
        /****************************************************************************/

        public bool CreateGameController(GameController result, GameControllerData data)
        {
            result.Init(_game);
            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateFirearm
        /****************************************************************************/

        public bool CreateFirearm(Firearm result, FirearmData data)
        {
            var accessories = new Firearm.AttachedAccessory[0];

            if (data.AvailableAccessories != null)
            {
                accessories = new Firearm.AttachedAccessory[data.AvailableAccessories.Count];

                for (var i = 0; i < accessories.Length; i++)
                {
                    if (data.AvailableAccessories[i].AccessoryID != 0)
                    {
                        var accessory = (Accessory)GetObject(data.AvailableAccessories[i].AccessoryID);
                        if (accessory == null)
                        {
                            PushToWaitingRoom(result, data);
                            return false;
                        }
                        accessories[i].Accessory = accessory;
                    }
                    else accessories[i].Accessory = null;

                    accessories[i].Genre = data.AvailableAccessories[i].Genre;
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

            var ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;
            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),
                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                      _renderingComponentsFactory.CreatePointLightComponent(result,
                                                                           false,
                                                                           data.Color,
                                                                           false,
                                                                           data.Intensity,
                                                                           data.LightRadius,
                                                                           data.LinearAttenuation,
                                                                           data.QuadraticAttenuation,
                                                                           data.LightLocalPoistion),
                      _particleFactory.CreateTracerParticleComponent(result,
                                                                    data.TBlendState,
                                                                    data.TColorMax,
                                                                    data.TEndSizeMax,
                                                                    data.TStartSizeMax,
                                                                    data.TColorMin,
                                                                    data.TEndSizeMin,
                                                                    data.TStartSizeMin,
                                                                    data.TParticleTexture,
                                                                    data.TParticlesEnabled,
                                                                    data.TTechnique,
                                                                    data.TSpeed),
                      accessories,
                      data.Icon,
                      data.SlotsIcon,
                      data.Description,
                      data.DescriptionWindowWidth,
                      data.DescriptionWindowHeight,
                      data.SideArm,
                      data.SelectiveFire,
                      data.SelectiveFireMode,

                      _particleFactory.CreateParticleEmitterComponent(result,
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
        // CreateParticleEmitter
        /****************************************************************************/

        public bool CreateParticleEmitter(ParticleEmitter result, ParticleEmitterData data)
        {
            result.Init(_particleFactory.CreateParticleEmitterComponent(result,
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
        // CreateAreaParticleEmitter
        /****************************************************************************/

        public bool CreateAreaParticleEmitter(AreaParticleEmitter result, AreaParticleEmitterData data)
        {
            result.Init(_particleFactory.CreateAreaParticleEmitterComponent(result,
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
        // CreateActionSwitch
        /****************************************************************************/

        public bool CreateActionSwitch(ActionSwitch result, ActionSwitchData data)
        {
            var feedback = GetObject(data.Feedback);
            if (feedback == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result,
                                                                           "switchset"),
                        _inputComponentsFactory.CreateMouseListenerComponent(result,
                                                                            true),
                        _inputComponentsFactory.CreateKeyboardListenerComponent(result,
                                                                               true),
                        data.Position,
                        data.Actions,
                        feedback,
                        data.ObjectName);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateMainMenu
        /****************************************************************************/

        public bool CreateMainMenu(MainMenu result, MainMenuData data)
        {
            if (string.IsNullOrEmpty(data.creditswindowtext))
            {
                data.creditswindowtext = "<empty>";
            }

            if (string.IsNullOrEmpty(data.optionswindowtext))
            {
                data.optionswindowtext = "<empty>";
            }

            result.Init(_guiComponentsFactory.CreateButtonComponent(data.newGametext,
                                                                   data.newGametag,
                                                                   data.newGamex,
                                                                   data.newGamey,
                                                                   data.newGamewidth,
                                                                   data.newGameheight),
                        data.newGametext,
                        data.newGametag,
                        data.levelToLoad,

                        _guiComponentsFactory.CreateButtonComponent(data.optionstext,
                                                                   data.optionstag,
                                                                   data.optionsx,
                                                                   data.optionsy,
                                                                   data.optionswidth,
                                                                   data.optionsheight),
                        data.optionstext,
                        data.optionstag,

                        _guiComponentsFactory.CreateButtonComponent(data.creditstext,
                                                                   data.creditstag,
                                                                   data.creditsx,
                                                                   data.creditsy,
                                                                   data.creditswidth,
                                                                   data.creditsheight),
                        data.creditstext,
                        data.creditstag,

                        _guiComponentsFactory.CreateButtonComponent(data.exittext,
                                                                   data.exittag,
                                                                   data.exitx,
                                                                   data.exity,
                                                                   data.exitwidth,
                                                                   data.exitheight),
                        data.exittext,
                        data.exittag,

                        _renderingComponentsFactory.CreateFrontEndComponent(result, "MainMenuWindow"),
                        data.windowx,
                        data.windowy,
                        data.windowheight,
                        data.windowwidth,
                        _guiComponentsFactory.CreateLabelComponent(data.creditswindowtext,
                                                                  data.creditswindowtextx,
                                                                  data.creditswindowtexty),
                        data.creditswindowx,
                        data.creditswindowy,
                        data.creditswindowwidth,
                        data.creditswindowheight,
                        data.creditswindowtext,
                        data.creditswindowtextx,
                        data.creditswindowtexty,
                        _guiComponentsFactory.CreateLabelComponent(data.optionswindowtext,
                                                                  data.optionswindowtextx,
                                                                  data.optionswindowtexty),
                        data.optionswindowx,
                        data.optionswindowy,
                        data.optionswindowwidth,
                        data.optionswindowheight,
                        data.optionswindowtext,
                        data.optionswindowtextx,
                        data.optionswindowtexty,
                        _renderingComponentsFactory.CreateFrontEndComponent(result, "MainMenuFrame"),

                        _renderingComponentsFactory.CreateFrontEndComponent(result, "SplashScreen"),

                        _renderingComponentsFactory.CreateFrontEndComponent(result, "Logo"),
                        data.logoX, data.logoY);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateDescriptionWindow
        /****************************************************************************/

        public bool CreateDescriptionWindow(DescriptionWindow result, DescriptionWindowData data)
        {
            result.Init(_guiComponentsFactory.CreateWindowComponent(data.Title,
                                                                   (_game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - (data.Width / 2),
                                                                   (_game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2) - (data.Height / 2),
                                                                   data.Width,
                                                                   data.Height,
                                                                   true),
                        _guiComponentsFactory.CreateButtonComponent("OK",
                                                                   "OK",
                                                                   2,
                                                                   data.Height - 30,
                                                                   data.Width - 4,
                                                                   30),
                        _guiComponentsFactory.CreateLabelComponent(data.Text,
                                                                  10,
                                                                  35));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateInventory
        /****************************************************************************/

        public bool CreateInventory(Inventory result, InventoryData data)
        {
            Mercenary merc = (data.Mercenary == 0 ? null : (Mercenary)GetObject(data.Mercenary));
            Mercenary merc2 = (data.Mercenary2 == 0 ? null : (Mercenary)GetObject(data.Mercenary2));
            MercenariesManager mercManager = (data.MercenariesManager == 0 ? null : (MercenariesManager)GetObject(data.MercenariesManager));
            Container container = (data.Container == 0 ? null : (Container)GetObject(data.Container));

            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "InventorySet"),
                        _inputComponentsFactory.CreateKeyboardListenerComponent(result, true),
                        _inputComponentsFactory.CreateMouseListenerComponent(result, true),
                        merc,
                        merc2,
                        mercManager,
                        container);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateCompass
        /****************************************************************************/

        public bool CreateCompass(Compass result, CompassData data)
        {
            var camera = (LinkedCamera)GetObject(data.LinkedCamera);

            if (camera == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "compass"),
                        data.Target,
                        camera,
                        data.Orientation);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateContainer
        /****************************************************************************/

        public bool CreateContainer(Container result, ContainerData data)
        {
            var items = new Dictionary<StorableObject, ItemPosition>();

            if (data.Items != null)
            {
                foreach (var itemData in data.Items)
                {
                    var storable = GetObject(itemData[0]) as StorableObject;

                    if (storable == null)
                    {
                        PushToWaitingRoom(result, data);
                        return false;
                    }

                    items.Add(storable, new ItemPosition(itemData[1], itemData[2]));
                }
            }

            result.Init(_renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              data.Model,
                                                                              data.Diffuse,
                                                                              data.Specular,
                                                                              data.Normals),
                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics,
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
                        items);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateAmmoClip
        /****************************************************************************/

        public bool CreateAmmoClip(AmmoClip result, AmmoClipData data)
        {
            var ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;

            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            var content = new Stack<Bullet>(data.Capacity);

            for (var i = 0; i < data.Capacity && i < data.Content.Count; i++)
            {
                content.Push(data.Content.ElementAt(i));
            }

            var compability = new List<String>();
            if (data.Compability != null)
            {
                foreach (var sss in data.Compability)
                {
                    compability.Add(sss.String);
                }
            }

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                       data.EnabledMesh,
                                                                       false),
                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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

                      _particleFactory.CreateParticleEmitterComponent(result,
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
        // CreateAmmunition
        /****************************************************************************/

        public bool CreateAmmunition(Ammunition result, AmmunitionData data)
        {
            result.Init(data.AmmunitionInfo);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateAmmoBox
        /****************************************************************************/

        public bool CreateAmmoBox(AmmoBox result, AmmoBoxData data)
        {
            var ammunition = GetObject(GlobalGameObjects.Ammunition) as Ammunition;

            if (ammunition == null)
            {
                PushToWaitingRoom(result, data);
                return false;
            }

            AmmunitionInfo info;
            AmmunitionVersionInfo versionInfo;

            try
            {
                var ammunitionInfoID = ammunition.NameToID[data.AmmunitionName];
                info = ammunition.AmmunitionData[ammunitionInfoID];
                versionInfo = ammunition.AmmunitionVersionData[ammunitionInfoID][data.Version];
            }
            catch
            {
                return false;
            }

            var diffuse = "Firearms\\Jackal";

            switch (info.Genre)
            {
                case 1: diffuse += "Pistol"; break;
                case 2: diffuse += "Intermediate"; break;
                case 3: diffuse += "Rifle"; break;
                case 4: diffuse += "Shotgun"; break;
            }

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                            "Firearms\\JackalAmmo",
                                                            diffuse,
                                                            String.Empty,
                                                            String.Empty,
                                                            InstancingModes.DynamicInstancing,
                                                            data.EnabledMesh,
                                                            false),
            _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
            _particleFactory.CreateParticleEmitterComponent(result,
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
        // Create Accessory
        /****************************************************************************/

        public bool CreateAccessory(Accessory result, AccessoryData data)
        {
            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                     data.Model,
                                                                     data.Diffuse,
                                                                     data.Specular,
                                                                     data.Normals,
                                                                     Renderer.UIntToInstancingMode(data.InstancingMode),
                                                                     data.EnabledMesh,
                                                                     false),
                      _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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

                    _particleFactory.CreateParticleEmitterComponent(result,
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
        // Create FlammableBarrel
        /****************************************************************************/

        public bool CreateFlammableBarrel(FlammableBarrel result, FlammableBarrelData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),
             _physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                     _renderingComponentsFactory.CreatePointLightComponent(result,
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
        // Create Fire
        /****************************************************************************/

        public bool CreateFire(Fire result, FireData data)
        {
            result.Init(
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                     _renderingComponentsFactory.CreatePointLightComponent(result,
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
        // CreateFireSelector
        /****************************************************************************/

        public bool CreateFireSelector(FireSelector result, FireSelectorData data)
        {
            result.Init(_renderingComponentsFactory.CreateFrontEndComponent(result, "selector"),
                        _inputComponentsFactory.CreateMouseListenerComponent(result, true),
                        (Firearm)GetObject(data.Firearm));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateBloom
        /****************************************************************************/

        public bool CreateBloom(Bloom result, BloomData data)
        {
            result.Init(data.BloomIntensity, data.BaseIntensity, data.BloomSaturation, data.BaseSaturation, data.BloomThreshold);

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateDirection
        /****************************************************************************/

        public bool CreateDirection(Direction result, DirectionData data)
        {
            result.Init(_particleFactory.CreateParticleEmitterComponent(result,
                                                                       1,
                                                                       0.25f,
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       Vector3.Zero,
                                                                       Color.White,
                                                                       3,
                                                                       0,
                                                                       50,
                                                                       0,
                                                                       3,
                                                                       0,
                                                                       Color.White,
                                                                       3,
                                                                       0,
                                                                       0,
                                                                       3,
                                                                       0,
                                                                       "Particles\\arrow",
                                                                       5,
                                                                       Vector3.Zero,
                                                                       result.World,
                                                                       true,
                                                                       2),
                    _renderingComponentsFactory.CreateFrontEndComponent(result, null),
                    _inputComponentsFactory.CreateMouseListenerComponent(result, true),
                    GetObject(data.Feedback));

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        /// CreateDirection
        /****************************************************************************/

        public bool CreateDeadBody(DeadBody result, DeadBodyData data)
        {
            var items = new Dictionary<StorableObject, ItemPosition>();
            if (data.Items != null)
            {
                foreach (var itemData in data.Items)
                {
                    var storable = GetObject(itemData[0]) as StorableObject;

                    if (storable == null)
                    {
                        PushToWaitingRoom(result, data);
                        return false;
                    }

                    items.Add(storable, new ItemPosition(itemData[1], itemData[2]));
                }
            }
            result.Init(_renderingComponentsFactory.CreateSkinnedMeshComponent(result,
                                                                              data.Model,
                                                                              data.Diffuse,
                                                                              data.Specular,
                                                                              data.Normals),
                        _physicsComponentFactory.CreateCapsuleBodyComponent(data.EnabledPhysics, result,
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
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        data.Slots,
                        items);

            return true;
        }

        /****************************************************************************/
        // CreatePainKillers
        /****************************************************************************/

        public bool CreatePainKillers(PainKillers result, PainKillersData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        _physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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
                        data.Icon,
                        data.SlotsIcon,
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                                                                        data.Technique), data.BleedingIntensity, data.Amount
                                                                       );

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateMedKit
        /****************************************************************************/

        public bool CreateMedKit(MedKit result, MedKitData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       data.Static),

                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                                                                        data.Technique), data.HealthBoost, data.Amount
                                                                       );

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // CreateCylindricalStorableObject
        /****************************************************************************/

        public bool CreateCylindricalStorableObject(CylindricalStorableObject result, CylindricalStorableObjectData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       false),

                        _physicsComponentFactory.CreateCylindricalBodyComponent(data.EnabledPhysics, result,
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
                        data.Icon,
                        data.SlotsIcon,
                        data.Description,
                        data.DescriptionWindowWidth,
                        data.DescriptionWindowHeight,
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                                                                        data.Technique)
                                                                       );

            return true;
        }

        /****************************************************************************/

        /****************************************************************************/
        // SquareStorableObject
        /****************************************************************************/

        public bool CreateSquareStorableObject(SquareStorableObject result, SquareStorableObjectData data)
        {
            var instancingMode = Renderer.UIntToInstancingMode(data.InstancingMode);

            result.Init(_renderingComponentsFactory.CreateMeshComponent(result,
                                                                       data.Model,
                                                                       data.Diffuse,
                                                                       data.Specular,
                                                                       data.Normals,
                                                                       instancingMode,
                                                                       data.EnabledMesh,
                                                                       false),

                        _physicsComponentFactory.CreateSquareBodyComponent(data.EnabledPhysics, result,
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
                        _particleFactory.CreateParticleEmitterComponent(result,
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
                                                                        data.Technique)
                                                                       );

            return true;
        }

        /****************************************************************************/
    }

    /********************************************************************************/
}

/************************************************************************************/