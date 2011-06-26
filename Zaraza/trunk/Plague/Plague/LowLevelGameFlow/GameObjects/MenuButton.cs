using System;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using PlagueEngine.GUI.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class MenuButton : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        ButtonComponent _buttonComponent;
        /********************************************************************************/




        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(ButtonComponent buttonComponent)
        {
            _buttonComponent = buttonComponent;
            EventHandler handler = Handle;
            buttonComponent.SetDelegate(handler);
        }
        /********************************************************************************/

        /****************************************************************************/
        /// Handle - sending event here
        /****************************************************************************/
        public void Handle(object oSender, EventArgs oEventArgs)
        {
#if DEBUG
            Diagnostics.PushLog(LoggingLevel.INFO, this, "Button " + _buttonComponent.Tag + " was pressed.");
#endif
        }
        /****************************************************************************/
        


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            _buttonComponent.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new MenuButtonData();
            GetData(data);
            //TODO: uzupełnić GO buttona
            data.Text = _buttonComponent.Control.Text;
            return data;
        }
        /********************************************************************************/



    }
    /********************************************************************************/


    /********************************************************************************/
    /// CylindricalBodyMeshData
    /********************************************************************************/
    [Serializable]
    public class MenuButtonData : GameObjectInstanceData
    {
        [CategoryAttribute("Text"),
        DescriptionAttribute("Button's text / label")]
        public String Text { get; set; }

        [CategoryAttribute("Tag"),
        DescriptionAttribute("Button's event tag")]
        public String Tag { get; set; }

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

