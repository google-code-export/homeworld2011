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
    /// WaterSurface
    /********************************************************************************/
    class WaterSurface : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private WaterSurfaceComponent waterSurfaceComponent = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Initialization
        /****************************************************************************/
        public void Init(WaterSurfaceComponent waterSurfaceComponent)
        {
            this.waterSurfaceComponent = waterSurfaceComponent;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ReleaseComponents
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            waterSurfaceComponent.ReleaseMe();
            waterSurfaceComponent = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            WaterSurfaceData data = new WaterSurfaceData();
            GetData(data);

            data.Length = waterSurfaceComponent.Length;
            data.Width = waterSurfaceComponent.Width;
            data.Color = waterSurfaceComponent.WaterColor;
            data.ColorAmount = waterSurfaceComponent.ColorAmount;
            data.WaveLength = waterSurfaceComponent.WaveLength;
            data.WaveHeight = waterSurfaceComponent.WaveHeight;
            data.WaveSpeed = waterSurfaceComponent.WaveSpeed;
            data.NormalMap = waterSurfaceComponent.NormalMap;
            data.Bias = waterSurfaceComponent.Bias;
            data.WTextureTiling = waterSurfaceComponent.TextureTiling;
            data.ClipPlaneAdjustment = waterSurfaceComponent.ClipPlaneAdjustment;
            data.SpecularStength = waterSurfaceComponent.SpecularStrength;
            
            return data;
        }
        /****************************************************************************/
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Water Surface Data
    /********************************************************************************/
    [Serializable]
    public class WaterSurfaceData : GameObjectInstanceData
    {
        [CategoryAttribute("Size")]
        public float Length { get; set; }
        [CategoryAttribute("Size")]
        public float Width { get; set; }        
        [CategoryAttribute("Color")]
        public Vector3 Color { get; set; }
        [CategoryAttribute("Color")]
        public float ColorAmount { get; set; }
        [CategoryAttribute("Wave")]
        public float WaveLength { get; set; }
        [CategoryAttribute("Wave")]
        public float WaveHeight { get; set; }
        [CategoryAttribute("Wave")]
        public float WaveSpeed { get; set; }
        [CategoryAttribute("Wave")]
        public String NormalMap { get; set; }
        [CategoryAttribute("Wave")]
        public float WTextureTiling { get; set; }
        [CategoryAttribute("Color")]
        public float Bias { get; set; }
        [CategoryAttribute("Misc")]
        public float ClipPlaneAdjustment { get; set; }
        [CategoryAttribute("Misc")]
        public float SpecularStength { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/
