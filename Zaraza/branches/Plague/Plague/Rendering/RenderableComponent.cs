﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
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
        internal static Renderer  renderer   = null;
        protected Effect          effect     = null;
        protected bool            preRender  = false;
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
                                   Effect effect) : base(gameObject)        
        {
            this.effect   = effect;
            
            renderer.renderableComponents.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public abstract void Draw();
        /****************************************************************************/


        /****************************************************************************/
        /// Draw Depth
        /****************************************************************************/
        public abstract void DrawDepth(ref Matrix ViewProjection, ref Vector3 LightPosition,float depthPrecision);      
        /****************************************************************************/


        /****************************************************************************/
        /// Set Clip Plane
        /****************************************************************************/
        public void SetClipPlane(Vector4 plane)
        {
            effect.Parameters["ClipPlaneEnabled"].SetValue(true);
            effect.Parameters["ClipPlane"].SetValue(plane);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable Clip Plane
        /****************************************************************************/
        public void DisableClipPlane()
        {
            effect.Parameters["ClipPlaneEnabled"].SetValue(false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Pre Render
        /****************************************************************************/
        public virtual void PreRender(CameraComponent camera)
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
        /// Effect
        /****************************************************************************/
        public Effect Effect
        {
            get
            {
                return effect;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// FrustrumInteresction
        /****************************************************************************/
        public abstract bool FrustrumInteresction(BoundingFrustum frustrum);
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.ReleaseRenderableComponent(this);
            if (preRender) renderer.ReleasePreRenderComponent(this);
            base.ReleaseMe();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/

