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
        /// CreateTracerParticleComponent
        /********************************************************************************/
        public TracerParticleComponent CreateTracerParticleComponent(
            GameObjectInstance gameObject,
            int blendState,
            Color maxColor,
            float maxEndSize,
            float maxStartSize,
            Color minColor,
            float minEndSize,
            float minStartSize,
            String particleTexture,
            bool enabled,
            int technique,
            float speed)
        {


            enabled = true;
            technique = 2;

                

            if (blendState == 0)
            {
                blendState = 2;
            }
            if (maxColor == new Color(0, 0, 0, 0))
            {
                maxColor = new Color(255, 0, 0, 255);
            }
            if(minColor == new Color(0,0,0,0))
            {
                minColor=new Color(255,0,0,255);
            }
            if(maxEndSize==0)
            {
                maxEndSize=5;
            }
            if(minEndSize==0)
            {
                minEndSize=5;
            }
            if(maxStartSize==0)
            {
                maxStartSize=5;
            }
            if(minStartSize==0)
            {
                minStartSize=5;
            }
            if(String.IsNullOrEmpty(particleTexture))
            {
                particleTexture=@"Particles\Fireshot";
            }
            
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


            settings.MaxColor = maxColor;
            settings.MaxEndSize = maxEndSize;
            settings.MaxStartSize = maxStartSize;
            settings.MinColor = minColor;
            settings.MinEndSize = minEndSize;
            settings.MinStartSize = minStartSize;
            settings.TextureName = particleTexture;
            settings.Technique = technique;



            Effect particleEffect = content.LoadEffect("ParticleEffect");
            particleEffect.Parameters["DepthTexture"].SetValue(renderer.depth);
            ParticleSystem particleSystem = new ParticleSystem(renderer.Device, renderer, content.LoadTexture2D(particleTexture), particleEffect.Clone(), settings);
            TracerParticleComponent result = new TracerParticleComponent(particleSystem, gameObject, enabled,speed);
            return result;




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