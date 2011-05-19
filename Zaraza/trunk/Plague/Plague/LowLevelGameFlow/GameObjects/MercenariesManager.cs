using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlagueEngine.EventsSystem;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input.Components;


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
        private bool            leftControl         = false;
        private bool            leftAlt             = false;

        private EventsSnifferComponent    sniffer  = new EventsSnifferComponent();
        private KeyboardListenerComponent keyboard = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public List<Mercenary>  Mercenaries  { get; private set; }
        public LinkedCamera     LinkedCamera { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(LinkedCamera              linkedCamera,
                         KeyboardListenerComponent keyboard)
        {
            SelectedMercenaries = new List<Mercenary>();
            LinkedCamera        = linkedCamera;
            this.keyboard       = keyboard;

            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(CreateEvent));

            keyboard.SubscibeKeys(OnKey,  Keys.Tab,Keys.LeftControl, Keys.LeftAlt,Keys.OemTilde,
                                          Keys.D1, Keys.D2,Keys.D3,Keys.D4,Keys.D5,
                                          Keys.D6, Keys.D7,Keys.D8,Keys.D9,Keys.D0);

            Mercenaries = new List<Mercenary>();
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
                    foreach (Mercenary merc in SelectedMercenaries) merc.Marker = false;
                    SelectedMercenaries.Clear();
                    SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                    commandMode = false;
                    return;
                }

                Mercenary m = selectedObjectEvent.gameObject as Mercenary;

                if (m != null)
                {
                    foreach (Mercenary merc in SelectedMercenaries) merc.Marker = false;
                    SelectedMercenaries.Clear();

                    SelectedMercenaries.Add(m);
                    m.Marker = true;

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
                    m.Marker = true;

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
                    m.Marker = false;

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


        /****************************************************************************/
        /// On Sniffed Event
        /****************************************************************************/
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            
            /*************************************/
            /// CreateEvent
            /*************************************/
            if (e.GetType().Equals(typeof(CreateEvent)) && sender.GetType().Equals(typeof(Mercenary)))
            {
                Mercenaries.Add(sender as Mercenary);  
            }
            /*************************************/

        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        public void OnKey(Keys key, ExtendedKeyState state)
        {

            /************************************************************************/
            /// Tab
            /************************************************************************/
            if (key == Keys.Tab && state.WasPressed())
            {
                if (Mercenaries.Count == 0) return;

                SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                commandMode = true;

                if (SelectedMercenaries.Count == 0)
                {
                    Mercenaries.ElementAt(0).Marker = true;
                    SelectedMercenaries.Add(Mercenaries.ElementAt(0));
                }
                else
                {
                    if (Mercenaries.Count == 1) return;
                    
                    int i = Mercenaries.IndexOf(SelectedMercenaries.ElementAt(0));

                    foreach (Mercenary m in SelectedMercenaries) m.Marker = false;
                    SelectedMercenaries.Clear();

                    if (leftControl)
                    {
                        if (i == 0) i = Mercenaries.Count -1;
                        else --i;
                    }
                    else
                    {
                        if (i == Mercenaries.Count - 1) i = 0;
                        else ++i;
                    }

                    Mercenaries.ElementAt(i).Marker = true;
                    SelectedMercenaries.Add(Mercenaries.ElementAt(i));
                }
            }
            /************************************************************************/
            /// Left Control
            /************************************************************************/
            else if (key == Keys.LeftControl) leftControl = state.IsDown();
            /************************************************************************/
            /// Left Alt
            /************************************************************************/
            else if (key == Keys.LeftAlt) leftAlt = state.IsDown();
            /************************************************************************/
            /// Tilde
            /************************************************************************/
            else if (key == Keys.OemTilde && state.WasPressed())
            {
                if(SelectedMercenaries.Count == 1)
                {
                    SendEvent(new MoveToObjectCommandEvent(SelectedMercenaries.ElementAt(0)), Priority.High, LinkedCamera);
                }
            }
            /************************************************************************/
            /// 1,2,3,4,5,6,7,8,9,0
            /************************************************************************/
            else if(state.WasPressed())
            {
                switch (key)
                {
                    case Keys.D1: ProcessMercenary(0); break;
                    case Keys.D2: ProcessMercenary(1); break;
                    case Keys.D3: ProcessMercenary(2); break;
                    case Keys.D4: ProcessMercenary(3); break;
                    case Keys.D5: ProcessMercenary(4); break;
                    case Keys.D6: ProcessMercenary(5); break;
                    case Keys.D7: ProcessMercenary(6); break;
                    case Keys.D8: ProcessMercenary(7); break;
                    case Keys.D9: ProcessMercenary(8); break;
                    case Keys.D0: ProcessMercenary(9); break;                                                
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Process Mercenary
        /****************************************************************************/
        private void ProcessMercenary(int i)
        {
            if (Mercenaries.Count > i)
            {
                if (leftAlt)
                {
                    Mercenary m = Mercenaries.ElementAt(i);
                    m.Marker = false;
                    if (SelectedMercenaries.Contains(m)) SelectedMercenaries.Remove(m);
                    
                    if (SelectedMercenaries.Count == 0)
                    {
                        SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                        commandMode = false;
                    }
                }
                else if (!leftControl && !leftAlt)
                {
                    foreach (Mercenary m1 in SelectedMercenaries) m1.Marker = false;
                    SelectedMercenaries.Clear();

                    Mercenary m = Mercenaries.ElementAt(i);
                    m.Marker = true;
                    if (!SelectedMercenaries.Contains(m)) SelectedMercenaries.Add(m);
                    SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                    commandMode = true;
                }                
                else
                {
                    Mercenary m = Mercenaries.ElementAt(i);
                    m.Marker = true;
                    if (!SelectedMercenaries.Contains(m)) SelectedMercenaries.Add(m);
                    SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                    commandMode = true;
                }                
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MercenariesManagerData data = new MercenariesManagerData();
            GetData(data);

            data.LinkedCamera = LinkedCamera.ID;       

            return data;
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
        [CategoryAttribute("References")]
        public int LinkedCamera { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/