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
    class SunLightComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private Renderer renderer      = null;
        private Vector3  ambientColor  = Vector3.Zero;
        private Vector3  diffuseColor  = Vector3.Zero;
        private Vector3  specularColor = Vector3.Zero;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SunLightComponent(GameObjectInstance gameObject,
                                 Renderer           renderer,
                                 Vector3            ambientColor,
                                 Vector3            diffuseColor,
                                 Vector3            specularColor)
            : base(gameObject)
        {
            this.gameObject     = gameObject;
            this.renderer       = renderer;
            this.ambientColor   = ambientColor;
            this.diffuseColor   = diffuseColor;
            this.specularColor  = specularColor;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            if (renderer.SunLight == this) renderer.SunLight = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Vector3 AmbientColor  { get { return ambientColor;             } }
        public Vector3 DiffuseColor  { get { return diffuseColor;             } }
        public Vector3 SpecularColor { get { return specularColor;            } }
        
        public Vector3 Direction     
        { 
            get 
            {
                return gameObject.World.Forward;
            } 
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/