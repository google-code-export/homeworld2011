﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Arcade;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;

/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{
    /********************************************************************************/
    /// GUI Panel Component
    /********************************************************************************/
    class PanelComponent : GUIComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public PanelControl panel { get; private set; }
        /****************************************************************************/

        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public PanelComponent() : base()
        {
            panel = new PanelControl();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// initialize
        /****************************************************************************/
        public bool Initialize(UniRectangle bounds)
        {
            if (panel != null && gui != null)
            {
                panel.Bounds = bounds;
#if DEBUG
                Diagnostics.PushLog("Label component initialized successfully");
#endif
                return true;
            }
#if DEBUG
            Diagnostics.PushLog("Label component wasn't initialized");
#endif
            return false;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void Register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.panel);
        }
        /****************************************************************************/



        /****************************************************************************/
        /// add
        /****************************************************************************/
        public void add(Control control)
        {
            this.panel.Children.Add(control);
        }
        /****************************************************************************/

        /****************************************************************************/
        /// dispose
        /****************************************************************************/
        public void dispose()
        {
            this.panel.Children.Clear();
        }
        /****************************************************************************/

        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            gui.Manager.Screen.Desktop.Children.Remove(this.panel);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
    
