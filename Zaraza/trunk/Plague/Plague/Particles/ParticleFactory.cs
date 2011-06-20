using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering;
using PlagueEngine.Resources;
using PlagueEngine.Particles.Components;
using PlagueEngine.LowLevelGameFlow;

/********************************************************************************/
/// PlagueEngine.Particles
/********************************************************************************/
namespace PlagueEngine.Particles
{

    /********************************************************************************/
    /// ParticleFactory
    /********************************************************************************/
    class ParticleFactory
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        private ContentManager content = null;
        
        private Renderer renderer = null;
        /********************************************************************************/





        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public ParticleFactory(ContentManager content)
        {
            this.content = content;
        }
        /********************************************************************************/



        /********************************************************************************/
        /// SetRenderer
        /********************************************************************************/
        public void SetRenderer(Renderer renderer)
        {
            this.renderer = renderer;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// CreateParticleEmitter
        /********************************************************************************/
        public ParticleEmitterComponent CreateParticleEmitterComponent(
            GameObjectInstance gameObject,
            int blendState,
            double duration,
            float durationRandomnes,
            float emitterVelocitySensitivity,
            float endVelocity,
            Vector3 gravity,
            Color maxColor,
            float maxEndSize,
            float maxHorizontalVelocity,
            int maxParticles,
            float maxRotateSpeed,
            float maxStartSize,
            float maxVerticalVelocity,
            Color minColor,
            float minEndSize,
            float minHorizontalVelocity,
            float minRotateSpeed,
            float minStartSize,
            float minVerticalVelocity,
            String particleTexture,
            float particlesPerSecond,
            Vector3 particleTranslation,
            Matrix world,
            bool enabled,
            int technique)
        {
            ParticleSettings settings = new ParticleSettings();
            switch(blendState)
            {
                case 1:
                    settings.BlendState=BlendState.Additive;
                    break;
                case 2:
                    settings.BlendState=BlendState.AlphaBlend;
                    break;
                case 3:
                    settings.BlendState=BlendState.NonPremultiplied;
                    break;
                case 4:
                    settings.BlendState=BlendState.Opaque;
                    break;
            }


            settings.Duration = TimeSpan.FromSeconds(duration);
            settings.DurationRandomness = durationRandomnes;
            settings.EmitterVelocitySensitivity = emitterVelocitySensitivity;
            settings.EndVelocity=endVelocity;
            settings.Gravity=gravity;
            settings.MaxColor=maxColor;
            settings.MaxEndSize=maxEndSize;
            settings.MaxHorizontalVelocity=maxHorizontalVelocity;
            settings.MaxParticles=maxParticles;
            settings.MaxRotateSpeed=maxRotateSpeed;
            settings.MaxStartSize=maxStartSize;
            settings.MaxVerticalVelocity=maxVerticalVelocity;
            settings.MinColor=minColor;
            settings.MinEndSize=minEndSize;
            settings.MinHorizontalVelocity=minHorizontalVelocity;
            settings.MinRotateSpeed=minRotateSpeed;
            settings.MinStartSize=minStartSize;
            settings.MinVerticalVelocity=minVerticalVelocity;
            settings.TextureName=particleTexture;
            settings.DurationInSeconds = duration;
            settings.Technique = technique;

            Effect particleEffect = content.LoadEffect("ParticleEffect");
            particleEffect.Parameters["DepthTexture"].SetValue(renderer.depth);
            ParticleSystem particleSystem = new ParticleSystem(renderer.Device, renderer, content.LoadTexture2D(particleTexture), particleEffect.Clone(), settings);
            ParticleEmitterComponent result = new ParticleEmitterComponent(particleSystem, particlesPerSecond, gameObject, particleTranslation,world,enabled);
            return result;

                
                
                
        }
        /********************************************************************************/






        /********************************************************************************/
        /// CreateAreaParticleEmitter
        /********************************************************************************/
        public AreaParticleEmitterComponent CreateAreaParticleEmitterComponent(
            GameObjectInstance gameObject,
            int blendState,
            double duration,
            float durationRandomnes,
            float emitterVelocitySensitivity,
            float endVelocity,
            Vector3 gravity,
            Color maxColor,
            float maxEndSize,
            float maxHorizontalVelocity,
            int maxParticles,
            float maxRotateSpeed,
            float maxStartSize,
            float maxVerticalVelocity,
            Color minColor,
            float minEndSize,
            float minHorizontalVelocity,
            float minRotateSpeed,
            float minStartSize,
            float minVerticalVelocity,
            String particleTexture,
            float particlesPerSecond,
            Vector3 particleTranslation,
            Matrix world,
            float width,
            float lenght,
            bool enabled)
        {
            ParticleSettings settings = new ParticleSettings();

            switch (blendState)
            {
                case 1:
                    settings.BlendState = BlendState.Additive;
                    break;
                case 2:
                    settings.BlendState = BlendState.AlphaBlend;
                    break;
                case 3:
                    settings.BlendState = BlendState.NonPremultiplied;
                    break;
                case 4:
                    settings.BlendState = BlendState.Opaque;
                    break;
            }


            settings.Duration = TimeSpan.FromSeconds(duration);
            settings.DurationRandomness = durationRandomnes;
            settings.EmitterVelocitySensitivity = emitterVelocitySensitivity;
            settings.EndVelocity = endVelocity;
            settings.Gravity = gravity;
            settings.MaxColor = maxColor;
            settings.MaxEndSize = maxEndSize;
            settings.MaxHorizontalVelocity = maxHorizontalVelocity;
            settings.MaxParticles = maxParticles;
            settings.MaxRotateSpeed = maxRotateSpeed;
            settings.MaxStartSize = maxStartSize;
            settings.MaxVerticalVelocity = maxVerticalVelocity;
            settings.MinColor = minColor;
            settings.MinEndSize = minEndSize;
            settings.MinHorizontalVelocity = minHorizontalVelocity;
            settings.MinRotateSpeed = minRotateSpeed;
            settings.MinStartSize = minStartSize;
            settings.MinVerticalVelocity = minVerticalVelocity;
            settings.TextureName = particleTexture;
            settings.DurationInSeconds = duration;

            Effect particleEffect = content.LoadEffect("ParticleEffect");
            particleEffect.Parameters["DepthTexture"].SetValue(renderer.depth);
            
            Texture2D texture=content.LoadTexture2D(particleTexture);


            ParticleSystem particleSystem = new ParticleSystem(renderer.Device, renderer,texture , particleEffect.Clone(), settings);
            AreaParticleEmitterComponent result = new AreaParticleEmitterComponent(particleSystem, particlesPerSecond, gameObject, particleTranslation, world,width,lenght,enabled);
            return result;




        }
        /********************************************************************************/












    }
    /********************************************************************************/


}
/********************************************************************************/