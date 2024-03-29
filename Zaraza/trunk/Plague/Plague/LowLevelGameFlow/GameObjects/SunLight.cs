﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

using PlagueEngine.Rendering.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Sun Light
    /********************************************************************************/
    class Sunlight : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private SunlightComponent SunlightComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SunlightComponent SunlightComponent)
        {
            this.SunlightComponent = SunlightComponent;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Direction
        /****************************************************************************/
        public Vector3 Direction
        {
            get
            {
                Vector3 result = World.Forward;
                result.Normalize();
                return result;
            }

            set
            {
                if (!value.Equals(Vector3.Up))
                {
                    World.Forward = value;
                    World.Up      = Vector3.Up;
                    World.Right   = Vector3.Cross(value, Vector3.Up);                    
                }
                else
                {
                    World.Forward = value;
                    World.Right   = Vector3.Right;
                    World.Up      = Vector3.Cross(value, Vector3.Right);                    
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            SunlightData data = new SunlightData();
            GetData(data);

            data.Enabled   = SunlightComponent.Enabled;
            data.Diffuse   = SunlightComponent.DiffuseColor;
            data.Intensity = SunlightComponent.Intensity;
            data.DepthBias = SunlightComponent.DepthBias;
            data.ShadowIntensity = SunlightComponent.ShadowIntensity;

            data.FogColor = SunlightComponent.FogColor;
            data.FogRange = SunlightComponent.FogRange;
            data.Fog = SunlightComponent.Fog;
            data.Ambient = SunlightComponent.Ambient;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            SunlightComponent.ReleaseMe();
        }
        /****************************************************************************/
               
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Sun Light Data
    /********************************************************************************/
    [Serializable]
    public class SunlightData : GameObjectInstanceData
    {

        /****************************************************************************/
        /// Properties
        /****************************************************************************/        
        public bool Enabled   { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Diffuse   { get; set; }
        [CategoryAttribute("Light")]
        public float   Intensity { get; set; }

        [CategoryAttribute("Fog")]
        public bool Fog { get; set; }
        [CategoryAttribute("Fog")]
        public Vector3 FogColor { get; set; }
        [CategoryAttribute("Fog")]
        public Vector2 FogRange { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Ambient { get; set; }     

        [CategoryAttribute("Shadows")]
        public float DepthBias { get; set; }
        
        [CategoryAttribute("Shadows")]
        public float ShadowIntensity { get; set; }

        [CategoryAttribute("Direction")]
        public Vector3 Direction
        {
            get
            {
                return this.World.Forward;
            }

            set
            {
                if (!value.Equals(Vector3.Up))
                {
                    World.Forward = value;
                    World.Up = Vector3.Up;
                    World.Right = Vector3.Cross(value, Vector3.Up);
                }
                else
                {
                    World.Forward = value;
                    World.Right = Vector3.Right;
                    World.Up = Vector3.Cross(value, Vector3.Right);
                }
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


}
/************************************************************************************/