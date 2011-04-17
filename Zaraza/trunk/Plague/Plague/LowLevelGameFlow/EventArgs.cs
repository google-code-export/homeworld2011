using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Empty Events
    /********************************************************************************/
    class CreateEvent  : EventArgs { };
    class DestroyEvent : EventArgs { };
    /********************************************************************************/




    /********************************************************************************/
    /// GameObjectClicked && GameObjectReleased
    /********************************************************************************/
    class GameObjectClicked : EventArgs
    {
            public uint gameObjectID;

            public GameObjectClicked(uint gameObjectID)
            {
                this.gameObjectID = gameObjectID;
            }

            public override string ToString()
            {
                return "gameObjectID :" + gameObjectID.ToString();
            }
    }


    class GameObjectReleased : EventArgs { }
    /********************************************************************************/


}
