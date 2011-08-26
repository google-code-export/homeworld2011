using System;
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

    internal class CreateEvent : EventArgs { };

    internal class DestroyEvent : EventArgs { };

    internal class SwitchEvent : EventArgs { };

    internal class GameObjectReleased : EventArgs { };

    internal class ExamineEvent : EventArgs { };

    internal class ActionDoneEvent : EventArgs { };

    internal class StopActionEvent : EventArgs { };

    internal class CloseEvent : EventArgs { };

    internal class ExitGameEvent : EventArgs { };

    internal class InGameMenuClose : EventArgs { };

    internal class FadeInEvent : EventArgs { };

    internal class FadeInCancelEvent : EventArgs { };

    internal class FadeOutEvent : EventArgs { };

    internal class DropItemCommandEvent : EventArgs { };

    internal class ReloadCommandEvent : EventArgs { };

    internal class SwitchToWeaponCommandEvent : EventArgs { };

    internal class SwitchToSideArmCommandEvent : EventArgs { };

    /********************************************************************************/

    /********************************************************************************/
    /// GameObjectClicked
    /********************************************************************************/

    internal class GameObjectClicked : EventArgs
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

    internal class SelectedObjectEvent : EventArgs
    {
        public GameObjectInstance gameObject;
        public Vector3 position;

        public SelectedObjectEvent(GameObjectInstance gameObject, Vector3 position)
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
    /// CommandOnObjectEvent
    /********************************************************************************/

    internal class CommandOnObjectEvent : EventArgs
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

    internal class ExSwitchEvent : EventArgs
    {
        public String name;
        public bool value;

        public ExSwitchEvent(String name, bool value)
        {
            this.name = name;
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

    internal class AddToSelectionEvent : EventArgs
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

    internal class RemoveFromSelectionEvent : EventArgs
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

    internal class MoveToPointCommandEvent : EventArgs
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
    /// LookAtPointEvent
    /********************************************************************************/

    internal class LookAtPointEvent : EventArgs
    {
        public Vector3 point;

        public LookAtPointEvent(Vector3 point)
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
    /// RunToPointCommandEvent
    /********************************************************************************/

    internal class RunToPointCommandEvent : EventArgs
    {
        public Vector3 point;

        public RunToPointCommandEvent(Vector3 point)
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

    internal class OpenFireCommandEvent : EventArgs
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

    internal class OpenFireToTargetCommandEvent : EventArgs
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

    internal class MoveToObjectCommandEvent : EventArgs
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

    internal class ChangeLevelEvent : EventArgs
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

    internal class SaveLevelEvent : EventArgs
    {
        public String Level;

        public SaveLevelEvent(String level)
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

    internal class CreateObjectEvent : EventArgs
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

    internal class ObjectCreatedEvent : EventArgs
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

    internal class DestroyObjectEvent : EventArgs
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

    internal class SelectedActionEvent : EventArgs
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

    internal class ChangeSpeedEvent : EventArgs
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

    internal class GrabObjectCommandEvent : EventArgs
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

    internal class OpenContainerCommandEvent : EventArgs
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

    internal class ExamineObjectCommandEvent : EventArgs
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

    internal class FollowObjectCommandEvent : EventArgs
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

    internal class ExchangeItemsCommandEvent : EventArgs
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
    /// RegisterMercenaryEvent
    /********************************************************************************/

    internal class RegisterMercenaryEvent : EventArgs
    {
        public Mercenary mercenary;

        public RegisterMercenaryEvent(Mercenary mercenary)
        {
            this.mercenary = mercenary;
        }

        public override string ToString()
        {
            if (mercenary != null) return "ID: " + mercenary.ID.ToString();
            else return "Null";
        }

        public override bool Equals(object obj)
        {
            if (obj as RegisterMercenaryEvent == null)
            {
                return false;
            }
            else
            {
                return mercenary.Equals((obj as RegisterMercenaryEvent).mercenary);
            }
        }
    }

    /********************************************************************************/

    /********************************************************************************/
    /// OpenEvent
    /********************************************************************************/

    internal class OpenEvent : EventArgs
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

    internal class ExchangeItemsEvent : EventArgs
    {
        public Mercenary mercenary1;
        public Mercenary mercenary2;

        public ExchangeItemsEvent(Mercenary mercenary1, Mercenary mercenary2)
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

    internal class ActivateObjectEvent : EventArgs
    {
        public GameObjectInstance gameObject;

        public ActivateObjectEvent(GameObjectInstance gameObject)
        {
            this.gameObject = gameObject;
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

    internal class ObjectActivatedEvent : EventArgs
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

    internal class HealCommandEvent : EventArgs
    {
        public Mercenary merc;

        public HealCommandEvent(Mercenary merc)
        {
            this.merc = merc;
        }

        public HealCommandEvent() : this(null) { }

        public override string ToString()
        {
            return "Heal " + (merc == null ? "self." : merc.ToString());
        }
    }

    /********************************************************************************/
    /// NewDialogMessageEvent
    /********************************************************************************/

    internal class NewDialogMessageEvent : EventArgs
    {
        public String name;
        public String text;
        public Rectangle icon;

        public NewDialogMessageEvent(String name, String text, Rectangle icon)
        {
            this.name = name;
            this.text = text;
            this.icon = icon;
        }

        public override string ToString()
        {
            return name + " is saying: " + text;
        }
    }

    /********************************************************************************/

    /********************************************************************************/
    /// SetBloomEvent
    /********************************************************************************/

    internal class SetBloomEvent : EventArgs
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