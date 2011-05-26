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

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class BuildingWithRoof:GameObjectInstance
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public MeshComponent mesh;
        public MeshComponent mesh2;

        public SquareSkinComponent body;
        /********************************************************************************/


        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(MeshComponent mesh,MeshComponent mesh2, SquareSkinComponent physcisComponent)
        {
            this.mesh = mesh;
            this.mesh2 = mesh2;
            this.body = physcisComponent;
        }
        /********************************************************************************/



        /********************************************************************************/
        /// ReleaseComponents
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            mesh2.ReleaseMe();
            body.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            BuildingWithRoofData data = new BuildingWithRoofData();
            GetData(data);
            data.Model1 = mesh.Model.Name;
            data.Diffuse1 = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular1 = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals1 = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);
            data.InstancingMode1 = Renderer.InstancingModeToUInt(mesh.InstancingMode);
            data.EnabledMesh1 = mesh.Enabled;

            data.Model2 = mesh2.Model.Name;
            data.Diffuse2 = (mesh2.Textures.Diffuse == null ? String.Empty : mesh2.Textures.Diffuse.Name);
            data.Specular2 = (mesh2.Textures.Specular == null ? String.Empty : mesh2.Textures.Specular.Name);
            data.Normals2 = (mesh2.Textures.Normals == null ? String.Empty : mesh2.Textures.Normals.Name);
            data.InstancingMode2 = Renderer.InstancingModeToUInt(mesh2.InstancingMode);
            data.EnabledMesh2 = mesh2.Enabled;

            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;

            return data;
        }
        /********************************************************************************/


    }



    /********************************************************************************/
    /// BuildingWithRoofData
    /********************************************************************************/
    [Serializable]
    public class BuildingWithRoofData : GameObjectInstanceData
    {
        [CategoryAttribute("Model")]
        public String Model1 { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse1 { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular1 { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals1 { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode1 { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh1 { get; set; }


        [CategoryAttribute("Model")]
        public String Model2 { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse2 { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular2 { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals2 { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode2 { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh2 { get; set; }


        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }

        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Width { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Height { get; set; }

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
