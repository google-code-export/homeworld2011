﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Input
/************************************************************************************/
namespace PlagueEngine.Input
{

    /********************************************************************************/
    /// Input Component
    /********************************************************************************/
    abstract class InputComponent : GameObjectComponent
    {
        
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        internal static Input input = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public InputComponent(GameObjectInstance gameObject)
            : base(gameObject)
        { 
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/