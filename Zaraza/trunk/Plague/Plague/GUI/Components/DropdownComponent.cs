using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.GUI.Components
{
    class ListComponent : GUIComponent
    {
        public ListControl List { get; set; }

        
         
        public ListComponent(List<string> Items,  int x, int y, int width, int height, ListSelectionMode mode)
        {
            List = new ListControl();
            
            foreach(string item in Items)
            {
                List.Items.Add(GlobalGameObjects.StringManager.Load<string>(item) );
            }
            List.Bounds = new UniRectangle(new UniScalar(x),
                                              new UniScalar(y),
                                              new UniScalar(width),
                                              new UniScalar(height));
            List.SelectionMode = mode;
        }

        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void Register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.List);

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unregister
        /****************************************************************************/
        public void Unregister()
        {
            gui.Manager.Screen.Desktop.Children.Remove(this.List);

        }
        /****************************************************************************/


        /****************************************************************************/
        /// setDelegate
        /****************************************************************************/
        public void setDelegate(EventHandler handler)
        {
            List.SelectionChanged += handler;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            List.Parent.Children.Remove(List);
            base.ReleaseMe();
        }
        /****************************************************************************/
    
    
    }
}
