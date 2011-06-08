using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.Audio.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;
using PlagueEngine.ArtificialIntelligence;
using PlagueEngine.Rendering;

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    public enum Action { IDLE, MOVE, TO_IDLE, PICK, EXAMINE, OPEN, FOLLOW, ATTACK_IDLE, ENGAGE, EXCHANGE, ATTACK, ACTIVATE };
    abstract class AbstractAIController : IAIController, IAttackable, IEventsReceiver
    {
        public static AI ai;
        
        public AbstractLivingBeing controlledObject;
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected Vector3 target;
        protected GameObjectInstance objectTarget;
        public    GameObjectInstance attackTarget { get; protected set; }

        protected Action action = Action.IDLE;
        protected Attack attack;

        public float RotationSpeed  { get; protected set; }
        public float MovingSpeed    { get; protected set; }
        public float Distance       { get; protected set; }
        public float AnglePrecision { get; protected set; }

        public float SightDistance  { get; protected set; }

        protected IEventsReceiver receiver = null;
        protected Timer cooldownTimer;

        protected Dictionary<Action, String> animationBinding;
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
        /****************************************************************************/

        protected bool isDisposed = false;
        
        public AbstractAIController(AbstractLivingBeing being, uint MaxHP, uint HP)
        {
            this.HP = HP;
            this.MaxHP = MaxHP;
            this.SightDistance = (float)100.0;
            this.attackTarget = null;
            //TODO: zrobić poprawne ustawianie ataków.
            this.attack = new Attack((float)(0.0), (float)(3.0), 10, 10, 30);
            this.controlledObject = being;
            this.cooldownTimer = new Timer(new TimeSpan(), 1, useAttack);
        }

        protected virtual void useAttack()
        {
            if (isDisposed) return;
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
            if (isDisposed) return;
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                #region MoveToPoint
                if (!controlledObject.SoundEffectComponent.IsPlaying()){
                    controlledObject.SoundEffectComponent.PlayRandomSound("OnMove");
                }
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
                    this.controlledObject.SendEvent(args, Priority.Normal, AbstractAIController.ai);
                    this.Dispose();
                }
                else
                {
                    this.HP -= (uint)evt.amount;
                    if (this.attackTarget == null)
                    {
                        this.attackTarget = evt.attacker;
                        action = PlagueEngine.ArtificialIntelligence.Controllers.Action.ENGAGE;
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
            else if (e.GetType().Equals(typeof(EnemyNoticed)))
            {
                #region Enemy noticed - attack or engage
                //TODO: czy tu aby nie cos jeszcze?
                EnemyNoticed evt = e as EnemyNoticed;
                attackTarget = evt.ClosestNoticedEnemy;
                action = Action.ENGAGE;
                #endregion
            }
            else if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                #region Stop Attacking Killed Enemy
                EnemyKilled evt = e as EnemyKilled;
                if (evt.DeadEnemy.Equals(attackTarget))
                {
                    controlledObject.Mesh.CancelAnimationsEndSubscription(animationBinding[Action.ATTACK]);
                    attackTarget = null;
                    this.action = Action.IDLE;
                    controlledObject.Controller.StopMoving();
                    controlledObject.Mesh.BlendTo(animationBinding[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(AnimationEndEvent)))
            {
                #region Attack or Chase or Idle
               if (action == Action.ATTACK_IDLE)
                {
                    if (this.attackTarget != null)
                    {
                        double currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Y),
                                                           new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Y));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            action = Action.ATTACK;
                        }
                        else
                        {
                            this.controlledObject.Mesh.CancelAnimationsEndSubscription(animationBinding[Action.ATTACK]);
                            action = Action.ENGAGE;
                        }
                    }
                    else
                    {
                        this.controlledObject.Mesh.CancelAnimationsEndSubscription(animationBinding[Action.ATTACK]);
                        action = Action.IDLE;
                    }
                }
                else
                {
                    this.controlledObject.Mesh.CancelAnimationsEndSubscription(animationBinding[Action.ATTACK]);
                }
                #endregion
            }
            /*************************************/

        }

        public virtual void Update(TimeSpan deltaTime)
        {
            if (isDisposed) return;
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
                        controlledObject.Mesh.BlendTo(animationBinding[Action.IDLE], TimeSpan.FromSeconds(0.3f));
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

                        controlledObject.Controller.MoveForward(MovingSpeed);

                        if (controlledObject.Mesh.CurrentClip != animationBinding[Action.MOVE])
                        {
                            controlledObject.Mesh.BlendTo(animationBinding[Action.MOVE], TimeSpan.FromSeconds(0.5f));
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
                    else
                    {
                        double currDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                             new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z));  
                        if (currDistance < 4)
                        {
                            #region Stop Chasing
                            controlledObject.Controller.StopMoving();
                            if (controlledObject.Mesh.CurrentClip != animationBinding[Action.IDLE])
                            {
                                controlledObject.Mesh.BlendTo(animationBinding[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                            }
                            return;
                            #endregion
                        }
                        else if (controlledObject.Mesh.CurrentClip == animationBinding[Action.IDLE] && currDistance > 8)
                        {
                            #region Resume Chase
                            Vector3 direction = controlledObject.World.Translation - objectTarget.World.Translation;
                            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                            float det = v1.X * v2.Y - v1.Y * v2.X;
                            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                            if (det < 0) angle = -angle;

                            if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                            controlledObject.Controller.MoveForward(MovingSpeed);

                            if (controlledObject.Mesh.CurrentClip != animationBinding[Action.MOVE])
                            {
                                controlledObject.Mesh.BlendTo(animationBinding[Action.MOVE], TimeSpan.FromSeconds(0.3f));
                            }
                            #endregion
                        }
                        else if (controlledObject.Mesh.CurrentClip != animationBinding[Action.IDLE])
                        {
                            Vector3 direction = controlledObject.World.Translation - objectTarget.World.Translation;
                            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            Vector2 v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                            float det = v1.X * v2.Y - v1.Y * v2.X;
                            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                            if (det < 0) angle = -angle;

                            if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                            controlledObject.Controller.MoveForward(MovingSpeed);

                            if (controlledObject.Mesh.CurrentClip != animationBinding[Action.MOVE])
                            {
                                controlledObject.Mesh.BlendTo(animationBinding[Action.MOVE], TimeSpan.FromSeconds(0.3f));
                            }
                        }
                    }
#endregion
                    return;
                case Action.ENGAGE:
                    #region Engage to Enemy
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Y),
                                                               new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Y));
                    if (currentDistance < attack.maxAttackDistance - 1)
                    {
                        action = Action.ATTACK;
                        controlledObject.Controller.StopMoving();
                    }
                    else if (currentDistance > this.SightDistance)
                    {
                        this.attackTarget = null;
                        this.action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        this.controlledObject.Mesh.BlendTo(animationBinding[Action.IDLE], TimeSpan.FromSeconds(0.3f));
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

                        controlledObject.Controller.MoveForward(MovingSpeed);

                        if (controlledObject.Mesh.CurrentClip != animationBinding[Action.MOVE])
                        {
                            controlledObject.Mesh.BlendTo(animationBinding[Action.MOVE], TimeSpan.FromSeconds(0.5f));
                        }
                    }
                    #endregion
                    return;
                case Action.ATTACK:
                    #region Attack Enemy
                    {
                        currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Y),
                                                           new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Y));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            //if (controlledObject.Mesh.CurrentClip != animationBinding[Action.ATTACK])
                            //{
                            //    controlledObject.Mesh.BlendTo(animationBinding[Action.ATTACK], TimeSpan.FromSeconds(0.5f));
                            //}
                            //else
                            //{
                                controlledObject.Mesh.StartClip(animationBinding[Action.ATTACK]);
                            //}
                            //controlledObject.Mesh.PlayClip();
                            controlledObject.Mesh.SubscribeAnimationsEnd(animationBinding[Action.ATTACK]);
                            //this.cooldownTimer.Reset(attack.cooldown, 1);                        
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
                default:
                    break;
            }
            
        }

        public bool IsDisposed()
        {
            return isDisposed;
        }

        public virtual void Dispose()
        {
            this.objectTarget = null;
            this.receiver = null;
            this.attack = null;
            this.attackTarget = null;
            this.animationBinding = null;
            this.cooldownTimer = null;
            this.objectTarget = null;
            this.controlledObject.SendEvent(new DestroyObjectEvent(this.controlledObject.ID),Priority.Normal, GlobalGameObjects.GameController);
            this.isDisposed = true;
        }
    }
}
