using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

using PlagueEngine.Resources;
using PlagueEngine.Rendering.Components;


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
        private  GraphicsDeviceManager       graphics               = null;
        internal ContentManager              contentManager         = null;
        private  RenderingComponentsFactory  componentsFactory      = null;

        internal List<RenderableComponent>   renderableComponents   = new List<RenderableComponent>();
        internal List<RenderableComponent>   preRender              = new List<RenderableComponent>();        

        private  CameraComponent             currentCamera          = null;
        private  SunlightComponent           sunlight               = null;
        private  Color                       clearColor             = Color.CornflowerBlue;
        private  Vector3                     ambient                = new Vector3(0.1f, 0.1f, 0.1f);

        internal BatchedMeshes               batchedMeshes          = null;
        internal DebugDrawer                 debugDrawer            = null;
        /****************************************************************************/
            

        /****************************************************************************/
        /// Constants
        /****************************************************************************/
        private const SurfaceFormat     surfaceFormat   = SurfaceFormat.Color;
        private const DepthFormat       depthFormat     = DepthFormat.Depth24Stencil8;
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

            Physics.PhysicsUlitities.graphics = this.graphics;

            MeshComponent.renderer       = this;
            RenderableComponent.renderer = this;
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
                graphics.PreferredDepthStencilFormat    = depthFormat;
                graphics.PreferredBackBufferFormat      = surfaceFormat;

                graphics.PreferredBackBufferHeight      = value.Height;
                graphics.PreferredBackBufferWidth       = value.Width;
                graphics.PreferMultiSampling            = value.Multisampling;
                graphics.IsFullScreen                   = value.FullScreen;
                graphics.SynchronizeWithVerticalRetrace = value.VSync;

                graphics.ApplyChanges();
                if (currentCamera != null) currentCamera.Aspect = Device.Viewport.AspectRatio;

                Diagnostics.PushLog("Presentation Parameters Changed" +
                                    ". Resolution: "    + Device.PresentationParameters.BackBufferWidth.ToString()    +
                                    " x "               + Device.PresentationParameters.BackBufferHeight.ToString()   + 
                                    " x "               + Device.PresentationParameters.BackBufferFormat.ToString()   + 
                                    " x "               + Device.PresentationParameters.DepthStencilFormat.ToString() +
                                    ". Multisampling: " + Device.PresentationParameters.MultiSampleCount.ToString()   +
                                    ". Fullscreen: "    + Device.PresentationParameters.IsFullScreen.ToString()       +
                                    ". VSync: "         + graphics.SynchronizeWithVerticalRetrace.ToString());
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw()
        {
            if (currentCamera == null) return;

            foreach (RenderableComponent renderableComponent in preRender)
            {
                renderableComponent.PreRender(currentCamera);
            }

            Device.Clear(clearColor);

            foreach (RenderableComponent renderableComponent in renderableComponents)
            {
                renderableComponent.Effect.Parameters["Ambient"       ].SetValue(ambient);
                renderableComponent.Effect.Parameters["CameraPosition"].SetValue(currentCamera.Position);
                renderableComponent.Effect.Parameters["View"          ].SetValue(currentCamera.View);
                renderableComponent.Effect.Parameters["Projection"    ].SetValue(currentCamera.Projection);
                renderableComponent.Effect.Parameters["ViewProjection"].SetValue(currentCamera.ViewProjection);

                if (sunlight != null && sunlight.Enabled)
                {
                    renderableComponent.Effect.Parameters["SunlightEnabled"  ].SetValue(true);
                    renderableComponent.Effect.Parameters["SunlightDirection"].SetValue(sunlight.Direction);
                    renderableComponent.Effect.Parameters["SunlightDiffuse"  ].SetValue(sunlight.DiffuseColor);
                    renderableComponent.Effect.Parameters["SunlightSpecular" ].SetValue(sunlight.SpecularColor);
                }
                else
                {
                    renderableComponent.Effect.Parameters["SunlightEnabled"].SetValue(false);
                }

                renderableComponent.Draw();
            }

            batchedMeshes.SetEffectParameter("Ambient",         ambient);
            batchedMeshes.SetEffectParameter("CameraPosition",  currentCamera.Position);
            batchedMeshes.SetEffectParameter("View",            currentCamera.View);
            batchedMeshes.SetEffectParameter("Projection",      currentCamera.Projection);
            batchedMeshes.SetEffectParameter("ViewProjection",  currentCamera.ViewProjection);

            if (sunlight != null && sunlight.Enabled)
            {
                batchedMeshes.SetEffectParameter("SunlightEnabled",   true);
                batchedMeshes.SetEffectParameter("SunlightDirection", sunlight.Direction);
                batchedMeshes.SetEffectParameter("SunlightDiffuse",   sunlight.DiffuseColor);
                batchedMeshes.SetEffectParameter("SunlightSpecular",  sunlight.SpecularColor);
            }
            else
            {
                batchedMeshes.SetEffectParameter("SunlightEnabled", false);
            }
                        
            batchedMeshes.Draw();

            if (debugDrawer != null) debugDrawer.Draw(currentCamera.View,currentCamera.Projection);
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

    }
    /********************************************************************************/

}
/************************************************************************************/