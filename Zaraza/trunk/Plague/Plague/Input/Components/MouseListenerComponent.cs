﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Input;


/************************************************************************************/
/// PlagueEngine.Input.Components
/************************************************************************************/
namespace PlagueEngine.Input.Components
{

    /********************************************************************************/
    /// Mouse Key Action
    /********************************************************************************/
    enum MouseKeyAction
    {
        LeftClick,RightClick,MiddleClick
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Mouse Move Action
    /********************************************************************************/
    enum MouseMoveAction
    {
        Move,Scroll
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Mouse Listener Component
    /********************************************************************************/
    class MouseListenerComponent : InputComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private bool  active = true;
        /****************************************************************************/


        /****************************************************************************/
        /// Mouse Listener Component
        /****************************************************************************/
        public MouseListenerComponent(GameObjectInstance gameObject,bool active):base(gameObject)
        {
            this.active=active;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Active
        /****************************************************************************/
        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Keys
        /****************************************************************************/
        public void SubscribeKeys(OnMouseKey onMouseKey,params MouseKeyAction[] mouseKeyAction)
        {
            foreach (MouseKeyAction mouseKey in mouseKeyAction)
            {
                input.SubscribeMouseKeyListener(this, mouseKey, onMouseKey);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Subscribe Mouse Move
        /****************************************************************************/
        public void SubscribeMouseMove(OnMouseMove onMouseMove, params MouseMoveAction[] mouseMoveAction)
        {
            foreach(MouseMoveAction mouseAction in mouseMoveAction)
            {
                input.SubscribeMouseMoveListener(this,mouseAction,onMouseMove);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Lock Cursor
        /****************************************************************************/
        public void LockCursor()
        {
            input.LockCursor();    
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Unlock Cursor
        /****************************************************************************/
        public void UnlockCursor()
        {
            input.UnlockCursor();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            input.ReleaseMouseListenerComponent(this);
        }
        /****************************************************************************/

    }
    /********************************************************************************/
}
/************************************************************************************/
