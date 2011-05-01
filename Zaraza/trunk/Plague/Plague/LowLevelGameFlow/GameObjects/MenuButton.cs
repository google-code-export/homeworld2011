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
        public void Init(ButtonComponent buttonComponent,Matrix world)
        {
            this.buttonComponent = buttonComponent;
            this.World = world;
        }
        /********************************************************************************/


        


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
            data.Text = this.buttonComponent.button.Text;
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

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Vertical bounds of the button (first pair)")]
        public Vector2 Vertical { get; set; }

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Horizontal bounds of the button (second pair)")]
        public Vector2 Horizontal { get; set; }

        [CategoryAttribute("Bounds"),
        DescriptionAttribute("Actual size of the button (third pair)")]
        public Vector2 Size { get; set; }

        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }

    }
    /********************************************************************************/
}

