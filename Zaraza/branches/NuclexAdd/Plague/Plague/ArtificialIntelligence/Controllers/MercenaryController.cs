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

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MercenaryController : AbstractAIController
    {

        public Rectangle Icon { get; protected set; }
        public Rectangle InventoryIcon { get; protected set; }
        public uint TinySlots { get; protected set; }
        public uint Slots { get; protected set; }

        protected override Action Action
        {
            set
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
                        break;
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
                        break;
                    default:
                        base.Action = value;
                        break;
                }
            }
        }
     
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
        }

        Timer usefulTimer;
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
                    merc = controlledObject as Mercenary;
                    if ((merc.CurrentObject as Firearm) != null && AttackTarget != null)
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
                        else merc.Reload();
                    }
                    Action = Action.ATTACK_IDLE;
                    TimeControlSystem.TimeControl.CreateFrameCounter(60, 0, delegate() { if (Action == Action.ATTACK_IDLE) { Action = Action.ATTACK; } });
                    return;
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
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
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
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        #endregion
                    }
                    else if (Action == Action.EXCHANGE)
                    {
                        #region EXCHANGE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        
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
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));

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
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
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
                base.OnEvent(null, new MoveToPointCommandEvent(evt.friend.World.Translation + 2 * Vector3.Cross(controlledObject.World.Forward, Vector3.Up)));
                #endregion
            }
            else if (e.GetType().Equals(typeof(TakeDamage)))
            {
                #region MercenaryHit broadcast
                TakeDamage evt = e as TakeDamage;
                MercenaryHit newEvt = new MercenaryHit((int)evt.amount);
                this.controlledObject.Broadcast(newEvt);
                if (evt.attacker.GetType().Equals(typeof(Mercenary)))
                {
                    //TODO: jakiś okrzyk
                    FriendlyFire newEvent = new FriendlyFire(this.controlledObject);
                    SendEvent(newEvent, Priority.Normal, evt.attacker.ObjectAIController);
                    evt.attacker = null;

                    base.OnEvent(sender, e);
                }
                else
                {
                    base.OnEvent(sender, e);
                }
                return;
                #endregion
            }
            else if (e.GetType().Equals(typeof(LookAtPointEvent)))
            {
                #region LookAtPoint
                LookAtPointEvent LookAtPointEvent = e as LookAtPointEvent;
                Action = Action.IDLE;
                controlledObject.mesh.BlendTo("Fire_Carabine", TimeSpan.FromSeconds(0.5f));
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
            }
            else if (e.GetType().Equals(typeof(OpenFireToTargetCommandEvent)))
            {
                Mercenary merc = controlledObject as Mercenary;
                OpenFireToTargetCommandEvent OpenFireToTargetCommandEvent = null;
                bool canShoot = true;
                if ((merc.CurrentObject as Firearm) != null)
                {
                    OpenFireToTargetCommandEvent = e as OpenFireToTargetCommandEvent;
                    AttackTarget = OpenFireToTargetCommandEvent.target;
                    canShoot = (merc.CurrentObject as Firearm).SureFire(OpenFireToTargetCommandEvent.target.World.Translation);
                    if (canShoot)
                    {
                        Vector3 direction = controlledObject.World.Translation - OpenFireToTargetCommandEvent.target.World.Translation;
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
                if (canShoot && OpenFireToTargetCommandEvent != null)
                {
                    if (OpenFireToTargetCommandEvent.target.GetType().Equals(typeof(Creature)))
                    {
                        Action = Action.ATTACK_IDLE;
                        TimeControlSystem.TimeControl.CreateFrameCounter(60, 0, delegate() { if(Action == Action.ATTACK_IDLE) Action = Action.ATTACK; });
                    }
                }
                controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, sender as IEventsReceiver);
            }
            else
            {
                base.OnEvent(sender, e);
            }
        }
        public override void Dispose()
        {
            (controlledObject as Mercenary).IsDisposed = true;   
            base.Dispose();
        }
    
    }

    
}
