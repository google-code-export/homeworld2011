using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

using PlagueEngine.Resources;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Tools;
using PlagueEngine.LowLevelGameFlow.GameObjects;

using Microsoft.Xna.Framework.Input;
using PlagueEngine.Particles;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{  

    /********************************************************************************/
    /// Renderer
    /********************************************************************************/
    class Renderer
    {
     
        /****************************************************************************/
        /// Fields
        /****************************************************************************/

        /**********************/
        /// Basics
        /**********************/
        private  GraphicsDeviceManager      graphics          = null;
        internal ContentManager             contentManager    = null;
        private  RenderingComponentsFactory componentsFactory = null;
        /**********************/


        /*************************/
        /// Renderable Components
        /*************************/
        internal List<RenderableComponent> renderableComponents = new List<RenderableComponent>();
        internal List<RenderableComponent> preRender            = new List<RenderableComponent>();
        /*************************/


        /*************************/
        /// Particles
        /*************************/
        private ParticleManager particleManager = null;
        /*************************/


        /**********************/
        /// Global Components
        /**********************/
        private  CameraComponent   currentCamera = null;
        /**********************/
                       

        /**********************/
        /// Rect
        /**********************/
        private bool            drawRect   = false;
        private Color           rectColor  = Color.Green;
        private Effect          rectEffect = null;
        private VertexPositionColor[] rect = new VertexPositionColor[5];
        /**********************/


        /**********************/
        /// Helpers
        /**********************/
        internal BatchedMeshes        batchedMeshes         = null;
        internal BatchedSkinnedMeshes batchedSkinnedMeshes  = null;
        internal DebugDrawer          debugDrawer           = null;
        internal LightsManager        lightsManager         = null;        
        private  SpriteBatch          spriteBatch           = null;        
        internal List<FrontEndComponent> frontEndComponents = new List<FrontEndComponent>();
        internal Dictionary<String, SpriteFont> fonts       = new Dictionary<String, SpriteFont>();
        private Effect colorCorrection = null;        
        private float brightness = 0.0f;
        private float contrast   = 1.0f;
        /**********************/


        /**********************/
        /// Deferred Shading
        /**********************/
        private Vector2 TextureSize;
        private Vector2 HalfPixel;

        private  RenderTarget2D color            = null;
        private  RenderTarget2D normal           = null;
        internal RenderTarget2D depth            = null;
        private  RenderTarget2D light            = null;
        private  RenderTarget2D test             = null;

        private Quad           fullScreenQuad   = null;
        private Effect         clearEffect      = null;
        private Effect         debugEffect      = null;

        private Effect         composition      = null;
        
        private Quad topLeft     = null;
        private Quad topRight    = null;
        private Quad bottomLeft  = null;
        private Quad bottomRight = null;
        /**********************/


        /**********************/
        /// SSAO
        /**********************/
        private Effect         ssaoEffect     = null;        
        private Effect         ssaoBlurEffect = null;
        private Texture2D      ditherTexture  = null;
        public  float          sampleRadius   = 0.00005f;
        public  float          distanceScale  = 1000.0f;
        private RenderTarget2D ssao           = null;
        private RenderTarget2D ssaoDepth      = null;
        private RenderTarget2D ssaoBlur       = null;
        public  bool           ssaoEnabled    = false;
        /**********************/


        /**********************/
        /// Bloom 
        /**********************/
        private Effect bloomEffect       = null;
        private Effect bloomComposition  = null;
        private RenderTarget2D bloom     = null;
        private RenderTarget2D bloomBlur = null;
        private float bloomIntensity;
        private float baseIntensity;
        private float bloomSaturation;
        private float baseSaturation;
        private float bloomThreshold;
        /**********************/
        private RenderTarget2D final = null;


        /**********************/
        /// G-O EDITOR
        /**********************/
        internal static GameObjectEditorWindow editor = null;
        public List<MeshComponent> meshes = new List<MeshComponent>();
        public List<SkinnedMeshComponent> skinnedMeshes = new List<SkinnedMeshComponent>();
        /**********************/

        internal FogOfWarComponent fogOfWar = null;
                
        /****************************************************************************/

        public float Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                if (colorCorrection != null)
                {
                    colorCorrection.Parameters["Brightness"].SetValue(brightness);
                }
            }
        }       
        public float Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                if (colorCorrection != null)
                {
                    colorCorrection.Parameters["Contrast"].SetValue(contrast);
                }
            }
        }


        public float BloomIntensity
        {
            get { return bloomIntensity; }
            set
            {
                bloomIntensity = value;
                if (bloomComposition != null)
                {
                    bloomComposition.Parameters["BloomIntensity"].SetValue(bloomIntensity);
                }
            }
        }
        public float BloomSaturation
        {
            get { return bloomSaturation; }
            set
            {
                bloomSaturation = value;
                if (bloomComposition != null)
                {
                    bloomComposition.Parameters["BloomSaturation"].SetValue(bloomSaturation);
                }
            }
        }
        public float BaseIntensity
        {
            get { return baseIntensity; }
            set
            {
                baseIntensity = value;
                if (bloomComposition != null)
                {
                    bloomComposition.Parameters["BaseIntensity"].SetValue(baseIntensity);
                }
            }
        }
        public float BaseSaturation
        {
            get { return baseSaturation; }
            set
            {
                baseSaturation = value;
                if (bloomComposition != null)
                {
                    bloomComposition.Parameters["BaseSaturation"].SetValue(baseSaturation);
                }
            }
        }
        public float BloomThreshold
        {
            get { return bloomThreshold; }
            set
            {
                bloomThreshold = value;
                if (bloomEffect != null)
                {
                    bloomEffect.Parameters["BloomThreshold"].SetValue(bloomThreshold);
                }
            }
        }

        /****************************************************************************/
        /// Constants
        /****************************************************************************/
        private const SurfaceFormat     surfaceFormat   = SurfaceFormat.Color;
        private const DepthFormat       depthFormat     = DepthFormat.Depth24;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public Renderer(Game game,RenderConfig config)
        {
            graphics               = new GraphicsDeviceManager(game);           
            contentManager         = game.ContentManager;           
            CurrentConfiguration   = config;
            componentsFactory      = new RenderingComponentsFactory(this);            
            batchedMeshes          = new BatchedMeshes(contentManager, this);
            batchedSkinnedMeshes   = new BatchedSkinnedMeshes(contentManager, this);
            particleManager        = game.ParticleManager;
            Physics.PhysicsUlitities.graphics = this.graphics;

            FreeCamera.renderer           = this;
            MeshComponent.renderer        = this;
            RenderableComponent.renderer  = this;
            SkinnedMeshComponent.renderer = this;
            Quad.renderer                 = this;
            PointLightComponent.renderer  = this;
            SpotLightComponent.renderer   = this;
            FrontEndComponent.renderer    = this;
            OptionsMenu.renderer          = this;

            InGameMenu.renderer           = this;

            FogOfWarComponent.renderer = this;

            spriteBatch = new SpriteBatch(Device);
            
            ExtendedMouseMovementState.Display = graphics.GraphicsDevice.DisplayMode;            

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init Debug Drawer
        /****************************************************************************/
        public void InitDebugDrawer(Physics.PhysicsManager physicsManager)
        {
            debugDrawer = new DebugDrawer(this, physicsManager);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Enumerate Available Resolutions
        /****************************************************************************/
        public List<int[]> EnumerateAvailableResolutions()
        { 
            List<int[]> result  = new List<int[]>();
            int[] resolution    = null;

            foreach (DisplayMode displayMode in Device.Adapter.SupportedDisplayModes)
            {
                if (displayMode.Format == SurfaceFormat.Color)
                {
                    resolution = new int[2];
                    
                    resolution[0] = displayMode.Width;
                    resolution[1] = displayMode.Height;

                    result.Add(resolution);
                }
            }

            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// VSync
        /****************************************************************************/
        public bool VSync
        {
            get
            {
                return graphics.SynchronizeWithVerticalRetrace;
            }
            
            set
            {
                graphics.SynchronizeWithVerticalRetrace = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Current Configuration
        /****************************************************************************/
        public RenderConfig CurrentConfiguration
        {
            get
            {
                return new RenderConfig(Device.PresentationParameters.BackBufferWidth,
                                        Device.PresentationParameters.BackBufferHeight,
                                        graphics.IsFullScreen,
                                        graphics.PreferMultiSampling,
                                        graphics.SynchronizeWithVerticalRetrace,
                                        brightness,
                                        contrast,
                                        ssaoEnabled,
                                        bloomIntensity,
                                        baseIntensity,
                                        bloomSaturation,
                                        baseSaturation,
                                        bloomThreshold);
            }
            
            set
            {
                Brightness = value.Brightness;
                Contrast = value.Contrast;
                ssaoEnabled = value.SSAO;
                BloomIntensity = value.BaseIntensity;
                BaseIntensity = value.BaseIntensity;
                BloomSaturation = value.BloomSaturation;
                BaseSaturation = value.BaseSaturation;
                BloomThreshold = value.BloomThreshold;

                graphics.PreferredDepthStencilFormat = depthFormat;
                graphics.PreferredBackBufferFormat = surfaceFormat;

                graphics.PreferredBackBufferHeight = value.Height;
                graphics.PreferredBackBufferWidth = value.Width;
                graphics.PreferMultiSampling = value.Multisampling;
                graphics.IsFullScreen = value.FullScreen;
                graphics.SynchronizeWithVerticalRetrace = value.VSync;

                graphics.ApplyChanges();

                if (currentCamera != null) currentCamera.Aspect = Device.Viewport.AspectRatio;
#if DEBUG
                Diagnostics.PushLog("Presentation Parameters Changed" +
                                    ". Resolution: " + Device.PresentationParameters.BackBufferWidth.ToString() +
                                    " x " + Device.PresentationParameters.BackBufferHeight.ToString() +
                                    " x " + Device.PresentationParameters.BackBufferFormat.ToString() +
                                    " x " + Device.PresentationParameters.DepthStencilFormat.ToString() +
                                    ". Multisampling: " + Device.PresentationParameters.MultiSampleCount.ToString() +
                                    ". Fullscreen: " + Device.PresentationParameters.IsFullScreen.ToString() +
                                    ". VSync: " + graphics.SynchronizeWithVerticalRetrace.ToString());
#endif
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Rect
        /****************************************************************************/
        private void DrawRect()
        {
            if (!drawRect) return;

            rectEffect.CurrentTechnique.Passes[0].Apply();
            
            Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, rect, 0, 4);

            drawRect = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Selection Rect
        /****************************************************************************/
        public void DrawSelectionRect(Rectangle selectionRect)
        {
            float x =         (float)selectionRect.Left   / (float)Device.PresentationParameters.BackBufferWidth;
            float y = 1.0f - ((float)selectionRect.Bottom / (float)Device.PresentationParameters.BackBufferHeight);
            float w =         (float)selectionRect.Right  / (float)Device.PresentationParameters.BackBufferWidth;
            float h = 1.0f - ((float)selectionRect.Top    / (float)Device.PresentationParameters.BackBufferHeight);

            rect[0].Position = new Vector3(x,y,0);
            rect[0].Color = rectColor;

            rect[1].Position = new Vector3(w,y,0);
            rect[1].Color = rectColor;

            rect[2].Position = new Vector3(w,h,0);
            rect[2].Color = rectColor;

            rect[3].Position = new Vector3(x,h,0);
            rect[3].Color = rectColor;

            rect[4] = rect[0];

            drawRect = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw(TimeSpan time, GameTime gameTime)
        {
            batchedSkinnedMeshes.DeltaTime = time;

            if (currentCamera == null) return;

            if (spriteBatch.GraphicsDevice.IsDisposed) spriteBatch = new SpriteBatch(Device);

            /*************************/
            /// Cleaning Nuclex Shit
            /*************************/            
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            /*************************/


            /*************************/
            /// Render Fog of War
            /*************************/
            if (fogOfWar != null)
            {
                if (fogOfWar.Enabled) fogOfWar.Update(spriteBatch);
            }
            /*************************/
            
            Device.BlendState = BlendState.Opaque;

            foreach (RenderableComponent renderableComponent in preRender)
            {
                if (!renderableComponent.FrustrumInteresction(CurrentCamera.Frustrum)) continue;
                renderableComponent.PreRender(currentCamera);
            }

            /*********************************/
            /// Clear GBuffer
            /*********************************/
            Device.SetRenderTargets(color, normal, depth, ssaoDepth);
            Device.DepthStencilState = DepthStencilState.DepthRead;
            clearEffect.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();
            Device.DepthStencilState = DepthStencilState.Default;
            /*********************************/

            Vector3 CameraPosition = currentCamera.Position;
            Matrix View = currentCamera.View;
            Matrix Projection = currentCamera.Projection;
            Matrix ViewProjection = currentCamera.ViewProjection;
            Matrix InverseViewProjection = currentCamera.InverseViewProjection;
            BoundingFrustum Frustrum = currentCamera.Frustrum;

            Render(ref CameraPosition, ref View, ref Projection, ref ViewProjection, Frustrum);

            lightsManager.RenderShadows(Frustrum);

            lightsManager.RenderLights(ref ViewProjection, ref InverseViewProjection, ref CameraPosition, Frustrum);

            if (ssaoEnabled) RenderSSAO(ref Projection, ref View, currentCamera.ZFar, currentCamera.Aspect);
            
            Device.SetRenderTarget(test);

            composition.Parameters["Ambient"].SetValue(lightsManager.sunlight.Ambient);
            composition.Parameters["FogEnabled"].SetValue(lightsManager.sunlight.Fog);
            composition.Parameters["FogColor"].SetValue(lightsManager.sunlight.FogColor);
            composition.Parameters["FogRange"].SetValue(lightsManager.sunlight.FogRange);
            composition.Parameters["SSAOEnabled"].SetValue(ssaoEnabled);            

            composition.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();

            particleManager.DrawParticles(gameTime);
            Device.BlendState = BlendState.Opaque;


            Device.SetRenderTarget(bloom);
            bloomEffect.Parameters["Texture"].SetValue(test);
            bloomEffect.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.Draw();
            
            Device.SetRenderTarget(bloomBlur);
            ssaoBlurEffect.Parameters["Texture"].SetValue(bloom);
            ssaoBlurEffect.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(bloom);
            ssaoBlurEffect.Parameters["Texture"].SetValue(bloomBlur);
            ssaoBlurEffect.CurrentTechnique.Passes[1].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(final);
            Device.Clear(Color.Black);

            if (fogOfWar != null)
            {
                bloomComposition.Parameters["FogOfWarEnabled"].SetValue(fogOfWar.Enabled);
                if (fogOfWar.Enabled)
                {
                    bloomComposition.Parameters["FogColor"].SetValue(lightsManager.sunlight.FogColor);
                    bloomComposition.Parameters["FogOfWar"].SetValue(fogOfWar.Fog);
                    bloomComposition.Parameters["FogOfWar2"].SetValue(fogOfWar.FogSmall);
                    bloomComposition.Parameters["FogOfWarSize"].SetValue(fogOfWar.FogSize / fogOfWar.FogScale);
                    bloomComposition.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
                }
            }

            bloomComposition.Parameters["Texture"].SetValue(test);
            bloomComposition.Parameters["Bloom"].SetValue(bloom);
            bloomComposition.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.JustDraw();
            
            DrawFrontEnd(ViewProjection);            
#if DEBUG
            DrawIcons(ViewProjection);
#endif    
            DrawRect();

            
            Device.SetRenderTarget(null);
            colorCorrection.Parameters["Texture"].SetValue(final);
            colorCorrection.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();

            //Device.SetRenderTarget(null);
            //if (fogOfWar != null)
            //{
            //    debugEffect.Parameters["Texture"].SetValue(fogOfWar.Fog);
            //    debugEffect.Techniques[0].Passes[0].Apply();
            //    topLeft.Draw();
            //}

            //debugEffect.Parameters["Texture"].SetValue(normal);
            //debugEffect.Techniques[0].Passes[0].Apply();
            //topRight.Draw();

            //debugEffect.Parameters["Texture"].SetValue(light);
            //debugEffect.Techniques[0].Passes[0].Apply();
            //bottomLeft.Draw();

            //debugEffect.Parameters["Texture"].SetValue(test);
            //debugEffect.Techniques[0].Passes[0].Apply();
            //bottomRight.Draw();

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render
        /****************************************************************************/
        internal void Render(ref Vector3 cameraPosition,ref Matrix view,ref Matrix projection,ref Matrix viewProjection,BoundingFrustum frustrum)
        {

            /************************************/
            /// Batched Skinned Meshes
            /************************************/
            batchedSkinnedMeshes.Effect.Parameters["View"].SetValue(view);
            batchedSkinnedMeshes.Effect.Parameters["Projection"].SetValue(projection);
            batchedSkinnedMeshes.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
            batchedSkinnedMeshes.Draw(frustrum);
            /************************************/
            
            
            /************************************/
            /// Renderable Components
            /************************************/
            foreach (RenderableComponent renderableComponent in renderableComponents)
            {
                if (!renderableComponent.FrustrumInteresction(frustrum)) continue;
                
                renderableComponent.Effect.Parameters["View"].SetValue(view);
                renderableComponent.Effect.Parameters["Projection"].SetValue(projection);
                renderableComponent.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
                
                renderableComponent.Draw();
            }
            /************************************/


            /************************************/
            /// Batched Meshes
            /************************************/
            batchedMeshes.SetEffectParameter("View", view);
            batchedMeshes.SetEffectParameter("Projection", projection);
            batchedMeshes.SetEffectParameter("ViewProjection", viewProjection);
            batchedMeshes.Draw(frustrum);
            /************************************/


            /************************************/
            /// Debug Drawer
            /************************************/
            if (debugDrawer != null) debugDrawer.Draw(currentCamera.View, currentCamera.Projection);
            /************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pre Render
        /****************************************************************************/
        internal void PreRender(Vector3 cameraPosition, Matrix view, Matrix projection, Matrix viewProjection, BoundingFrustum frustrum, RenderTarget2D renderTarget,bool clip,Vector4 clipPlane)
        {
            Vector3 CameraPosition = cameraPosition;
            Matrix View = view;
            Matrix Projection = projection;
            Matrix ViewProjection = viewProjection;
            Matrix InverseViewProjection = Matrix.Invert(viewProjection);
            BoundingFrustum Frustrum = frustrum;


            /*********************************/
            /// Clear GBuffer
            /*********************************/
            Device.SetRenderTargets(color, normal, depth, ssaoDepth);
            Device.DepthStencilState = DepthStencilState.DepthRead;
            clearEffect.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();
            Device.DepthStencilState = DepthStencilState.Default;
            /*********************************/


            /************************************/
            /// Batched Skinned Meshes
            /************************************/
            batchedSkinnedMeshes.Effect.Parameters["View"].SetValue(view);
            batchedSkinnedMeshes.Effect.Parameters["Projection"].SetValue(projection);
            batchedSkinnedMeshes.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
            batchedSkinnedMeshes.Effect.Parameters["ClipPlaneEnabled"].SetValue(clip);
            batchedSkinnedMeshes.Effect.Parameters["ClipPlane"].SetValue(clipPlane);
            batchedSkinnedMeshes.Draw(frustrum);
            batchedSkinnedMeshes.Effect.Parameters["ClipPlaneEnabled"].SetValue(false);
            /************************************/


            /************************************/
            /// Renderable Components
            /************************************/
            foreach (RenderableComponent renderableComponent in renderableComponents)
            {
                if (!renderableComponent.FrustrumInteresction(frustrum)) continue;

                renderableComponent.Effect.Parameters["View"].SetValue(view);
                renderableComponent.Effect.Parameters["Projection"].SetValue(projection);
                renderableComponent.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
                renderableComponent.Effect.Parameters["ClipPlaneEnabled"].SetValue(clip);
                renderableComponent.Effect.Parameters["ClipPlane"].SetValue(clipPlane);
                renderableComponent.Draw();
                renderableComponent.Effect.Parameters["ClipPlaneEnabled"].SetValue(false);
            }
            /************************************/


            /************************************/
            /// Batched Meshes
            /************************************/
            batchedMeshes.SetEffectParameter("View", view);
            batchedMeshes.SetEffectParameter("Projection", projection);
            batchedMeshes.SetEffectParameter("ViewProjection", viewProjection);
            batchedMeshes.SetEffectParameter("ClipPlaneEnabled",clip);
            batchedMeshes.SetEffectParameter("ClipPlane",clipPlane);                
            batchedMeshes.Draw(frustrum);
            batchedMeshes.SetEffectParameter("ClipPlaneEnabled", false);            
            /************************************/

            Device.RasterizerState = RasterizerState.CullCounterClockwise;
            lightsManager.RenderLights(ref ViewProjection, ref InverseViewProjection, ref CameraPosition, Frustrum);

            if (ssaoEnabled) RenderSSAO(ref Projection, ref View, currentCamera.ZFar, currentCamera.Aspect);

            Device.SetRenderTarget(renderTarget);

            composition.Parameters["Ambient"].SetValue(lightsManager.sunlight.Ambient);
            composition.Parameters["FogEnabled"].SetValue(lightsManager.sunlight.Fog);
            composition.Parameters["FogColor"].SetValue(lightsManager.sunlight.FogColor);
            composition.Parameters["FogRange"].SetValue(lightsManager.sunlight.FogRange);
            composition.Parameters["SSAOEnabled"].SetValue(ssaoEnabled);  

            composition.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();

            Device.SetRenderTarget(null);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render SSAO
        /****************************************************************************/
        private void RenderSSAO(ref Matrix Projection,ref Matrix View, float zFar, float aspect)
        {            
            fullScreenQuad.SetBuffers();

            Device.SetRenderTarget(ssaoBlur);

            Vector3 cornerFrustum = Vector3.Zero;
            cornerFrustum.Y = (float)Math.Tan(Math.PI / 3.0 / 2.0) * zFar;
            cornerFrustum.X = cornerFrustum.Y * aspect;
            cornerFrustum.Z = zFar;

            ssaoEffect.Parameters["Projection"].SetValue(Projection);
            ssaoEffect.Parameters["View"].SetValue(View);
            ssaoEffect.Parameters["SampleRadius"].SetValue(sampleRadius);
            ssaoEffect.Parameters["DistanceScale"].SetValue(distanceScale);
            ssaoEffect.Parameters["CornerFrustrum"].SetValue(cornerFrustum);

            ssaoEffect.Techniques[0].Passes[0].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(ssao);
            ssaoBlurEffect.Parameters["Texture"].SetValue(ssaoBlur);
            ssaoBlurEffect.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(ssaoBlur);
            ssaoBlurEffect.Parameters["Texture"].SetValue(ssao);
            ssaoBlurEffect.CurrentTechnique.Passes[1].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(null);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Front End
        /****************************************************************************/
        private void DrawFrontEnd(Matrix ViewProjection)
        {
            if (spriteBatch.GraphicsDevice.IsDisposed)
            {
                spriteBatch = new SpriteBatch(Device);
            }


            int screenWidth = Device.PresentationParameters.BackBufferWidth;
            int screenHeight = Device.PresentationParameters.BackBufferHeight;

            spriteBatch.Begin();

            foreach (FrontEndComponent component in frontEndComponents)
            {
                component.Draw(spriteBatch, ref ViewProjection, screenWidth, screenHeight);
            }

            spriteBatch.End();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// DrawIcons
        /****************************************************************************/
        private void DrawIcons(Matrix ViewProjection)
        {
            int screenWidth = Device.PresentationParameters.BackBufferWidth;
            int screenHeight = Device.PresentationParameters.BackBufferHeight;

            spriteBatch.Begin();
            editor.DrawIcons(spriteBatch, ref ViewProjection, screenWidth, screenHeight);
            spriteBatch.End();
        }
        /****************************************************************************/




        /****************************************************************************/
        /// Components Factory
        /****************************************************************************/
        public RenderingComponentsFactory ComponentsFactory
        {
            get
            {
                return componentsFactory;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Graphics Device
        /****************************************************************************/
        public GraphicsDevice Device
        {
            get
            {
                return graphics.GraphicsDevice;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Graphics Manager
        /****************************************************************************/
        public GraphicsDeviceManager Manager
        {
            get
            {
                return graphics;
            }
        }
        /****************************************************************************/



        /****************************************************************************/
        /// SpriteBatch
        /****************************************************************************/
        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Current Camera
        /****************************************************************************/
        public CameraComponent CurrentCamera
        {
            get
            {
                return currentCamera;
            }

            set
            {
                currentCamera = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Renderable Component
        /****************************************************************************/
        public void ReleaseRenderableComponent(RenderableComponent component)
        {
            renderableComponents.Remove(component);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release PreRender Component
        /****************************************************************************/
        public void ReleasePreRenderComponent(RenderableComponent component)
        {
            preRender.Remove(component);
        }        
        /****************************************************************************/
                

        /****************************************************************************/
        /// Instancing Mode To UInt
        /****************************************************************************/
        public static uint InstancingModeToUInt(InstancingModes instancingMode)
        {
            switch (instancingMode)
            {
                case InstancingModes.NoInstancing:      return 1;
                case InstancingModes.DynamicInstancing: return 3;
                default: return 0;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// UInt To Instancing Mode
        /****************************************************************************/
        public static InstancingModes UIntToInstancingMode(uint value)
        {
            switch (value)
            {
                case 1:  return InstancingModes.NoInstancing;
                default: return InstancingModes.DynamicInstancing;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Effects
        /****************************************************************************/
        public void LoadEffects()
        {
            batchedMeshes.LoadEffects();
            batchedSkinnedMeshes.LoadEffect();

            clearEffect      = contentManager.LoadEffect("DSClear");
            debugEffect      = contentManager.LoadEffect("DSDebug");
            composition      = contentManager.LoadEffect("DSComposition");
            ssaoEffect       = contentManager.LoadEffect("SSAO");
            ssaoBlurEffect   = contentManager.LoadEffect("GaussianBlur15");
            rectEffect       = contentManager.LoadEffect("SSLineEffect");
            colorCorrection  = contentManager.LoadEffect("ColorCorrection");
            bloomEffect      = contentManager.LoadEffect("Bloom");
            bloomComposition = contentManager.LoadEffect("BloomComposition");
            
            composition.Parameters["GBufferColor"].SetValue(color);
            composition.Parameters["GBufferDepth"].SetValue(depth);
            composition.Parameters["LightMap"].SetValue(light);
            composition.Parameters["SSAOTexture"].SetValue(ssaoBlur);
            composition.Parameters["HalfPixel"].SetValue(HalfPixel);
                        
            debugEffect.Parameters["HalfPixel"].SetValue(HalfPixel);

            bloomEffect.Parameters["HalfPixel"].SetValue(HalfPixel);
            bloomComposition.Parameters["HalfPixel"].SetValue(HalfPixel);
            bloomEffect.Parameters["BloomThreshold"].SetValue(bloomThreshold);
            bloomComposition.Parameters["BloomIntensity"].SetValue(bloomIntensity);
            bloomComposition.Parameters["BloomSaturation"].SetValue(bloomSaturation);
            bloomComposition.Parameters["BaseIntensity"].SetValue(baseIntensity);
            bloomComposition.Parameters["BaseSaturation"].SetValue(baseSaturation);
            bloomComposition.Parameters["GBufferDepth"].SetValue(depth);

            colorCorrection.Parameters["HalfPixel"].SetValue(HalfPixel);
            colorCorrection.Parameters["Brightness"].SetValue(brightness);
            colorCorrection.Parameters["Contrast"].SetValue(contrast);
                       
            ditherTexture = contentManager.LoadTexture2D("RandomNormals");            

            ssaoEffect.Parameters["DitherTexture"].SetValue(ditherTexture);
            ssaoEffect.Parameters["GBufferNormal"].SetValue(normal);
            ssaoEffect.Parameters["GBufferDepth"].SetValue(ssaoDepth);
            ssaoEffect.Parameters["HalfPixel"].SetValue(HalfPixel);

            ssaoBlurEffect.Parameters["Texture"].SetValue(ssao);            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Load Fonts
        /****************************************************************************/
        public void LoadFonts(params String[] fonts)
        {
            SpriteFont spriteFont;
            foreach (String font in fonts)
            {
                spriteFont = contentManager.Load<SpriteFont>("Fonts\\" + font);
                this.fonts.Add(font, spriteFont);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init Deferred Helpers
        /****************************************************************************/
        public void InitHelpers()
        {
            TextureSize.X = Device.PresentationParameters.BackBufferWidth;
            TextureSize.Y = Device.PresentationParameters.BackBufferHeight;

            HalfPixel.X = 0.5f / TextureSize.X;
            HalfPixel.Y = 0.5f / TextureSize.Y;

            color = new RenderTarget2D(Device,
                                       Device.PresentationParameters.BackBufferWidth,
                                       Device.PresentationParameters.BackBufferHeight,
                                       false,
                                       SurfaceFormat.Color,
                                       DepthFormat.Depth24Stencil8);

            normal = new RenderTarget2D(Device,
                                        Device.PresentationParameters.BackBufferWidth,
                                        Device.PresentationParameters.BackBufferHeight,
                                        false,
                                        SurfaceFormat.Color,
                                        DepthFormat.None);

            depth = new RenderTarget2D(Device,
                                       Device.PresentationParameters.BackBufferWidth,
                                       Device.PresentationParameters.BackBufferHeight,
                                       false,
                                       SurfaceFormat.Single,
                                       DepthFormat.None);

            light = new RenderTarget2D(Device,
                                       Device.PresentationParameters.BackBufferWidth,
                                       Device.PresentationParameters.BackBufferHeight,
                                       false,
                                       SurfaceFormat.HdrBlendable,
                                       DepthFormat.None);
            
            test = new RenderTarget2D(Device,
                                      Device.PresentationParameters.BackBufferWidth,
                                      Device.PresentationParameters.BackBufferHeight,
                                      false,
                                      SurfaceFormat.Color,
                                      DepthFormat.Depth24Stencil8);

            ssao = new RenderTarget2D(Device,
                                      Device.PresentationParameters.BackBufferWidth,
                                      Device.PresentationParameters.BackBufferHeight,
                                      false,
                                      SurfaceFormat.Color,
                                      DepthFormat.None);

            ssaoDepth = new RenderTarget2D(Device,
                                           Device.PresentationParameters.BackBufferWidth,
                                           Device.PresentationParameters.BackBufferHeight,
                                           false,
                                           SurfaceFormat.Single,
                                           DepthFormat.None);

            ssaoBlur = new RenderTarget2D(Device,
                                          Device.PresentationParameters.BackBufferWidth,
                                          Device.PresentationParameters.BackBufferHeight,
                                          false,
                                          SurfaceFormat.Color,
                                          DepthFormat.None);

            bloom = new RenderTarget2D(Device,
                              Device.PresentationParameters.BackBufferWidth/2,
                              Device.PresentationParameters.BackBufferHeight/2,
                              false,
                              SurfaceFormat.Color,
                              DepthFormat.None);

            bloomBlur = new RenderTarget2D(Device,
                              Device.PresentationParameters.BackBufferWidth/2,
                              Device.PresentationParameters.BackBufferHeight/2,
                              false,
                              SurfaceFormat.Color,
                              DepthFormat.None);

            final = new RenderTarget2D(Device,
                              Device.PresentationParameters.BackBufferWidth,
                              Device.PresentationParameters.BackBufferHeight,
                              false,
                              SurfaceFormat.Color,
                              DepthFormat.None);

            fullScreenQuad = new Quad(-1, 1, 1, -1);

            topLeft     = new Quad(-1.0f, 1.0f, 0.0f,  0.0f);
            topRight    = new Quad( 0.0f, 1.0f, 1.0f,  0.0f);
            bottomLeft  = new Quad(-1.0f, 0.0f, 0.0f, -1.0f);
            bottomRight = new Quad( 0.0f, 0.0f, 1.0f, -1.0f);

            lightsManager = new LightsManager(normal, depth, light, fullScreenQuad, HalfPixel, this, contentManager);

            //float[] w = new float[7];
            //Vector2[] o = new Vector2[7];

            //calcSettings(1.0f/2048.0f, 1.0f/2048.0f,out w,out o);

        }
        /****************************************************************************/


        //void calcSettings(float w, float h, out float[] weights, out Vector2[] offsets)
        //{
        //    // 15 Samples
        //    weights = new float[7];
        //    offsets = new Vector2[7];
        //    // Calculate values for center pixel
        //    weights[0] = gaussianFn(0);
        //    offsets[0] = new Vector2(0, 0);
        //    float total = weights[0];
        //    // Calculate samples in pairs
        //    for (int i = 0; i < 3; i++)
        //    {
        //        // Weight each pair of samples according to Gaussian function
        //        float weight = gaussianFn(i + 1);
        //        weights[i * 2 + 1] = weight;
        //        weights[i * 2 + 2] = weight;
        //        total += weight * 2;
        //        // Samples are offset by 1.5 pixels, to make use of
        //        // filtering halfway between pixels
        //        float offset = i * 2 + 1.5f;
        //        Vector2 offsetVec = new Vector2(w, h) * offset;
        //        offsets[i * 2 + 1] = offsetVec;
        //        offsets[i * 2 + 2] = -offsetVec;
        //    }
        //    // Divide all weights by total so they will add up to 1
        //    for (int i = 0; i < weights.Length; i++)
        //        weights[i] /= total;
        //}


        //float gaussianFn(float x)
        //{
        //    return (float)((1.0f / Math.Sqrt(2 * Math.PI * 1 * 1)) * Math.Exp(-(x * x) / (2 * 1 * 1)));
        //}
    }
    /********************************************************************************/

}
/************************************************************************************/