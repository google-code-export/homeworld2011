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
        SphericalBodyComponent body  = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(PointLightComponent lightComponent, SphericalBodyComponent physcisComponent, Matrix world)
        {
            this.light = lightComponent;
            this.body  = physcisComponent;
            this.World = world;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            light.ReleaseMe();
            body.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            PointLightData data = new PointLightData();
            GetData(data);

            data.Mass = body.Mass;
            data.Elasticity = body.Elasticity;
            data.StaticRoughness = body.StaticRoughness;
            data.DynamicRoughness = body.DynamicRoughness;
            data.Radius = body.Radius;
            data.Immovable = body.Immovable;
            data.Translation = body.SkinTranslation;
            data.SkinPitch = body.Pitch;
            data.SkinRoll = body.Roll;
            data.SkinYaw = body.Yaw;

            data.Enabled = light.Enabled;
            data.Color = light.Color;
            data.LightRadius = light.Radius;
            data.LinearAttenuation = light.LinearAttenuation;
            data.QuadraticAttenuation = light.QuadraticAttenuation;
            data.Intensity = light.Intensity;

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
        [CategoryAttribute("Physics")]
        public float Mass { get; set; }

        [CategoryAttribute("Physics")]
        public float Elasticity { get; set; }

        [CategoryAttribute("Physics")]
        public float StaticRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public float DynamicRoughness { get; set; }

        [CategoryAttribute("Physics")]
        public bool Immovable { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float Radius { get; set; }

        [CategoryAttribute("Collision Skin")]
        public Vector3 Translation { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinYaw { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinPitch { get; set; }

        [CategoryAttribute("Collision Skin")]
        public float SkinRoll { get; set; }

        [CategoryAttribute("Light")]
        public bool Enabled { get; set; }

        [CategoryAttribute("Light")]
        public Vector3 Color { get; set; }

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