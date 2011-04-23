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
        private GuiManager Manager = null;
        public GUIComponentsFactory ComponentsFactory = null;
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(Game game, GameServiceContainer Services)
        {
            //this.game         = game;
            ComponentsFactory = new GUIComponentsFactory(this);
            Manager = new GuiManager(Services);
            
            //GUIComponent.gui = this;
        }
        /****************************************************************************/

        internal void Draw(GameTime gameTime)
        {
            Manager.Draw(gameTime);
        }
    }
}
