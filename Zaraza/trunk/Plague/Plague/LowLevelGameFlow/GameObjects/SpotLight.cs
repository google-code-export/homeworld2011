using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.Rendering.Components;

/************************************************************************************/
///  PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// SpotLight
    /********************************************************************************/
    class SpotLight : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        SpotLightComponent light = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(SpotLightComponent lightComponent, Matrix world)
        {
            this.light = lightComponent;
            this.World = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            light.ReleaseMe();
            light = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            SpotLightData data = new SpotLightData();
            GetData(data);

            data.Enabled              = light.Enabled;
            data.Color                = light.Color;
            data.Radius               = light.Radius;
            data.LinearAttenuation    = light.LinearAttenuation;
            data.QuadraticAttenuation = light.QuadraticAttenuation;
            
            data.NearPlane      = light.NearPlane;
            data.FarPlane       = light.FarPlane;
            data.LocalTransform = light.LocalTransform;
            data.Texture        = light.AttenuationTexture;
            data.Intensity      = light.Intensity;
            data.ShadowsEnabled = light.ShadowsEnabled;
            data.Specular       = light.Specular;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// SpotLightData
    /********************************************************************************/
    [Serializable]
    public class SpotLightData : GameObjectInstanceData
    {
        [CategoryAttribute("Misc")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Color")]
        public Vector3 Color { get; set; }
        [CategoryAttribute("Color")]
        public float Intensity { get; set; }
        [CategoryAttribute("Color")]
        public bool Specular { get; set; }

        [CategoryAttribute("Size")]
        public float Radius { get; set; }
        [CategoryAttribute("Size")]
        public float NearPlane { get; set; }
        [CategoryAttribute("Size")]
        public float FarPlane { get; set; }

        [CategoryAttribute("Transform")]
        public Matrix LocalTransform { get; set; }

        [CategoryAttribute("Attenuation")]
        public float LinearAttenuation  { get; set; }
        [CategoryAttribute("Attenuation")]
        public float QuadraticAttenuation { get; set; }
        [CategoryAttribute("Attenuation")]
        public String Texture               { get; set; }

        [CategoryAttribute("Shadows")]
        public bool ShadowsEnabled  { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/