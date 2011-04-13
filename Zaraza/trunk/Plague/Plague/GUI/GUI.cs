using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using PlagueEngine.GUI.Components;

namespace PlagueEngine.GUI
{
    /********************************************************************************/
    /// GUI
    /********************************************************************************/
    class GUI
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private GuiManager manager = null;
        private GUIComponentsFactory componentsFactory = null;
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(Game game)
        {
            //this.game         = game;
            componentsFactory = new GUIComponentsFactory(this);
            
            //GUIComponent.gui = this;
        }
        /****************************************************************************/           
    }
}
