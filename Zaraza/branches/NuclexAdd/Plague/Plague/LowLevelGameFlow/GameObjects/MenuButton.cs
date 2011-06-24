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
    class MenuButton : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        ButtonComponent buttonComponent = null;
        //TODO: wywalić zbedne komenty

        /********************************************************************************/




        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(ButtonComponent buttonComponent)
        {
            this.buttonComponent = buttonComponent;
            EventHandler handler = new EventHandler(handle);
            buttonComponent.setDelegate(handler);
        }
        /********************************************************************************/

        /****************************************************************************/
        /// Handle - sending event here
        /****************************************************************************/
        public void handle(object oSender, EventArgs oEventArgs)
        {
            Diagnostics.PushLog("BUTTON " + this.buttonComponent.Tag + " PRESSED");
        }
        /****************************************************************************/
        


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            this.buttonComponent.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MenuButtonData data = new MenuButtonData();
            GetData(data);
            //TODO: uzupełnić GO buttona
            data.Text = this.buttonComponent.Control.Text;
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

