using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Sun Light Component
    /********************************************************************************/
    class SunlightComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Renderer renderer      = null;
        private Vector3  diffuseColor  = Vector3.Zero;
        private bool     enabled       = true;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SunlightComponent(GameObjectInstance gameObject,
                                 Renderer           renderer,
                                 Vector3            diffuseColor,
                                 float              intensity,
                                 bool               enabled)
            : base(gameObject)
        {
            this.gameObject     = gameObject;
            this.renderer       = renderer;
            this.diffuseColor   = diffuseColor;            
            this.enabled        = enabled;
            Intensity           = intensity;            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool    Enabled       { get { return enabled;       } set { enabled       = value; } }
        public Vector3 DiffuseColor  { get { return diffuseColor;  } set { diffuseColor  = value; } }
        public float   Intensity     { get; set; }

        public Vector3 Direction     
        { 
            get 
            {
                Vector3 result = gameObject.World.Forward;
                return result;
            }

            set
            {
                gameObject.World.Forward = value;
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/