using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.TimeControlSystem;

using PlagueEngine.EventsSystem;
using PlagueEngine.Input.Components;
using PlagueEngine.Rendering.Components;
using PlagueEngine.ArtificialIntelligence;
using PlagueEngine.ArtificialIntelligence.Controllers;

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
        private List<Mercenary> _selectedMercenaries;
        private bool _commandMode;
        private bool _leftControl;
        private bool _leftAlt;
        private bool _useGUI;
        private bool _mouseOnActions;

        private readonly EventsSnifferComponent _sniffer = new EventsSnifferComponent();
        private KeyboardListenerComponent _keyboard;
        private MouseListenerComponent _mouse;
        private FrontEndComponent _frontEnd;
        private FogOfWarComponent _fogOfWar;

        private GameObjectInstance _targetGameObject;
        private Mercenary _currentMercenary;
        private Inventory _inventory;
        private List<int> _blackList;

        private int _screenWidthOver2;
        private int _mouseX;
        private int _mouseOnMerc;
        private Vector3 goTo;

        private int _iconsOffset;
        private FireSelector fireSelector = null;
        private Clock clock = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Dictionary<Mercenary, List<EventArgs>> Mercenaries        { get; private set; }
        public Dictionary<Mercenary, uint>          WoundedMercenaries { get; private set; }
        public LinkedCamera LinkedCamera { get; set; }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(LinkedCamera linkedCamera,
                         KeyboardListenerComponent keyboard,
                         MouseListenerComponent mouse,
                         FrontEndComponent frontEnd,
                         FogOfWarComponent fogOfWar,
                         List<int> ignoreMercenaries)
        {
            _selectedMercenaries = new List<Mercenary>();
            LinkedCamera = linkedCamera;
            _keyboard = keyboard;
            _mouse = mouse;

            _fogOfWar = fogOfWar;
            if (ignoreMercenaries == null) _blackList = new List<int>();
            else _blackList = ignoreMercenaries;

            _frontEnd = frontEnd;
            frontEnd.Draw = OnDraw;

            _sniffer.SetOnSniffedEvent(OnSniffedEvent);
            _sniffer.SubscribeEvents(typeof(CreateEvent),
                                    typeof(DestroyEvent),
                                    typeof(MercenaryHit));

            keyboard.SubscibeKeys(OnKey, Keys.Tab, Keys.LeftControl, Keys.LeftAlt, Keys.OemTilde,
                                          Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
                                          Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
                                          Keys.E, Keys.F,Keys.Q);

            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);
            mouse.SubscribeKeys(OnMouseKey, MouseKeyAction.LeftClick,
                                            MouseKeyAction.MiddleClick,
                                            MouseKeyAction.RightClick);

            Mercenaries = new Dictionary<Mercenary, List<EventArgs>>();
            WoundedMercenaries = new Dictionary<Mercenary, uint>();
            clock = TimeControl.CreateClock();

            RequiresUpdate = true;

            MercenaryController.MercManager = this;
        }
        /****************************************************************************/

        private void AddToSelection(Mercenary m)
        {
            if (m == null) return;
            if (!_selectedMercenaries.Contains(m))
            {
                _selectedMercenaries.Add(m);
            }
            _commandMode = true;
            m.Marker = true;
            SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
        }

        private void RemoveFromSelection(Mercenary m)
        {
            if (m == null) return;
            _selectedMercenaries.Remove(m);
            m.Marker = false;
            if (_selectedMercenaries.Count != 0) return;
            SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
            _commandMode = false;
        }

        private void ClearSelection()
        {
            foreach (var merc in _selectedMercenaries)
            {
                merc.Marker = false;
            }
            _selectedMercenaries.Clear();
        }
        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (sender == null) return;
            var argsType = e.GetType();


            /*****************************************/
            // RegisterMercenaryEvent 
            /*****************************************/
            if (argsType.Equals(typeof(RegisterMercenaryEvent)))
            {
                RegisterMercenaryEvent RegisterMercenaryEvent = e as RegisterMercenaryEvent;
                var m = RegisterMercenaryEvent.mercenary;
                if (m != null)
                {
                    if (Mercenaries.ContainsKey(m)) return;                    
                    
                    Mercenaries.Add(m, new List<EventArgs>());
                    WoundedMercenaries.Add(m, 0);
                    _iconsOffset = (66 * Mercenaries.Count) / 2;
                }
            }
            /*****************************************/
            // LookAtPointEvent 
            /*****************************************/
            else if (argsType.Equals(typeof(LookAtPointEvent)))
            {
                if (sender.GetType().Equals(typeof(LinkedCamera)))
                {
                    foreach (var m in _selectedMercenaries)
                    {
                        if (Mercenaries[m].Count == 0) SendEvent(e, Priority.High, m);
                    }                    
                }
                else
                {
                    if (_selectedMercenaries.Count == 1)
                    {
                        if (LinkedCamera.Run) QueueEvent(new RunToPointCommandEvent(goTo), !_leftControl, _selectedMercenaries.ToArray());
                        else                  QueueEvent(new MoveToPointCommandEvent(goTo), !_leftControl, _selectedMercenaries.ToArray());
                        if((e as LookAtPointEvent).point != Vector3.Zero) QueueEvent(e, false, _selectedMercenaries.ToArray());
                    }
                    else
                    {
                        //calculate center
                        var center = Vector3.Zero;
                        foreach (var merc in _selectedMercenaries)
                        {
                            center += merc.World.Translation;
                        }
                        center /= _selectedMercenaries.Count;
                        //check if moving to center or to point
                        var toCenter = false;
                        foreach (var merc in _selectedMercenaries)
                        {
                            double distToCenter = Vector3.Distance(merc.World.Translation, center);
                            double distToTarget = Vector3.Distance(merc.World.Translation, goTo);
                            if (distToCenter <= distToTarget) continue;
                            toCenter = true;
                            break;
                        }
                        if (toCenter)
                        {
                            foreach (var merc in _selectedMercenaries)
                            {
                                var target = goTo + Vector3.Normalize(merc.World.Translation - center);

                                if (LinkedCamera.Run) QueueEvent(new RunToPointCommandEvent(target), !_leftControl, merc);
                                else QueueEvent(new MoveToPointCommandEvent(target), !_leftControl, merc);
                                if ((e as LookAtPointEvent).point != Vector3.Zero)  QueueEvent(e, false, merc);

                            }
                        }
                        else
                        {
                            foreach (var merc in _selectedMercenaries)
                            {
                                var target = (merc.World.Translation - center) + goTo;
                                if (LinkedCamera.Run) QueueEvent(new RunToPointCommandEvent(target), !_leftControl, merc);
                                else QueueEvent(new MoveToPointCommandEvent(target), !_leftControl, merc);
                                if ((e as LookAtPointEvent).point != Vector3.Zero)  QueueEvent(e, false, merc);
                            }
                        }
                    }
                }
            }
            /*****************************************/
            // SelectedObjectEvent 
            /*****************************************/
            else if (argsType.Equals(typeof(SelectedObjectEvent)))
            {
                var selectedObjectEvent = e as SelectedObjectEvent;
                if (selectedObjectEvent == null) return;
                ClearSelection();
                AddToSelection(selectedObjectEvent.gameObject as Mercenary);
            }
            /*****************************************/
            // AddToSelectionEvent
            /*****************************************/
            else if (argsType.Equals(typeof(AddToSelectionEvent)))
            {
                var addToSelectionEvent = e as AddToSelectionEvent;
                if (addToSelectionEvent == null) return;
                AddToSelection(addToSelectionEvent.gameObject as Mercenary);
            }
            /*****************************************/
            // RemoveFromSelectionEvent
            /*****************************************/
            else if (argsType.Equals(typeof(RemoveFromSelectionEvent)))
            {
                var removeFromSelectionEvent = e as RemoveFromSelectionEvent;
                if (removeFromSelectionEvent == null) return;
                RemoveFromSelection(removeFromSelectionEvent.gameObject as Mercenary);
            }
            /*****************************************/
            // CommandOnObjectEvent
            /*****************************************/
            else if (argsType.Equals(typeof(CommandOnObjectEvent)))
            {
                if (LinkedCamera.FireMode)
                {
                    var commandOnObjectEvent = e as CommandOnObjectEvent;
                    if (commandOnObjectEvent == null) return;

                    if (commandOnObjectEvent.gameObject.Status == GameObjectStatus.Targetable)
                    {
                        QueueEvent(new OpenFireToTargetCommandEvent(commandOnObjectEvent.gameObject), !_leftControl, _selectedMercenaries.ToArray());
                    }
                    else
                    {
                        QueueEvent(new OpenFireCommandEvent(commandOnObjectEvent.position), !_leftControl, _selectedMercenaries.ToArray());
                    }
                }
                else if (_commandMode)
                {
                    var commandOnObjectEvent = e as CommandOnObjectEvent;
                    if (commandOnObjectEvent == null) return;

                    if (commandOnObjectEvent.gameObject.Status == GameObjectStatus.Targetable)
                    {
                        QueueEvent(new OpenFireToTargetCommandEvent(commandOnObjectEvent.gameObject), !_leftControl, _selectedMercenaries.ToArray());
                    }
                    else if (commandOnObjectEvent.gameObject.Status == GameObjectStatus.Passable)
                    {
                            DirectionData data = new DirectionData();
                            data.Feedback = this.ID;
                            data.World = Matrix.CreateTranslation(commandOnObjectEvent.position + new Vector3(0,0.5f,0));
                            SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);

                            goTo = commandOnObjectEvent.position;                            
                    }
                    else if (commandOnObjectEvent.gameObject.Status != GameObjectStatus.Nothing)
                    {
                        var activeGameObject = commandOnObjectEvent.gameObject as IActiveGameObject;
                        if (activeGameObject != null)
                        {
                            var data = new ActionSwitchData
                                           {
                                               Feedback = ID,
                                               ObjectName = commandOnObjectEvent.gameObject.Name
                                           };

                            if (_selectedMercenaries.Count == 1)
                            {
                                _currentMercenary = _selectedMercenaries.ElementAt(0);
                                data.Actions = activeGameObject.GetActions(_currentMercenary);
                            }
                            else
                            {
                                data.Actions = activeGameObject.GetActions();
                                _currentMercenary = null;
                            }

                            data.Position = commandOnObjectEvent.position;
                            if (data.Actions.Length != 0)
                            {
                                SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                                _targetGameObject = commandOnObjectEvent.gameObject;
                            }
                        }
                    }
                }
            }
            /*****************************************/
            // SelectedActionEvent
            /*****************************************/
            else if (argsType.Equals(typeof(SelectedActionEvent)))
            {
                var selectedActionEvent = e as SelectedActionEvent;
                if (selectedActionEvent == null) return;


                if (_currentMercenary != null)
                {
                    switch (selectedActionEvent.Action)
                    {
                        case "Attack": QueueEvent(new AttackOrderEvent(_targetGameObject as AbstractLivingBeing), !_leftControl, _currentMercenary); break;
                        case "Grab": QueueEvent(new GrabObjectCommandEvent(_targetGameObject), !_leftControl, _currentMercenary); break;
                        case "Open": QueueEvent(new OpenContainerCommandEvent(_targetGameObject as Container), !_leftControl, _currentMercenary); break;
                        case "Get Items": QueueEvent(new OpenContainerCommandEvent(_targetGameObject as DeadBody), !_leftControl, _currentMercenary); break;
                        case "Activate": QueueEvent(new ActivateObjectEvent(_targetGameObject), !_leftControl, _currentMercenary); break;
                        case "Examine": QueueEvent(new ExamineObjectCommandEvent(_targetGameObject), !_leftControl, _currentMercenary); break;
                        case "Follow": QueueEvent(new FollowObjectCommandEvent(_targetGameObject), !_leftControl, _currentMercenary); break;
                        case "Exchange Items": QueueEvent(new ExchangeItemsCommandEvent(_targetGameObject as Mercenary), !_leftControl, _currentMercenary); break;
                        case "Drop Item": QueueEvent(new DropItemCommandEvent(), !_leftControl, _currentMercenary); break;
                        case "Reload": QueueEvent(new ReloadCommandEvent(), !_leftControl, _currentMercenary); break;
                        case "Switch to Weapon": QueueEvent(new SwitchToWeaponCommandEvent(), !_leftControl, _currentMercenary); break;
                        case "Switch to Side Arm": QueueEvent(new SwitchToSideArmCommandEvent(), !_leftControl, _currentMercenary); break;
                        case "Heal": QueueEvent(new HealCommandEvent(), !_leftControl, _currentMercenary); break;
                        case "Heal Him": QueueEvent(new HealCommandEvent(_targetGameObject as Mercenary), !_leftControl, _currentMercenary); break;
                        case "Inventory":
                            var data = new InventoryData
                                                {
                                                    MercenariesManager = ID,
                                                    Mercenary = _targetGameObject.ID
                                                };
                            if (_inventory != null) SendEvent(new DestroyObjectEvent(_inventory.ID), Priority.High, GlobalGameObjects.GameController);
                            SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                            break;
                    }
                }
                else
                {
                    switch (selectedActionEvent.Action)
                    {
                        case "Attack": QueueEvent(new AttackOrderEvent(_targetGameObject as AbstractLivingBeing), !_leftControl, _selectedMercenaries.ToArray()); break;
                        case "Grab": QueueEvent(new GrabObjectCommandEvent(_targetGameObject), !_leftControl, _selectedMercenaries.ToArray()); break;
                        case "Activate": QueueEvent(new ActivateObjectEvent(_targetGameObject), !_leftControl, _selectedMercenaries.ToArray()); break;
                        case "Examine": QueueEvent(new ExamineObjectCommandEvent(_targetGameObject), !_leftControl, _selectedMercenaries.ToArray()); break;
                        case "Follow": QueueEvent(new FollowObjectCommandEvent(_targetGameObject), !_leftControl, _selectedMercenaries.ToArray()); break;
                        case "Exchange Items": QueueEvent(new ExchangeItemsCommandEvent(_targetGameObject as Mercenary), !_leftControl, _selectedMercenaries.ElementAt(0)); break;
                        case "Inventory":
                            var data = new InventoryData
                                                          {
                                                              MercenariesManager = ID,
                                                              Mercenary = _targetGameObject.ID
                                                          };
                            if (_inventory != null) SendEvent(new DestroyObjectEvent(_inventory.ID), Priority.High, GlobalGameObjects.GameController);
                            SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                            break;

                    }
                }
            }
            /*****************************************/
            // ActionDoneEvent
            /*****************************************/
            else if (argsType.Equals(typeof(ActionDoneEvent)))
            {
                var m = sender as Mercenary;
                if (m != null)
                {
                    if (Mercenaries.ContainsKey(m))
                    {
                        var actions = Mercenaries[m];

                        if (actions.Count != 0)
                        {
                            actions.RemoveAt(0);
                            if (actions.Count != 0)
                            {
                                SendEvent(actions.ElementAt(0), Priority.Normal, sender as Mercenary);
                                if (actions.ElementAt(0) as LookAtPointEvent != null)
                                {
                                    actions.RemoveAt(0);
                                    if (actions.Count != 0)
                                    {
                                        SendEvent(actions.ElementAt(0), Priority.Normal, sender as Mercenary);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            /*****************************************/
            // ExchangeItemsEvent
            /*****************************************/
            else if (argsType.Equals(typeof(ExchangeItemsEvent)))
            {
                var exchangeItemsEvent = e as ExchangeItemsEvent;
                if (exchangeItemsEvent == null) return;
                var data = new InventoryData
                                         {
                                             MercenariesManager = ID,
                                             Mercenary = exchangeItemsEvent.mercenary1.ID,
                                             Mercenary2 = exchangeItemsEvent.mercenary2.ID
                                         };
                if (_inventory != null) SendEvent(new DestroyObjectEvent(_inventory.ID), Priority.High, GlobalGameObjects.GameController);
                SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
            }
            /*****************************************/
            // ObjectCreatedEvent
            /*****************************************/
            else if (argsType.Equals(typeof(ObjectCreatedEvent)))
            {
                var objectCreatedEvent = e as ObjectCreatedEvent;
                if (objectCreatedEvent == null) return;
                if (objectCreatedEvent.GameObject.GetType().Equals(typeof(Inventory)))
                {
                    _inventory = objectCreatedEvent.GameObject as Inventory;
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Sniffed Event
        /****************************************************************************/
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            if (e.GetType().Equals(typeof(CreateEvent)) && sender.GetType().Equals(typeof(Mercenary)))
            {                
                var m = sender as Mercenary;
                if (m != null)
                {
                    if (_blackList.Contains(m.ID)) return;
                    Mercenaries.Add(m, new List<EventArgs>());
                    WoundedMercenaries.Add(m,0);
                    _iconsOffset = (66 * Mercenaries.Count) / 2;
                }
            }
            else if (e.GetType().Equals(typeof(DestroyEvent)))
            {
                if (sender.GetType().Equals(typeof(Mercenary)))
                {
                    var m = sender as Mercenary;
                    if (m != null)
                    {
                        _selectedMercenaries.Remove(m);
                        if (_selectedMercenaries.Count == 0)
                        {
                            SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                            _commandMode = false;
                        }
                        Mercenaries.Remove(m);
                        WoundedMercenaries.Remove(m);
                        _iconsOffset = (66 * Mercenaries.Count) / 2;
                    }
                }
                else if (sender.GetType().Equals(typeof(Inventory)))
                {
                    _inventory = null;
                }
            }
            else if (e.GetType().Equals(typeof(MercenaryHit)))
            {
                Mercenary m = sender as Mercenary;

                MercenaryHit MercenaryHit = e as MercenaryHit;

                if (WoundedMercenaries.ContainsKey(m))
                {
                    WoundedMercenaries[m] += (uint)MercenaryHit.damage;
                    TimeControlSystem.TimeControl.CreateTimer(TimeSpan.FromSeconds(0.05f), MercenaryHit.damage, delegate()
                                                                                                {
                                                                                                    if (WoundedMercenaries.ContainsKey(m)) --WoundedMercenaries[m];
                                                                                                });
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
            if (key == Keys.Tab && state.WasPressed())
            {
                if (Mercenaries.Count == 0) return;

                SendEvent(new ExSwitchEvent("UseCommands", true), Priority.Normal, LinkedCamera);
                _commandMode = true;

                if (_selectedMercenaries.Count == 0)
                {
                    Mercenaries.ElementAt(0).Key.Marker = true;
                    _selectedMercenaries.Add(Mercenaries.ElementAt(0).Key);
                }
                else
                {
                    if (Mercenaries.Count == 1) return;

                    var i = GetMercenaryIndex(_selectedMercenaries.ElementAt(0));

                    ClearSelection();

                    if (_leftControl)
                    {
                        if (i == 0) i = Mercenaries.Count - 1;
                        else --i;
                    }
                    else
                    {
                        if (i == Mercenaries.Count - 1) i = 0;
                        else ++i;
                    }

                    Mercenaries.ElementAt(i).Key.Marker = true;
                    _selectedMercenaries.Add(Mercenaries.ElementAt(i).Key);
                }
            }
            /************************************************************************/
            // Left Control
            /************************************************************************/
            else if (key == Keys.LeftControl) _leftControl = state.IsDown();
            /************************************************************************/
            // Left Alt
            /************************************************************************/
            else if (key == Keys.LeftAlt) _leftAlt = state.IsDown();
            /************************************************************************/
            // Tilde
            /************************************************************************/
            else if (key == Keys.OemTilde && state.WasPressed())
            {
                if (_selectedMercenaries.Count == 1)
                {
                    SendEvent(new MoveToObjectCommandEvent(_selectedMercenaries.ElementAt(0)), Priority.High, LinkedCamera);
                }
            }
            /************************************************************************/
            // E
            /************************************************************************/
            else if (key == Keys.E && state.WasPressed())
            {
                if (_selectedMercenaries.Count == 1)
                {
                    var data = new InventoryData
                                             {
                                                 MercenariesManager = ID,
                                                 Mercenary = _selectedMercenaries.ElementAt(0).ID
                                             };
                    if (_inventory != null) SendEvent(new DestroyObjectEvent(_inventory.ID), Priority.High, GlobalGameObjects.GameController);
                    SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                }
            }
            /************************************************************************/
            // F
            /************************************************************************/
            else if (key == Keys.F)
            {
                if (state.WasPressed())
                {
                    LinkedCamera.FireMode = true;
                    if (_selectedMercenaries.Count == 1)
                    {
                        if (_selectedMercenaries.ElementAt(0).CurrentObject as Firearm != null)
                        {
                            Firearm firearm = _selectedMercenaries.ElementAt(0).CurrentObject as Firearm;
                            if (firearm.SelectiveFire.Count > 1)
                            { 
                                FireSelectorData data = new FireSelectorData();
                                data.Firearm = firearm.ID;

                                fireSelector = (FireSelector)CreateGameObject(data);
                                LinkedCamera.StopScrolling = true;
                            }
                        }
                    }
                }
                else if (state.WasReleased())
                {
                    if (fireSelector != null)
                    {
                        SendEvent(new DestroyObjectEvent(fireSelector.ID), Priority.High, GlobalGameObjects.GameController);
                        fireSelector = null;
                        LinkedCamera.StopScrolling = false;
                    }
                    LinkedCamera.FireMode = false;
                }
            }
            /************************************************************************/
            // Q
            /************************************************************************/
            else if (key == Keys.Q)
            {
                LinkedCamera.Run = state.IsDown();
            }            
            /************************************************************************/
            // 1,2,3,4,5,6,7,8,9,0
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
                    if (mouseMovementState.Position.X > (_screenWidthOver2 - _iconsOffset) &&
                        mouseMovementState.Position.X < (_screenWidthOver2 + _iconsOffset))
                    {
                        _useGUI = true;

                        _mouseOnActions = mouseMovementState.Position.Y > 69;

                        _mouseX = (int)mouseMovementState.Position.X - (_screenWidthOver2 - _iconsOffset);

                        LinkedCamera.MouseListenerComponent.Active = false;

                        _mouse.SetCursor("Default");
                        return;
                    }
                }
            }
            _useGUI = false;
            LinkedCamera.MouseListenerComponent.Active = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {
            if (!_useGUI) return;
            if (!mouseKeyState.WasPressed()) return;
            _mouseOnMerc = _mouseX / 66;
            /*********************************/
            // Left Click
            /*********************************/
            switch (mouseKeyAction)
            {
                case MouseKeyAction.LeftClick:
                    if (_mouseOnActions)
                    {
                        var x = _mouseX % 66;
                        x /= 16;
                        if (x == 0)
                        {
                            var merc = Mercenaries.Keys.ElementAt(_mouseOnMerc);
                            if (Mercenaries[merc].Count == 1)
                            {
                                Mercenaries[merc].RemoveAt(0);
                                SendEvent(new StopActionEvent(), Priority.High, merc);
                            }
                            else if (Mercenaries[merc].Count > 1)
                            {
                                Mercenaries[merc].RemoveAt(0);
                                SendEvent(new StopActionEvent(), Priority.High, merc);
                                SendEvent(Mercenaries[merc].ElementAt(0), Priority.Normal, merc);
                            }
                        }
                        else if (x < 4)
                        {
                            var merc = Mercenaries.Keys.ElementAt(_mouseOnMerc);
                            if (Mercenaries[merc].Count > x)
                            {
                                Mercenaries[merc].RemoveAt(x);
                            }
                        }
                    }
                    else if (_leftControl)
                    {
                        if (!_selectedMercenaries.Contains(Mercenaries.ElementAt(_mouseOnMerc).Key))
                        {
                            AddToSelection(Mercenaries.ElementAt(_mouseOnMerc).Key);
                        }
                    }
                    else if (_leftAlt)
                    {
                        if (_selectedMercenaries.Contains(Mercenaries.ElementAt(_mouseOnMerc).Key))
                        {
                            RemoveFromSelection(Mercenaries.ElementAt(_mouseOnMerc).Key);
                        }
                    }
                    else
                    {
                        ClearSelection();
                        AddToSelection(Mercenaries.ElementAt(_mouseOnMerc).Key);
                    }
                    break;
                case MouseKeyAction.MiddleClick:
                    if (!_mouseOnActions)
                    {
                        SendEvent(new MoveToObjectCommandEvent(Mercenaries.ElementAt(_mouseOnMerc).Key), Priority.High, LinkedCamera);
                    }
                    break;
                case MouseKeyAction.RightClick:
                    if (!_mouseOnActions)
                    {
                        var data = new InventoryData
                                       {
                                           MercenariesManager = ID,
                                           Mercenary = Mercenaries.ElementAt(_mouseOnMerc).Key.ID
                                       };
                        if (_inventory != null) SendEvent(new DestroyObjectEvent(_inventory.ID), Priority.High, GlobalGameObjects.GameController);
                        SendEvent(new CreateObjectEvent(data), Priority.High, GlobalGameObjects.GameController);
                    }
                    break;
            }
            /*********************************/
        }

        /****************************************************************************/

        public bool isSomeoneSelected()
        {
            return _selectedMercenaries.Count > 0;
        }
        /****************************************************************************/
        /// Get Mercenary From Icon
        /****************************************************************************/
        public Mercenary GetMercenaryFromIcon(int mousex, int mousey)
        {
            if (mousey > 5 && mousey < 69)
            {
                if (mousex > (_screenWidthOver2 - _iconsOffset) &&
                    mousex < (_screenWidthOver2 + _iconsOffset))
                {
                    var merc = (mousex - (_screenWidthOver2 - _iconsOffset)) / 66;
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
            if (Mercenaries.Count <= i) return;
            if (_leftAlt)
            {
                var m = Mercenaries.ElementAt(i).Key;
                m.Marker = false;
                if (_selectedMercenaries.Contains(m)) _selectedMercenaries.Remove(m);

                if (_selectedMercenaries.Count == 0)
                {
                    SendEvent(new ExSwitchEvent("UseCommands", false), Priority.Normal, LinkedCamera);
                    _commandMode = false;
                }
            }
            else if (!_leftControl && !_leftAlt)
            {
                ClearSelection();
                AddToSelection(Mercenaries.ElementAt(i).Key);
            }
            else
            {
                AddToSelection(Mercenaries.ElementAt(i).Key);
            }
        }

        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new MercenariesManagerData();
            GetData(data);

            if (LinkedCamera != null)
            {
                data.LinkedCamera = LinkedCamera.ID;
            }

            data.FogScale = _fogOfWar.FogScale;
            data.FogSize  = _fogOfWar.FogSize;
            data.Enabled  = _fogOfWar.Enabled;
            data.SpotSize = _fogOfWar.SpotSize;

            data.IgnoreMercenaries = _blackList;

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Draw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix viewProjection, int screenWidth, int screenHeight)
        {
            var i = 0;
            _screenWidthOver2 = screenWidth / 2;
            float offset = _screenWidthOver2;
            offset -= (66 * Mercenaries.Count) / 2.0f;
            foreach (var pair in Mercenaries)
            {
                spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i), 5), _selectedMercenaries.Contains(pair.Key) ? new Rectangle(196, 0, 64, 64) : new Rectangle(0, 0, 64, 64), Color.White);
                spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i), 5), pair.Key.Icon, Color.White);
                spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i), 5), new Rectangle(64, 0, 64, 64), GetColor(pair.Key.ObjectAIController.HP, pair.Key.ObjectAIController.MaxHP));
                if (WoundedMercenaries[pair.Key] >= 0)
                {
                    float colorFactor = MathHelper.Clamp((float)WoundedMercenaries[pair.Key] / 10.0f, 0, 1);                    
                    spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i), 5), new Rectangle(128, 0, 64, 64), Color.FromNonPremultiplied(255,255,255,(int)(colorFactor*255)));                
                }

                if (pair.Key.ObjectAIController.IsBleeding)
                {
                    float timeOffset = (float)((clock.Time.TotalSeconds/5) * pair.Key.ObjectAIController.BleedingIntensity + i*0.25f)%1;                    
                    spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i) + 5, 5), new Rectangle(340 + (int)(54 * timeOffset), 0, 54, 64), GetColor(pair.Key.ObjectAIController.HP, pair.Key.ObjectAIController.MaxHP));
                }

                var j = 0;
                foreach (var e in pair.Value)
                {
                    spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i) + (16 * j), 69), new Rectangle(0, 384, 16, 16), Color.White);
                    spriteBatch.Draw(_frontEnd.Texture, new Vector2(offset + (66 * i) + (16 * j), 69), GetActionRect(e), Color.White);
                    j++;
                }

                ++i;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Color
        /****************************************************************************/
        private static Color GetColor(uint hp, uint maxHp)
        {
            float green, red;
            if (hp > maxHp / 2)
            {
                green = 1;
                red = 1.0f - (hp - (float)maxHp / 2) / ((float)maxHp / 2);
            }
            else
            {
                green = hp / ((float)maxHp / 2);
                red = 1;
            }

            return Color.FromNonPremultiplied(new Vector4(red, green, 0, 0.5f));
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetMercenaryIndex
        /****************************************************************************/
        public int GetMercenaryIndex(Mercenary mercenary)
        {
            var i = -1;
            foreach (var merc in Mercenaries.Keys)
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
            var i = GetMercenaryIndex(mercenary);

            if (i == Mercenaries.Count - 1) i = 0;
            else ++i;

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
        private void QueueEvent(EventArgs e, bool clear, params Mercenary[] mercenaries)
        {
            List<EventArgs> actions;

            foreach (var mercenary in mercenaries)
            {
                if (!Mercenaries.ContainsKey(mercenary)) continue;
                actions = Mercenaries[mercenary];

                if (actions.Count != 0 && clear)
                {
                    //SendEvent(new StopActionEvent(), Priority.High, mercenary);
                    actions.Clear();

                    actions.Add(e);
                    SendEvent(e, Priority.Normal, mercenary);
                }
                else if (actions.Count != 0)
                {
                    if (actions.Count < 4) actions.Add(e);
                }
                else
                {
                    actions.Add(e);
                    SendEvent(e, Priority.Normal, mercenary);
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            if (Mercenaries.Count == 0 && clock.Time.TotalSeconds > 5) Broadcast(new FadeInEvent(), EventsSystem.Priority.Normal);

            foreach (var merc in Mercenaries)
            { 
                _fogOfWar.DrawSpot(new Vector2(merc.Key.World.Translation.X,merc.Key.World.Translation.Z));
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// GetActionRect
        /****************************************************************************/
        private static Rectangle GetActionRect(EventArgs e)
        {
            var argsType = e.GetType();
            if (argsType.Equals(typeof(MoveToPointCommandEvent)))
            {
                return new Rectangle(16, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(GrabObjectCommandEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(OpenContainerCommandEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(ActivateObjectEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (e.GetType().Equals(typeof(OpenEvent)))
            {
                return new Rectangle(32, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(ExamineObjectCommandEvent)))
            {
                return new Rectangle(48, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(FollowObjectCommandEvent)))
            {
                return new Rectangle(64, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(ExchangeItemsCommandEvent)))
            {
                return new Rectangle(80, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(ReloadCommandEvent)))
            {
                return new Rectangle(96, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(DropItemCommandEvent)))
            {
                return new Rectangle(112, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(SwitchToWeaponCommandEvent)))
            {
                return new Rectangle(128, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(SwitchToSideArmCommandEvent)))
            {
                return new Rectangle(144, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(LookAtPointEvent)))
            {
                return new Rectangle(176, 384, 16, 16);
            }
            else if (argsType.Equals(typeof(OpenFireCommandEvent)) || argsType.Equals(typeof(OpenFireToTargetCommandEvent)))
            {
                return new Rectangle(192, 384, 16, 16);
            }
            return new Rectangle();
        }

        /****************************************************************************/

        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            _sniffer.ReleaseMe();
            _keyboard.ReleaseMe();
            _mouse.ReleaseMe();
            _frontEnd.ReleaseMe();
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
        public MercenariesManagerData()
        {
            IgnoreMercenaries = new List<int>();
        }

        [CategoryAttribute("References")]
        public int LinkedCamera { get; set; }

        [CategoryAttribute("References")]
        public List<int> IgnoreMercenaries { get; set; }

        [CategoryAttribute("Fog of War")]
        public float FogScale { get; set; }
        [CategoryAttribute("Fog of War")]
        public float SpotSize { get; set; }
        [CategoryAttribute("Fog of War")]
        public Vector2 FogSize { get; set; }
        [CategoryAttribute("Fog of War")]
        public bool Enabled { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/