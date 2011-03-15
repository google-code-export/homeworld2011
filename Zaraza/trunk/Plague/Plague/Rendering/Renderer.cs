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
        private  GraphicsDeviceManager       graphics            = null;
        private  GraphicsDevice              device              = null;
        internal ContentManager              contentManager      = null;
        private  RenderingComponentsFactory  componentsFactory   = null;

        internal List<BasicMeshComponent>    basicMeshComponents = new List<BasicMeshComponent>();
        private  CameraComponent             currentCamera       = null;
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

            foreach (DisplayMode displayMode in device.Adapter.SupportedDisplayModes)
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
                return new RenderConfig(device.DisplayMode.Width,
                                        device.DisplayMode.Height,
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
                device = graphics.GraphicsDevice;
                if (currentCamera != null) currentCamera.Aspect = device.Viewport.AspectRatio;

                Diagnostics.PushLog("Presentation Parameters Changed" +
                                    ". Resolution: "    + device.PresentationParameters.BackBufferWidth.ToString()    +
                                    " x "               + device.PresentationParameters.BackBufferHeight.ToString()   + 
                                    " x "               + device.PresentationParameters.BackBufferFormat.ToString()   + 
                                    " x "               + device.PresentationParameters.DepthStencilFormat.ToString() +
                                    ". Multisampling: " + device.PresentationParameters.MultiSampleCount.ToString()   +
                                    ". Fullscreen: "    + device.PresentationParameters.IsFullScreen.ToString()       +
                                    ". VSync: "         + graphics.SynchronizeWithVerticalRetrace.ToString());
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw()
        {
            if (device.IsDisposed) device = graphics.GraphicsDevice;
            device.Clear(Color.CornflowerBlue);

            if (currentCamera == null) return;

            foreach (BasicMeshComponent basicMeshComponent in basicMeshComponents)
            { 
                basicMeshComponent.Model.Draw(basicMeshComponent.GameObject.World,
                                              currentCamera.View,
                                              currentCamera.Projection);
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


    }
    /********************************************************************************/
    
}
/************************************************************************************/