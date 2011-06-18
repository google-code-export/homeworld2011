using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Particles.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Flashlight
    /********************************************************************************/
    class Flashlight : Accessory
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public SpotLightComponent light = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         SquareBodyComponent body,
                         SpotLightComponent light,
                         Rectangle icon,
                         Rectangle slotsIcon,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         ParticleEmitterComponent particle,
                         float damageModulation,
                         float accuracyModulation,
                         float rangeModulation,
                         float penetrationModulation,
                         float recoilModulation,
                         float stoppingPowerModulation,
                         String genre)
        {
            this.light = light;

            Init(mesh, body, icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight,
                 particle, damageModulation, accuracyModulation, rangeModulation, penetrationModulation,
                 recoilModulation, stoppingPowerModulation, genre);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected override void OnOwning(GameObjectInstance owner)
        {
            if (owner != null)
            {
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
        /// Switch
        /****************************************************************************/
        public override void Switch(bool on)
        {
            light.Enabled = on;
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

            base.ReleaseComponents();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FlashlightData data = new FlashlightData();

            GetData(data);
            
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
            data.SpecularEnabled = light.Specular;
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

            base.OnStoring();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Dropping
        /****************************************************************************/
        public override void OnDropping()
        {
            body.EnableBody();
            mesh.Enabled = true;
            light.Enabled = true;
            base.OnDropping();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Picking
        /****************************************************************************/
        public override void OnPicking()
        {
            mesh.Enabled = true;
            body.DisableBody();

            base.OnPicking();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnOwnerStoring
        /****************************************************************************/
        public override void OnOwnerStoring()
        {
            light.Enabled = false;
            base.OnOwnerStoring();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Attach
        /****************************************************************************/
        public override void OnAttach(Firearm firearm, Vector3 translation)
        {
            owner = firearm;
            OwnerBone = -1;
            getWorld = GetOwnerWorld;
            World = Matrix.CreateTranslation(translation);
            body.DisableBody();
            mesh.Enabled = true;
            emitter.DisableEmitter();
            light.Enabled = true;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// FlashlightData
    /********************************************************************************/
    [Serializable]
    public class FlashlightData : AccessoryData
    {
        [CategoryAttribute("Misc")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Color")]
        public Vector3 Color { get; set; }
        [CategoryAttribute("Color")]
        public float Intensity { get; set; }
        [CategoryAttribute("Color")]
        public bool SpecularEnabled { get; set; }

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
    }
    /********************************************************************************/
}
/************************************************************************************/