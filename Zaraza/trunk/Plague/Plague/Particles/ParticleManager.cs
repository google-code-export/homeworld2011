using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
using PlagueEngine.Particles.Components;


/********************************************************************************/
/// PlagueEngine.Particles
/********************************************************************************/
namespace PlagueEngine.Particles
{


    /********************************************************************************/
    /// ParticleManager
    /********************************************************************************/
    class ParticleManager
    {


        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public List<ParticleEmitterComponent> particleEmitters = new List<ParticleEmitterComponent>();

        public List<AreaParticleEmitterComponent> AreaparticleEmitters = new List<AreaParticleEmitterComponent>();
        public List<TracerParticleComponent> TracerParticleComponents = new List<TracerParticleComponent>();
        public ParticleFactory particleFactory = null;
        /********************************************************************************/


        /********************************************************************************/
        /// Constructor
        /********************************************************************************/
        public ParticleManager()
        {
            ParticleEmitterComponent.particleManager = this;
            AreaParticleEmitterComponent.particleManager = this;
            TracerParticleComponent.particleManager = this;
        }
        /********************************************************************************/



        /********************************************************************************/
        /// CreateFactory
        /********************************************************************************/
        public void CreateFactory(ContentManager content,Renderer renderer)
        {
            particleFactory = new ParticleFactory(content);
            particleFactory.SetRenderer(renderer);
            
        }
        /********************************************************************************/





        /********************************************************************************/
        /// Update
        /********************************************************************************/
        public void Update(GameTime gameTime)
        {

            foreach (ParticleEmitterComponent emitter in particleEmitters)
            {
                emitter.Update(gameTime);
            }

            foreach (AreaParticleEmitterComponent emitter in AreaparticleEmitters)
            {
                emitter.Update(gameTime);
            }
            foreach (TracerParticleComponent emitter in TracerParticleComponents )
            {
                emitter.Update(gameTime);
            }
        }
        /********************************************************************************/




        /********************************************************************************/
        /// DrawParticles
        /********************************************************************************/
        public void DrawParticles(GameTime gameTime)
        {


            foreach (ParticleEmitterComponent emmiter in particleEmitters)
            {
                emmiter.Draw(gameTime);
            }

            foreach (AreaParticleEmitterComponent emmiter in AreaparticleEmitters)
            {
                emmiter.Draw(gameTime);
            }
            foreach (TracerParticleComponent emmiter in TracerParticleComponents)
            {
                emmiter.Draw(gameTime);
            }
        }
        /********************************************************************************/



    }
    /********************************************************************************/


}
/********************************************************************************/