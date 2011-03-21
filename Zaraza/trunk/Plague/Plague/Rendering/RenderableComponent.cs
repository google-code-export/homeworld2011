﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Resources;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Renderable Component
    /********************************************************************************/
    abstract class RenderableComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected Renderer       renderer  = null;
        protected Effect         effect    = null;
        protected bool           preRender = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Device
        /****************************************************************************/
        protected GraphicsDevice device
        {
            get
            {
                return renderer.Device;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RenderableComponent(GameObjectInstance gameObject,
                                   Renderer renderer,
                                   Effect effect) : base(gameObject)        
        {
            this.renderer = renderer;
            this.effect   = effect;
            
            renderer.renderableComponents.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public abstract void Draw(Matrix view, Matrix projection);
        /****************************************************************************/


        /****************************************************************************/
        /// Set Clip Plane
        /****************************************************************************/
        public void SetClipPlane(Vector4 plane)
        {
            if (effect == null) return;

            effect.Parameters["ClipPlaneEnabled"].SetValue(true);
            effect.Parameters["ClipPlane"].SetValue(plane);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable Clip Plane
        /****************************************************************************/
        public void DisableClipPlane()
        {
            if (effect == null) return;

            effect.Parameters["ClipPlaneEnabled"].SetValue(false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pre Render
        /****************************************************************************/
        public virtual void PreRender(Matrix view, Matrix projection)
        {         
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Using Pre Render
        /****************************************************************************/
        public bool UsingPreRender
        {
            get
            {
                return preRender;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.ReleaseRenderableComponent(this);
            if (preRender) renderer.ReleasePreRenderComponent(this);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/