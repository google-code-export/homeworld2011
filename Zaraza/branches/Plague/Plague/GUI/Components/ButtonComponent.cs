﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

using PlagueEngine.Input.Components;


/************************************************************************************/
/// PlagueEngine.GUI.Components
/************************************************************************************/
namespace PlagueEngine.GUI.Components
{

    /********************************************************************************/
    /// GUI Button Component
    /********************************************************************************/
    class ButtonComponent : GUIComponent
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        //TODO: sprawdzić czemu tu public
        public ButtonControl Control { get; private set; }
        public String Tag            { get; private set; }
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ButtonComponent(String text, UniRectangle bounds, String tag)
            : base()
        {
            Control = new ButtonControl();
            Control.Text = text;
            Control.Bounds = bounds;

            Tag = tag;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// register
        /****************************************************************************/
        public override void Register()
        {
            gui.Manager.Screen.Desktop.Children.Add(this.Control);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// setDelegate
        /****************************************************************************/
        public void setDelegate(EventHandler handler)
        {
            Control.Pressed += handler;            
        }
        /****************************************************************************/
        


        /****************************************************************************/
        /// Release Me 
        /****************************************************************************/
        public override void ReleaseMe()
        {
            Control.Parent.Children.Remove(Control);
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
    /********************************************************************************/
}