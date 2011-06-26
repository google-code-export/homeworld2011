using System;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using PlagueEngine.GUI.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Input : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        InputComponent _inputComponent;
        /********************************************************************************/

        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(InputComponent inputComponent)
        {
            _inputComponent = inputComponent;
        }
        /********************************************************************************/

        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            _inputComponent.ReleaseMe();
        }
        /********************************************************************************/

        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new InputData();
            GetData(data);
            //TODO: uzupełnić GO inputu
            data.Text = _inputComponent.Input.Text;
            return data;
        }
        /********************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class InputData : GameObjectInstanceData
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

