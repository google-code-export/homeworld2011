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


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Creature
    /********************************************************************************/
    class Creature : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        SkinnedMeshComponent meshComponent = null;
        KeyboardListenerComponent keyboard = null;
        CapsuleBodyComponent body = null;
        PhysicsController controller = null;

        int isMoving = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent meshComponent,KeyboardListenerComponent keyboard,CapsuleBodyComponent body, Matrix world)
        {
            this.meshComponent = meshComponent;
            this.keyboard      = keyboard;
            this.body          = body;
            this.World         = world;

            keyboard.SubscibeKeys(OnKey, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D8, Keys.D9, Keys.Y, Keys.H, Keys.G, Keys.J, Keys.P);
            meshComponent.SubscribeAnimationsEnd("Attack");
            controller = new PhysicsController(body);
            controller.EnableControl();
            meshComponent.StartClip("Idle");
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (key == Keys.Y)
            {
                if(state.WasPressed())
                {
                    isMoving++;

                    if (meshComponent.currentAnimation.Clip.Name != "Walk")
                    {
                        meshComponent.BlendTo("Walk", TimeSpan.FromSeconds(0.5));
                    }
                }
                else if(state.IsDown())
                {
                    controller.MoveForward(5.0f);
                }
                else if(state.WasReleased())
                {
                    if (--isMoving == 0)
                    {
                        controller.StopMoving();
                        meshComponent.BlendTo("Idle", TimeSpan.FromSeconds(0.5));
                    }
                }
            }


            if (key == Keys.H)
            {
                if (state.WasPressed())
                {
                    isMoving++;

                    if (meshComponent.currentAnimation.Clip.Name != "Walk")
                        meshComponent.BlendTo("Walk", TimeSpan.FromSeconds(0.5));
                }
                else if (state.IsDown())
                {
                    controller.MoveBackward(5.0f);
                }
                else if (state.WasReleased())
                {
                    if (--isMoving == 0)
                    {
                        controller.StopMoving();
                        meshComponent.BlendTo("Idle", TimeSpan.FromSeconds(0.5));
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
                    case Keys.D0: meshComponent.Stop();
                        break;
                    case Keys.D1: meshComponent.StartClip("Idle");
                        break;
                    case Keys.D2: meshComponent.BlendTo("Attack", TimeSpan.FromSeconds(0.3));
                        break;
                    case Keys.D3: meshComponent.BlendTo("Walk",TimeSpan.FromSeconds(0.3));
                        break;
                    case Keys.D4: meshComponent.TimeRatio *= 2;
                        break;
                    case Keys.D5: meshComponent.TimeRatio /= 2;
                        break;
                    case Keys.D8:
                        {
                            if (meshComponent.IsPaused())
                            {
                                meshComponent.Resume();
                            }
                            else
                            {
                                meshComponent.PauseAnimation();
                            }
                        }
                        break;
                    case Keys.D9:
                        meshComponent.Reset();
                        break;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(Rendering.AnimationEndEvent)))
            {
                meshComponent.BlendTo("Idle", TimeSpan.FromSeconds(0.3));
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
            body.ReleaseMe();
            keyboard.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CreatureData data = new CreatureData();
            GetData(data);
            
            data.Model    = meshComponent.Model.Name;
            
            data.Diffuse  = (meshComponent.Textures.Diffuse  == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals  = (meshComponent.Textures.Normals  == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.TimeRatio       = meshComponent.TimeRatio;
            data.CurrentClip     = (meshComponent.currentAnimation.Clip == null ? String.Empty : meshComponent.currentAnimation.Clip.Name);
            data.CurrentTime     = meshComponent.currentAnimation.ClipTime.TotalSeconds;
            data.CurrentKeyframe = meshComponent.currentAnimation.Keyframe;
            data.Pause           = meshComponent.Pause;

            data.Blend         = meshComponent.Blend;
            data.BlendDuration = meshComponent.BlendDuration.TotalSeconds;
            data.BlendTime     = meshComponent.BlendTime.TotalSeconds;
            data.BlendClip     = (meshComponent.blendAnimation.Clip == null ? String.Empty : meshComponent.blendAnimation.Clip.Name);
            data.BlendClipTime = meshComponent.blendAnimation.ClipTime.TotalSeconds;
            data.BlendKeyframe = meshComponent.blendAnimation.Keyframe;

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

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// StaticSkinnedMeshData
    /********************************************************************************/
    [Serializable]
    public class CreatureData : GameObjectInstanceData
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
    }
    /********************************************************************************/

}
/************************************************************************************/