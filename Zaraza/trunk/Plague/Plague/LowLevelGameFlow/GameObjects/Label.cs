using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Rendering;
using PlagueEngine.GUI;
using PlagueEngine.GUI.Components;
using Microsoft.Xna.Framework.Input;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Label : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        LabelComponent labelComponent = null;
        /********************************************************************************/

        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(LabelComponent labelComponent)
        {
            this.labelComponent = labelComponent;
        }
        /********************************************************************************/

        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            this.labelComponent.ReleaseMe();
        }
        /********************************************************************************/

        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            InputData data = new InputData();
            GetData(data);
            //TODO: uzupełnić GO labelki
            data.Text = this.labelComponent.Control.Text;
            return data;
        }
        /********************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class LabelData : GameObjectInstanceData
    {
        [CategoryAttribute("Text"),
        DescriptionAttribute("Button's text / label")]
        public String Text { get; set; }

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Vertical position of the button. First value describes % of the screen height, and second offset in pixels")]
        public Vector2 Vertical { get; set; }

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Horizontal position of the button. First value describes % of the screen width, and second offset in pixels")]
        public Vector2 Horizontal { get; set; }

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Actual size of the button (in pixels)")]
        public Vector2 Size { get; set; }
    }
    /********************************************************************************/
}
