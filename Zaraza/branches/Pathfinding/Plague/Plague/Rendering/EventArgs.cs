using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{
    /********************************************************************************/
    /// Events
    /********************************************************************************/


        /****************************************************************************/
        /// Animation End Event
        /****************************************************************************/
        class AnimationEndEvent : EventArgs 
        {
            public String animation;

            public AnimationEndEvent(String animation)
            {
                this.animation = animation;
            }

            public override string ToString()
            {
                return "Animation :" + animation;
            }
        };
        /****************************************************************************/


    /********************************************************************************/
}
