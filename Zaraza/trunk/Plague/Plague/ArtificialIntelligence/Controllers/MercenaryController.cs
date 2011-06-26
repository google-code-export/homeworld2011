using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.Physics;
using Microsoft.Xna.Framework;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.Rendering;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MercenaryController : AbstractAIController
    {
        static public IEventsReceiver MercManager { protected get; set; }

        public Rectangle Icon { get; protected set; }
        public Rectangle InventoryIcon { get; protected set; }
        public uint TinySlots { get; protected set; }
        public uint Slots { get; protected set; }
        long attackTimerID = -1;

        protected override Action Action
        {
            set
            {
                if (Action != Action.RELOAD)
                {
                    Mercenary merc = null;
                    switch (value)
                    {
                        case Action.FOLLOW:
                            merc = controlledObject as Mercenary;
                            //TODO: dorobić test czy nie spełnia warunku wounded.
                            if (merc.CurrentObject as Firearm != null)
                            {
                                Firearm firearm = merc.CurrentObject as Firearm;
                                if (firearm.SideArm)
                                {
                                    base.MoveAction = Action.TACTICAL_MOVE_SIDEARM;
                                }
                                else
                                {
                                    base.MoveAction = Action.TACTICAL_MOVE_CARABINE;
                                }
                            }
                            base.Action = value;
                            return;
                        case Action.MOVE:
                            merc = controlledObject as Mercenary;
                            //TODO: dorobić test czy nie spełnia warunku wounded.
                            if (merc.CurrentObject as Firearm != null)
                            {
                                Firearm firearm = merc.CurrentObject as Firearm;
                                if (firearm.SideArm)
                                {
                                    base.Action = Action.TACTICAL_MOVE_SIDEARM;
                                    base.MoveAction = Action.TACTICAL_MOVE_SIDEARM;
                                }
                                else
                                {
                                    base.Action = Action.TACTICAL_MOVE_CARABINE;
                                    base.MoveAction = Action.TACTICAL_MOVE_CARABINE;
                                }
                            }
                            else
                            {
                                base.Action = value;
                                base.MoveAction = value;
                            }
                            return;
                        default:
                            base.Action = value;
                            return;
                    }
                }
                else if (value == Action.ATTACK || value == Action.IDLE)
                {
                    base.Action = value;
                }
#if DEBUG
                else
                {
                    Diagnostics.PushLog(LoggingLevel.INFO, "State not changed because of reloading");
                }
#endif
            }
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="rotationSpeed"></param>
        /// <param name="movingSpeed"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="MaxHP"></param>
        /// <param name="HP"></param>
        /// <param name="AnimationMapping"></param>
        public MercenaryController(AbstractLivingBeing lb,
                                   float rotationSpeed,
                                   float movingSpeed,
                                   float distance,
                                   float angle,
                                   uint MaxHP,
                                   uint HP,
                                   List<AnimationBinding> AnimationMapping
            ):base(lb, MaxHP, HP, rotationSpeed, movingSpeed, distance, angle, AnimationMapping)
        {
            ai.registerController(this);
            this.attack.maxAttackDistance = 100;
            this.AnimationToActionMapping.Add(Action.RELOAD_CARABINE, "Reload_Carabine");
            this.AnimationToActionMapping.Add(Action.RELOAD_SIDEARM, "Reload_Pistol");
            this.AnimationToActionMapping.Add(Action.WOUNDED_MOVE, "Wounded");
            this.AnimationToActionMapping.Add(Action.WOUNDED_IDLE, "Wounded_Idle");
            this.AnimationToActionMapping.Add(Action.LOAD_CARTRIDGE, "Load_Ammo");
            this.AnimationToActionMapping.Add(Action.DIE, "Dying");
            //this.AnimationToActionMapping.Add(Action.RELOAD, "Reload_Pistol");

        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Update(TimeSpan deltaTime)
        {
            Mercenary merc;
            if (isDisposed) return;
            switch (Action)
            {
                case Action.EXCHANGE:
                case Action.EXAMINE:
                case Action.PICK:
                case Action.ACTIVATE:
                case Action.OPEN:
                    #region PickOrExamine
                    if (objectTarget.IsDisposed() || objectTarget.Owner != null)
                    {
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        objectTarget = null;
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                        objectTarget = null;
                        return;
                    }
                    {
                        Vector3 direction = controlledObject.World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed);
                        
                        if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[Action.MOVE])
                        {
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.MOVE], TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    #endregion
                    return;
                case Action.ATTACK:
                    #region Attack
                    merc = controlledObject as Mercenary;
                    if ((merc.CurrentObject as Firearm) != null)
                    {
                        if ((merc.CurrentObject as Firearm).SureFire(AttackTarget.World.Translation))
                        {
                            Vector3 direction = controlledObject.World.Translation - AttackTarget.World.Translation;
                            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                            float det = v1.X * v2.Y - v1.Y * v2.X;
                            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));
                            if (det < 0) angle = -angle;
                            if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle));
                            TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); });
                        }
                        else
                        {
                            #region Przeładuj
                            
                            if (merc.Reload())
                            {
                                #region Przeładuj magazynek
                                //Animacja przeładowania, subskrypcja końca animacji.
                                Action = Action.RELOAD;
                                if (attackTimerID != -1)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                    attackTimerID = -1;
                                }
                                if ((merc.CurrentObject as Firearm).SideArm)
                                {
                                    AnimationToActionMapping[Action] = AnimationToActionMapping[Action.RELOAD_SIDEARM];
                                }
                                else
                                {
                                    AnimationToActionMapping[Action] = AnimationToActionMapping[Action.RELOAD_CARABINE];
                                }

                                controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action]);
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3));
                                (merc.CurrentObject as Firearm).Freeze();
                                #endregion
                            }
                            else
                            {
                                Action = Action.RELOAD;
                                if (attackTimerID != -1)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                    attackTimerID = -1;
                                }
                                AnimationToActionMapping[Action] = AnimationToActionMapping[Action.LOAD_CARTRIDGE];

                                controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action]);
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3));
                                (merc.CurrentObject as Firearm).Freeze();
                            }
                            #endregion
                        }
                    }
                    Action = Action.ATTACK_IDLE;
                    return;
                    #endregion
                default:
                    base.Update(deltaTime);
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (isDisposed) return;
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                #region MoveToPoint
                if (!controlledObject.SoundEffectComponent.IsPlaying())
                {
                    controlledObject.SoundEffectComponent.PlayRandomSound("OnMove");
                }
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                receiver = sender as IEventsReceiver;
                target = moveToPointCommandEvent.point;
                if (controlledObject.PathfinderComponent.GetPath(controlledObject.World.Translation, moveToPointCommandEvent.point))
                {
                    target = controlledObject.PathfinderComponent.NextNode();
                }
                Diagnostics.PushLog(LoggingLevel.WARN, "Position:" + controlledObject.World.Translation.ToString());
                Diagnostics.PushLog(LoggingLevel.WARN, "Target:" + target.ToString());
                Action = Action.MOVE;
                #endregion
            }
            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                #region GrabEvent
                GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                if (moveToObjectCommandEvent.gameObject != this.controlledObject)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    Action = Action.PICK;
                    controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(ExchangeItemsCommandEvent)))
            {
                #region Exchange Items Event
                ExchangeItemsCommandEvent exchangeEvent = e as ExchangeItemsCommandEvent;
                receiver = sender as IEventsReceiver;

                if (exchangeEvent.mercenary != this.controlledObject)
                {
                    objectTarget = exchangeEvent.mercenary;
                    Action = Action.EXCHANGE;
                    controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                #region CollisionEvent
                CollisionEvent collisionEvent = e as CollisionEvent;

                if (collisionEvent.gameObject == objectTarget)
                {
                    if (Action == Action.PICK)
                    {
                        #region PICK Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        if (objectTarget.Status == GameObjectStatus.Pickable)
                        {
                             (controlledObject as Mercenary).PickItem(objectTarget as StorableObject);
                        }

                        objectTarget = null;
                        Action = Action.IDLE;
                        controlledObject.Body.Immovable = true;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        #endregion
                    }
                    else if (Action == Action.EXAMINE)
                    {
                        #region EXAMINE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        controlledObject.SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);

                        objectTarget = null;
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        #endregion
                    }
                    else if (Action == Action.EXCHANGE)
                    {
                        #region EXCHANGE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                        
                        controlledObject.SendEvent(new ExchangeItemsEvent(controlledObject as Mercenary, objectTarget as Mercenary), Priority.Normal, receiver);
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.Normal, receiver);
                        objectTarget = null;
                        
                        #endregion
                    }
                    else if (Action == Action.OPEN)
                    {
                        #region OPEN Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));

                        controlledObject.SendEvent(new OpenEvent(controlledObject as Mercenary), Priority.Normal, objectTarget);
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.Normal, receiver);
                        objectTarget = null;

                        #endregion
                    }
                    else if (Action == Action.ACTIVATE)
                    {
                        #region ACTIVATE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ObjectActivatedEvent(), Priority.Normal, objectTarget);
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.Normal, receiver);
                        objectTarget = null;

                        #endregion
                    }
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                #region ExamineEvent
                ExamineObjectCommandEvent ExamineObjectCommandEvent = e as ExamineObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                objectTarget = ExamineObjectCommandEvent.gameObject;
                Action = Action.EXAMINE;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                #endregion
            }
            else if (e.GetType().Equals(typeof(OpenContainerCommandEvent)))
            {
                #region OpenContainerCommandEvent
                OpenContainerCommandEvent OpenContainerCommandEvent = e as OpenContainerCommandEvent;

                receiver = sender as IEventsReceiver;

                objectTarget = OpenContainerCommandEvent.container;
                Action = Action.OPEN;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                #endregion
            }
            else if (e.GetType().Equals(typeof(ActivateObjectEvent)))
            {
                #region Activate Object Event
                ActivateObjectEvent activateEvent = e as ActivateObjectEvent;
                receiver = sender as IEventsReceiver;


                objectTarget = activateEvent.gameObject;
                Action = Action.ACTIVATE;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);

                #endregion
            }
            else if (e.GetType().Equals(typeof (AttackOrderEvent)))
            {
                #region Attack Order Event
                AttackOrderEvent evt = e as AttackOrderEvent;
                Action = Action.ENGAGE;
                this.AttackTarget = evt.EnemyToAttack;
                #endregion 
            }
            else if (e.GetType().Equals(typeof(FriendlyFire)))
            {
                #region Cease fire and move, idiot!
                FriendlyFire evt = e as FriendlyFire;
                controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                Action = Action.MOVE;
                AttackTarget = null;
                this.OnEvent(null, new MoveToPointCommandEvent(evt.friend.World.Translation + 2 * Vector3.Cross(controlledObject.World.Forward, Vector3.Up)));
                #endregion
            }
            else if (e.GetType().Equals(typeof(TakeDamage)))
            {
                #region MercenaryHit broadcast
                TakeDamage evt = e as TakeDamage;
                MercenaryHit newEvt = new MercenaryHit((int)evt.amount);
                this.controlledObject.Broadcast(newEvt);
                
                if (evt.causesBleeding)
                {
                    IsBleeding = true;
                }

                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(controlledObject);
                    SendEvent(args, Priority.Normal, AbstractAIController.ai);
                    //Dispose();
                    //controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.DIE], TimeSpan.FromSeconds(0.3f));
                    TimeControl.CreateTimer(TimeSpan.FromSeconds(0.6f), 0, delegate() { ai.MercernaryDied((Mercenary)this.controlledObject); });
                }
                else
                {
                    HP -= (uint)evt.amount;
                    if (evt.attacker != null && AttackTarget == null)
                    {
                        if (evt.attacker.GetType().Equals(typeof(Mercenary)))
                        {
                            //TODO: jakiś okrzyk
                            FriendlyFire newEvent = new FriendlyFire(this.controlledObject);
                            SendEvent(newEvent, Priority.Normal, evt.attacker.ObjectAIController);
                            evt.attacker = null;
                        }
                        else
                        {
                            this.OnEvent(null, new OpenFireToTargetCommandEvent(evt.attacker));
                        }
                    }
                }
                return;
                #endregion
            }
            else if (e.GetType().Equals(typeof(LookAtPointEvent)))
            {
                #region LookAtPoint
                if ((controlledObject as Mercenary).CurrentObject == null) return;
                LookAtPointEvent LookAtPointEvent = e as LookAtPointEvent;
                Action = Action.IDLE;              
                if(!((controlledObject as Mercenary).CurrentObject as Firearm).SideArm) controlledObject.mesh.BlendTo("LookAt_Carabine", TimeSpan.FromSeconds(0.5f));
                else controlledObject.mesh.BlendTo("LookAt_Pistol", TimeSpan.FromSeconds(0.5f));
                Vector3 direction = controlledObject.World.Translation - LookAtPointEvent.point;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));
                if (det < 0) angle = -angle;
                if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle));
                TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); });
                #endregion
            }
            else if (e.GetType().Equals(typeof(OpenFireCommandEvent)))
            {
                #region Prowadź ostrzał
                Mercenary merc = controlledObject as Mercenary;
                if ((merc.CurrentObject as Firearm) != null)
                {
                    OpenFireCommandEvent OpenFireCommandEvent = e as OpenFireCommandEvent;

                    if ((merc.CurrentObject as Firearm).Fire())
                    {
                        Vector3 direction = controlledObject.World.Translation - OpenFireCommandEvent.point;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));
                        if (det < 0) angle = -angle;
                        if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle));
                        TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); });
                    }
                    else merc.Reload();
                }
                controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
                #endregion
            }
            else if (e.GetType().Equals(typeof(OpenFireToTargetCommandEvent)))
            {
                #region Atakuj cel
                Mercenary merc = controlledObject as Mercenary;
                OpenFireToTargetCommandEvent OpenFireToTargetCommandEvent = null;
                

                #region Jeśli najemnik jest uzbrojony...
                if ((merc.CurrentObject as Firearm) != null)
                {
                    OpenFireToTargetCommandEvent = e as OpenFireToTargetCommandEvent;
                    //AttackTarget = OpenFireToTargetCommandEvent.target;
                    //jeśli attack taget to mob to timer ataku on
                    //stan w atak i juz

                    //( było, ale chyba zbędne. attackTimerID == -1)
                    #region ...i nie atakuje już podanego celu...
                    if (attackTimerID == -1 || OpenFireToTargetCommandEvent.target != AttackTarget)
                    {
                        #region ... strzel,
                        if ((merc.CurrentObject as Firearm).SureFire(OpenFireToTargetCommandEvent.target.World.Translation))
                        {
                            #region Namierz na cel
                            Vector3 direction = controlledObject.World.Translation - OpenFireToTargetCommandEvent.target.World.Translation;
                            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                            float det = v1.X * v2.Y - v1.Y * v2.X;
                            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));
                            if (det < 0) angle = -angle;
                            if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle));
                            TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { 
                                controlledObject.Controller.StopMoving(); 
                            });
                            #endregion
                            #region Kontynuuj ostrzał, jeśli to mob
                            if (OpenFireToTargetCommandEvent.target.GetType().Equals(typeof(Creature)))
                            {
                                #region Jeśli atakował kogoś innego, skasuj timer.
                                if (attackTimerID != -1)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint) attackTimerID);
                                }
                                #endregion
                                this.receiver = sender as IEventsReceiver;
                                #region Wyznacz cel
                                AttackTarget = OpenFireToTargetCommandEvent.target;
                                Action = Action.ATTACK_IDLE;
                                #endregion
                                #region Włacz Timer Ataku.
                                attackTimerID = TimeControlSystem.TimeControl.CreateTimer(new TimeSpan(0, 0, 1), -1, delegate()
                                {
                                    if (Action == Action.ATTACK_IDLE)
                                    {
                                        Action = Action.ATTACK;
                                    }
                                    else if (Action != Action.RELOAD)
                                    {
                                        TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                        attackTimerID = -1;
                                    }
                                });
                                #endregion
                                return;
                            }

                            #endregion
                        }
                        else
                        {
                            #region Przeładuj
                            if (merc.Reload())
                            {
                                //Animacja przeładowania, subskrypcja końca animacji.
                                Action = Action.RELOAD;
                                if (attackTimerID != -1)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                    attackTimerID = -1;
                                }
                                if ((merc.CurrentObject as Firearm).SideArm)
                                {
                                    AnimationToActionMapping[Action] = AnimationToActionMapping[Action.RELOAD_SIDEARM];
                                }
                                else
                                {
                                    AnimationToActionMapping[Action] = AnimationToActionMapping[Action.RELOAD_CARABINE];
                                }

                                controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action]);
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3));
                            }
                            else
                            {
                                Action = Action.RELOAD;
                                if (attackTimerID != -1)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                    attackTimerID = -1;
                                }
                                AnimationToActionMapping[Action] = AnimationToActionMapping[Action.LOAD_CARTRIDGE];

                                controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action]);
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3));
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
                #endregion
            }
            else if (e.GetType().Equals(typeof(AnimationEndEvent)))
            {
                #region Zakończ przeładunek
                AnimationEndEvent evt = e as AnimationEndEvent;
                if (evt.animation == AnimationToActionMapping[Action.RELOAD_SIDEARM] || evt.animation == AnimationToActionMapping[Action.RELOAD_CARABINE])
                {
                    ((controlledObject as Mercenary).CurrentObject as Firearm).Unfreeze();
                    controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.RELOAD]);
                    if (AttackTarget != null)
                    {
                        Action = Action.ATTACK;
                        #region Włacz Timer Ataku.
                        attackTimerID = TimeControlSystem.TimeControl.CreateTimer(new TimeSpan(0, 0, 1), -1, delegate()
                        {
                            if (Action == Action.ATTACK_IDLE)
                            {
                                Action = Action.ATTACK;
                            }
                            else if (Action != Action.RELOAD)
                            {
                                TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                attackTimerID = -1;
                            }
                        });
                        #endregion
                    }
                    else
                    {
                        Action = Action.IDLE;
                    }
                }
                else if (evt.animation == AnimationToActionMapping[Action.LOAD_CARTRIDGE])
                {
                    ((controlledObject as Mercenary).CurrentObject as Firearm).Unfreeze();
                    Mercenary merc = controlledObject as Mercenary;
                    Firearm firearm = merc.CurrentObject as Firearm;
                    AmmoClip clip = firearm.AmmoClip; 
                    if (clip.Content.Count < clip.Capacity )
                    {
                        //tu ladowanie po naboju.
                        //controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.LOAD_CARTRIDGE], TimeSpan.FromSeconds(0.3));
                        foreach (var item in merc.Items)
                        {
                            AmmoBox box = item.Key as AmmoBox; 
                            if (box != null && item.Value.Slot < merc.TinySlots)
                            {
                                #region mamy pudełko które jest w kieszeni
                                //zgadza sie typ amunicji z typem broni
                                if (box.AmmunitionInfo == clip.AmmunitionInfo)
                                {
                                    //ilosc w pudle wieksza niz 0
                                    if (box.Amount > 0)
                                    {
                                        //wyjmij z pudła
                                        box.Amount--;
                                        Bullet bullet = new Bullet();
                                        bullet.Version = box.AmmunitionVersionInfo.Version;
                                        bullet.PPP = box.PPP;
                                        //wetknij w magazynek
                                        clip.Content.Push(bullet);

                                        //jesli puste pudlo to je wywal
                                        if (box.Amount == 0)
                                        {
                                            //AmmoSlots[ammoSlot] = null;
                                            merc.Items.Remove(box);
                                            SendEvent(new DestroyObjectEvent(box.ID), Priority.High, GlobalGameObjects.GameController);
                                        }
                                        //animacja i jeszcze raz.
                                        controlledObject.Mesh.PlayClip(AnimationToActionMapping[Action.LOAD_CARTRIDGE]);
                                        return;
                                    }
                                }
                                #endregion
                            }
                            
                        }

                        //jeśli tu dotarło, to znaczy, że nie ma amunicji.

                        controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.LOAD_CARTRIDGE]);

                        if (clip.Content.Count > 0)
                        {
                            if (AttackTarget != null)
                            {
                                Action = Action.ATTACK;
                                #region Włacz Timer Ataku.
                                attackTimerID = TimeControlSystem.TimeControl.CreateTimer(new TimeSpan(0, 0, 1), -1, delegate()
                                {
                                    if (Action == Action.ATTACK_IDLE)
                                    {
                                        Action = Action.ATTACK;
                                    }
                                    else if (Action != Action.RELOAD)
                                    {
                                        TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                        attackTimerID = -1;
                                    }
                                });
                                #endregion
                            }
                            else
                            {
                                Action = Action.IDLE;
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3f));
                            }
                        }
                        else
                        {
                            Action = Action.IDLE;//WHAT TO DO!?!?
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3f));
                            AttackTarget = null;

                        }
                    }
                    else
                    {
                        controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.LOAD_CARTRIDGE]);
                        if (AttackTarget != null)
                        {
                            Action = Action.ATTACK;
                            #region Włacz Timer Ataku.
                            attackTimerID = TimeControlSystem.TimeControl.CreateTimer(new TimeSpan(0, 0, 1), -1, delegate()
                            {
                                if (Action == Action.ATTACK_IDLE)
                                {
                                    Action = Action.ATTACK;
                                }
                                else if (Action != Action.RELOAD)
                                {
                                    TimeControlSystem.TimeControl.ReleaseTimer((uint)attackTimerID);
                                    attackTimerID = -1;
                                }
                            });
                            #endregion
                        }
                        else
                        {
                            Action = Action.IDLE;
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.3));
                        }
                    }
                }
                else
                {
                    base.OnEvent(sender, e);
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                #region Enemy killed
                if ((e as EnemyKilled).DeadEnemy == AttackTarget)
                {
                    controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                }
                #endregion
                base.OnEvent(sender, e);
            }
            else
            {
                base.OnEvent(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            (controlledObject as Mercenary).IsDisposed = true;   
            base.Dispose();
        }
    
    }

    
}
