using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.ArtificialIntelligence.Controllers;

using PlagueEngine.Particles.Components;
/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Firearm
    /********************************************************************************/
    class Firearm : StorableObject
    {   

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public MeshComponent                mesh = null;
        public SquareBodyComponent          body = null;
        public AreaParticleEmitterComponent particle;
        public Attack basicAttack;
        public Attack additionalAttack;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool SideArm { get; private set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(MeshComponent       mesh,
                         SquareBodyComponent body,
                         Attack basicAttack,
                         Attack additionalAttack,
                         Rectangle icon,
                         Rectangle slotsIcon,
                         String description,
                         int descriptionWindowWidth,
                         int descriptionWindowHeight,
                         bool sideArm,
                         ParticleEmitterComponent particle)
        {
            this.mesh = mesh;
            this.body = body;            

            this.basicAttack = basicAttack;
            this.additionalAttack = additionalAttack;
            
            SideArm = sideArm;

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
                World *= Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                if (mesh != null) mesh.Enabled = true;
                if (body != null) body.DisableBody();
                if (particle != null) particle.enabled = false;
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body     != null) body.EnableBody();
                if (mesh     != null) mesh.Enabled = true;
                if (particle != null) particle.enabled = true;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            FirearmData data = new FirearmData();
            GetData(data);
            data.Model    = mesh.Model.Name;
            data.Diffuse  = (mesh.Textures.Diffuse == null ? String.Empty : mesh.Textures.Diffuse.Name);
            data.Specular = (mesh.Textures.Specular == null ? String.Empty : mesh.Textures.Specular.Name);
            data.Normals  = (mesh.Textures.Normals == null ? String.Empty : mesh.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(mesh.InstancingMode);

            data.AACooldownTicks   = additionalAttack.cooldown.Ticks;
            data.AAMaximumDamage   = additionalAttack.maxInflictedDamage;
            data.AAMaximumDistance = additionalAttack.maxAttackDistance;
            data.AAMinimumDamage   = additionalAttack.minInflictedDamage;
            data.AAMinimumDistance = additionalAttack.minAttackDistance;

            data.BACooldownTicks   = basicAttack.cooldown.Ticks;
            data.BAMaximumDamage   = basicAttack.maxInflictedDamage;
            data.BAMaximumDistance = basicAttack.maxAttackDistance;
            data.BAMinimumDamage   = basicAttack.minInflictedDamage;
            data.BAMinimumDistance = basicAttack.minAttackDistance;

            data.Mass             = body.Mass;
            data.Elasticity       = body.Elasticity;
            data.StaticRoughness  = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Lenght           = body.Length;
            data.Width            = body.Width;
            data.Height           = body.Height;
            data.Immovable        = body.Immovable;
            data.Translation      = body.SkinTranslation;
            data.SkinPitch        = body.Pitch;
            data.SkinRoll         = body.Roll;
            data.SkinYaw          = body.Yaw;
            data.EnabledPhysics   = body.Enabled;
            data.EnabledMesh      = mesh.Enabled;
            
            data.SideArm          = SideArm;

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

            if (body != null)
            {
                body.ReleaseMe();
                body = null;
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
    /// FirearmData
    /********************************************************************************/
    [Serializable]
    public class FirearmData : StorableObjectData
    {

        public FirearmData()
        {
            Type = typeof(Firearm);
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

        [CategoryAttribute("Basic Attack")]
        public float BAMinimumDistance { get; set; }
        [CategoryAttribute("Basic Attack")]
        public float BAMaximumDistance { get; set; }
        [CategoryAttribute("Basic Attack")]
        public long BACooldownTicks { get; set; }
        [CategoryAttribute("Basic Attack")]
        public int BAMaximumDamage { get; set; }
        [CategoryAttribute("Basic Attack")]
        public int BAMinimumDamage { get; set; }

        [CategoryAttribute("Additional Attack")]
        public float AAMinimumDistance { get; set; }
        [CategoryAttribute("Additional Attack")]
        public float AAMaximumDistance { get; set; }
        [CategoryAttribute("Additional Attack")]
        public long AACooldownTicks { get; set; }
        [CategoryAttribute("Additional Attack")]
        public int AAMaximumDamage { get; set; }
        [CategoryAttribute("Additional Attack")]
        public int AAMinimumDamage { get; set; }


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

        [CategoryAttribute("EnabledMesh")]
        public bool EnabledMesh { get; set; }

        [CategoryAttribute("Weapon Genre")]
        public bool SideArm { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/
