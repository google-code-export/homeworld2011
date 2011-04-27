using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using PlagueEngine.GUI.Components;
using Nuclex.UserInterface;


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
            //GUIComponent.gui = gui;
            //this.renderer   = renderer;
            //this.content    = renderer.contentManager;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ButtonComponent createButtonComponent(String text,
                                                     Vector2 vertical,
                                                     Vector2 horizontal,
                                                     Vector2 other,
                                                     EventHandler eventHandler)
        {
            UniScalar ver =  new UniScalar(  vertical.X,   vertical.Y);
            UniScalar hor =  new UniScalar(horizontal.X, horizontal.Y);
            
            UniRectangle rectangle = new UniRectangle(ver, hor, other.X, other.Y);

            ButtonComponent component = new ButtonComponent();
            if (component.Initialize(text == null ? String.Empty : text, rectangle))
            {
                component.setDelegate(eventHandler);
                component.register();
                return component;
            }
            Diagnostics.PushLog("Creating button component failed due to initialization failure");
            return null;
        }
        /****************************************************************************/
    }
}
