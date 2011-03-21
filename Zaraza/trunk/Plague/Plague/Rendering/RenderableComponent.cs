using System;
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
        protected Renderer       renderer = null;
        protected Effect         effect   = null;
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
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.ReleaseRenderableComponent(this);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/