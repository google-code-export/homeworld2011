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


        public float maxSpeed=100;
        /********************************************************************************/


        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public TracerParticleComponent(ParticleSystem particleSystem, GameObjectInstance gameObject,  bool enabled)
            : base(gameObject)
        {
            this.particleSystem = particleSystem;
            this.enabled = enabled;
            this.gameObject = gameObject;

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

            float distance = Vector3.Distance(startPos, endPos);
            //this.particleSystem.SetGravity(Vector3.Normalize(endPos - startPos) * maxSpeed);
            this.particleSystem.SetDuration(distance / maxSpeed);
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