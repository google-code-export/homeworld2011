﻿using System;
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
        private Vector3  specularColor = Vector3.Zero;
        private bool     enabled       = true;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SunlightComponent(GameObjectInstance gameObject,
                                 Renderer           renderer,
                                 Vector3            diffuseColor,
                                 Vector3            specularColor,
                                 bool               enabled)
            : base(gameObject)
        {
            this.gameObject     = gameObject;
            this.renderer       = renderer;
            this.diffuseColor   = diffuseColor;
            this.specularColor  = specularColor;
            this.enabled        = enabled;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            if (renderer.Sunlight == this) renderer.Sunlight = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool    Enabled       { get { return enabled;                  } }
        public Vector3 DiffuseColor  { get { return diffuseColor;             } }
        public Vector3 SpecularColor { get { return specularColor;            } }
        
        public Vector3 Direction     
        { 
            get 
            {
                Vector3 result = gameObject.World.Forward;
       //         result.Normalize();
                return result;
            } 
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/