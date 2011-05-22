﻿using System;
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
    class CreateEvent        : EventArgs { };
    class DestroyEvent       : EventArgs { };
    class SwitchEvent        : EventArgs { };
    class GameObjectReleased : EventArgs { };
    class ExamineEvent       : EventArgs { };
    /********************************************************************************/

    
    /********************************************************************************/
    /// GameObjectClicked
    /********************************************************************************/
    class GameObjectClicked : EventArgs
    {
            public int gameObjectID;
            public GameObjectClicked(int gameObjectID
            {
                this.gameObjectID = gameObjectID;
            }

            public override string ToString()
            {
                return "gameObjectID :" + gameObjectID.ToString();
            }
    }    
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

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString() + ", Position: " + position.ToString();
            else return "Null, Position: " + position.ToString(); 
        }
    }
    /********************************************************************************/

    
    /********************************************************************************/
    /// CommandOnObjectEvent
    /********************************************************************************/
    class CommandOnObjectEvent : EventArgs
    {
        public GameObjectInstance gameObject;
        public Vector3 position;

        public CommandOnObjectEvent(GameObjectInstance gameObject, Vector3 position)
        {
            this.gameObject = gameObject;
            this.position = position;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString() + ", Position: " + position.ToString();
            else return "Null, Position: " + position.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ExSwitchEvent
    /********************************************************************************/
    class ExSwitchEvent : EventArgs
    {
        public String name;
        public bool   value;

        public ExSwitchEvent(String name, bool value)
        {
            this.name  = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name + ": " + value.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// AddToSelectionEvent
    /********************************************************************************/
    class AddToSelectionEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public AddToSelectionEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// RemoveFromSelectionEvent
    /********************************************************************************/
    class RemoveFromSelectionEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public RemoveFromSelectionEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// MoveToPointCommandEvent
    /********************************************************************************/
    class MoveToPointCommandEvent : EventArgs
    {
        public Vector3 point;

        public MoveToPointCommandEvent(Vector3 point)
        {
            this.point = point;
        }

        public override string ToString()
        {
            return point.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// MoveToObjectCommandEvent
    /********************************************************************************/
    class MoveToObjectCommandEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public MoveToObjectCommandEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ChangeLevelEvent
    /********************************************************************************/
    class ChangeLevelEvent : EventArgs
    {
        public String Level;

        public ChangeLevelEvent(String level)
        {
            Level = level;
        }

        public override string ToString()
        {
            return Level;
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// CreateObjectEvent
    /********************************************************************************/
    class CreateObjectEvent : EventArgs
    {
        public GameObjectInstanceData Data;

        public CreateObjectEvent(GameObjectInstanceData data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ObjectCreatedEvent
    /********************************************************************************/
    class ObjectCreatedEvent : EventArgs
    {
        public GameObjectInstance GameObject;

        public ObjectCreatedEvent(GameObjectInstance gameObject)
        {
            GameObject = gameObject;
        }

        public override string ToString()
        {
            return GameObject.ID.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// DestroyObjectEvent
    /********************************************************************************/
    class DestroyObjectEvent : EventArgs
    {
        public int ID;

        public DestroyObjectEvent(int id)
        {
            ID = id;
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// SelectedActionEvent
    /********************************************************************************/
    class SelectedActionEvent : EventArgs
    {
        public String Action;

        public SelectedActionEvent(String action)
        {
            Action = action;
        }

        public override string ToString()
        {
            return Action;
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ChangeSpeedEvent
    /********************************************************************************/
    class ChangeSpeedEvent : EventArgs
    {
        public float Amount;

        public ChangeSpeedEvent(float amount)
        {
            Amount = amount;
        }

        public override string ToString()
        {
            return Amount.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// GrabObjectCommandEvent
    /********************************************************************************/
    class GrabObjectCommandEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public GrabObjectCommandEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ExamineObjectCommandEvent
    /********************************************************************************/
    class ExamineObjectCommandEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public ExamineObjectCommandEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// FollowObjectCommandEvent
    /********************************************************************************/
    class FollowObjectCommandEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public FollowObjectCommandEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            if (gameObject != null) return "ID: " + gameObject.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/

   
}
