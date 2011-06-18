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

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MercenaryController : AbstractAIController
    {

        public Rectangle Icon { get; protected set; }
        public Rectangle InventoryIcon { get; protected set; }
        public uint TinySlots { get; protected set; }
        public uint Slots { get; protected set; }
        

        public MercenaryController(AbstractLivingBeing lb,
                                   float rotationSpeed,
                                   float movingSpeed,
                                   float distance,
                                   float angle,
                                   uint MaxHP,
                                   uint HP,
                                   Dictionary<Action,String> animationMapping
            ):base(lb, MaxHP, HP, rotationSpeed, movingSpeed, distance, angle, animationMapping)
        {
            ai.registerController(this);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Update(TimeSpan deltaTime)
        {
            if (isDisposed) return;
            switch (action)
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
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo(AnimationBinding[Action.IDLE], TimeSpan.FromSeconds(0.3f));
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
                        
                        if (controlledObject.Mesh.CurrentClip != AnimationBinding[Action.MOVE])
                        {
                            controlledObject.Mesh.BlendTo(AnimationBinding[Action.MOVE], TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    #endregion
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
            if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                #region GrabEvent
                GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                if (moveToObjectCommandEvent.gameObject != this.controlledObject)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    action = Action.PICK;
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
                    action = Action.EXCHANGE;
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
                    if (action == Action.PICK)
                    {
                        #region PICK Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        if (objectTarget.Status == GameObjectStatus.Pickable)
                        {
                             (controlledObject as Mercenary).PickItem(objectTarget as StorableObject);
                        }

                        objectTarget = null;
                        action = Action.IDLE;
                        controlledObject.Body.Immovable = true;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        #endregion
                    }
                    else if (action == Action.EXAMINE)
                    {
                        #region EXAMINE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);

                        controlledObject.SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);

                        objectTarget = null;
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        #endregion
                    }
                    else if (action == Action.EXCHANGE)
                    {
                        #region EXCHANGE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        
                        controlledObject.SendEvent(new ExchangeItemsEvent(controlledObject as Mercenary, objectTarget as Mercenary), Priority.Normal, receiver);
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.Normal, receiver);
                        objectTarget = null;
                        
                        #endregion
                    }
                    else if (action == Action.OPEN)
                    {
                        #region OPEN Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));

                        controlledObject.SendEvent(new OpenEvent(controlledObject as Mercenary), Priority.Normal, objectTarget);
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.Normal, receiver);
                        objectTarget = null;

                        #endregion
                    }
                    else if (action == Action.ACTIVATE)
                    {
                        #region ACTIVATE Action
                        controlledObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                        action = Action.IDLE;
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
                action = Action.EXAMINE;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                #endregion
            }
            else if (e.GetType().Equals(typeof(OpenContainerCommandEvent)))
            {
                #region OpenContainerCommandEvent
                OpenContainerCommandEvent OpenContainerCommandEvent = e as OpenContainerCommandEvent;

                receiver = sender as IEventsReceiver;

                objectTarget = OpenContainerCommandEvent.container;
                action = Action.OPEN;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);
                #endregion
            }
            else if (e.GetType().Equals(typeof(ActivateObjectEvent)))
            {
                #region Activate Object Event
                ActivateObjectEvent activateEvent = e as ActivateObjectEvent;
                receiver = sender as IEventsReceiver;


                objectTarget = activateEvent.gameObject;
                action = Action.ACTIVATE;
                controlledObject.Body.SubscribeCollisionEvent(objectTarget.ID);

                #endregion
            }
            else if (e.GetType().Equals(typeof (AttackOrderEvent)))
            {
                #region Attack Order Event
                AttackOrderEvent evt = e as AttackOrderEvent;
                action = Action.ENGAGE;
                this.attackTarget = evt.EnemyToAttack;
                #endregion 
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
