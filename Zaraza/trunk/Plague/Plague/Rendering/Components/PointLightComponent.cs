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
    /// PointLightComponent
    /********************************************************************************/
    class PointLightComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public static Renderer renderer = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public PointLightComponent(GameObjectInstance gameObject,
                                   bool enabled,
                                   Vector3 color,
                                   float radius,
                                   float linearAttenuation,
                                   float quadraticAttenuation,
                                   Vector3 localPosition)
            : base(gameObject)
        {
            Enabled              = enabled;
            Color                = color;
            Radius               = radius;
            LinearAttenuation    = linearAttenuation;
            QuadraticAttenuation = quadraticAttenuation;
            LocalPosition        = localPosition;

            renderer.pointLights.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.pointLights.Remove(this);            
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool    Enabled              { get; set; }
        public Vector3 Color                { get; set; }
        public float   Radius               { get; set; }
        public float   LinearAttenuation    { get; set; }
        public float   QuadraticAttenuation { get; set; }

        public Vector3        LocalPosition  { get; private set; }
        public Vector3        Position       { get { return Vector3.Transform(LocalPosition,gameObject.World); } }
        public Matrix         World          { get { return Matrix.CreateScale(Radius) * Matrix.CreateTranslation(Position); } }
        public BoundingSphere BoundingSphere { get { return new BoundingSphere(Position, Radius); } }
        /****************************************************************************/

    }
    /********************************************************************************/
}
/************************************************************************************/