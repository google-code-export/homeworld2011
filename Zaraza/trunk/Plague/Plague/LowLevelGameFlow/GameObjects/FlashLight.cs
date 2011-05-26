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
    class Flashlight : GameObjectInstance, IActiveGameObject, IStorable
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent mesh = null;
        public CylindricalBodyComponent body = null;
        SpotLightComponent       light = null;

        private Rectangle Icon;
        private Rectangle SlotsIcon;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         CylindricalBodyComponent body,
                         SpotLightComponent light,
                         Rectangle icon,
                         Rectangle slotsIcon)
        {
            this.mesh  = mesh;
            this.body  = body;
            this.light = light;
            Icon = icon;
            SlotsIcon = slotsIcon;
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
                mesh.Enabled = true;
                light.Enabled = true;
                if(body != null) body.DisableBody();
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if(body != null) body.EnableBody();
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
            data.Icon = Icon;
            data.SlotsIcon = SlotsIcon;
            data.EnabledMesh = mesh.Enabled;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public String[] GetActions()
        {
            return new String[] {};
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActions
        /****************************************************************************/
        public string[] GetActions(Mercenary mercenary)
        {
            return new String[] { "Grab", "Examine" };
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(ExamineEvent)))
            { 
                DescriptionWindowData data = new DescriptionWindowData();
                data.Title = "Flashlight";
                data.Text = "A flashlight (usually called a torch outside North America) is\n" +
                            "a hand-held electric-powered light source. Usually the light\n"   +
                            "source is a small incandescent lightbulb or light-emitting \n"    +
                            "diode (LED). Typical flashlight designs consist of the light \n"  +
                            "source mounted in a parabolic or other shaped reflector, \n"      + 
                            "a transparent lens to protect the light source from damage \n"    +
                            "and debris, a power source (typically electric batteries), \n"    +
                            "and an electric power switch.";

                data.Width  = 410;
                data.Height = 220;

                SendEvent(new CreateObjectEvent(data), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }
        }
        /****************************************************************************/


        public Rectangle GetIcon()
        {
            return Icon;
        }

        public Rectangle GetSlotsIcon()
        {
            return SlotsIcon;
        }


        public void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled  = false;
            light.Enabled = false;
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// FlashlightData
    /********************************************************************************/
    [Serializable]
    public class FlashlightData : GameObjectInstanceData
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

        [CategoryAttribute("Icon")]
        public Rectangle Icon      { get; set; }
        [CategoryAttribute("Icon")]
        public Rectangle SlotsIcon { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

    }
    /********************************************************************************/
}
/************************************************************************************/