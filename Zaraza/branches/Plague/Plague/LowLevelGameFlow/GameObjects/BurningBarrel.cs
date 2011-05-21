using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Particles.Components;

/************************************************************************************/
///PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// BurningBarrel
    /********************************************************************************/
    class BurningBarrel : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        ParticleEmitterComponent particleEmitter = null;
        MeshComponent meshComponent = null;
        CylindricalBodyComponent physicsComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(ParticleEmitterComponent particleEmitter,MeshComponent mesh,CylindricalBodyComponent physicsComponent)
        {
            this.particleEmitter = particleEmitter;
            this.meshComponent = mesh;
            this.physicsComponent = physicsComponent;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            meshComponent.ReleaseMe();
            particleEmitter.ReleaseEmitter();
            physicsComponent.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            BurningBarrelData data = new BurningBarrelData();

            GetData(data);

            data.Model = meshComponent.Model.Name;
            data.Diffuse = (meshComponent.Textures.Diffuse == null ? String.Empty : meshComponent.Textures.Diffuse.Name);
            data.Specular = (meshComponent.Textures.Specular == null ? String.Empty : meshComponent.Textures.Specular.Name);
            data.Normals = (meshComponent.Textures.Normals == null ? String.Empty : meshComponent.Textures.Normals.Name);

            data.InstancingMode = Renderer.InstancingModeToUInt(meshComponent.InstancingMode);

            data.Mass = physicsComponent.Mass;
            data.Elasticity = physicsComponent.Elasticity;
            data.StaticRoughness = physicsComponent.StaticRoughness;
            data.DynamicRoughness = physicsComponent.DynamicRoughness;
            data.Lenght = physicsComponent.Length;
            data.Radius = physicsComponent.Radius;
            data.Immovable = physicsComponent.Immovable;
            data.Translation = physicsComponent.SkinTranslation;
            data.SkinPitch = physicsComponent.Pitch;
            data.SkinRoll = physicsComponent.Roll;
            data.SkinYaw = physicsComponent.Yaw;
          
                    if(particleEmitter.particleSystem.settings.BlendState==BlendState.Additive)
                    {
                        data.blendState=1;
                    }
                    else if(particleEmitter.particleSystem.settings.BlendState==BlendState.AlphaBlend)
                    {
                        data.blendState=2;
                    }
                    else if(particleEmitter.particleSystem.settings.BlendState==BlendState.Opaque)
                    {
                        data.blendState=4;
                    }
                    else
                    {
                        data.blendState=3;
                    }
                  

            data.duration=particleEmitter.particleSystem.settings.DurationInSeconds;
            data.durationRandomnes=particleEmitter.particleSystem.settings.DurationRandomness;
            data.emitterVelocitySensitivity=particleEmitter.particleSystem.settings.EmitterVelocitySensitivity;
            data.endVelocity=particleEmitter.particleSystem.settings.EndVelocity;
            data.gravity=particleEmitter.particleSystem.settings.Gravity;
            data.maxColor=particleEmitter.particleSystem.settings.MaxColor;
            data.maxEndSize=particleEmitter.particleSystem.settings.MaxEndSize;
            data.maxHorizontalVelocity=particleEmitter.particleSystem.settings.MaxHorizontalVelocity;
            data.maxParticles=particleEmitter.particleSystem.settings.MaxParticles;
            data.maxRotateSpeed=particleEmitter.particleSystem.settings.MaxRotateSpeed;
            data.maxStartSize=particleEmitter.particleSystem.settings.MaxStartSize;
            data.maxVerticalVelocity=particleEmitter.particleSystem.settings.MaxVerticalVelocity;
            data.minColor=particleEmitter.particleSystem.settings.MinColor;
            data.minEndSize=particleEmitter.particleSystem.settings.MinEndSize;
            data.minHorizontalVelocity=particleEmitter.particleSystem.settings.MinHorizontalVelocity;
            data.minRotateSpeed=particleEmitter.particleSystem.settings.MinRotateSpeed;
            data.minStartSize=particleEmitter.particleSystem.settings.MinStartSize;
            data.minVerticalVelocity=particleEmitter.particleSystem.settings.MinVerticalVelocity;
            data.particleTexture=particleEmitter.particleSystem.settings.TextureName;
            data.particlesPerSecond=particleEmitter.particlesPerSecond;
            data.particleTranslation = particleEmitter.particleTranslation;




            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// BurningBarrelData
    /********************************************************************************/
    [Serializable]
    public class BurningBarrelData : GameObjectInstanceData
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
        public float Lenght { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }




        [CategoryAttribute("Time"),
        DescriptionAttribute("How long these particles will last.")]
           public double duration { get; set; }
        [CategoryAttribute("Time"),
        DescriptionAttribute("If greater than zero, some particles will last a shorter time than others")]
           public float durationRandomnes { get; set; }

        [CategoryAttribute("Initialization"),
        DescriptionAttribute("Translation against gameObject position.")]
           public Vector3 particleTranslation { get; set; }
        [CategoryAttribute("Initialization"),
        DescriptionAttribute("Alpha blending settings.1 - Additive, 2 - AlphaBlend, 3 - NonPremultiplied, 4 - Opaque. ")]
           public int blendState { get; set; }
        [CategoryAttribute("Initialization"),
        DescriptionAttribute("Maximum number of particles that can be displayed at one time.")]
           public int maxParticles { get; set; }
        [CategoryAttribute("Initialization"),
        DescriptionAttribute("Name of the texture used by this particle system.")]
           public String particleTexture { get; set; }
        [CategoryAttribute("Initialization"),
        DescriptionAttribute("Frequency at which particles will be created.")]
           public float particlesPerSecond { get; set; }

        [CategoryAttribute("Color"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public Color minColor { get; set; }
        [CategoryAttribute("Color"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public Color maxColor { get; set; }



        [CategoryAttribute("Size"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float minStartSize { get; set; }
        [CategoryAttribute("Size"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float maxStartSize { get; set; }
        [CategoryAttribute("Size"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float minEndSize { get; set; }
        [CategoryAttribute("Size"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float maxEndSize { get; set; }



        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Controls how the particle velocity will change over their lifetime. If set to 1, particles will keep going at the same speed as when they were created.If set to 0, particles will come to a complete stop right before they die. Values greater than 1 make the particles speed up over time.")]
           public float endVelocity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how much Y axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float minVerticalVelocity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float maxVerticalVelocity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float minHorizontalVelocity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
           public float maxHorizontalVelocity { get; set; }

        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Direction and strength of the gravity effect. Note that this can point in any direction, not just down! The fire effect points it upward to make the flames rise, and the smoke plume points it sideways to simulate wind.")]
           public Vector3 gravity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Controls how much particles are influenced by the velocity of the object which created them. You can see this in action with the explosion effect, where the flames continue to move in the same direction as the source projectile. The projectile trail particles, on the other hand, set this value very low so they are less affected by the velocity of the projectile.")]
           public float emitterVelocitySensitivity { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
           public float maxRotateSpeed { get; set; }
        [CategoryAttribute("Velocity"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
           public float minRotateSpeed { get; set; }


           


    }
    /********************************************************************************/

}
/************************************************************************************/