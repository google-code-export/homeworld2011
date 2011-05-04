using System;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Particles
{
    /// <summary>
    /// Helper for objects that want to leave particles behind them as they
    /// move around the world. This emitter implementation solves two related
    /// problems:
    /// 
    /// If an object wants to create particles very slowly, less than once per
    /// frame, it can be a pain to keep track of which updates ought to create
    /// a new particle versus which should not.
    /// 
    /// If an object is moving quickly and is creating many particles per frame,
    /// it will look ugly if these particles are all bunched up together. Much
    /// better if they can be spread out along a line between where the object
    /// is now and where it was on the previous frame. This is particularly
    /// important for leaving trails behind fast moving objects such as rockets.
    /// 
    /// This emitter class keeps track of a moving object, remembering its
    /// previous position so it can calculate the velocity of the object. It
    /// works out the perfect locations for creating particles at any frequency
    /// you specify, regardless of whether this is faster or slower than the
    /// game update rate.
    /// </summary>
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


        Vector3 RandomPointOnCircle()
        {
            const float radius = 30;
            const float height = 40;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            
            return new Vector3(245 + x * radius,24+ y * radius + height, 45);
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
            const int fireParticlesPerFrame = 20;

            // Create a number of fire particles, randomly positioned around a circle.
            for (int i = 0; i < fireParticlesPerFrame; i++)
            {
                particleSystem.AddParticle(RandomPointOnCircle(), Vector3.Zero);
            }
        }
    }
}
