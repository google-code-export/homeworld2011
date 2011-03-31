using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;

/************************************************************************************/
/// Plague.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// StaticMesh
    /********************************************************************************/
    class StaticMesh : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        MeshComponent meshComponent = null;

        /****************************************************************************/
       

        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(MeshComponent meshComponent, Matrix world)
        {
            this.meshComponent = meshComponent;
            this.World         = world;
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
            StaticMeshData data = new StaticMeshData();
            GetData(data);
            data.Model          = meshComponent.Model.Name;
            data.Diffuse        = (meshComponent.Textures.Diffuse  == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular       = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals        = (meshComponent.Textures.Normals  == null ? String.Empty : meshComponent.Textures.Normals.Name);
            data.InstancingMode = Renderer.InstancingModeToUInt(meshComponent.InstancingMode);                        
            return data;
        }
        /****************************************************************************/


    }
    /********************************************************************************/



    /********************************************************************************/
    /// StaticMeshData
    /********************************************************************************/
    [Serializable]
    public class StaticMeshData : GameObjectInstanceData
    {
        [CategoryAttribute("Model")]
        public String Model           { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse         { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular        { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals         { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint   InstancingMode  { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/