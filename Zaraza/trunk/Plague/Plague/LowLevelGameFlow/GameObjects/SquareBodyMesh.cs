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

/********************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/********************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// SquareBodyMesh
    /********************************************************************************/
    class SquareBodyMesh : GameObjectInstance
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        MeshComponent meshComponent = null;
        SquareBodyComponent physicsComponent = null;
        /********************************************************************************/



        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(MeshComponent meshComponent, SquareBodyComponent physcisComponent, Matrix world)
        {
            this.meshComponent = meshComponent;
            this.physicsComponent = physcisComponent;
            this.World = world;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// ReleaseComponents
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
            physicsComponent.ReleaseMe();
        }
        /********************************************************************************/





        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            SquareBodyMeshData data = new SquareBodyMeshData();
            GetData(data);
            data.Model = meshComponent.Model.Name;
            data.Diffuse = (meshComponent.Textures.Diffuse == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals = (meshComponent.Textures.Normals == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(meshComponent.InstancingMode);

            data.Mass = physicsComponent.Mass;
            data.Elasticity = physicsComponent.Elasticity;
            data.StaticRoughness = physicsComponent.StaticRoughness;
            data.DynamicRoughness = physicsComponent.DynamicRoughness;
            data.Lenght = physicsComponent.Length;
            data.Width = physicsComponent.Width;
            data.Height = physicsComponent.Height;
            data.Immovable = physicsComponent.Immovable;

            return data;
        }   
        /********************************************************************************/



    }
    /********************************************************************************/




    /********************************************************************************/
    /// SquareBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class SquareBodyMeshData : GameObjectInstanceData
    {
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

        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Width { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Height { get; set; }
        /**************************************/


    }
    /********************************************************************************/



}
/********************************************************************************/