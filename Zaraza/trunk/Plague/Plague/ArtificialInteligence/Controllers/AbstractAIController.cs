﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.Audio.Components;
using PlagueEngine.Physics;
using PlagueEngine.ArtificialIntelligence.Controllers;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.ArtificialIntelligence;

namespace PlagueEngine.ArtificialInteligence.Controllers
{
    public enum Action { IDLE, MOVE, TO_IDLE, PICK, EXAMINE, FOLLOW, ATTACK_IDLE, ENGAGE, EXCHANGE, ATTACK };
    abstract class AbstractAIController : IAIController, IAttackable, IEventsReceiver
    {
        public static AI ai;
        
        public AbstractLivingBeing controlledObject;
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected Vector3 target;
        protected GameObjectInstance objectTarget;
        protected GameObjectInstance attackTarget;

        protected Action action = Action.IDLE;
        protected Attack attack;

        public float RotationSpeed  { get; protected set; }
        public float MovingSpeed    { get; protected set; }
        public float Distance       { get; protected set; }
        public float AnglePrecision { get; protected set; }

        protected IEventsReceiver receiver = null;
        protected Timer cooldownTimer;
        /****************************************************************************/

        /****************************************************************************/
        /// Slots
        /****************************************************************************/
        //public GameObjectInstance currentObject = null;
        //public Dictionary<IStorable, ItemPosition> Items { get; private set; }
        /****************************************************************************/

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint MaxHP              { get; protected set; }
        public uint HP                 { get; protected set; }
        public Rectangle Icon          { get; protected set; }
        public Rectangle InventoryIcon { get; protected set; }
        public uint TinySlots          { get; protected set; }
        public uint Slots              { get; protected set; }
        /****************************************************************************/

        public AbstractAIController(AbstractLivingBeing being)
        {
            this.controlledObject = being;
            PlagueEngine.TimeControlSystem.Timer.CallbackDelegate2 cd2 = new PlagueEngine.TimeControlSystem.Timer.CallbackDelegate2(useAttack);
            this.cooldownTimer = new Timer(new TimeSpan(), 1, cd2);
        }

        protected virtual void useAttack()
        {
            action = Action.ATTACK;
            //TakeDamage dmg = new TakeDamage(attack.minInflictedDamage, this.controlledObject);
            //this.controlledObject.SendEvent(dmg, Priority.Normal, this.attackTarget);
        }

        /****************************************************************************/
        /// EVENTS
        /****************************************************************************/

        /****************************************************************************/

        /****************************************************************************/
        public virtual void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                #region MoveToPoint
                controlledObject.SoundEffectComponent.PlaySound("yesSir");
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                receiver = sender as IEventsReceiver;
                target = moveToPointCommandEvent.point;
                action = Action.MOVE;
                #endregion
            }
            else if (e.GetType().Equals(typeof(TakeDamage)))
            {
                 #region Take Damage
                TakeDamage evt = e as TakeDamage;
                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(this.controlledObject);
                    this.controlledObject.SendEvent(args, Priority.Normal, this.receiver);
                }
                else
                {
                    this.HP -= (uint)evt.amount;
                    if (this.attackTarget == null)
                    {
                        this.attackTarget = evt.attacker;
                        action = PlagueEngine.ArtificialInteligence.Controllers.Action.ENGAGE;
                    }
                }
                
                #endregion
            }
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                #region FollowEvent
                FollowObjectCommandEvent FollowObjectCommandEvent = e as FollowObjectCommandEvent;

                receiver = sender as IEventsReceiver;

                if (FollowObjectCommandEvent.gameObject != this.controlledObject)
                {
                    objectTarget = FollowObjectCommandEvent.gameObject;
                    action = Action.FOLLOW;
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(StopActionEvent)))
            {
                #region StopEvent
                action = Action.IDLE;
                objectTarget = null;
                controlledObject.Controller.StopMoving();
                controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
            }
            else if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                #region Stop Attacking Killed Enemy
                EnemyKilled evt = e as EnemyKilled;
                if (evt.DeadEnemy.Equals(attackTarget))
                {
                    this.cooldownTimer.Reset(TimeSpan.Zero, 0);
                    attackTarget = null;
                    this.action = Action.IDLE;
                }
                #endregion
            }
            /*************************************/

        }

        public virtual void Update(TimeSpan deltaTime)
        {
            controlledObject.SoundEffectComponent.SetPosiotion(controlledObject.World.Translation);
            double currentDistance;
            
            switch (action)
            {
                case Action.MOVE:
                    #region MoveAction
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X,
                                                 controlledObject.World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)); 
                    if (currentDistance < Distance)
                    {
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                    }
                    else
                    {
                        Vector3 direction = controlledObject.World.Translation - target;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed * (float)deltaTime.TotalSeconds);

                        if (controlledObject.Mesh.CurrentClip != "Run")
                        {
                            controlledObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    #endregion
                    return;
                case Action.FOLLOW:
                    #region FollowAction
                    if (objectTarget.IsDisposed())
                    {
                        #region TurnIdleIfTargetDisposed
                        objectTarget = null;
                        action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        return;
                        #endregion
                    }
                    else if (Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                             new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z)) < 4)
                    {
                        controlledObject.Controller.StopMoving();
                        if (controlledObject.Mesh.CurrentClip != "Idle")
                        {
                            controlledObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        }
                        return;
                    }
                    else if (controlledObject.Mesh.CurrentClip == "Idle" && Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z), new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z)) > 8)
                    {
                        Vector3 direction = controlledObject.World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed * (float)deltaTime.TotalSeconds);

                        if (controlledObject.Mesh.CurrentClip != "Run")
                        {
                            controlledObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                        }
                    }
                    else if (controlledObject.Mesh.CurrentClip != "Idle")
                    {
                        Vector3 direction = controlledObject.World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed * (float)deltaTime.TotalSeconds);

                        if (controlledObject.Mesh.CurrentClip != "Run")
                        {
                            controlledObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                        }
                    }
#endregion
                    return;
                case Action.ENGAGE:
                    #region Engage to Enemy
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Y),
                                                               new Vector2(attackTarget.World.Forward.X, attackTarget.World.Forward.Y));
                    if (currentDistance < attack.maxAttackDistance && currentDistance > attack.minAttackDistance)
                    {
                        action = Action.ATTACK_IDLE;
                        controlledObject.SendEvent(new TakeDamage(4.5, this.controlledObject), Priority.Normal, this.attackTarget);
                        if (controlledObject.Mesh.CurrentClip != "Attack")
                        {
                            controlledObject.Mesh.BlendTo("Attack", TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    else
                    {
                        Vector3 direction = controlledObject.World.Translation - attackTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed * (float)deltaTime.TotalSeconds);

                        if (controlledObject.Mesh.CurrentClip != "Run")
                        {
                            controlledObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    #endregion
                    return;
                case Action.ATTACK:
                    #region Attack Enemy
                    {
                        currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Y),
                                                           new Vector2(attackTarget.World.Forward.X, attackTarget.World.Forward.Y));
                        if (currentDistance < attack.maxAttackDistance && currentDistance > attack.minAttackDistance)
                        {
                            this.cooldownTimer.Reset(attack.cooldown, 1);
                            controlledObject.SendEvent(new TakeDamage(attack.maxInflictedDamage, this.controlledObject), Priority.Normal, this.attackTarget);
                            action = Action.ATTACK_IDLE;
                        }
                        else
                        {
                            action = Action.ENGAGE;
                        }
                    }
                    #endregion
                    return;
                case Action.ATTACK_IDLE:
                    #region Attack Idle
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Y),
                                                       new Vector2(attackTarget.World.Forward.X, attackTarget.World.Forward.Y));
                    if (currentDistance > attack.maxAttackDistance)
                    {
                        this.cooldownTimer.Reset(TimeSpan.Zero, 0);
                        action = Action.ENGAGE;
                    }
                    #endregion
                    return;
                default:
                    break;
            }
            
        }


        public bool IsDisposed()
        {
            return false;
            //TODO: is Disposed?
        }

        public void Dispose()
        {
            this.objectTarget = null;
            this.receiver = null;
            //TODO: Dispose
            
        }
    }
}
