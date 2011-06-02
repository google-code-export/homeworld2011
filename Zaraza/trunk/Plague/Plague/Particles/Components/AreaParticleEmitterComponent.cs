﻿using System;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;

/********************************************************************************/
/// PlagueEngine.Particles.Components
/********************************************************************************/
namespace PlagueEngine.Particles.Components
{



    /********************************************************************************/
    /// ParticleEmitter
    /********************************************************************************/
    class AreaParticleEmitterComponent:GameObjectComponent
    {
       


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public ParticleSystem particleSystem;
        public float particlesPerSecond;
        float timeBetweenParticles;
        public Vector3 previousPosition;
        Vector3 newPosition;
        float timeLeftOver;

        Random random = new Random();
        
        internal static ParticleManager particleManager;
        public Vector3 particleTranslation = Vector3.Zero;

        public float areaWidth, areaLength;
        public bool enabled;
        /********************************************************************************/


        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public AreaParticleEmitterComponent(ParticleSystem particleSystem,
                               float particlesPerSecond, GameObjectInstance gameObject,Vector3 particleTranslation,Matrix world,float width,float length,bool enabled)
            : base(gameObject)
        {
            this.particleSystem = particleSystem;
            this.particlesPerSecond = particlesPerSecond;
            timeBetweenParticles = 1.0f / particlesPerSecond;
            this.enabled = enabled;
            this.gameObject = gameObject;
            this.particleTranslation = particleTranslation;
            this.areaLength = length;
            this.areaWidth = width;


            Vector3 pos = world.Translation;
            world.M41 = 0;
            world.M42 = 0;
            world.M43 = 0;
            Vector3 t = Vector3.Transform(particleTranslation, world);
            previousPosition = pos+t;
            newPosition = pos + t;

            if (enabled)
            {
                particleManager.AreaparticleEmitters.Add(this);
            }
        }
        /********************************************************************************/





        /********************************************************************************/
        /// ReleaseEmitter
        /********************************************************************************/
        public void ReleaseMe()
        {
            if (enabled)
            {
                particleManager.AreaparticleEmitters.Remove(this);
            }
            base.ReleaseMe();
        }
        /********************************************************************************/





        /********************************************************************************/
        /// SetNewposition
        /********************************************************************************/
        public void SetNewposition(Vector3 newPosition)
        {
            this.newPosition = newPosition;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// Draw
        /********************************************************************************/
        public void Draw(GameTime gameTime)
        {
            if (enabled)
            {
                particleSystem.Draw(gameTime);
            }
        }
        /********************************************************************************/



        /********************************************************************************/
        /// Update
        /********************************************************************************/
        public void Update(GameTime gameTime)
        {
            if (enabled)
            {

                particleSystem.Update(gameTime);

                Vector3 pos = gameObject.World.Translation;
                Matrix world = gameObject.World;
                world.M41 = 0;
                world.M42 = 0;
                world.M43 = 0;
                Vector3 t = Vector3.Transform(particleTranslation, world);
                newPosition = pos + t;

                if (gameTime == null)
                    throw new ArgumentNullException("gameTime");

                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (elapsedTime > 0)
                {
                    Vector3 velocity = (newPosition - previousPosition) / elapsedTime;

                    float timeToSpend = timeLeftOver + elapsedTime;

                    float currentTime = -timeLeftOver;

                    while (timeToSpend > timeBetweenParticles)
                    {
                        currentTime += timeBetweenParticles;
                        timeToSpend -= timeBetweenParticles;

                        float mu = currentTime / elapsedTime;

                        Vector3 position = Vector3.Lerp(previousPosition, newPosition, mu);


                        Random r = new Random();
                        float dlength = (float)r.NextDouble() * areaLength - areaLength / 2.0f;
                        float dwidth = (float)r.NextDouble() * areaWidth - areaWidth / 2.0f;
                        position.X += dlength;
                        position.Z += dwidth;
                        particleSystem.AddParticle(position, velocity);
                    }

                    timeLeftOver = timeToSpend;
                }

                previousPosition = newPosition;
            }
        }
        /********************************************************************************/



    }
    /********************************************************************************/

}
/********************************************************************************/