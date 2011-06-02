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
                         AreaParticleEmitterComponent particle)
        {
            this.mesh = mesh;
            this.body = body;

            this.particle = particle;

            this.basicAttack = basicAttack;
            this.additionalAttack = additionalAttack;


            SideArm = sideArm;

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
                World *= Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                if (mesh != null) mesh.Enabled = true;
                if (body != null) body.DisableBody();
                if(particle!=null) particle.enabled = false;
            }
            else
            {
                if (getWorld != null) World = GetWorld();
                if (body != null) body.EnableBody();
                if (mesh != null) mesh.Enabled = true;
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


            data.duration = particle.particleSystem.settings.DurationInSeconds;
            data.durationRandomnes = particle.particleSystem.settings.DurationRandomness;
            data.emitterVelocitySensitivity = particle.particleSystem.settings.EmitterVelocitySensitivity;
            data.endVelocity = particle.particleSystem.settings.EndVelocity;
            data.gravity = particle.particleSystem.settings.Gravity;
            data.maxColor = particle.particleSystem.settings.MaxColor;
            data.maxEndSize = particle.particleSystem.settings.MaxEndSize;
            data.maxHorizontalVelocity = particle.particleSystem.settings.MaxHorizontalVelocity;
            data.maxParticles = particle.particleSystem.settings.MaxParticles;
            data.maxRotateSpeed = particle.particleSystem.settings.MaxRotateSpeed;
            data.maxStartSize = particle.particleSystem.settings.MaxStartSize;
            data.maxVerticalVelocity = particle.particleSystem.settings.MaxVerticalVelocity;
            data.minColor = particle.particleSystem.settings.MinColor;
            data.minEndSize = particle.particleSystem.settings.MinEndSize;
            data.minHorizontalVelocity = particle.particleSystem.settings.MinHorizontalVelocity;
            data.minRotateSpeed = particle.particleSystem.settings.MinRotateSpeed;
            data.minStartSize = particle.particleSystem.settings.MinStartSize;
            data.minVerticalVelocity = particle.particleSystem.settings.MinVerticalVelocity;
            data.particleTexture = particle.particleSystem.settings.TextureName;
            data.particlesPerSecond = particle.particlesPerSecond;
            data.particleTranslation = particle.particleTranslation;
            data.SpawnAreaLength = particle.areaLength;
            data.SpawnAreaWidth = particle.areaWidth;
            data.ParticlesEnabled = particle.enabled;


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


            particle.ReleaseMe();


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





        [CategoryAttribute("ParticlesSettings")]
        public bool ParticlesEnabled { get; set; }


        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("How long these particles will last.")]
        public double duration { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("If greater than zero, some particles will last a shorter time than others")]
        public float durationRandomnes { get; set; }

        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Translation against gameObject position.")]
        public Vector3 particleTranslation { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Alpha blending settings.1 - Additive, 2 - AlphaBlend, 3 - NonPremultiplied, 4 - Opaque. ")]
        public int blendState { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Maximum number of particles that can be displayed at one time.")]
        public int maxParticles { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Name of the texture used by this particle system.")]
        public String particleTexture { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Frequency at which particles will be created.")]
        public float particlesPerSecond { get; set; }

        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color minColor { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color maxColor { get; set; }



        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float minStartSize { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float maxStartSize { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float minEndSize { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float maxEndSize { get; set; }



        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Controls how the particle velocity will change over their lifetime. If set to 1, particles will keep going at the same speed as when they were created.If set to 0, particles will come to a complete stop right before they die. Values greater than 1 make the particles speed up over time.")]
        public float endVelocity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how much Y axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float minVerticalVelocity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float maxVerticalVelocity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float minHorizontalVelocity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float maxHorizontalVelocity { get; set; }

        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Direction and strength of the gravity effect. Note that this can point in any direction, not just down! The fire effect points it upward to make the flames rise, and the smoke plume points it sideways to simulate wind.")]
        public Vector3 gravity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Controls how much particles are influenced by the velocity of the object which created them. You can see this in action with the explosion effect, where the flames continue to move in the same direction as the source projectile. The projectile trail particles, on the other hand, set this value very low so they are less affected by the velocity of the projectile.")]
        public float emitterVelocitySensitivity { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float maxRotateSpeed { get; set; }
        [CategoryAttribute("ParticlesSettings"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float minRotateSpeed { get; set; }


        [CategoryAttribute("ParticlesSettings")]
        public float SpawnAreaLength { get; set; }
        [CategoryAttribute("ParticlesSettings")]
        public float SpawnAreaWidth { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/
