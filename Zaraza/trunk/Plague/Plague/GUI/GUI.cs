﻿using System;
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
        public GuiManager Manager = null;
        public GUIComponentsFactory ComponentsFactory = null;
        public WindowControl window = null;
        public InputManager input = null;
        /****************************************************************************/
        
        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public GUI(Game game, GameServiceContainer Services)
        {
            //this.game         = game;
            ComponentsFactory = new GUIComponentsFactory(this);
            Manager = new GuiManager(Services);
            input = new InputManager(Services);
            game.Components.Add(input);
            //GUIComponent.gui = this;
        }
        /****************************************************************************/

        public void onDisposing(object o, EventArgs ea)
        {
            GraphicsDevice gd = (GraphicsDevice)o;
            Viewport viewport = gd.Viewport;
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);
            this.Manager.Screen = mainScreen;
        }

        public void Initialize(GraphicsDevice GraphicsDevice)
        {
            GUIComponent.gui = this;
            //GraphicsDevice.DeviceReset += onDisposing;
            this.Manager.DrawOrder = 1000;
            Viewport viewport = GraphicsDevice.Viewport;
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);
            this.Manager.Screen = mainScreen;

            // Each screen has a 'desktop' control. This invisible control by default
            // stretches across the whole screen and serves as the root of the control
            // tree in which all visible controls are managed. All controls are positioned
            // using a system of fractional coordinates and pixel offset coordinates.
            // We now adjust the position of the desktop window to prevent GUI or HUD
            // elements from appearing outside of the title-safe area.
            mainScreen.Desktop.Bounds = new UniRectangle(
              new UniScalar(0.1f, 0.0f), new UniScalar(0.1f, 0.0f), // x and y = 10%
              new UniScalar(0.8f, 0.0f), new UniScalar(0.8f, 0.0f) // width and height = 80%
            );
            this.Manager.Initialize();


            //TODO: Usunąć ten hard-kodowany guzik po zrobieniu GUI do końca.
            
            // Button through which the user can quit the application
            ButtonControl quitButton = new ButtonControl();
            quitButton.Text = "Quit";
            quitButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -80.0f), new UniScalar(1.0f, -32.0f), 80, 32
            );
            quitButton.Pressed += delegate(object sender, EventArgs arguments) { Diagnostics.PushLog("button klikniety"); };
            mainScreen.Desktop.Children.Add(quitButton);
        
        }

      
        internal void Draw(GameTime gameTime)
        {
            Manager.Draw(gameTime);
        }

    }
}
