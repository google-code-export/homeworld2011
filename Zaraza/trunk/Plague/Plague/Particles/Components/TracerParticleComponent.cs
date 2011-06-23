using System;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;

/********************************************************************************/
/// PlagueEngine.Particles.Components
/********************************************************************************/
namespace PlagueEngine.Particles.Components
{



    /********************************************************************************/
    /// TracerParticleComponent
    /********************************************************************************/
    class TracerParticleComponent : GameObjectComponent
    {



        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public ParticleSystem particleSystem;
        internal static ParticleManager particleManager;

        public bool enabled;


        public float maxSpeed;
        /********************************************************************************/


        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public TracerParticleComponent(ParticleSystem particleSystem, GameObjectInstance gameObject,  bool enabled,float speed)
            : base(gameObject)
        {
            this.particleSystem = particleSystem;
            this.enabled = enabled;
            this.gameObject = gameObject;
            this.maxSpeed = speed;
            if (enabled)
            {
                particleManager.TracerParticleComponents.Add(this);
            }
        }
        /********************************************************************************/

        /********************************************************************************/
        /// EnableEmitter
        /********************************************************************************/
        public void EnableEmitter()
        {
            enabled = true;
        }
        /********************************************************************************/



        /********************************************************************************/
        /// EnableEmitter
        /********************************************************************************/
        public void DisableEmitter()
        {
            enabled = false;
        }
        /********************************************************************************/




        /********************************************************************************/
        /// ReleaseEmitter
        /********************************************************************************/
        public void ReleaseMe()
        {
            if (enabled)
            {
                particleManager.TracerParticleComponents.Remove(this);
            }
            base.ReleaseMe();
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


        public void SpawnNewParticle(Vector3 startPos, Vector3 endPos)
        {
            Random r = new Random();
            float randomNumber=(float)r.NextDouble();
            startPos = startPos + (endPos - startPos) * 0.5f * randomNumber;
            float distance = Vector3.Distance(startPos, endPos);
            
            this.particleSystem.SetDuration(distance / maxSpeed);

            this.particleSystem.SetOrientation(endPos - startPos);

            particleSystem.AddParticle(startPos, Vector3.Normalize(endPos - startPos) * maxSpeed);

        }



        /********************************************************************************/
        /// Update
        /********************************************************************************/
        public void Update(GameTime gameTime)
        {


            particleSystem.Update(gameTime);

        }
        /********************************************************************************/



    }
    /********************************************************************************/

}
/********************************************************************************/