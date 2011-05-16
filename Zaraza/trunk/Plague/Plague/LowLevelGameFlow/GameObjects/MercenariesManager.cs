using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using PlagueEngine.EventsSystem;
using PlagueEngine.HighLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// MercenariesManager
    /********************************************************************************/
    class MercenariesManager : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields        
        /****************************************************************************/
        private List<Mercenary> SelectedMercenaries = null;
        private bool            commandMode         = false;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public LinkedCamera LinkedCamera { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(List<Mercenary> mercenaries,LinkedCamera linkedCamera)
        {
            SelectedMercenaries = mercenaries;
            LinkedCamera        = linkedCamera;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            /*************************************/
            /// SelectedObjectEvent
            /*************************************/
            if (e.GetType().Equals(typeof(SelectedObjectEvent)))
            {
                SelectedObjectEvent selectedObjectEvent = e as SelectedObjectEvent;

                if (selectedObjectEvent.gameObject == null)
                {
                    foreach (Mercenary merc in SelectedMercenaries) merc.Marker.Enabled = false;
                    SelectedMercenaries.Clear();
                    SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                    commandMode = false;
                    return;
                }

                Mercenary m = selectedObjectEvent.gameObject as Mercenary;

                if (m != null)
                {
                    foreach (Mercenary merc in SelectedMercenaries) merc.Marker.Enabled = false;
                    SelectedMercenaries.Clear();

                    SelectedMercenaries.Add(m);
                    m.Marker.Enabled = true;

                    SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                    commandMode = true;
                }
            }
            /*************************************/
            /// AddToSelectionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(AddToSelectionEvent)))
            {
                AddToSelectionEvent addToSelectionEvent = e as AddToSelectionEvent;

                Mercenary m = addToSelectionEvent.gameObject as Mercenary;

                if (m != null)
                {
                    SelectedMercenaries.Add(m);
                    m.Marker.Enabled = true;

                    SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                    commandMode = true;
                }
            }
            /*************************************/
            /// RemoveFromSelectionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(RemoveFromSelectionEvent)))
            {
                RemoveFromSelectionEvent removeFromSelectionEvent = e as RemoveFromSelectionEvent;

                Mercenary m = removeFromSelectionEvent.gameObject as Mercenary;

                if (m != null)
                {
                    SelectedMercenaries.Remove(m);
                    m.Marker.Enabled = false;

                    if (SelectedMercenaries.Count == 0)
                    {
                        SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                        commandMode = false;
                    }
                }
            }
            /*************************************/
            /// CommandOnObjectEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(CommandOnObjectEvent)))
            {
                if (commandMode)
                {
                    CommandOnObjectEvent commandOnObjectEvent = e as CommandOnObjectEvent;

                    switch (commandOnObjectEvent.gameObject.Status)
                    {
                        case GameObjectStatus.Walk: 
                            SendEvent(new MoveToPointCommandEvent(commandOnObjectEvent.position), Priority.High, SelectedMercenaries.ToArray());
                            break;
                        case GameObjectStatus.Interesting:
                        case GameObjectStatus.Mercenary:
                        case GameObjectStatus.Pickable:
                            SendEvent(new MoveToObjectCommandEvent(commandOnObjectEvent.gameObject), Priority.High, SelectedMercenaries.ToArray());
                            break;
                    }
                }                               
            }
            /*************************************/

        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// MercenariesManager Data
    /********************************************************************************/
    [Serializable]
    public class MercenariesManagerData : GameObjectInstanceData
    {
        public List<int> SelectedMercenaries { get; set; }

        [CategoryAttribute("References")]
        public int LinkedCamera { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/