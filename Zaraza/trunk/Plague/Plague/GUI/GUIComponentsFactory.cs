using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using PlagueEngine.GUI.Components;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;


namespace PlagueEngine.GUI
{
    class GUIComponentsFactory
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private GUI        gui    = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUIComponentsFactory(GUI gui)
        {
            this.gui = gui;
        }
        /****************************************************************************/
        public void onClick(object oSender, EventArgs oEventArgs)
        {
#if DEBUG
            Diagnostics.PushLog("Button pressed");
#endif
        }

        /****************************************************************************/
        /// Create Button Component
        /****************************************************************************/
        public ButtonComponent createButtonComponent(String text,
                                                     String tag,
                                                     Vector2 vertical,
                                                     Vector2 horizontal,
                                                     Vector2 other,
                                                     GameObjectInstance go)
        {
            UniScalar ver =  new UniScalar(  vertical.X,   vertical.Y);
            UniScalar hor =  new UniScalar(horizontal.X, horizontal.Y);
            
            UniRectangle rectangle = new UniRectangle(ver, hor, other.X, other.Y);

            ButtonComponent component = new ButtonComponent();
            if (component.Initialize(text == null ? String.Empty : text, rectangle, tag))
            {
                component.register();
                return component;
            }
#if DEBUG
            Diagnostics.PushLog("Creating button component failed due to initialization failure");
#endif
            return null;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Create Label Component
        /****************************************************************************/
        public LabelComponent createLabelComponent(String text,
                                                     Vector2 vertical,
                                                     Vector2 horizontal,
                                                     Vector2 other)
        {
            UniScalar ver = new UniScalar(vertical.X, vertical.Y);
            UniScalar hor = new UniScalar(horizontal.X, horizontal.Y);

            UniRectangle rectangle = new UniRectangle(ver, hor, other.X, other.Y);

            LabelComponent component = new LabelComponent();
            if (component.Initialize(text == null ? String.Empty : text, rectangle))
            {
                component.register();
                return component;
            }
#if DEBUG
            Diagnostics.PushLog("Creating label component failed due to initialization failure");
#endif
            return null;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Create Input Component
        /****************************************************************************/
        public InputComponent createInputComponent(String text,
                                                     Vector2 vertical,
                                                     Vector2 horizontal,
                                                     Vector2 other)
        {
            UniScalar ver = new UniScalar(vertical.X, vertical.Y);
            UniScalar hor = new UniScalar(horizontal.X, horizontal.Y);

            UniRectangle rectangle = new UniRectangle(ver, hor, other.X, other.Y);

            InputComponent component = new InputComponent();
            if (component.Initialize(text == null ? String.Empty : text, rectangle))
            {
                component.register();
                return component;
            }
#if DEBUG
            Diagnostics.PushLog("Creating Input component failed due to initialization failure");
#endif
            return null;
        }
        /****************************************************************************/
        
        
        /****************************************************************************/
        /// Create Panel Component
        /****************************************************************************/
        public PanelComponent createPanelComponent(  Vector2 vertical,
                                                     Vector2 horizontal,
                                                     Vector2 other)
        {
            UniScalar ver = new UniScalar(vertical.X, vertical.Y);
            UniScalar hor = new UniScalar(horizontal.X, horizontal.Y);

            UniRectangle rectangle = new UniRectangle(ver, hor, other.X, other.Y);

            PanelComponent component = new PanelComponent();
            if (component.Initialize(rectangle))
            {
                component.register();
                return component;
            }
#if DEBUG
            Diagnostics.PushLog("Creating Panel component failed due to initialization failure");
#endif
            return null;
        }
        /****************************************************************************/
    }
}
