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

            keyboard.SubscibeKeys(OnKey, Keys.Space);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (state.WasPressed())
            {
                if (meshComponent.AnimationPlayer.CurrentClip == null)
                {
                    meshComponent.StartClip("Take 001");
                }
                else
                {
                    meshComponent.AnimationPlayer.Stop();
                }
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