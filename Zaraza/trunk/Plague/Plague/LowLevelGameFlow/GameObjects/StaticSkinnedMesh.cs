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
    class StaticSkinnedMesh : GameObjectInstance
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
                    case Keys.D3: meshComponent.Blend("Run",TimeSpan.FromSeconds(0.5));
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
                                meshComponent.Pause();
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
                meshComponent.Blend("Run", TimeSpan.FromSeconds(0.1));
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
            StaticSkinnedMeshData data = new StaticSkinnedMeshData();
            GetData(data);
            
            data.Model    = meshComponent.Model.Name;
            
            data.Diffuse  = (meshComponent.Textures.Diffuse  == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals  = (meshComponent.Textures.Normals  == null ? String.Empty : meshComponent.Textures.Normals.Name);

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// StaticSkinnedMeshData
    /********************************************************************************/
    [Serializable]
    public class StaticSkinnedMeshData : GameObjectInstanceData
    {
        [CategoryAttribute("Model")]
        public String Model    { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse  { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals  { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/