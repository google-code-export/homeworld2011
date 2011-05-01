using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

using PlagueEngine.Resources;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;

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
        private  Vector3 ambient    = new Vector3(0.2f, 0.2f, 0.2f);
        private  Vector3 fogColor   = new Vector3(0.5f, 0.5f, 0.5f);
        private  Vector2 fogRange   = new Vector2(0.995f, 1.0f);
        private  bool    fogEnabled = true;
        /**********************/


        /**********************/
        /// Helpers
        /**********************/
        internal BatchedMeshes        batchedMeshes        = null;
        internal BatchedSkinnedMeshes batchedSkinnedMeshes = null;
        internal DebugDrawer          debugDrawer          = null;
        
        internal List<PointLightComponent> pointLights = new List<PointLightComponent>();
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
        private Effect         composition      = null;
        private BlendState     lightBlendState  = null;

        private PlagueEngineModel pointLightModel = null;

        private Quad topLeft     = null;
        private Quad topRight    = null;
        private Quad bottomLeft  = null;
        private Quad bottomRight = null;
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

            Physics.PhysicsUlitities.graphics = this.graphics;

            MeshComponent.renderer        = this;
            RenderableComponent.renderer  = this;
            SkinnedMeshComponent.renderer = this;
            Quad.renderer                 = this;
            PointLightComponent.renderer  = this;
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
        /// Draw
        /****************************************************************************/
        public void Draw(TimeSpan time)
        {
            batchedSkinnedMeshes.DeltaTime = time;

            if (currentCamera == null) return;

            //foreach (RenderableComponent renderableComponent in preRender)
            //{
            //    if (!renderableComponent.FrustrumInteresction(CurrentCamera.Frustrum)) continue;
            //    renderableComponent.PreRender(currentCamera);
            //}

            //Render( currentCamera.Position,
            //        currentCamera.View,
            //        currentCamera.Projection,
            //        currentCamera.ViewProjection,
            //        false,
            //        Vector4.Zero);
             
            Device.SetRenderTargets(color, normal, depth);
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.BlendState = BlendState.Opaque;             
            clearEffect.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();            
            Device.DepthStencilState = DepthStencilState.Default;
            
            Render(currentCamera.Position,
                   currentCamera.View,
                   currentCamera.Projection,
                   currentCamera.ViewProjection);           
            
            RenderLights(currentCamera.ViewProjection,currentCamera.InverseViewProjection,currentCamera.Position);

            //Device.SetRenderTarget(test);
            Device.SetRenderTarget(null);

            Device.Clear(clearColor);

            composition.Parameters["Ambient"           ].SetValue(ambient);
            composition.Parameters["FogEnabled"        ].SetValue(fogEnabled);
            composition.Parameters["FogColor"          ].SetValue(fogColor);
            composition.Parameters["FogRange"          ].SetValue(fogRange);
            
            composition.Techniques[0].Passes[0].Apply();
            fullScreenQuad.Draw();

            //Device.SetRenderTarget(null);

            //debugEffect.Parameters["Texture"].SetValue(color);
            //debugEffect.Techniques[0].Passes[0].Apply();
            //topLeft.Draw();

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
        internal void Render(Vector3 cameraPosition,Matrix view,Matrix projection,Matrix viewProjection)
        {

            /************************************/
            /// Renderable Components
            /************************************/
            foreach (RenderableComponent renderableComponent in renderableComponents)
            {
                if (!renderableComponent.FrustrumInteresction(CurrentCamera.Frustrum)) continue;
                
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
            batchedMeshes.Draw();
            /************************************/


            /************************************/
            /// Batched Skinned Meshes
            /************************************/
            batchedSkinnedMeshes.Effect.Parameters["View"].SetValue(view);
            batchedSkinnedMeshes.Effect.Parameters["Projection"].SetValue(projection);
            batchedSkinnedMeshes.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
            batchedSkinnedMeshes.Draw();
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
        private void RenderLights(Matrix ViewProjection,Matrix InverseViewProjection,Vector3 CameraPosition)
        {
            Device.SetRenderTarget(light);
            Device.Clear(Color.Transparent);
            Device.BlendState = lightBlendState;
            Device.DepthStencilState = DepthStencilState.DepthRead;

            if (sunlight != null)
            {
                if (sunlight.Enabled)
                {
                    directionalLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
                    directionalLight.Parameters["CameraPosition"].SetValue(CameraPosition);

                    directionalLight.Parameters["LightDirection"].SetValue(sunlight.Direction);
                    directionalLight.Parameters["LightColor"].SetValue(sunlight.DiffuseColor);

                    directionalLight.Techniques[0].Passes[0].Apply();
                    fullScreenQuad.Draw();
                }
            }

            pointLight.Parameters["ViewProjection"].SetValue(ViewProjection);
            pointLight.Parameters["InverseViewProjection"].SetValue(InverseViewProjection);
            pointLight.Parameters["CameraPosition"].SetValue(CameraPosition);

            Device.SetVertexBuffer(pointLightModel.VertexBuffer);
            Device.Indices = pointLightModel.IndexBuffer;
            
            foreach (PointLightComponent pointLightComponent in pointLights)
            {
                if (!pointLightComponent.Enabled) continue;
                if (!currentCamera.Frustrum.Intersects(pointLightComponent.BoundingSphere)) continue;

                pointLight.Parameters["World"               ].SetValue(pointLightComponent.World);
                pointLight.Parameters["LightPosition"       ].SetValue(pointLightComponent.Position);
                pointLight.Parameters["LightRadius"         ].SetValue(pointLightComponent.Radius);
                pointLight.Parameters["LightColor"          ].SetValue(pointLightComponent.Color);
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
                               
            Device.BlendState        = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState   = RasterizerState.CullCounterClockwise;
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
            composition      = contentManager.LoadEffect("DSComposition");

            composition.Parameters["GBufferColor"].SetValue(color);
            composition.Parameters["GBufferDepth"].SetValue(depth);
            composition.Parameters["LightMap"].SetValue(light);
            composition.Parameters["HalfPixel"].SetValue(HalfPixel);
            
            directionalLight.Parameters["GBufferNormal"].SetValue(normal);
            directionalLight.Parameters["GBufferDepth"].SetValue(depth);
            directionalLight.Parameters["HalfPixel"].SetValue(HalfPixel);

            pointLight.Parameters["GBufferNormal"].SetValue(normal);
            pointLight.Parameters["GBufferDepth"].SetValue(depth);
            pointLight.Parameters["HalfPixel"].SetValue(HalfPixel);

            debugEffect.Parameters["HalfPixel"].SetValue(HalfPixel);

            pointLightModel = contentManager.LoadModel("Sphere");
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init Deferred Helpers
        /****************************************************************************/
        public void InitDeferredHelpers()
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
                                        DepthFormat.Depth24Stencil8);

            depth = new RenderTarget2D(Device,
                                       Device.PresentationParameters.BackBufferWidth,
                                       Device.PresentationParameters.BackBufferHeight,
                                       false,
                                       SurfaceFormat.Single,
                                       DepthFormat.Depth24Stencil8);

            light = new RenderTarget2D(Device,
                                       Device.PresentationParameters.BackBufferWidth,
                                       Device.PresentationParameters.BackBufferHeight,
                                       false,
                                       SurfaceFormat.Color,
                                       DepthFormat.Depth24Stencil8);
            
            test = new RenderTarget2D(Device,
                                      Device.PresentationParameters.BackBufferWidth,
                                      Device.PresentationParameters.BackBufferHeight,
                                      false,
                                      SurfaceFormat.Color,
                                      DepthFormat.Depth24Stencil8);

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