using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Flashlight
    /********************************************************************************/
    class Flashlight : StorableObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent mesh = null;
        public CylindricalBodyComponent body = null;
        SpotLightComponent       light = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent            mesh,
                         CylindricalBodyComponent body,
                         SpotLightComponent       light,
                         Rectangle                icon,
                         Rectangle                slotsIcon,
                         String                   description,
                         int                      descriptionWindowWidth,
                         int                      descriptionWindowHeight)
        {
            this.mesh  = mesh;
            this.body  = body;
            this.light = light;

            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected override void OnOwning(GameObjectInstance owner)
        {
            if (owner != null)
            {
                World = Matrix.Identity;
                World *= Matrix.CreateRotationY(MathHelper.ToRadians(180));

                if (mesh  != null) mesh.Enabled     = true;
                if (light != null) light.Enabled    = true;
                if (body  != null) body.DisableBody();
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body     != null) body.EnableBody();
                if (mesh     != null) mesh.Enabled = true;
                if (light    != null) light.Enabled = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            mesh.ReleaseMe();
            mesh = null;
            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }
            light.ReleaseMe();
            light = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FlashlightData data = new FlashlightData();

            GetData(data);
            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Immovable = body.Immovable;

            data.Enabled = light.Enabled;
            data.Color = light.Color;
            data.Radius = light.Radius;
            data.LinearAttenuation = light.LinearAttenuation;
            data.QuadraticAttenuation = light.QuadraticAttenuation;

            data.NearPlane = light.NearPlane;
            data.FarPlane = light.FarPlane;
            data.LocalTransform = light.LocalTransform;
            data.AttenuationTexture = light.AttenuationTexture;
            data.Intensity = light.Intensity;
            data.ShadowsEnabled = light.ShadowsEnabled;
            data.Specular = light.Specular;
            data.DepthBias = light.DepthBias;
            data.EnabledMesh = mesh.Enabled;
            data.EnabledPhysics = body.Enabled;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Storing
        /****************************************************************************/
        public override void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled  = false;
            light.Enabled = false;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// FlashlightData
    /********************************************************************************/
    [Serializable]
    public class FlashlightData : StorableObjectData
    {
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

        [CategoryAttribute("Misc")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Color")]
        public Vector3 Color { get; set; }
        [CategoryAttribute("Color")]
        public float Intensity { get; set; }
        [CategoryAttribute("Color")]
        public bool Specular { get; set; }

        [CategoryAttribute("Size")]
        public float Radius { get; set; }
        [CategoryAttribute("Size")]
        public float NearPlane { get; set; }
        [CategoryAttribute("Size")]
        public float FarPlane { get; set; }

        [CategoryAttribute("Transform")]
        public Matrix LocalTransform { get; set; }

        [CategoryAttribute("Attenuation")]
        public float LinearAttenuation { get; set; }
        [CategoryAttribute("Attenuation")]
        public float QuadraticAttenuation { get; set; }
        [CategoryAttribute("Attenuation")]
        public String AttenuationTexture { get; set; }

        [CategoryAttribute("Shadows")]
        public bool ShadowsEnabled { get; set; }
        [CategoryAttribute("Shadows")]
        public float DepthBias { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

    }
    /********************************************************************************/
}
/************************************************************************************/