using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// GlowStick
    /********************************************************************************/
    class GlowStick : StorableObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent mesh = null;
        public CylindricalBodyComponent body = null;
        PointLightComponent      light1 = null;
        PointLightComponent      light2 = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent            mesh,
                         CylindricalBodyComponent body,
                         PointLightComponent      light1,
                         PointLightComponent      light2,
                         Rectangle                icon,
                         Rectangle                slotsIcon,
                         String                   description,
                         int                      descriptionWindowWidth,
                         int                      descriptionWindowHeight,
                         Particles.Components.ParticleEmitterComponent emitter)
        {
            this.mesh   = mesh;
            this.body   = body;
            this.light1 = light1;
            this.light2 = light2;
            
            if (!body.Immovable)
            {
                body.SubscribeCollisionEvent(typeof(Terrain));
            }

            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight, emitter);
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
                if (mesh   != null) mesh.Enabled   = true;
                if (light1 != null) light1.Enabled = true;
                if (light2 != null) light2.Enabled = true;
                if (body   != null) body.DisableBody();
            }
            else
            {                
                if(getWorld != null) World = GetWorld();
                if(body != null) body.EnableBody();

                if (mesh != null) mesh.Enabled = true;
                if (light1 != null) light1.Enabled = true;
                if (light2 != null) light2.Enabled = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (mesh != null)
            {
                mesh.ReleaseMe();
                mesh = null;
            }
            
            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }

            light1.ReleaseMe();
            light1 = null;
            light2.ReleaseMe();
            light2 = null;

            base.ReleaseComponents();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            GlowStickData data = new GlowStickData();

            GetData(data);
            
            data.Texture = mesh.Textures.Diffuse.Name;
            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Immovable = body.Immovable;

            data.Enabled              = light1.Enabled;
            data.Color                = light1.Color;
            data.Intensity = light1.Intensity;
            data.Radius = light1.Radius;
            data.LinearAttenuation = light1.LinearAttenuation;
            data.QuadraticAttenuation = light1.QuadraticAttenuation;


            data.EnabledMesh = mesh.Enabled;
            data.EnabledPhysics = body.Enabled;
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnStoring
        /****************************************************************************/
        public override void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled   = false;
            light1.Enabled = false;
            light2.Enabled = false;

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
            light1.Enabled = true;
            light2.Enabled = true;
            body.Immovable = false;
            body.SubscribeCollisionEvent(typeof(Terrain));
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
            light1.Enabled = true;
            light2.Enabled = true;

            base.OnPicking();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent CollisionEvent = e as CollisionEvent;
                if (CollisionEvent.gameObject.GetType().Equals(typeof(Terrain)))
                {
                    body.Immovable = true;
                    body.CancelCollisionWithGameObjectsType(typeof(Terrain));
                }
            }
            else
            {
                base.OnEvent(sender, e);
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// GlowStickData
    /********************************************************************************/
    [Serializable]
    public class GlowStickData : StorableObjectData
    {
        [CategoryAttribute("Textures")]
        public String Texture { get; set; }

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

        [CategoryAttribute("Light")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Color { get; set; }

        [CategoryAttribute("Light")]
        public float Radius { get; set; }

        [CategoryAttribute("Light")]
        public float Intensity { get; set; }

        [CategoryAttribute("Light")]
        public float LinearAttenuation { get; set; }

        [CategoryAttribute("Light")]
        public float QuadraticAttenuation { get; set; }

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/