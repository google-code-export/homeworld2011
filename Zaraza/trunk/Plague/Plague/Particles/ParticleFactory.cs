using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering;

using PlagueEngine.Resources;

namespace PlagueEngine.Particles
{
    class ParticleFactory
    {
        private ContentManager content = null;
        
        private Renderer renderer = null;
        public ParticleFactory(ContentManager content)
        {
            this.content = content;
        }


        public void SetRenderer(Renderer renderer)
        {
            this.renderer = renderer;
        }

        public ParticleEmitter CreateParticleEmitter(
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
            Vector3 initialPosition)
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

            Effect particleEffect = content.LoadEffect("ParticleEffect");
            particleEffect.Parameters["DepthTexture"].SetValue(renderer.depth);
            ParticleSystem particleSystem = new ParticleSystem(renderer.Device, renderer.CurrentCamera, content.LoadTexture2D(particleTexture), particleEffect.Clone(), settings);
            ParticleEmitter result = new ParticleEmitter(particleSystem,particlesPerSecond,initialPosition);
            return result;

                
                
                
        }
    }
}
