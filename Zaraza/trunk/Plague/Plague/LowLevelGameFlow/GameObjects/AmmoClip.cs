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
    /// Firearm
    /********************************************************************************/
    class AmmoClip : StorableObject
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent       mesh = null;
        public SquareBodyComponent body = null;

        public AmmunitionInfo AmmunitionInfo { get; private set; }
        private Dictionary<uint, AmmunitionVersionInfo> AmmunitionVersions  = null;

        public Stack<Bullet> Content = null;
        public int Capacity { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent mesh,
                         SquareBodyComponent body,
                         Rectangle icon,
                         Rectangle slotsIcon,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         ParticleEmitterComponent particle,
                         AmmunitionInfo ammunitionInfo,
                         Dictionary<uint, AmmunitionVersionInfo> ammunitionVersions,
                         Stack<Bullet> content,
                         int capacity)
        {
            this.mesh = mesh;
            this.body = body;            

            AmmunitionInfo      = ammunitionInfo;
            AmmunitionVersions  = ammunitionVersions;
            Content             = content;
            Capacity            = capacity;

            if (!body.Immovable)
            {
                body.SubscribeCollisionEvent(typeof(Terrain));
            }

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
                World = Matrix.Identity;
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
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            AmmoClipData data = new AmmoClipData();
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
            data.Width = body.Width;
            data.Height = body.Height;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;
            data.EnabledPhysics = body.Enabled;
            data.EnabledMesh = mesh.Enabled;

            data.Ammunition = AmmunitionInfo.Name;
            data.Capacity   = Capacity;
            data.Content    = Content.ToList();

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            if (body != null)
            {
                mesh.ReleaseMe();
                mesh = null;
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

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Bullet
    /********************************************************************************/
    [Serializable]
    public struct Bullet
    {
        public uint Version { get; set; }
        public bool PPP     { get; set; }
    }
    /********************************************************************************/
    

    /********************************************************************************/
    /// AmmoClipData
    /********************************************************************************/
    [Serializable]
    public class AmmoClipData : StorableObjectData
    {

        public AmmoClipData()
        {
            Type = typeof(AmmoClip);
            Content = new List<Bullet>();
        }

        [CategoryAttribute("Model")]
        public String Model { get; set; }

        [CategoryAttribute("Model")]
        public String Diffuse { get; set; }
        [CategoryAttribute("Model")]
        public String Specular { get; set; }
        [CategoryAttribute("Model")]
        public String Normals { get; set; }

        [CategoryAttribute("Model"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }
        [CategoryAttribute("Model")]
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

        [CategoryAttribute("Ammunition")]
        public String Ammunition { get; set; }
        [CategoryAttribute("Ammunition")]
        public int Capacity { get; set; }
        [CategoryAttribute("Ammunition")]
        public List<Bullet> Content { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/