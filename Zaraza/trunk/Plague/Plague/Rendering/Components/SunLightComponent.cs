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
        private bool     enabled       = true;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public SunlightComponent(GameObjectInstance gameObject,
                                 Renderer           renderer,
                                 Vector3            diffuseColor,
                                 float              intensity,
                                 bool               enabled,
                                 float              depthBias,
                                 float              shadowIntensity,
                                 Vector3 fogColor,
                                 Vector2 fogRange,
                                 bool fog,
                                 Vector3 ambient)
            : base(gameObject)
        {
            this.gameObject     = gameObject;
            this.renderer       = renderer;
            this.diffuseColor   = diffuseColor;            
            this.enabled        = enabled;
            Intensity           = intensity;
            DepthBias           = depthBias;
            ShadowIntensity     = shadowIntensity;

            FogColor = fogColor;
            FogRange = fogRange;
            Fog = fog;
            Ambient = ambient;

            renderer.lightsManager.sunlight = this;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.lightsManager.sunlight = null;
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool    Enabled       { get { return enabled;       } set { enabled       = value; } }
        public Vector3 DiffuseColor  { get { return diffuseColor;  } set { diffuseColor  = value; } }
        public float   Intensity     { get; set; }
        public float   DepthBias     { get; set; }
        public float   ShadowIntensity { get; set; }

        public bool    Fog      { get; private set; }
        public Vector3 FogColor { get; private set; }
        public Vector2 FogRange { get; private set; }
        public Vector3 Ambient  { get; private set; }        


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