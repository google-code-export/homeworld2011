using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Input;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.Audio.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Creature
    /********************************************************************************/
    class Creature : AbstractLivingBeing, IActiveGameObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        
        int isMoving = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent mesh,
                         KeyboardListenerComponent keyboard,
                         CapsuleBodyComponent body,
                         float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         List<AnimationBinding> AnimationMapping
            )
        {
            this.mesh = mesh;
            this.SoundEffectComponent = new SoundEffectComponent();
            this.body = body;

            
            mesh.SubscribeAnimationsEnd("Attack03");
            Controller = new PhysicsController(body);
            Controller.EnableControl();
            mesh.StartClip("Idle");
            //TODO: Fix HP => get from data
            this.ObjectAIController = new MobController(this, rotationSpeed, movingSpeed, distance, angle, 100, 90, AnimationMapping);
            this.RequiresUpdate = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            /*if (key == Keys.Y)
            {
                if(state.WasPressed())
                {
                    isMoving++;

                    if (mesh.currentAnimation.Clip.Name != "Walk")
                    {
                        mesh.BlendTo("Walk", TimeSpan.FromSeconds(0.5));
                    }
                }
                else if(state.IsDown())
                {
                    controller.MoveForward(15.0f);
                }
                else if(state.WasReleased())
                {
                    if (--isMoving == 0)
                    {
                        controller.StopMoving();
                        mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.5));
                    }
                }
            }


            if (key == Keys.H)
            {
                if (state.WasPressed())
                {
                    isMoving++;

                    if (mesh.currentAnimation.Clip.Name != "Walk")
                        mesh.BlendTo("Walk", TimeSpan.FromSeconds(0.5));
                }
                else if (state.IsDown())
                {
                    controller.MoveBackward(15.0f);
                }
                else if (state.WasReleased())
                {
                    if (--isMoving == 0)
                    {
                        controller.StopMoving();
                        mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.5));
                    }
                }
            }


            if (key == Keys.G && state.IsDown())
            {
                controller.Rotate(1);
            }

            if (key == Keys.J && state.IsDown())
            {
                controller.Rotate(-1);                
            }
            

            if (state.WasPressed())
            {
                switch (key)
                {
                    case Keys.D0: mesh.Stop();
                        break;
                    case Keys.D1: mesh.StartClip("Idle");
                        break;
                    case Keys.D2: mesh.BlendTo("Attack", TimeSpan.FromSeconds(0.3));
                        break;
                    case Keys.D3: mesh.BlendTo("Walk",TimeSpan.FromSeconds(0.3));
                        break;
                    case Keys.D4: mesh.TimeRatio *= 2;
                        break;
                    case Keys.D5: mesh.TimeRatio /= 2;
                        break;
                    case Keys.D8:
                        {
                            if (mesh.IsPaused())
                            {
                                mesh.Resume();
                            }
                            else
                            {
                                mesh.PauseAnimation();
                            }
                        }
                        break;
                    case Keys.D9:
                        mesh.Reset();
                        break;
                }
            }*/
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            this.ObjectAIController.Update(deltaTime);
        }

        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            this.ObjectAIController.OnEvent(sender, e);
     
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get World
        /****************************************************************************/
        protected override Matrix GetMyWorld(int bone)
        {
            if (bone == -1)
                return World;
            else
                return mesh.WorldTransforms[bone];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            body.ReleaseMe();
            //keyboard.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CreatureData data = new CreatureData();
            GetData(data);
            
            data.Model    = mesh.Model.Name;
            
            data.Diffuse  = (mesh.Textures.Diffuse  == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals  = (mesh.Textures.Normals  == null ? String.Empty : mesh.Textures.Normals.Name);

            data.TimeRatio       = mesh.TimeRatio;
            data.CurrentClip     = (mesh.currentAnimation.Clip == null ? String.Empty : mesh.currentAnimation.Clip.Name);
            data.CurrentTime     = mesh.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = mesh.currentAnimation.Keyframe;
            data.Pause           = mesh.Pause;

            data.Blend         = mesh.Blend;
            data.BlendDuration = mesh.BlendDuration.TotalSeconds;
            data.BlendTime     = mesh.BlendTime.TotalSeconds;
            data.BlendClip     = (mesh.blendAnimation.Clip == null ? String.Empty : mesh.blendAnimation.Clip.Name);
            data.BlendClipTime = mesh.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe = mesh.blendAnimation.Keyframe;

            data.Immovable = body.Immovable;
            data.IsEnabled = body.IsEnabled;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Mass = body.Mass;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;

            data.Radius = body.Radius;
            data.Length = body.Length;
            data.EnabledPhysics = body.Enabled;

            data.MovingSpeed = (ObjectAIController as MobController).MovingSpeed;
            data.RotationSpeed = (ObjectAIController as MobController).RotationSpeed;
            data.DistancePrecision = (ObjectAIController as MobController).DistancePrecision;
            data.AnglePrecision = (ObjectAIController as MobController).AnglePrecision;

            return data;
        }
        /****************************************************************************/


        public string[] GetActions()
        {
            List<String> actions = new List<String>();

            actions.Add("Attack");

            return actions.ToArray();
        }

        public string[] GetActions(Mercenary mercenary)
        {
            List<String> actions = new List<String>();

            actions.Add("Attack");

            return actions.ToArray();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// StaticSkinnedMeshData
    /********************************************************************************/
    [Serializable]
    public class CreatureData : AbstractLivingBeingData
    {
        [CategoryAttribute("Model")]
        public String Model    { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse  { get; set; }
        [CategoryAttribute("Textures")]
        public String Specular { get; set; }
        [CategoryAttribute("Textures")]
        public String Normals  { get; set; }

        [CategoryAttribute("Animation")]
        public float  TimeRatio       { get; set; }
        [CategoryAttribute("Animation")]
        public String CurrentClip     { get; set; }
        [CategoryAttribute("Animation")]
        public double CurrentTime     { get; set; }
        [CategoryAttribute("Animation")]
        public int    CurrentKeyframe { get; set; }
        [CategoryAttribute("Animation")]
        public bool   Pause           { get; set; }

        [CategoryAttribute("Animation Blending")]
        public bool   Blend         { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendDuration { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendTime     { get; set; }
        [CategoryAttribute("Animation Blending")]
        public String BlendClip     { get; set; }
        [CategoryAttribute("Animation Blending")]
        public double BlendClipTime { get; set; }
        [CategoryAttribute("Animation Blending")]
        public int    BlendKeyframe { get; set; }

        [CategoryAttribute("Physics")]
        public bool  Immovable        { get; set; }
        [CategoryAttribute("Physics")]
        public bool  IsEnabled        { get; set; }
        [CategoryAttribute("Physics")]
        public float Elasticity       { get; set; }
        [CategoryAttribute("Physics")]
        public float StaticRoughness  { get; set; }
        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }
        [CategoryAttribute("Physics")]
        public float Mass             { get; set; }
        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }


        [CategoryAttribute("Collision Skin")]
        public float Length           { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Radius           { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        [CategoryAttribute("Movement")]
        public float RotationSpeed { get; set; }
        [CategoryAttribute("Movement")]
        public float MovingSpeed { get; set; }
        [CategoryAttribute("Movement")]
        public float DistancePrecision { get; set; }
        [CategoryAttribute("Movement")]
        public float AnglePrecision { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/