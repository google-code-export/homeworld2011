using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// StaticSkinnedMesh
    /********************************************************************************/
    class Piggy : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        SkinnedMeshComponent meshComponent = null;
        KeyboardListenerComponent keyboard = null;        
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SkinnedMeshComponent meshComponent,KeyboardListenerComponent keyboard, Matrix world)
        {
            this.meshComponent = meshComponent;
            this.keyboard      = keyboard;
            this.World         = world;

            keyboard.SubscibeKeys(OnKey, Keys.D0,Keys.D1,Keys.D2,Keys.D3,Keys.D4,Keys.D5,Keys.D8,Keys.D9);
            meshComponent.SubscribeAnimationsEnd("Jump");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (state.WasPressed())
            {
                switch (key)
                {
                    case Keys.D0: meshComponent.Stop();
                        break;
                    case Keys.D1: meshComponent.StartClip("Run");
                        break;
                    case Keys.D2: meshComponent.StartClip("Jump");
                        break;
                    case Keys.D3: meshComponent.BlendTo("Run",TimeSpan.FromSeconds(0.5));
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
                                meshComponent.PauseClip();
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
                meshComponent.BlendTo("Run", TimeSpan.FromSeconds(0.1));
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            PiggyData data = new PiggyData();
            GetData(data);
            
            data.Model    = meshComponent.Model.Name;
            
            data.Diffuse  = (meshComponent.Textures.Diffuse  == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals  = (meshComponent.Textures.Normals  == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.TimeRatio       = meshComponent.TimeRatio;
            data.CurrentClip     = meshComponent.CurrentClip.Name;
            data.CurrentTime     = meshComponent.CurrentTime.TotalSeconds;
            data.CurrentKeyframe = meshComponent.CurrentKeyframe;
            data.Pause           = meshComponent.Pause;

            data.Blend         = meshComponent.Blend;
            data.BlendDuration = meshComponent.BlendDuration.TotalSeconds;
            data.BlendTime     = meshComponent.BlendTime.TotalSeconds;
            data.BlendClip     = meshComponent.BlendClip.Name;
            data.BlendClipTime = meshComponent.BlendClipTime.TotalSeconds;
            data.BlendKeyframe = meshComponent.BlendKeyframe;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// StaticSkinnedMeshData
    /********************************************************************************/
    [Serializable]
    public class PiggyData : GameObjectInstanceData
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
               
    }
    /********************************************************************************/

}
/************************************************************************************/