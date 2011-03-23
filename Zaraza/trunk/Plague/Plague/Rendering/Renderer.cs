using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private  GraphicsDeviceManager       graphics             = null;
        internal ContentManager              contentManager       = null;
        private  RenderingComponentsFactory  componentsFactory    = null;

        internal List<RenderableComponent>   renderableComponents = new List<RenderableComponent>();
        internal List<RenderableComponent>   preRender            = new List<RenderableComponent>();

        private  CameraComponent             currentCamera        = null;
        private  SunLightComponent           sunLight             = null;
        private  Color                       clearColor           = Color.CornflowerBlue;
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
            graphics             = new GraphicsDeviceManager(game);
            contentManager       = game.ContentManager;           
            CurrentConfiguration = config;
            componentsFactory    = new RenderingComponentsFactory(this);
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
                if (renderableComponent.Effect != null)
                {
                    renderableComponent.Effect.Parameters["SunLightDirection"]  .SetValue(sunLight.Direction);
                    renderableComponent.Effect.Parameters["SunLightAmbient"]    .SetValue(sunLight.AmbientColor);
                    renderableComponent.Effect.Parameters["SunLightDiffuse"]    .SetValue(sunLight.DiffuseColor);
                    renderableComponent.Effect.Parameters["SunLightSpecular"]   .SetValue(sunLight.SpecularColor);
                    renderableComponent.Effect.Parameters["CameraPosition"]     .SetValue(currentCamera.Position);
                    renderableComponent.Effect.Parameters["View"]               .SetValue(currentCamera.View);
                    renderableComponent.Effect.Parameters["Projection"]         .SetValue(currentCamera.Projection);
                    renderableComponent.Effect.Parameters["ViewProjection"]     .SetValue(currentCamera.ViewProjection);
                }

                renderableComponent.Draw();
            }
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
        public SunLightComponent SunLight
        {
            get
            {
                return sunLight;
            }
            
            set
            {
                sunLight = value;
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

    }
    /********************************************************************************/
    
}
/************************************************************************************/