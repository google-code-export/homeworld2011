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
        public GameObjectInstance gameObject=null;

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



    /****************************************************************************/
    /// Lost Collision Event
    /****************************************************************************/
    class LostCollisionEvent : EventArgs
    {
        public GameObjectInstance gameObject = null;

        public LostCollisionEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            return "Lost Collision with gameObject ID :" + gameObject.ID;
        }
    };
    /****************************************************************************/


    /****************************************************************************/
    /// Start Collision Event
    /****************************************************************************/
    class StartCollisionEvent : EventArgs
    {
        public GameObjectInstance gameObject = null;

        public StartCollisionEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            return "Start Collision with gameObject ID :" + gameObject.ID;
        }
    };
    /****************************************************************************/



    /****************************************************************************/
    /// Any Collision Event
    /****************************************************************************/
    class AnyCollisionEvent : EventArgs
    {

        public AnyCollisionEvent()
        {  }

        public override string ToString()
        { return "Any Collision detected";}
    };
    /****************************************************************************/

    /****************************************************************************/
    /// ConeTest Event
    /****************************************************************************/
    class ConeTestEvent : EventArgs
    {
        public List<GameObjectInstance> gameobjects = null;

        public ConeTestEvent(List<GameObjectInstance> gameobjects)
        {
            this.gameobjects = gameobjects;
        }

        public override string ToString()
        {
            String tmp = String.Empty;
            foreach (GameObjectInstance go in gameobjects)
            {
                tmp = tmp + " " + go.ID.ToString();
            }
            return "Cone test detectected collision with :" + tmp;
        }
    };
    /****************************************************************************/

    /********************************************************************************/
}
