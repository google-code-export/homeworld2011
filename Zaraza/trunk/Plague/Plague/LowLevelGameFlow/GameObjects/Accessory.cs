using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Particles.Components;
using PlagueEngine.Physics;
using PlagueEngine.Rendering;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Accessory
    /********************************************************************************/
    class Accessory : StorableObject
    {

        /****************************************************************************/
        /// Fields/Properties
        /****************************************************************************/
        public MeshComponent       mesh = null;
        public SquareBodyComponent body = null;

        public float DamageModulation        { get; private set; }
        public float AccuracyModulation      { get; private set; }
        public float RangeModulation         { get; private set; }
        public float PenetrationModulation   { get; private set; }
        public float RecoilModulation        { get; private set; }
        public float StoppingPowerModulation { get; private set; }

        public String Genre { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent      mesh,
                         SquareBodyComponent body,
                         Rectangle          icon,
                         Rectangle          slotsIcon,
                         String             description,
                         int                descriptionWindowWidth,
                         int                descriptionWindowHeight,
                         ParticleEmitterComponent particle,
                         float              damageModulation,
                         float              accuracyModulation,
                         float              rangeModulation,
                         float              penetrationModulation,
                         float              recoilModulation,
                         float              stoppingPowerModulation,
                         String             genre)
        {
            this.mesh = mesh;
            this.body = body;

            DamageModulation        = damageModulation;
            AccuracyModulation      = accuracyModulation;
            RangeModulation         = rangeModulation;
            PenetrationModulation   = penetrationModulation;
            RecoilModulation        = recoilModulation;
            StoppingPowerModulation = stoppingPowerModulation;
            Genre                   = genre;

            Init(icon, slotsIcon, description, descriptionWindowWidth, descriptionWindowHeight, particle);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Owning
        /****************************************************************************/
        protected override void OnOwning(GameObjectInstance owner)
        {
            if (owner != null)
            {
                //World = Matrix.Identity;
                if (mesh != null) mesh.Enabled = true;
                if (body != null) body.DisableBody();
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body != null) body.EnableBody();
                if (mesh != null) mesh.Enabled = true;
            }
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


        /****************************************************************************/
        /// On Attach
        /****************************************************************************/
        public virtual void OnAttach(Firearm firearm, Vector3 translation)
        {
            owner     = firearm;
            OwnerBone = -1;
            getWorld  = GetOwnerWorld;
            World = Matrix.CreateTranslation(translation);
            body.DisableBody();
            mesh.Enabled = true;
            emitter.DisableEmitter();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnStoring
        /****************************************************************************/
        public override void OnStoring()
        {
            body.DisableBody();
            mesh.Enabled = false;

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

            base.OnPicking();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnOwnerPlacing
        /****************************************************************************/
        public virtual void OnOwnerPlacing()
        {
            mesh.Enabled = true;
            body.EnableBody();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnOwnerStoring
        /****************************************************************************/
        public virtual void OnOwnerStoring()
        {
            mesh.Enabled = false;
            body.Disable();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (body != null)
            {
                body.ReleaseMe();
                body = null;
            }

            if (mesh != null)
            {
                mesh.ReleaseMe();
                mesh = null;
            }

            base.ReleaseComponents();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Switch
        /****************************************************************************/
        public virtual void Switch(bool on)
        { 
        
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            AccessoryData data = new AccessoryData();
            GetData(data);

            data.Model    = mesh.Model.Name;
            data.Diffuse  = mesh.Textures.Diffuse  == null ? String.Empty : mesh.Textures.Diffuse.Name;
            data.Specular = mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name;
            data.Normals  = mesh.Textures.Normals  == null ? String.Empty : mesh.Textures.Normals.Name;

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;
            data.EnabledMesh = mesh.Enabled;

            data.Genre = Genre;

            data.DamageModulation        = DamageModulation;
            data.AccuracyModulation      = AccuracyModulation;
            data.RangeModulation         = RangeModulation;
            data.PenetrationModulation   = PenetrationModulation;
            data.RecoilModulation        = RecoilModulation;
            data.StoppingPowerModulation = StoppingPowerModulation;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public void GetData(AccessoryData data)
        {
            base.GetData(data);

            data.Model = mesh.Model.Name;
            data.Diffuse = mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name;
            data.Specular = mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name;
            data.Normals = mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name;

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght = body.Length;
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;
            data.EnabledMesh = mesh.Enabled;

            data.Genre = Genre;

            data.DamageModulation = DamageModulation;
            data.AccuracyModulation = AccuracyModulation;
            data.RangeModulation = RangeModulation;
            data.PenetrationModulation = PenetrationModulation;
            data.RecoilModulation = RecoilModulation;
            data.StoppingPowerModulation = StoppingPowerModulation;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Accessory Data
    /********************************************************************************/
    [Serializable]
    public class AccessoryData : StorableObjectData
    {
        public AccessoryData()
        {
            Type = typeof(Accessory);
        }

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
        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

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

        [CategoryAttribute("Firearm Parameters")]
        public float DamageModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float AccuracyModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float RangeModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float PenetrationModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float RecoilModulation { get; set; }
        [CategoryAttribute("Firearm Parameters")]
        public float StoppingPowerModulation { get; set; }
        [CategoryAttribute("Accessory Genre")]
        public String Genre { get; set; }
    }
}
/************************************************************************************/