﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.Input.Components
/************************************************************************************/
namespace PlagueEngine.Input.Components
{

    /********************************************************************************/
    /// Extended Mouse Key State
    /********************************************************************************/
    struct ExtendedMouseKeyState
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private readonly bool isDown;
        private readonly bool changed;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public ExtendedMouseKeyState(bool isDown, bool changed)
        {
            this.isDown  = isDown;
            this.changed = changed;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Down
        /****************************************************************************/
        public bool IsDown()
        {
            return isDown;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Is Up
        /****************************************************************************/
        public bool IsUp()
        {
            return !isDown;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Was Pressed
        /****************************************************************************/
        public bool WasPressed()
        {
            return (isDown && changed ? true : false);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Was Released
        /****************************************************************************/
        public bool WasReleased()
        {
            return (!isDown && changed ? true : false);
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/