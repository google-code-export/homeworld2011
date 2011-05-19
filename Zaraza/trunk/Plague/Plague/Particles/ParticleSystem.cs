using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;


/********************************************************************************/
/// PlagueEngine.Particles
/********************************************************************************/
namespace PlagueEngine.Particles
{


    /********************************************************************************/
    /// ParticleSystem
    /********************************************************************************/
    class ParticleSystem
    {




        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        public ParticleSettings settings = new ParticleSettings();
        Effect particleEffect;
        EffectParameter effectViewParameter;
        EffectParameter effectProjectionParameter;
        EffectParameter effectViewportScaleParameter;
        EffectParameter effectTimeParameter;
        ParticleVertex[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;
        float currentTime;
        int drawCounter;
        static Random random = new Random();
        private GraphicsDevice graphics=null;
        private Renderer renderer = null;
        public Texture2D texture=null;

        /********************************************************************************/





        /********************************************************************************/
            /// Constructor
        /********************************************************************************/
        public ParticleSystem(GraphicsDevice graphics,Renderer renderer,Texture2D texture,Effect effect,ParticleSettings settings)
        {
            this.graphics = graphics;
            this.renderer = renderer;
            this.texture = texture;
            this.particleEffect = effect;
            this.settings = settings;

            Initialize();
            LoadContent();

        }
        /********************************************************************************/




        /********************************************************************************/
            /// Initialize
        /********************************************************************************/
        private void Initialize()
        {
            particles = new ParticleVertex[settings.MaxParticles * 4];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

        }
        /********************************************************************************/



        /********************************************************************************/
            /// LoadContent
        /********************************************************************************/
        protected void LoadContent()
        {
            LoadParticleEffect();

            vertexBuffer = new DynamicVertexBuffer(graphics, ParticleVertex.VertexDeclaration,
                                                   settings.MaxParticles * 4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[settings.MaxParticles * 6];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);

                indices[i * 6 + 3] = (ushort)(i * 4 + 0);
                indices[i * 6 + 4] = (ushort)(i * 4 + 2);
                indices[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            indexBuffer = new IndexBuffer(graphics, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }
        /********************************************************************************/






        /********************************************************************************/
        /// LoadParticleEffect
        /********************************************************************************/
        void LoadParticleEffect()
        {
          

            EffectParameterCollection parameters = particleEffect.Parameters;

            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportScaleParameter = parameters["ViewportScale"];
            effectTimeParameter = parameters["CurrentTime"];
            parameters["Duration"].SetValue((float)settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(settings.DurationRandomness);
            parameters["Gravity"].SetValue(settings.Gravity);
            parameters["EndVelocity"].SetValue(settings.EndVelocity);
            parameters["MinColor"].SetValue(settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(
                new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));

            parameters["StartSize"].SetValue(
                new Vector2(settings.MinStartSize, settings.MaxStartSize));

            parameters["EndSize"].SetValue(
                new Vector2(settings.MinEndSize, settings.MaxEndSize));

    
            parameters["Texture"].SetValue(texture);
        }

        /********************************************************************************/




        /********************************************************************************/
        /// ParticleSettings
        /********************************************************************************/
        public void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            if (firstActiveParticle == firstFreeParticle)
                currentTime = 0;

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }
        /********************************************************************************/



        /********************************************************************************/
        /// RetireActiveParticles
        /********************************************************************************/
        void RetireActiveParticles()
        {
            float particleDuration = (float)settings.Duration.TotalSeconds;

            while (firstActiveParticle != firstNewParticle)
            {

                float particleAge = currentTime - particles[firstActiveParticle * 4].Time;

                if (particleAge < particleDuration)
                    break;

                particles[firstActiveParticle * 4].Time = drawCounter;

                firstActiveParticle++;

                if (firstActiveParticle >= settings.MaxParticles)
                    firstActiveParticle = 0;
            }
        }
        /********************************************************************************/





        /********************************************************************************/
        /// FreeRetiredParticles
        /********************************************************************************/
        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {

                int age = drawCounter - (int)particles[firstRetiredParticle * 4].Time;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= settings.MaxParticles)
                    firstRetiredParticle = 0;
            }
        }
        /********************************************************************************/





        /********************************************************************************/
            /// Draw
        /********************************************************************************/
        public void Draw(GameTime gameTime)
        {


            effectViewParameter.SetValue(renderer.CurrentCamera.View);
            effectProjectionParameter.SetValue(renderer.CurrentCamera.Projection);
       
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }
            if (firstActiveParticle != firstFreeParticle)
            {
                graphics.BlendState = settings.BlendState;
                graphics.DepthStencilState = DepthStencilState.None;

                effectViewportScaleParameter.SetValue(new Vector2(0.5f / graphics.Viewport.AspectRatio, -0.5f));

                effectTimeParameter.SetValue(currentTime);

                graphics.SetVertexBuffer(vertexBuffer);
                graphics.Indices = indexBuffer;

                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
                    }
                    else
                    {
                        graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (settings.MaxParticles - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (settings.MaxParticles - firstActiveParticle) * 2);

                        if (firstFreeParticle > 0)
                        {
                            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, firstFreeParticle * 4,
                                                         0, firstFreeParticle * 2);
                        }
                    }
                }

                graphics.DepthStencilState = DepthStencilState.Default;
            }

            drawCounter++;
        }
        /********************************************************************************/


        /********************************************************************************/
        /// AddNewParticlesToVertexBuffer
        /********************************************************************************/
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (firstFreeParticle - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (settings.MaxParticles - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle * 4,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            firstNewParticle = firstFreeParticle;
        }
        /********************************************************************************/





        /********************************************************************************/
        /// AddParticle
        /********************************************************************************/
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= settings.MaxParticles)
                nextFreeParticle = 0;

            if (nextFreeParticle == firstRetiredParticle)
                return;

            velocity *= settings.EmitterVelocitySensitivity;

            float horizontalVelocity = MathHelper.Lerp(settings.MinHorizontalVelocity,
                                                       settings.MaxHorizontalVelocity,
                                                       (float)random.NextDouble());

            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity,
                                          settings.MaxVerticalVelocity,
                                          (float)random.NextDouble());

            Color randomValues = new Color((byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255));

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].Random = randomValues;
                particles[firstFreeParticle * 4 + i].Time = currentTime;
            }

            firstFreeParticle = nextFreeParticle;
        }
        /********************************************************************************/




       
    }
    /********************************************************************************/







}
/********************************************************************************/