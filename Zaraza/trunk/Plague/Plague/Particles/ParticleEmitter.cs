﻿using System;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Particles
{

    class ParticleEmitter
    {
        #region Fields

        public ParticleSystem particleSystem;
        public float particlesPerSecond;
        float timeBetweenParticles;
        public Vector3 previousPosition;
        Vector3 newPosition;
        float timeLeftOver;

        Random random = new Random();

        internal static ParticleManager particleManager;
        #endregion


        /// <summary>
        /// Constructs a new particle emitter object.
        /// </summary>
        public ParticleEmitter(ParticleSystem particleSystem,
                               float particlesPerSecond, Vector3 initialPosition)
        {
            this.particleSystem = particleSystem;
            this.particlesPerSecond = particlesPerSecond;
            timeBetweenParticles = 1.0f / particlesPerSecond;

            previousPosition = initialPosition;
            newPosition = initialPosition;

            particleManager.particleEmitters.Add(this);
        }


        public void ReleaseEmitter()
        {
            particleManager.particleEmitters.Remove(this);
        }

        /// <summary>
        /// Updates the emitter, creating the appropriate number of particles
        /// in the appropriate positions.
        /// </summary>
        /// 

        public void SetNewposition(Vector3 newPosition)
        {
            this.newPosition = newPosition;
        }

        public void Draw(GameTime gameTime)
        {
            particleSystem.Draw(gameTime);
        }


        public void Update(GameTime gameTime)
        {
            particleSystem.Update(gameTime);

            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // Work out how fast we are moving.
                Vector3 velocity = (newPosition - previousPosition) / elapsedTime;

                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;
                
                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    // Work out the optimal position for this particle. This will produce
                    // evenly spaced particles regardless of the object speed, particle
                    // creation frequency, or game update rate.
                    float mu = currentTime / elapsedTime;

                    Vector3 position = Vector3.Lerp(previousPosition, newPosition, mu);

                    // Create the particle.
                    particleSystem.AddParticle(position, velocity);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }

            previousPosition = newPosition;
        
        }
    }
}
