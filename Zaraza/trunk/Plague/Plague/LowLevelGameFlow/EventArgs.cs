using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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


    /********************************************************************************/
    /// SelectedObjectEvent
    /********************************************************************************/
    class SelectedObjectEvent : EventArgs
    {
        public GameObjectInstance gameObject;
        public Vector3            position;

        public SelectedObjectEvent(GameObjectInstance gameObject, Vector3 position)
        {
            this.gameObject = gameObject;
            this.position   = position;
        }
    }
    /********************************************************************************/
}
