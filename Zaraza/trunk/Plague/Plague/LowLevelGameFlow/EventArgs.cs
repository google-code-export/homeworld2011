using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PlagueEngine.LowLevelGameFlow.GameObjects;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Empty Events
    /********************************************************************************/
    class CreateEvent          : EventArgs { };
    class DestroyEvent         : EventArgs { };
    class SwitchEvent          : EventArgs { };
    class GameObjectReleased   : EventArgs { };
    class ExamineEvent         : EventArgs { };    
    class ActionDoneEvent      : EventArgs { };
    class StopActionEvent      : EventArgs { };
    class CloseEvent           : EventArgs { };
    class ExitGameEvent             : EventArgs { };

    class DropItemCommandEvent        : EventArgs { };
    class ReloadCommandEvent          : EventArgs { };    
    class SwitchToWeaponCommandEvent  : EventArgs { };
    class SwitchToSideArmCommandEvent : EventArgs { };
    /********************************************************************************/

    
    /********************************************************************************/
    /// GameObjectClicked
    /********************************************************************************/
    class GameObjectClicked : EventArgs
    {
            public int gameObjectID;
            public GameObjectClicked(int gameObjectID)
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
    /// Open Fire Command Event
    /********************************************************************************/
    class OpenFireCommandEvent : EventArgs
    {
        public Vector3 point;

        public OpenFireCommandEvent(Vector3 point)
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
    /// Open Fire To Target Command Event
    /********************************************************************************/
    class OpenFireToTargetCommandEvent : EventArgs
    {
        public GameObjectInstance target;

        public OpenFireToTargetCommandEvent(GameObjectInstance target)
        {
            this.target = target;
        }

        public override string ToString()
        {
            return target.ToString();
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
    /// OpenContainerCommandEvent
    /********************************************************************************/
    class OpenContainerCommandEvent : EventArgs
    {
        public Container container;

        public OpenContainerCommandEvent(Container container)
        {
            this.container = container;
        }

        public override string ToString()
        {
            if (container != null) return "ID: " + container.ID.ToString();
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


    /********************************************************************************/
    /// ExchangeItemsCommandEvent
    /********************************************************************************/
    class ExchangeItemsCommandEvent : EventArgs
    {
        public Mercenary mercenary;

        public ExchangeItemsCommandEvent(Mercenary mercenary)
        {
            this.mercenary = mercenary;
        }

        public override string ToString()
        {
            if (mercenary != null) return "ID: " + mercenary.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// OpenEvent
    /********************************************************************************/
    class OpenEvent : EventArgs
    {
        public Mercenary mercenary;

        public OpenEvent(Mercenary mercenary)
        {
            this.mercenary = mercenary;
        }

        public override string ToString()
        {
            if (mercenary != null) return "ID: " + mercenary.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ExchangeItemsEvent
    /********************************************************************************/
    class ExchangeItemsEvent : EventArgs
    {
        public Mercenary mercenary1;
        public Mercenary mercenary2;

        public ExchangeItemsEvent(Mercenary mercenary1,Mercenary mercenary2)
        {
            this.mercenary1 = mercenary1;
            this.mercenary2 = mercenary2;
        }

        public override string ToString()
        {
            if (mercenary1 != null && mercenary2 != null) return "ID1: " + mercenary1.ID.ToString() + ", ID2: " + mercenary2.ID.ToString();
            else return "Null";
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ActivateObjectEvent
    /********************************************************************************/
    class ActivateObjectEvent : EventArgs
    {
        public GameObjectInstance gameObject;
        public ActivateObjectEvent(GameObjectInstance gameObject)
        {
            this.gameObject= gameObject;
        }

        public override string ToString()
        {
            return "Activate :" + gameObject.ID.ToString();
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// ObjectActivatedEvent
    /********************************************************************************/
    class ObjectActivatedEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public ObjectActivatedEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
        }
        public ObjectActivatedEvent()
        {
        }

        public override string ToString()
        {
            if (gameObject != null)
            {
                return "Activated :" + gameObject.ID.ToString();
            }
            return null;
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// SetBloomEvent
    /********************************************************************************/
    class SetBloomEvent : EventArgs
    {
        public float BloomIntensity;
        public float BaseIntensity;
        public float BloomSaturation;
        public float BaseSaturation;
        public float BloomThreshold;

        public SetBloomEvent(float bloomIntensity,
                            float baseIntensity,
                            float bloomSaturation,
                            float baseSaturation,
                            float bloomThreshold)
        {
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
            BloomThreshold = bloomThreshold;
        }
        
        public SetBloomEvent()
        {
        }
    }
    /********************************************************************************/
}
