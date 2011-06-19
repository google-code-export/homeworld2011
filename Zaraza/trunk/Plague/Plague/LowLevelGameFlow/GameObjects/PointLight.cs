using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;


/************************************************************************************/
///  PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// PointLight
    /********************************************************************************/
    class PointLight : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        PointLightComponent    light = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(PointLightComponent lightComponent)
        {
            this.light = lightComponent;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            light.ReleaseMe();            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            PointLightData data = new PointLightData();
            GetData(data);

            data.Enabled = light.Enabled;
            data.Color = light.Color;
            data.LightRadius = light.Radius;
            data.LinearAttenuation = light.LinearAttenuation;
            data.QuadraticAttenuation = light.QuadraticAttenuation;
            data.Intensity = light.Intensity;
            data.Specular = light.Specular;
            
            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// PointLightData
    /********************************************************************************/
    [Serializable]
    public class PointLightData : GameObjectInstanceData
    {

        public PointLightData()
        {
            Type = typeof(PointLight);
        }

        [CategoryAttribute("Light")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Color { get; set; }

        [CategoryAttribute("Light")]
        public bool Specular { get; set; }

        [CategoryAttribute("Light")]
        public float Intensity { get; set; }

        [CategoryAttribute("Light")]
        public float LightRadius { get; set; }

        [CategoryAttribute("Light")]
        public float LinearAttenuation { get; set; }

        [CategoryAttribute("Light")]
        public float QuadraticAttenuation { get; set; }

    }
    /********************************************************************************/

}
/************************************************************************************/