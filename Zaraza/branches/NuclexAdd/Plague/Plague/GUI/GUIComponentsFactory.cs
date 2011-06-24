using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using PlagueEngine.GUI.Components;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;
using Nuclex.UserInterface.Controls.Desktop;


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

        public ListComponent CreateListComponent(List<string> Items, int x, int y, int width, int height, ListSelectionMode mode)
        {
            ListComponent result = new ListComponent(Items, x, y, width, height, mode);
            return result;
        }

        /****************************************************************************/
        /// Create Button Component
        /****************************************************************************/
        public ButtonComponent CreateButtonComponent(String text,
                                                     String tag,
                                                     int x, int y,
                                                     int width, int height)
        {

            ButtonComponent result = new ButtonComponent(text,
                                                         x,
                                                         y,
                                                         width,
                                                         height,
                                                         tag);
            return result;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Create Label Component
        /****************************************************************************/
        public LabelComponent CreateLabelComponent(String text,
                                                   int x, int y)
        {
            LabelComponent result = new LabelComponent(text, x,y,1,1);
            return result;
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
                component.Register();
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
                component.Register();
                return component;
            }
#if DEBUG
            Diagnostics.PushLog("Creating Panel component failed due to initialization failure");
#endif
            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// CreateWindowComponent
        /****************************************************************************/
        public WindowComponent CreateWindowComponent(String title,
                                                     int x, int y,
                                                     int width, int height,
                                                     bool enableDragging)
        {
            WindowComponent result = new WindowComponent(title,
                                                         x,
                                                         y,
                                                         width,
                                                         height,
                                                         enableDragging);            

            return result;
        }                                                     
        /****************************************************************************/
    }
}
