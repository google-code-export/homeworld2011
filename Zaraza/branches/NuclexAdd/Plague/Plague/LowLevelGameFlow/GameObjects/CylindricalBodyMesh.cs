﻿using System;
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
using PlagueEngine.Input.Components;
using PlagueEngine.Input;

using Microsoft.Xna.Framework.Input;

/********************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/********************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// CylindricalBodyMesh
    /********************************************************************************/
    class CylindricalBodyMesh : GameObjectInstance
    {

        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        DeformableMeshComponentAdapter mesh = null;
        CylindricalBodyComponent body = null;
        KeyboardListenerComponent listener = null;
        /********************************************************************************/




        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(MeshComponent meshComponent, CylindricalBodyComponent physcisComponent, KeyboardListenerComponent listener)
        {
            this.mesh = new DeformableMeshComponentAdapter(this, meshComponent);
            this.body = physcisComponent;
            this.listener = listener;

            this.listener.SubscibeKeys(this.onKey, Keys.P);
            //this.mesh.deform();
            //this.mesh.deform();
            
        }
        /********************************************************************************/



        public void onKey(Keys keys, ExtendedKeyState extKS)
        {
            if (keys == Keys.P && extKS.WasPressed())
            {
                this.mesh.deform();
            }
        }




        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            body.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CylindricalBodyMeshData data = new CylindricalBodyMeshData();
            GetData(data);
            data.Model = mesh.Model.Name;
            data.Diffuse = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Radius = body.Radius;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledMesh = mesh.Enabled;
            data.Static = mesh.Static;
            data.EnabledPhysics = body.Enabled;
            return data;
        }
        /********************************************************************************/



    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class CylindricalBodyMeshData : GameObjectInstanceData
    {
        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Textures")]
        public String Diffuse { get; set; }

        [CategoryAttribute("Textures")]
        public String Specular { get; set; }

        [CategoryAttribute("Textures")]
        public String Normals { get; set; }

        [CategoryAttribute("Mesh")]
        public bool Static { get; set; }

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

        [CategoryAttribute("Physics")]
        public bool EnabledPhysics { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

    }
    /********************************************************************************/



}
/********************************************************************************/