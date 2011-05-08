using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

using PlagueEngine.Resources;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;

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
        private  SunlightComponent sunlight      = null;
        /**********************/
        

        /**********************/
        // Specials
        /**********************/
        private  Color   clearColor = Color.FromNonPremultiplied(new Vector4(0.05f,0.05f,0.2f,1));
        private  Vector3 ambient    = new Vector3(0.1f, 0.1f, 0.1f);
        private  Vector3 fogColor   = new Vector3(0.5f, 0.5f, 0.5f);
        private  Vector2 fogRange   = new Vector2(0.997f, 1.0f);
        private  bool    fogEnabled = false;
        /**********************/


        /**********************/
        /// For picking
        /**********************/
        public List<SkinnedMeshComponent> skinnedMeshes = new List<SkinnedMeshComponent>();
        public List<MeshComponent> meshes = new List<MeshComponent>();

        private bool            drawRect   = false;
        private Color           rectColor  = Color.Green;
        private Effect          rectEffect = null;
        private VertexPositionColor[] rect = new VertexPositionColor[5];
        /**********************/


        /**********************/
        /// Helpers
        /**********************/
        internal BatchedMeshes        batchedMeshes        = null;
        internal BatchedSkinnedMeshes batchedSkinnedMeshes = null;
        internal DebugDrawer          debugDrawer          = null;
        
        internal List<PointLightComponent>                       pointLights = new List<PointLightComponent>();
        internal Dictionary<Texture2D, List<SpotLightComponent>> spotLights  = new Dictionary<Texture2D, List<SpotLightComponent>>();
        /**********************/


        /**********************/
        /// Deferred Shading
        /**********************/
        private Vector2 TextureSize;
        private Vector2 HalfPixel;

        private RenderTarget2D color            = null;
        private RenderTarget2D normal           = null;
        private RenderTarget2D depth            = null;
        private RenderTarget2D light            = null;
        private RenderTarget2D test             = null;

        private Quad           fullScreenQuad   = null;
        private Effect         clearEffect      = null;
        private Effect         debugEffect      = null;
        private Effect         directionalLight = null;
        private Effect         pointLight       = null;
        private Effect         spotLight        = null;
        private Effect         composition      = null;
        private BlendState     lightBlendState  = null;

        private PlagueEngineModel pointLightModel = null;
        private PlagueEngineModel spotLightModel  = null;

        private Quad topLeft     = null;
        private Quad topRight    = null;
        private Quad bottomLeft  = null;
        private Quad bottomRight = null;
        /**********************/


        /**********************/
        /// SSAO
        /**********************/
        private Effect    ssaoEffect     = null;
        private Effect    ssaoBlurEffect = null;
        private Texture2D ditherTexture  = null;
        private float     sampleRadius   = 0.62f;
        private float     distanceScale  = 304.5f;
        private RenderTarget2D ssao      = null;
        private RenderTarget2D ssaoDepth = null;
        private RenderTarget2D ssaoBlur  = null;
        float ssaoBias = 7.79f;
        public  bool           ssaoEnabled = false;
        /**********************/


        /**********************/
        /// Shadows
        /**********************/
        private RenderTarget2D shadowMapBlur = null;
        private Effect         gaussianBlur  = null;
        /**********************/


        /****************************************************************************/
            

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

            MeshComponent.renderer        = this;
            RenderableComponent.renderer  = this;
            SkinnedMeshComponent.renderer = this;
            Quad.renderer                 = this;
            PointLightComponent.renderer  = this;
            SpotLightComponent.renderer   = this;

            ExtendedMouseMovementState.display = graphics.GraphicsDevice.DisplayMode;            

            fogColor = clearColor.ToVector3();

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
                return new RenderConfig(Device.DisplayMode.Width,
                                        Device.DisplayMode.Height,
                                        graphics.IsFullScreen,
                                        graphics.PreferMultiSampling,
                                        graphics.SynchronizeWithVerticalRetrace);
            }
            
            set
            {
                graphics.PreferredDepthStencilFormat = depthFormat;
                graphics.PreferredBackBufferFormat = surfaceFormat;

                graphics.PreferredBackBufferHeight = value.Height;
                graphics.PreferredBackBufferWidth = value.Width;
                graphics.PreferMultiSampling = value.Multisampling;
                graphics.IsFullScreen = value.FullScreen;
                graphics.SynchronizeWithVerticalRetrace = value.VSync;

                graphics.ApplyChanges();

                if (currentCamera != null) currentCamera.Aspect = Device.Viewport.AspectRatio;

                Diagnostics.PushLog("Presentation Parameters Changed" +
                                    ". Resolution: " + Device.PresentationParameters.BackBufferWidth.ToString() +
                                    " x " + Device.PresentationParameters.BackBufferHeight.ToString() +
                                    " x " + Device.PresentationParameters.BackBufferFormat.ToString() +
                                    " x " + Device.PresentationParameters.DepthStencilFormat.ToString() +
                                    ". Multisampling: " + Device.PresentationParameters.MultiSampleCount.ToString() +
                                    ". Fullscreen: " + Device.PresentationParameters.IsFullScreen.ToString() +
                                    ". VSync: " + graphics.SynchronizeWithVerticalRetrace.ToString());
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
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.M)) sampleRadius += 0.01f;
            if (state.IsKeyDown(Keys.N)) sampleRadius -= 0.01f;

            if (state.IsKeyDown(Keys.B)) distanceScale += 0.10f;
            if (state.IsKeyDown(Keys.V)) distanceScale -= 0.10f;

            if (state.IsKeyDown(Keys.O)) ssaoBias += 0.01f;
            if (state.IsKeyDown(Keys.P)) ssaoBias -= 0.01f;

            batchedSkinnedMeshes.DeltaTime = time;

            if (currentCamera == null) return;

            /*************************/
            /// Cleaning Nuclex Shit
            /*************************/
            Device.BlendState = BlendState.Opaque;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            /*************************/

            //foreach (RenderableComponent renderableComponent in preRender)
            //{
            //    if (!renderableComponent.FrustrumInteresction(CurrentCamera.Frustrum)) continue;
            //    renderableComponent.PreRender(currentCamera);
            //}

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

            RenderShadows(Frustrum);

            RenderLights(ref ViewProjection, ref InverseViewProjection, ref CameraPosition, Frustrum);

            if (ssaoEnabled) RenderSSAO(ref Projection, ref View, currentCamera.ZFar, currentCamera.Aspect);
            else Device.SetRenderTarget(null);



            Device.SetRenderTarget(null);

            composition.Parameters["Ambient"].SetValue(ambient);
            composition.Parameters["FogEnabled"].SetValue(fogEnabled);
            composition.Parameters["FogColor"].SetValue(fogColor);
            composition.Parameters["FogRange"].SetValue(fogRange);
            composition.Parameters["SSAOEnabled"].SetValue(ssaoEnabled);

            composition.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();

            //Device.SetRenderTarget(test);
            particleManager.DrawParticles(gameTime);
            DrawRect();

            //Device.SetRenderTarget(null);

            //debugEffect.Parameters["Texture"].SetValue(color);
            //debugEffect.Techniques[0].Passes[0].Apply();
            //topLeft.Draw();

            //debugEffect.Parameters["Texture"].SetValue(ssaoBlur);
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
            /// Batched Skinned Meshes
            /************************************/
            batchedSkinnedMeshes.Effect.Parameters["View"].SetValue(view);
            batchedSkinnedMeshes.Effect.Parameters["Projection"].SetValue(projection);
            batchedSkinnedMeshes.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
            batchedSkinnedMeshes.Draw(frustrum);
            /************************************/


            /************************************/
            /// Debug Drawer
            /************************************/
            if (debugDrawer != null) debugDrawer.Draw(currentCamera.View, currentCamera.Projection);
            /************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render Lights
        /****************************************************************************/
        private void RenderLights(ref Matrix ViewProjection,ref Matrix InverseViewProjection,ref Vector3 CameraPosition,BoundingFrustum frustrum)
        {
            Device.SetRenderTarget(light);
            Device.Clear(Color.Transparent);
            Device.BlendState = lightBlendState;
            Device.DepthStencilState = DepthStencilState.DepthRead;

            /*********************************/
            /// Directional Light
            /*********************************/
            if (sunlight != null)
            {
                if (sunlight.Enabled)
                {
                    directionalLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
                    directionalLight.Parameters["CameraPosition"].SetValue(CameraPosition);

                    directionalLight.Parameters["LightDirection"].SetValue(sunlight.Direction);
                    directionalLight.Parameters["LightColor"].SetValue(sunlight.DiffuseColor);
                    directionalLight.Parameters["LightIntensity"].SetValue(sunlight.Intensity);

                    directionalLight.Techniques[0].Passes[0].Apply();
                    fullScreenQuad.Draw();
                }
            }
            /*********************************/


            /*********************************/
            /// Point Light
            /*********************************/
            pointLight.Parameters["ViewProjection"].SetValue(ViewProjection);
            pointLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
            pointLight.Parameters["CameraPosition"].SetValue(CameraPosition);

            Device.SetVertexBuffer(pointLightModel.VertexBuffer);
            Device.Indices = pointLightModel.IndexBuffer;
            
            foreach (PointLightComponent pointLightComponent in pointLights)
            {
                if (!pointLightComponent.Enabled) continue;
                if (!frustrum.Intersects(pointLightComponent.BoundingSphere)) continue;

                pointLight.Parameters["World"               ].SetValue(pointLightComponent.World);
                pointLight.Parameters["LightPosition"       ].SetValue(pointLightComponent.Position);
                pointLight.Parameters["LightRadius"         ].SetValue(pointLightComponent.Radius);
                pointLight.Parameters["LightColor"          ].SetValue(pointLightComponent.Color);
                pointLight.Parameters["LightIntensity"      ].SetValue(pointLightComponent.Intensity);
                pointLight.Parameters["LinearAttenuation"   ].SetValue(pointLightComponent.LinearAttenuation);
                pointLight.Parameters["QuadraticAttenuation"].SetValue(pointLightComponent.QuadraticAttenuation);

                if (Vector3.Distance(CameraPosition, pointLightComponent.Position) < pointLightComponent.Radius * 3)
                {
                    Device.RasterizerState = RasterizerState.CullClockwise;
                }
                else
                {
                    Device.RasterizerState = RasterizerState.CullCounterClockwise;
                }

                pointLight.Techniques[0].Passes[0].Apply();

                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, pointLightModel.VertexCount, 0, pointLightModel.TriangleCount);
            }
            /*********************************/

            Device.RasterizerState = RasterizerState.CullCounterClockwise;

            /*********************************/
            /// Spot Light
            /*********************************/
            spotLight.Parameters["ViewProjection"].SetValue(ViewProjection);
            spotLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
            spotLight.Parameters["CameraPosition"].SetValue(CameraPosition);

            Device.SetVertexBuffer(spotLightModel.VertexBuffer);
            Device.Indices = spotLightModel.IndexBuffer;
            
            /************************/
            /// Helpers
            /************************/
            Matrix LightViewProjection;
            BoundingFrustum LightFrustrum = new BoundingFrustum(Matrix.Identity);
            Matrix World;
            Vector3 CamDir;
            float DL;
            Vector3 Direction;
            /************************/
           
            foreach (KeyValuePair<Texture2D, List<SpotLightComponent>> pair in spotLights)
            {
                spotLight.Parameters["AttenuationTexture"].SetValue(pair.Key);
                
                foreach (SpotLightComponent spotLightcomponent in pair.Value)
                {

                    LightViewProjection = spotLightcomponent.ViewProjection;
                    LightFrustrum.Matrix = LightViewProjection;
                    
                    if (!spotLightcomponent.Enabled) continue;
                    if (!frustrum.Intersects(LightFrustrum)) continue;
                    
                    World = spotLightcomponent.World;
                    Direction = World.Forward;
                    Direction.Normalize();

                    spotLight.Parameters["World"               ].SetValue(World);
                    spotLight.Parameters["LightColor"          ].SetValue(spotLightcomponent.Color);
                    spotLight.Parameters["LightIntensity"      ].SetValue(spotLightcomponent.Intensity);
                    spotLight.Parameters["LightPosition"       ].SetValue(World.Translation);
                    spotLight.Parameters["LightDirection"      ].SetValue(Direction);
                    spotLight.Parameters["LightRadius"         ].SetValue(spotLightcomponent.Radius);
                    spotLight.Parameters["LightAngleCos"       ].SetValue(spotLightcomponent.AngleCos);
                    spotLight.Parameters["LightFarPlane"       ].SetValue(spotLightcomponent.FarPlane);
                    spotLight.Parameters["LinearAttenuation"   ].SetValue(spotLightcomponent.LinearAttenuation);
                    spotLight.Parameters["QuadraticAttenuation"].SetValue(spotLightcomponent.QuadraticAttenuation);
                    spotLight.Parameters["LightViewProjection" ].SetValue(LightViewProjection);

                    spotLight.Parameters["DepthPrecision"].SetValue(spotLightcomponent.FarPlane);
                    spotLight.Parameters["DepthBias"].SetValue(spotLightcomponent.DepthBias);
                    spotLight.Parameters["ShadowMap"].SetValue(spotLightcomponent.ShadowMap);

                    CamDir = World.Translation - CameraPosition;
                    CamDir.Normalize();
                    DL = Math.Abs(Vector3.Dot(Direction, -CamDir));
                    if (DL * 2 > spotLightcomponent.AngleCos)
                    {
                        Device.RasterizerState = RasterizerState.CullClockwise;
                    }
                    else
                    {
                        Device.RasterizerState = RasterizerState.CullCounterClockwise;
                    }

                    spotLight.Techniques[0].Passes[0].Apply();
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, spotLightModel.VertexCount, 0, spotLightModel.TriangleCount);
                }
            }
            /*********************************/

                               
            Device.BlendState        = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState   = RasterizerState.CullCounterClockwise;
            Device.SetRenderTarget(null);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render Shadows
        /****************************************************************************/
        private void RenderShadows(BoundingFrustum frustrum)
        {
            BoundingFrustum LightFrustrum = new BoundingFrustum(Matrix.Identity);
            Matrix LightViewProjection;
            float depthPrecision;
            Vector3 Positon;

            foreach (KeyValuePair<Texture2D, List<SpotLightComponent>> pair in spotLights)
            {             
                foreach (SpotLightComponent spotLightcomponent in pair.Value)
                {
                    LightViewProjection = spotLightcomponent.ViewProjection;
                    LightFrustrum.Matrix = LightViewProjection;

                    if (!spotLightcomponent.Enabled) continue;
                    if (!frustrum.Intersects(LightFrustrum)) continue;

                    Device.SetRenderTarget(spotLightcomponent.ShadowMap);                  
                    depthPrecision = spotLightcomponent.FarPlane;
                    Positon = spotLightcomponent.World.Translation;
                    
                    /*********************************/
                    /// Renderable Components
                    /*********************************/
                    foreach (RenderableComponent renderableComponent in renderableComponents)
                    {
                        if (!renderableComponent.FrustrumInteresction(LightFrustrum)) continue;

                        renderableComponent.DrawDepth(ref LightViewProjection,ref Positon, depthPrecision);
                    }
                    /*********************************/


                    /*********************************/
                    /// Batched Meshes
                    /*********************************/
                    batchedMeshes.DrawDepth(LightViewProjection,
                                            LightFrustrum,
                                            Positon,
                                            depthPrecision);
                    /*********************************/


                    /*********************************/
                    /// Blur
                    /*********************************/
                    fullScreenQuad.SetBuffers();
                    Device.SetRenderTarget(shadowMapBlur);
                    gaussianBlur.Parameters["Texture"].SetValue(spotLightcomponent.ShadowMap);
                    gaussianBlur.CurrentTechnique.Passes[0].Apply();
                    fullScreenQuad.JustDraw();
                    Device.SetRenderTarget(spotLightcomponent.ShadowMap);
                    gaussianBlur.Parameters["Texture"].SetValue(shadowMapBlur);
                    gaussianBlur.CurrentTechnique.Passes[1].Apply();
                    fullScreenQuad.JustDraw();
                    /*********************************/
                }
            }
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
            ssaoEffect.Parameters["SSAOBias"].SetValue(ssaoBias);

            ssaoEffect.Techniques[0].Passes[1].Apply();
            fullScreenQuad.JustDraw();

            //Device.SetRenderTarget(ssaoBlur);
            //ssaoBlurEffect.Techniques[0].Passes[0].Apply();
            //fullScreenQuad.JustDraw();

            Device.SetRenderTarget(ssao);
            gaussianBlur.Parameters["Texture"].SetValue(ssaoBlur);
            gaussianBlur.CurrentTechnique.Passes[0].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(ssaoBlur);
            gaussianBlur.Parameters["Texture"].SetValue(ssao);
            gaussianBlur.CurrentTechnique.Passes[1].Apply();
            fullScreenQuad.JustDraw();

            Device.SetRenderTarget(null);
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
        /// Sun Light
        /****************************************************************************/
        public SunlightComponent Sunlight
        {
            get
            {
                return sunlight;
            }
            
            set
            {
                sunlight = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Clear Color
        /****************************************************************************/
        public Color ClearColor
        {
            get
            {
                return clearColor;    
            }

            set
            {
                clearColor = value;
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
                case InstancingModes.StaticInstancing:  return 2;
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
                case 2:  return InstancingModes.StaticInstancing;
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
            directionalLight = contentManager.LoadEffect("DSDirectionalLight");
            pointLight       = contentManager.LoadEffect("DSPointLight");
            spotLight        = contentManager.LoadEffect("DSSpotLight");
            composition      = contentManager.LoadEffect("DSComposition");
            ssaoEffect       = contentManager.LoadEffect("SSAO");
            ssaoBlurEffect   = contentManager.LoadEffect("SSAOBlur");
            gaussianBlur     = contentManager.LoadEffect("GaussianBlur");
            rectEffect       = contentManager.LoadEffect("SSLineEffect");

            composition.Parameters["GBufferColor"].SetValue(color);
            composition.Parameters["GBufferDepth"].SetValue(depth);
            composition.Parameters["LightMap"].SetValue(light);
            composition.Parameters["SSAOTexture"].SetValue(ssaoBlur);
            composition.Parameters["HalfPixel"].SetValue(HalfPixel);
            
            directionalLight.Parameters["GBufferNormal"].SetValue(normal);
            directionalLight.Parameters["GBufferDepth"].SetValue(depth);
            directionalLight.Parameters["HalfPixel"].SetValue(HalfPixel);

            pointLight.Parameters["GBufferNormal"].SetValue(normal);
            pointLight.Parameters["GBufferDepth"].SetValue(depth);
            pointLight.Parameters["HalfPixel"].SetValue(HalfPixel);

            spotLight.Parameters["GBufferNormal"].SetValue(normal);
            spotLight.Parameters["GBufferDepth"].SetValue(depth);
            spotLight.Parameters["HalfPixel"].SetValue(HalfPixel);

            debugEffect.Parameters["HalfPixel"].SetValue(HalfPixel);

            pointLightModel = contentManager.LoadModel("Sphere");
            spotLightModel = contentManager.LoadModel("Cone");

            ditherTexture = contentManager.LoadTexture2D("RandomNormals");

            ssaoEffect.Parameters["DitherTexture"].SetValue(ditherTexture);
            ssaoEffect.Parameters["GBufferNormal"].SetValue(normal);
            ssaoEffect.Parameters["GBufferDepth"].SetValue(ssaoDepth);
            ssaoEffect.Parameters["HalfPixel"].SetValue(HalfPixel);

            ssaoBlurEffect.Parameters["SSAOTexture"].SetValue(ssao);
            ssaoBlurEffect.Parameters["HalfPixel"].SetValue(HalfPixel);
            ssaoBlurEffect.Parameters["GBufferNormal"].SetValue(normal);
            ssaoBlurEffect.Parameters["GBufferDepth"].SetValue(ssaoDepth);            
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
                                       SurfaceFormat.Color,
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

            shadowMapBlur = new RenderTarget2D(Device,
                                               512,
                                               512,
                                               false,
                                               SurfaceFormat.HalfVector2,
                                               DepthFormat.None);

            fullScreenQuad = new Quad(-1, 1, 1, -1);

            topLeft     = new Quad(-1.0f, 1.0f, 0.0f,  0.0f);
            topRight    = new Quad( 0.0f, 1.0f, 1.0f,  0.0f);
            bottomLeft  = new Quad(-1.0f, 0.0f, 0.0f, -1.0f);
            bottomRight = new Quad( 0.0f, 0.0f, 1.0f, -1.0f);

            lightBlendState = new BlendState();
            lightBlendState.ColorSourceBlend        = Blend.One;
            lightBlendState.ColorDestinationBlend   = Blend.One;
            lightBlendState.ColorBlendFunction      = BlendFunction.Add;
            lightBlendState.AlphaSourceBlend        = Blend.One;
            lightBlendState.AlphaDestinationBlend   = Blend.One;
            lightBlendState.AlphaBlendFunction      = BlendFunction.Add;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/