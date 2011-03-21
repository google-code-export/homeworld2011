using System;
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
    class SunLight : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private SunLightComponent sunLightComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(SunLightComponent sunLightComponent, Matrix world)
        {
            this.sunLightComponent = sunLightComponent;
            this.World             = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            SunLightData data = new SunLightData();
            GetData(data);

            data.Ambient  = sunLightComponent.AmbientColor;
            data.Diffuse  = sunLightComponent.DiffuseColor;
            data.Specular = sunLightComponent.SpecularColor;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            sunLightComponent.ReleaseMe();
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Sun Light Data
    /********************************************************************************/
    [Serializable]
    public class SunLightData : GameObjectInstanceData
    {

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        [CategoryAttribute("Light Color")]
        public Vector3 Ambient  { get; set; }
        [CategoryAttribute("Light Color")]
        public Vector3 Diffuse  { get; set; }
        [CategoryAttribute("Light Color")]
        public Vector3 Specular { get; set; }
        /****************************************************************************/

    }
    /********************************************************************************/


}
/************************************************************************************/