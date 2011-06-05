using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using System.ComponentModel;
using PlagueEngine.Particles.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class ParticleEmitter : GameObjectInstance
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private ParticleEmitterComponent emitter = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(ParticleEmitterComponent emitter)
        {
            this.emitter = emitter;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            ParticleEmitterData data = new ParticleEmitterData();
            GetData(data);


            data.Duration = emitter.particleSystem.settings.DurationInSeconds;
            data.DurationRandomnes = emitter.particleSystem.settings.DurationRandomness;
            data.EmitterVelocitySensitivity = emitter.particleSystem.settings.EmitterVelocitySensitivity;
            data.VelocityEnd = emitter.particleSystem.settings.EndVelocity;
            data.Gravity = emitter.particleSystem.settings.Gravity;
            data.ColorMax = emitter.particleSystem.settings.MaxColor;
            data.EndSizeMax = emitter.particleSystem.settings.MaxEndSize;
            data.VelocityHorizontalMax = emitter.particleSystem.settings.MaxHorizontalVelocity;
            data.ParticlesMax = emitter.particleSystem.settings.MaxParticles;
            data.RotateSpeedMax = emitter.particleSystem.settings.MaxRotateSpeed;
            data.StartSizeMax = emitter.particleSystem.settings.MaxStartSize;
            data.VelocityVerticalMax = emitter.particleSystem.settings.MaxVerticalVelocity;
            data.ColorMin = emitter.particleSystem.settings.MinColor;
            data.EndSizeMin = emitter.particleSystem.settings.MinEndSize;
            data.VelocityHorizontalMin = emitter.particleSystem.settings.MinHorizontalVelocity;
            data.RotateSpeedMin = emitter.particleSystem.settings.MinRotateSpeed;
            data.StartSizeMin = emitter.particleSystem.settings.MinStartSize;
            data.VelocityVerticalMin = emitter.particleSystem.settings.MinVerticalVelocity;
            data.ParticleTexture = emitter.particleSystem.settings.TextureName;
            data.ParticlesPerSecond = emitter.particlesPerSecond;
            data.EmitterTranslation = emitter.particleTranslation;
            data.ParticlesEnabled = emitter.enabled;

            return data;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            emitter.ReleaseMe();
            emitter = null;
        }
        /****************************************************************************/

    }







    /********************************************************************************/
    /// ParticleEmitterData
    /********************************************************************************/
    [Serializable]
    public class ParticleEmitterData : GameObjectInstanceData
    {


        [CategoryAttribute("Particles")]
        public bool ParticlesEnabled { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("How long these particles will last.")]
        public double Duration { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("If greater than zero, some particles will last a shorter time than others")]
        public float DurationRandomnes { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Translation against gameObject position.")]
        public Vector3 EmitterTranslation { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Alpha blending settings.1 - Additive, 2 - AlphaBlend, 3 - NonPremultiplied, 4 - Opaque. ")]
        public int BlendState { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Maximum number of particles that can be displayed at one time.")]
        public int ParticlesMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Name of the texture used by this particle system.")]
        public String ParticleTexture { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Frequency at which particles will be created.")]
        public float ParticlesPerSecond { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color ColorMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling the particle color and alpha. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public Color ColorMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float StartSizeMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big the particles are when first created. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float StartSizeMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float EndSizeMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how big particles become at the end of their life. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float EndSizeMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Controls how the particle velocity will change over their lifetime. If set to 1, particles will keep going at the same speed as when they were created.If set to 0, particles will come to a complete stop right before they die. Values greater than 1 make the particles speed up over time.")]
        public float VelocityEnd { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much Y axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityVerticalMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityVerticalMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityHorizontalMin { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how much X and Z axis velocity to give each particle. Values for individual particles are randomly chosen from somewhere between min and max.")]
        public float VelocityHorizontalMax { get; set; }

        [CategoryAttribute("Particles"),
        DescriptionAttribute("Direction and strength of the gravity effect. Note that this can point in any direction, not just down! The fire effect points it upward to make the flames rise, and the smoke plume points it sideways to simulate wind.")]
        public Vector3 Gravity { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Controls how much particles are influenced by the velocity of the object which created them. You can see this in action with the explosion effect, where the flames continue to move in the same direction as the source projectile. The projectile trail particles, on the other hand, set this value very low so they are less affected by the velocity of the projectile.")]
        public float EmitterVelocitySensitivity { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float RotateSpeedMax { get; set; }
        [CategoryAttribute("Particles"),
        DescriptionAttribute("Range of values controlling how fast the particles rotate. Values for individual particles are randomly chosen from somewhere between min and max. If both these values are set to 0, the particle system will automatically switch to an alternative shader technique that does not support rotation, and thus requires significantly less GPU power. This means if you don't need the rotation effect, you may get a performance boost from leaving these values at 0.")]
        public float RotateSpeedMin { get; set; }
    }
    /********************************************************************************/



}
