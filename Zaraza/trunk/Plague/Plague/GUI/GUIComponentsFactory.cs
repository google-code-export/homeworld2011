using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.gui = gui;
            //this.renderer   = renderer;
            //this.content    = renderer.contentManager;
        }
        /****************************************************************************/

    }
}
