using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.EventsSystem;
using PlagueEngine.HighLevelGameFlow;
using PlagueEngine.Input.Components;
using PlagueEngine.Rendering.Components;
using PlagueEngine.ArtificialIntelligence;


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
        private bool            useGUI              = false;
        private bool            mouseOnActions      = false;

        private EventsSnifferComponent    sniffer  = new EventsSnifferComponent();
        private KeyboardListenerComponent keyboard = null;
        private MouseListenerComponent    mouse    = null;
        private FrontEndComponent         frontEnd = null;

        private GameObjectInstance targetGameObject = null;
        private Mercenary          currentMercenary = null;
        private Inventory          inventory        = null;

        private int screenWidthOver2 = 0;
        private int mouseX           = 0;
        private int mouseOnMerc      = 0;

        private int iconsOffset      = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Dictionary<Mercenary,List<EventArgs>>  Mercenaries  { get; private set; }
        public LinkedCamera                           LinkedCamera { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(LinkedCamera              linkedCamera,
                         KeyboardListenerComponent keyboard,
                         MouseListenerComponent    mouse,
                         FrontEndComponent         frontEnd)
        {
            SelectedMercenaries = new List<Mercenary>();
            LinkedCamera        = linkedCamera;
            this.keyboard       = keyboard;
            this.mouse          = mouse;

            this.frontEnd = frontEnd;
            frontEnd.Draw = OnDraw;

            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(CreateEvent),
                                    typeof(DestroyEvent));

            keyboard.SubscibeKeys(OnKey,  Keys.Tab,Keys.LeftControl, Keys.LeftAlt,Keys.OemTilde,
                                          Keys.D1, Keys.D2,Keys.D3,Keys.D4,Keys.D5,
                                          Keys.D6, Keys.D7,Keys.D8,Keys.D9,Keys.D0,
                                          Keys.E);
            
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);
            mouse.SubscribeKeys(OnMouseKey, MouseKeyAction.LeftClick,
                                            MouseKeyAction.MiddleClick,
                                            MouseKeyAction.RightClick);

            Mercenaries = new Dictionary<Mercenary,List<EventArgs>>();
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

                    if (commandOnObjectEvent.gameObject.Status == GameObjectStatus.Passable)
                    {
                        QueueEvent(new MoveToPointCommandEvent(commandOnObjectEvent.position),!leftControl, SelectedMercenaries.ToArray());
                    }
                    else if (commandOnObjectEvent.gameObject.Status != GameObjectStatus.Nothing)
                    {
                        IActiveGameObject activeGameObject = commandOnObjectEvent.gameObject as IActiveGameObject;
                        if (activeGameObject != null)
                        {
                            ActionSwitchData data = new ActionSwitchData();
                            data.Feedback = this.ID;
                            data.ObjectName = commandOnObjectEvent.gameObject.Name;

                            if (SelectedMercenaries.Count == 1)
                            {
                                currentMercenary = SelectedMercenaries.ElementAt(0);
                                data.Actions = activeGameObject.GetActions(currentMercenary);
                            }
                            else
                            {
                                data.Actions = activeGameObject.GetActions();
                                currentMercenary = null;
                            }
                            
                            data.Position = commandOnObjectEvent.position;

                            SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                            
                            targetGameObject = commandOnObjectEvent.gameObject;
                        }
                    }                                        
                }                               
            }
            /*************************************/
            /// SelectedActionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(SelectedActionEvent)))
            {
                SelectedActionEvent SelectedActionEvent = e as SelectedActionEvent;

                if (currentMercenary != null)
                {
                    switch (SelectedActionEvent.Action)
                    {
                        case "Attack"             : QueueEvent(new AttackOrderEvent(targetGameObject as AbstractLivingBeing), !leftControl, currentMercenary); break; 
                        case "Grab"               : QueueEvent(new GrabObjectCommandEvent   (targetGameObject),!leftControl,currentMercenary); break;
                        case "Activate"           : QueueEvent(new ActivateObjectEvent (targetGameObject), !leftControl, currentMercenary); break;
                        case "Examine"            : QueueEvent(new ExamineObjectCommandEvent(targetGameObject),!leftControl,currentMercenary); break;
                        case "Follow"             : QueueEvent(new FollowObjectCommandEvent (targetGameObject),!leftControl,currentMercenary); break;
                        case "Exchange Items"     : QueueEvent(new ExchangeItemsCommandEvent(targetGameObject as Mercenary), !leftControl, currentMercenary); break;
                        case "Drop Item"          : QueueEvent(new DropItemCommandEvent(),        !leftControl, currentMercenary); break;
                        case "Reload"             : QueueEvent(new ReloadCommandEvent(),          !leftControl, currentMercenary); break;
                        case "Switch to Weapon"   : QueueEvent(new SwitchToWeaponCommandEvent(),  !leftControl, currentMercenary); break;
                        case "Switch to Side Arm" : QueueEvent(new SwitchToSideArmCommandEvent(), !leftControl, currentMercenary); break;
                        case "Inventory": 
                                        InventoryData data      = new InventoryData();
                                        data.MercenariesManager = this.ID;
                                        data.Mercenary          = targetGameObject.ID;
                                        if (inventory != null) SendEvent(new DestroyObjectEvent(inventory.ID), Priority.High, GlobalGameObjects.GameController);                                        
                                        SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                                        break;                                                                                                                                                
                    }
                }
                else
                {
                    switch (SelectedActionEvent.Action)
                    {
                        case "Attack": QueueEvent(new AttackOrderEvent(targetGameObject as AbstractLivingBeing), !leftControl, SelectedMercenaries.ToArray()); break; 
                        case "Grab"          : QueueEvent(new GrabObjectCommandEvent   (targetGameObject),!leftControl, SelectedMercenaries.ToArray()); break;
                        case "Activate"      : QueueEvent(new ActivateObjectEvent(targetGameObject), !leftControl, SelectedMercenaries.ToArray()); break;
                        case "Examine"       : QueueEvent(new ExamineObjectCommandEvent(targetGameObject),!leftControl, SelectedMercenaries.ToArray()); break;
                        case "Follow"        : QueueEvent(new FollowObjectCommandEvent (targetGameObject),!leftControl, SelectedMercenaries.ToArray()); break;
                        case "Exchange Items": QueueEvent(new ExchangeItemsCommandEvent(targetGameObject as Mercenary), !leftControl, SelectedMercenaries.ElementAt(0)); break;
                        case "Inventory":
                                        InventoryData data      = new InventoryData();
                                        data.MercenariesManager = this.ID;
                                        data.Mercenary          = targetGameObject.ID;
                                        if (inventory != null) SendEvent(new DestroyObjectEvent(inventory.ID), Priority.High, GlobalGameObjects.GameController);                                        
                                        SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                                        break;

                    }                
                }
            }
            /*************************************/
            /// ActionDoneEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ActionDoneEvent)))
            {
                List<EventArgs> Actions = Mercenaries[sender as Mercenary];
                Actions.RemoveAt(0);
                if (Actions.Count != 0) SendEvent(Actions.ElementAt(0), Priority.Normal, sender as Mercenary);
            }
            /*************************************/
            /// ExchangeItemsEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ExchangeItemsEvent)))
            {
                ExchangeItemsEvent ExchangeItemsEvent = e as ExchangeItemsEvent;

                InventoryData data = new InventoryData();
                data.MercenariesManager = this.ID;
                data.Mercenary  = ExchangeItemsEvent.mercenary1.ID;
                data.Mercenary2 = ExchangeItemsEvent.mercenary2.ID;
                if (inventory != null) SendEvent(new DestroyObjectEvent(inventory.ID), Priority.High, GlobalGameObjects.GameController);          
                SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
            }
            /*************************************/
            /// ObjectCreatedEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ObjectCreatedEvent)))
            {
                ObjectCreatedEvent ObjectCreatedEvent = e as ObjectCreatedEvent;
                
                if (ObjectCreatedEvent.GameObject.GetType().Equals(typeof(Inventory)))
                {
                    inventory = ObjectCreatedEvent.GameObject as Inventory;
                }
            }
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
                Mercenaries.Add(sender as Mercenary, new List<EventArgs>());
                iconsOffset = (66 * Mercenaries.Count) / 2;
            }
            /*************************************/
            /// DestroyEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(DestroyEvent)))
            {
                if (sender.GetType().Equals(typeof(Mercenary)))
                {
                    SelectedMercenaries.Remove(sender as Mercenary);
                    if (SelectedMercenaries.Count == 0)
                    {
                        SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                        commandMode = false;
                    }
                    Mercenaries.Remove(sender as Mercenary);
                    iconsOffset = (66 * Mercenaries.Count) / 2;
                }
                else if (sender.GetType().Equals(typeof(Inventory)))
                {
                    inventory = null;
                }
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
                    Mercenaries.ElementAt(0).Key.Marker = true;
                    SelectedMercenaries.Add(Mercenaries.ElementAt(0).Key);
                }
                else
                {
                    if (Mercenaries.Count == 1) return;
                    
                    int i = GetMercenaryIndex(SelectedMercenaries.ElementAt(0));

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

                    Mercenaries.ElementAt(i).Key.Marker = true;
                    SelectedMercenaries.Add(Mercenaries.ElementAt(i).Key);
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
            /// E
            /************************************************************************/
            else if (key == Keys.E && state.WasPressed())
            {
                if (SelectedMercenaries.Count == 1)
                {
                    InventoryData data = new InventoryData();
                    data.MercenariesManager = this.ID;
                    data.Mercenary = SelectedMercenaries.ElementAt(0).ID;
                    if (inventory != null) SendEvent(new DestroyObjectEvent(inventory.ID), Priority.High, GlobalGameObjects.GameController);                                        
                    SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                }
            }
            /************************************************************************/
            /// 1,2,3,4,5,6,7,8,9,0
            /************************************************************************/
            else if (state.WasPressed())
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
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
        {
            if (mouseMoveAction == MouseMoveAction.Move)
            {
                if (mouseMovementState.Position.Y > 5 && mouseMovementState.Position.Y < 85)
                { 
                    if (mouseMovementState.Position.X > (screenWidthOver2 - iconsOffset) &&
                        mouseMovementState.Position.X < (screenWidthOver2 + iconsOffset))
                    {
                        useGUI = true;
                        
                        if (mouseMovementState.Position.Y > 69) mouseOnActions = true;
                        else mouseOnActions = false;

                        mouseX = (int)mouseMovementState.Position.X - (screenWidthOver2 - iconsOffset);

                        LinkedCamera.mouseListenerComponent.Active = false;
                        
                        mouse.SetCursor("Default");
                        return;
                    }                    
                }                
            }
            useGUI = false;
            LinkedCamera.mouseListenerComponent.Active = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {
            if (useGUI)
            {
                if(mouseKeyState.WasPressed())
                {
                    mouseOnMerc = mouseX / 66;
                    /*********************************/
                    /// Left Click
                    /*********************************/
                    if (mouseKeyAction == MouseKeyAction.LeftClick)
                    {                 
                        if(mouseOnActions)
                        {
                            int x = mouseX % 66;
                            x /= 16;
                            if (x == 0)
                            {
                                Mercenary merc = Mercenaries.Keys.ElementAt(mouseOnMerc);
                                if (Mercenaries[merc].Count == 1)
                                {
                                    Mercenaries[merc].RemoveAt(0);
                                    SendEvent(new StopActionEvent(), Priority.High, merc);
                                }
                                else if(Mercenaries[merc].Count > 1)
                                {
                                    Mercenaries[merc].RemoveAt(0);
                                    SendEvent(new StopActionEvent(), Priority.High, merc);
                                    SendEvent(Mercenaries[merc].ElementAt(0), Priority.Normal, merc);                                    
                                }
                            }
                            else if (x < 4)
                            {
                                Mercenary merc = Mercenaries.Keys.ElementAt(mouseOnMerc);
                                if (Mercenaries[merc].Count > x)
                                {
                                    Mercenaries[merc].RemoveAt(x);                                    
                                }                            
                            }
                        }
                        else if (leftControl)
                        {
                            if (!SelectedMercenaries.Contains(Mercenaries.ElementAt(mouseOnMerc).Key))
                            {
                                SelectedMercenaries.Add(Mercenaries.ElementAt(mouseOnMerc).Key);
                                Mercenaries.ElementAt(mouseOnMerc).Key.Marker = true;
                                SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                                commandMode = true;
                            }
                        }
                        else if (leftAlt)
                        {
                            if (SelectedMercenaries.Contains(Mercenaries.ElementAt(mouseOnMerc).Key))
                            {
                                SelectedMercenaries.Remove(Mercenaries.ElementAt(mouseOnMerc).Key);
                                Mercenaries.ElementAt(mouseOnMerc).Key.Marker = false;

                                if (SelectedMercenaries.Count == 0)
                                {
                                    SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                                    commandMode = false;
                                }
                            }
                        }
                        else
                        {
                            foreach (Mercenary m in SelectedMercenaries) m.Marker = false;
                            SelectedMercenaries.Clear();

                            SelectedMercenaries.Add(Mercenaries.ElementAt(mouseOnMerc).Key);
                            Mercenaries.ElementAt(mouseOnMerc).Key.Marker = true;
                            SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                            commandMode = true;
                        }                                        
                    }
                    /*********************************/                        
                    /// Middle Click
                    /*********************************/                        
                    else if (mouseKeyAction == MouseKeyAction.MiddleClick)
                    {
                        if (!mouseOnActions)
                        {
                            SendEvent(new MoveToObjectCommandEvent(Mercenaries.ElementAt(mouseOnMerc).Key), Priority.High, LinkedCamera);
                        }
                    }
                    /*********************************/                        
                    /// Right Click
                    /*********************************/
                    else if (mouseKeyAction == MouseKeyAction.RightClick)
                    {
                        if (!mouseOnActions)
                        {
                            InventoryData data = new InventoryData();
                            data.MercenariesManager = this.ID;
                            data.Mercenary = Mercenaries.ElementAt(mouseOnMerc).Key.ID;
                            if (inventory != null) SendEvent(new DestroyObjectEvent(inventory.ID), Priority.High, GlobalGameObjects.GameController);                                        
                            SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                        }                    
                    }
                    /*********************************/                        
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Mercenary From Icon
        /****************************************************************************/
        public Mercenary GetMercenaryFromIcon(int mouseX, int mouseY)
        {
            if (mouseY > 5 && mouseY < 69)
            {
                if (mouseX > (screenWidthOver2 - iconsOffset) &&
                    mouseX < (screenWidthOver2 + iconsOffset))
                {
                    int merc = (mouseX - (screenWidthOver2 - iconsOffset)) / 66;
                    return Mercenaries.Keys.ElementAt(merc);
                }
            }

            return null;
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
                    Mercenary m = Mercenaries.ElementAt(i).Key;
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

                    Mercenary m = Mercenaries.ElementAt(i).Key;
                    m.Marker = true;
                    if (!SelectedMercenaries.Contains(m)) SelectedMercenaries.Add(m);
                    SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                    commandMode = true;
                }                
                else
                {
                    Mercenary m = Mercenaries.ElementAt(i).Key;
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

            if(LinkedCamera != null)
            {
                data.LinkedCamera = LinkedCamera.ID;        
            }
            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Draw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            int i = 0;            
            screenWidthOver2 = screenWidth/2;
            float offset = screenWidthOver2;
            offset -= (66 * Mercenaries.Count)/2.0f;
            foreach (KeyValuePair<Mercenary,List<EventArgs>> pair in Mercenaries)
            {
                if (SelectedMercenaries.Contains(pair.Key))
                {
                    spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i), 5), new Rectangle(196, 0, 64, 64), Color.White);
                }
                else
                {
                    spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i), 5), new Rectangle(0, 0, 64, 64), Color.White);
                }

                spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i), 5), pair.Key.Icon, Color.White);
                spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i), 5), new Rectangle(64, 0, 64, 64), GetColor(pair.Key.ObjectAIController.HP, pair.Key.ObjectAIController.MaxHP));


                int j = 0;
                foreach (EventArgs e in pair.Value)
                {
                    spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i) + (16*j), 69), new Rectangle(0, 384, 16, 16), Color.White);
                    spriteBatch.Draw(frontEnd.Texture, new Vector2(offset + (66 * i) + (16 * j), 69), GetActionRect(e), Color.White);
                    j++;
                }

                ++i;
            }
        }   
        /****************************************************************************/


        /****************************************************************************/
        /// Get Color
        /****************************************************************************/
        private Color GetColor(uint HP, uint MaxHP)
        {
            float green = 0;
            float red   = 0;

            if (HP > MaxHP / 2)
            {
                green = 1;
                red   = 1.0f - ((float)HP - MaxHP/2) / ((float)MaxHP/2);
            }
            else
            {
                green = ((float)HP) / ((float)MaxHP/2);
                red   = 1;
            }

            return Color.FromNonPremultiplied(new Vector4(red, green, 0, 0.5f));
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetMercenaryIndex
        /****************************************************************************/
        public int GetMercenaryIndex(Mercenary mercenary)
        { 
            int i = -1;
            foreach(Mercenary merc in Mercenaries.Keys)
            {
                ++i;
                if (merc == mercenary) return i;
            }
            return -1;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Next Mercenary
        /****************************************************************************/
        public Mercenary GetNextMercenary(Mercenary mercenary)
        {
            int i = GetMercenaryIndex(mercenary);
            
            if (i == Mercenaries.Count -1) i = 0;
            else                           ++i;
            
            return Mercenaries.ElementAt(i).Key;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Prev Mercenary
        /****************************************************************************/
        public Mercenary GetPrevMercenary(Mercenary mercenary)
        {
            int i = GetMercenaryIndex(mercenary);

            if (i == 0) i = Mercenaries.Count - 1;
            else --i;

            return Mercenaries.ElementAt(i).Key;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// QueueEvent
        /****************************************************************************/
        private void QueueEvent(EventArgs e,bool clear, params Mercenary[] mercenaries)
        {
            List<EventArgs> Actions;

            foreach (Mercenary mercenary in mercenaries)
            {
                Actions = Mercenaries[mercenary];

                if (Actions.Count != 0 && clear)
                {
                    SendEvent(new StopActionEvent(), Priority.High, mercenary);
                    Actions.Clear();

                    Actions.Add(e);
                    SendEvent(e, Priority.Normal, mercenary);
                }
                else if (Actions.Count != 0)
                {
                    if (Actions.Count < 4) Actions.Add(e);
                }
                else
                {
                    Actions.Add(e);
                    SendEvent(e, Priority.Normal, mercenary);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActionRect
        /****************************************************************************/
        private Rectangle GetActionRect(EventArgs e)
        {
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                return new Rectangle(16, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(ActivateObjectEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                return new Rectangle(48, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                return new Rectangle(64, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(ExchangeItemsCommandEvent)))
            {
                return new Rectangle(80, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(ReloadCommandEvent)))
            {
                return new Rectangle(96, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(DropItemCommandEvent)))
            {
                return new Rectangle(112, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(SwitchToWeaponCommandEvent)))
            {
                return new Rectangle(128, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(SwitchToSideArmCommandEvent)))
            {
                return new Rectangle(144, 384, 16, 16);
            }
            else return new Rectangle();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            sniffer.ReleaseMe();
            keyboard.ReleaseMe();
            mouse.ReleaseMe();
            frontEnd.ReleaseMe();
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