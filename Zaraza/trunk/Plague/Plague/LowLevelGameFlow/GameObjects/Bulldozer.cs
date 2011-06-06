using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;


namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Bulldozer:ActivableObject
    {

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        public MeshComponent mesh;
        public SquareBodyComponent body;
        private PhysicsController _controller;
        private SoundEffectComponent _soundEffectComponent;
        private int _keyId = -1;
        private uint _timerId;
        private float _forceForward;
        private float _moveDuration;

        private bool _used;
        /****************************************************************************/



        /****************************************************************************/
        // Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         SquareBodyComponent body,
                         int[] activationRecievers,
                         int keyID,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,float forceForward,float moveDuration)
        {
            this.mesh = mesh;
            this.body = body;
            _keyId = keyID;
            _soundEffectComponent =  new SoundEffectComponent();
            _soundEffectComponent.LoadFolder("Bulldozer", 0.5f, 0, 0);
            _forceForward = forceForward;
            _moveDuration = moveDuration;
            Init(activationRecievers,description,descriptionWindowWidth,descriptionWindowHeight);
            
        }
        /****************************************************************************/


        private void StopMoving()
        {
            _controller.DisableControl();
            _soundEffectComponent.StopAllSounds();
            TimeControl.ReleaseFrameCounter(_timerId);
        }

        private void Move()
        {
            
            _controller.MoveBackward(_forceForward);
        }

        /****************************************************************************/
        /// On Action
        /****************************************************************************/
        protected override void OnActivation()
        {
            if (_used) return;
            if (_controller == null)
            {
                _controller = new PhysicsController(body);
                _controller.EnableControl();
            }
            _soundEffectComponent.PlaySound("Bulldozer", "startRun");
            _timerId = TimeControl.CreateFrameCounter(1, -1, Move);
            TimeControl.CreateTimer(TimeSpan.FromSeconds(_moveDuration+3), 1, StopMoving);
            _used = true;
            
        }
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            _soundEffectComponent.SetPosiotion(World.Translation);
            base.Update(deltaTime);
        }

        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public override string[] GetActions(Mercenary mercenary)
        {
            if (_keyId != -1 && !_used)
            {
                if (mercenary.HasItem(_keyId))
                {
                    return new[] { "Examine", "Activate" };
                }
            }
            return new[] { "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (_controller != null)
            {
                _controller.DisableControl();
                _controller.DisableController();
                _controller.RealeseMe();
            }

            if (body != null)
            {
                mesh.ReleaseMe();
                mesh = null;
            }

            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new BulldozerData();
            GetData(data);
            data.Model = mesh.Model.Name;
            data.Diffuse = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);


            
            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;
            data.EnabledMesh = mesh.Enabled;


            data.ActivationRecievers = activationRecievers;
            data.keyId = _keyId;
            data.ForceForward = _forceForward;
            data.MoveDuration = _moveDuration;

            return data;
        }
        /****************************************************************************/


    }


    
    /********************************************************************************/
    /// BulldozerData
    /********************************************************************************/
    [Serializable]
    public class BulldozerData : ActivableObjectData
    {
        public BulldozerData()
        {
            Type = typeof(Bulldozer);
        }


        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }
        [CategoryAttribute("Textures")]
        public String Specular { get; set; }
        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }

        [CategoryAttribute("Physics")]
        public float Mass { get; set; }
        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }
        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public bool Immovable { get; set; }
        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }



        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float Width { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float Height { get; set; }
        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }
        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

        public int[] ActivationRecievers { get; set; }
        public int keyId { get; set; }
        public float ForceForward { get; set; }
        public float MoveDuration { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/

