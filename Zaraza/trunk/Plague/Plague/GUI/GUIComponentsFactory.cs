using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.GUI.Components;

namespace PlagueEngine.GUI
{
    class GUIComponentsFactory
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private GUI        gui    = null;
        //private ContentManager  content     = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUIComponentsFactory(GUI gui)
        {
            //this.gui = gui;
            GUIComponent.gui = gui;
            //this.renderer   = renderer;
            //this.content    = renderer.contentManager;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ButtonComponent createButtonComponent(String text)
        {
            ButtonComponent component = new ButtonComponent();
            component.Initialize( text == null ? String.Empty : text );
            return component;
        }
        /****************************************************************************/
    }
}
