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
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Cylindrical Body Mesh
    /********************************************************************************/
    class CylindricalBodyMesh : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        MeshComponent         meshComponent    = null;
        CylinderBodyComponent physicsComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(MeshComponent meshComponent,CylinderBodyComponent physcisComponent, Matrix world)
        {
            this.meshComponent    = meshComponent;
            this.physicsComponent = physcisComponent;
            this.World            = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
            physicsComponent.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CylindricalBodyMeshData data = new CylindricalBodyMeshData();
            GetData(data);
            data.Model      = meshComponent.Model.Name;
            data.Diffuse    = (meshComponent.Textures.Diffuse == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular   = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals    = (meshComponent.Textures.Normals == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(meshComponent.InstancingMode);

            data.Mass             = physicsComponent.Mass;
            data.Elasticity       = physicsComponent.Elasticity;
            data.StaticRoughness  = physicsComponent.StaticRoughness;
            data.DynamicRoughness = physicsComponent.DynamicRoughness;
            data.Radius           = physicsComponent.Radius;
            data.Length           = physicsComponent.Length;
            data.Immovable        = physicsComponent.Immovable;
            data.SkinTransform    = physicsComponent.SkinTransform;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Cylindrical Body Mesh Data
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
        public float Radius { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Length { get; set; }


        /**************************************/
        /// Skin Transform
        /**************************************/
        public Matrix SkinTransform = Matrix.Identity;
        [CategoryAttribute("Collision Skin")]
        public Vector3 SkinOrinetationUp 
        {
            get
            {
                return SkinTransform.Up;    
            }

            set
            {
                SkinTransform.Up = value;
            }
        }

        [CategoryAttribute("Collision Skin")]
        public Vector3 SkinOrinetationRight 
        {
            get
            {
                return SkinTransform.Right;
            }

            set
            {
                SkinTransform.Right = value;
            }
        }
        
        [CategoryAttribute("Collision Skin")]
        public Vector3 SkinOrinetationForward 
        {
            get
            {
                return SkinTransform.Forward;
            }

            set
            {
                SkinTransform.Forward = value;
            }
        }

        [CategoryAttribute("Collision Skin")]
        public Vector3 SkinPosition 
        {
            get
            {
                return SkinTransform.Translation;
            }

            set
            {
                SkinTransform.Translation = value;
            }
        }
        /**************************************/
    }
    /********************************************************************************/
    
}
/************************************************************************************/