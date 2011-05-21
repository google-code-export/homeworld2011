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
    class Panel : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        PanelComponent panelComponent = null;
        /********************************************************************************/

        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(PanelComponent panelComponent)
        {
            this.panelComponent = panelComponent;
        }
        /********************************************************************************/

        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            this.panelComponent.ReleaseMe();
        }
        /********************************************************************************/

        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            InputData data = new InputData();
            GetData(data);
            //TODO: uzupełnić GO panelu
            return data;
        }
        /********************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class PanelData : GameObjectInstanceData
    {
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
