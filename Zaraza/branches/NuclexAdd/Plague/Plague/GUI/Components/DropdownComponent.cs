using System;
using System.Collections.Generic;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.GUI.Components
{
    class ListComponent : GUIComponent
    {
        public ListControl List { get; set; }

        public ListComponent(IEnumerable<string> items,  int x, int y, int width, int height, ListSelectionMode mode)
        {
            List = new ListControl();
            
            foreach(var item in items)
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
        // register
        /****************************************************************************/
        public override void Register()
        {
            GUI.Manager.Screen.Desktop.Children.Add(List);

        }
        /****************************************************************************/


        /****************************************************************************/
        // Unregister
        /****************************************************************************/
        public void Unregister()
        {
            GUI.Manager.Screen.Desktop.Children.Remove(List);

        }
        /****************************************************************************/


        /****************************************************************************/
        // setDelegate
        /****************************************************************************/
        public void SetDelegate(EventHandler handler)
        {
            List.SelectionChanged += handler;
        }
        /****************************************************************************/

        /****************************************************************************/
        // Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            List.Parent.Children.Remove(List);
            base.ReleaseMe();
        }
        /****************************************************************************/
    
    
    }
}
