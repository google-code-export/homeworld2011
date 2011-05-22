using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Physics;
using PlagueEngine.Physics.Components;


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
        public MeshComponent mesh = null;
        /****************************************************************************/
       

        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(MeshComponent mesh)
        {
            this.mesh      = mesh;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
        }
        /****************************************************************************/
        

        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            StaticMeshData data = new StaticMeshData();
            GetData(data);
            data.Model          = mesh.Model.Name;
            data.Diffuse        = (mesh.Textures.Diffuse  == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular       = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals        = (mesh.Textures.Normals  == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);
           
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