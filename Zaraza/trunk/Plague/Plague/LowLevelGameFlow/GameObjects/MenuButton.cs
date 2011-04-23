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
using PlagueEngine.Input.Components;
using PlagueEngine.Input;

using Microsoft.Xna.Framework.Input;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class MenuButton : GameObjectInstance
    {
        /********************************************************************************/
        /// Fields
        /********************************************************************************/
        ButtonComponent buttonComponent = null;
        
        KeyboardListenerComponent keyboardListener = null;

        bool forward = false;
        bool backward = false;
        /********************************************************************************/




        /********************************************************************************/
        /// Init
        /********************************************************************************/
        public void Init(ButtonComponent buttonComponent, Matrix world)
        {
            this.buttonComponent = buttonComponent;
            this.World = world;
            keyboardListener = new KeyboardListenerComponent(this, true);
            //keyboardListener.SubscibeKeys(OnKey, Keys.Y, Keys.H);
            
        }
        /********************************************************************************/


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
           //TODO: zdecydować co się zrobi z releaseComponents
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MovingBarrelData data = new MovingBarrelData();
            GetData(data);
            //TODO: uzupełnić GO buttona

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
        [CategoryAttribute("Instancing"),
        DescriptionAttribute("1 - No Instancing, 2 - Static Instancing, 3 - Dynamic Instancing.")]
        public uint InstancingMode { get; set; }
    }
    /********************************************************************************/
}

