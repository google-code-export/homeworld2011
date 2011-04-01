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
        PhysicsComponent physicsComponent = null;
        /****************************************************************************/
       

        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(MeshComponent meshComponent, Matrix world,PhysicsComponent physicsComponent)
        {
            this.meshComponent      = meshComponent;
            this.World              = world;
            this.physicsComponent   = physicsComponent;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();

            if (physicsComponent != null)
            {
                physicsComponent.ReleaseMe();
            }
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

            if (physicsComponent != null)
            {
                data.physicsComponentData = physicsComponent.GetData();
            }
            
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


        public PhysicsComponentData physicsComponentData;
        

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