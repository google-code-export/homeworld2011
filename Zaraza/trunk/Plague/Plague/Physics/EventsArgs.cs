using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow;

/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Physics
{
    /********************************************************************************/
    /// Events
    /********************************************************************************/


    /****************************************************************************/
    /// Collision Event
    /****************************************************************************/
    class CollisionEvent : EventArgs
    {
        GameObjectInstance gameObject=null;

        public CollisionEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            return "Collision with gameObject ID :" + gameObject.ID;
        }
    };
    /****************************************************************************/


    /********************************************************************************/
}
