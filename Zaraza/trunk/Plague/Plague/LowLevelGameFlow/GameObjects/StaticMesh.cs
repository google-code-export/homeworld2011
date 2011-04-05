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
                BoxPhysicsComponent bpc =(BoxPhysicsComponent)physicsComponent;
                data.Elasticity = bpc.elasticity;
                data.DynamicRoughness = bpc.dynamicRoughness;
                data.StaticRoughness = bpc.staticRoughness;
                data.BoxSize = bpc.boxSize;
                data.Immovable = bpc.immovable;
                data.Mass = bpc.mass;
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
        [CategoryAttribute("BoxPhysiscComponent")]
        public bool PhysicsActive      { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public float Elasticity        { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public float StaticRoughness   { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public float DynamicRoughness  { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public Vector3 BoxSize         { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public float Mass              { get; set; }
        [CategoryAttribute("BoxPhysiscComponent")]
        public bool Immovable         { get; set; }

        
       

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