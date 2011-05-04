using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Resources;
using PlagueEngine.Rendering;
namespace PlagueEngine.Particles
{
    class ParticleManager
    {
        public List<ParticleEmitter> particleEmitters = new List<ParticleEmitter>();
        public ParticleFactory particleFactory = null;

        public ParticleManager()
        {
            ParticleEmitter.particleManager = this;
        }

        public void CreateFactory(ContentManager content,Renderer renderer)
        {
            particleFactory = new ParticleFactory(content);
            particleFactory.SetRenderer(renderer);
            
        }



        public void Update(GameTime gameTime)
        {

            foreach (ParticleEmitter emitter in particleEmitters)
            {
                emitter.Update(gameTime);
            }
        }

        public void DrawParticles(GameTime gameTime)
        {


            foreach (ParticleEmitter emmiter in particleEmitters)
            {
                emmiter.Draw(gameTime);
            }
        }
    }
}
