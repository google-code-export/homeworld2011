using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

/*
 * Ten kod, wygląda jak kupa
 */

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Bloom : GameObjectInstance
    {
        public float BloomIntensity;
        public float BaseIntensity;
        public float BloomSaturation;
        public float BaseSaturation;
        public float BloomThreshold;


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(float bloomIntensity,
            float baseIntensity,
         float bloomSaturation,
         float baseSaturation,
         float bloomThreshold)
        {
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
            BloomThreshold = bloomThreshold;

            SendEvent(new SetBloomEvent(bloomIntensity,BaseIntensity,BloomSaturation,BaseSaturation,BloomThreshold),EventsSystem.Priority.High,GlobalGameObjects.GameController);
        }

        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            BloomData data = new BloomData();
            GetData(data);

            data.BloomIntensity = BloomIntensity;
            data.BaseIntensity = BaseIntensity;
            data.BloomSaturation = BloomSaturation;
            data.BaseSaturation = BaseSaturation;
            data.BloomThreshold = BloomThreshold;

            return data;
        }
        /****************************************************************************/

    }


        


    /********************************************************************************/
    /// BloomData
    /********************************************************************************/
    [Serializable]
    public class BloomData : GameObjectInstanceData
    {
        public BloomData()
        {
            Type = typeof(Bloom);
        }

        [CategoryAttribute("Bloom")]
        public float BloomIntensity {get; set;}
        [CategoryAttribute("Bloom")]
        public float BaseIntensity { get; set; }
        [CategoryAttribute("Bloom")]
        public float BloomSaturation { get; set; }
        [CategoryAttribute("Bloom")]
        public float BaseSaturation { get; set; }
        [CategoryAttribute("Bloom")]
        public float BloomThreshold { get; set; }
    }
    /********************************************************************************/

}
