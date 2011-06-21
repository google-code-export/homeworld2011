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
    abstract class AbstractAIController : EventsSender, IAIController, IAttackable, IEventsReceiver
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

        public float RotationSpeed     { get; protected set; }
        public float MovingSpeed       { get; protected set; }
        public float DistancePrecision { get; protected set; }
        public float AnglePrecision    { get; protected set; }

        public float SightRange        { get; protected set; }
        public float SightAngle        { get; protected set; }

        protected IEventsReceiver receiver = null;
        
        public Dictionary<Action, String> AnimationToActionMapping{ get; protected set; }
        /****************************************************************************/

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public uint MaxHP              { get; protected set; }
        public uint HP                 { get; protected set; }
        /****************************************************************************/

        protected bool isDisposed = false;
        
        /// <summary>
        /// Bleeding flag
        /// </summary>
        protected bool isBleeding;
        public bool IsBleeding
        {
            get
            {
                return isBleeding; 
            }
            set
            {
                if (!value)
                {
                    BleedingIntensity = 0;
                }
                else
                {
                    BleedingIntensity++;
                }
                isBleeding = value;
            }
        }
        protected ushort BleedingIntensity = 0;
        public bool IsBlinded  { get; set; }
        public bool IsBlind    { get; protected set; }
        
        /// <summary>
        /// Short Constructor, setting up current HP as MaxHP and using default precision values.
        /// </summary>
        /// <param name="being">Game Object to be controlled</param>
        /// <param name="MaxHP">Maximal HP of controlled unit, will be set as actual HP on start</param>
        /// <param name="RotationSpeed">Speed of turning around, rotation computed with precision of 0.1f</param>
        /// <param name="MovingSpeed">Speed of moving, distance calculated with precision of 1.0f</param>
        /// <param name="AnimationMapping">Dictionary mapping animation names (strings) on Actions.</param>
        public AbstractAIController(AbstractLivingBeing being,
                                    uint MaxHP,
                                    float RotationSpeed,
                                    float MovingSpeed,
                                    List<AnimationBinding> AnimationMapping
                                    )
            :this(being,
                  MaxHP,
                  MaxHP,
                  RotationSpeed,
                  MovingSpeed,
                  1.0f,
                  0.1f,
                  AnimationMapping)
        {
        }

        protected AbstractAIController()
        {
            this.IsBleeding = false;
            this.IsBlind    = false;
            this.IsBlinded  = false;
            this.SightAngle = 20.0f;
            this.SightRange = 80.0f;
        }

        /// <summary>
        /// Full Constructor setting up all possible parameters
        /// </summary>
        /// <param name="being">Game Object to be controlled</param>
        /// <param name="MaxHP">Maximal HP of controlled unit</param>
        /// <param name="HP">Actual (start) HP of controlled unit</param>
        /// <param name="RotationSpeed">Speed of turning around</param>
        /// <param name="MovingSpeed">Speed of moving</param>
        /// <param name="DistancePrecision">Precision used in distance calculations</param>
        /// <param name="AnglePrecision">Precision used in angle calculationa</param>
        /// <param name="AnimationMapping">Dictionary mapping animation names (strings) on Actions.</param>
        public AbstractAIController(AbstractLivingBeing being,                  
                                    uint MaxHP,
                                    uint HP,
                                    float RotationSpeed,
                                    float MovingSpeed,
                                    float DistancePrecision,
                                    float AnglePrecision,
                                    List<AnimationBinding> AnimationMapping
                                    ):this()
        {
            
            #region CREATE ACTIONS FROM STRINGS
            
            this.AnimationToActionMapping = new Dictionary<Action, string>();
            if(AnimationMapping != null){
                foreach (AnimationBinding pair in AnimationMapping)
                {
                    switch (pair.Action)
                    {
                        case "ACTIVATE":
                            this.AnimationToActionMapping.Add(Action.ACTIVATE, pair.Animation);
                            break;
                        case "ATTACK":
                            this.AnimationToActionMapping.Add(Action.ATTACK, pair.Animation);
                            break;
                        case "ATTACK_IDLE":
                            this.AnimationToActionMapping.Add(Action.ATTACK_IDLE, pair.Animation);
                            break;
                        case "ENGAGE":
                            this.AnimationToActionMapping.Add(Action.ENGAGE, pair.Animation);
                            break;
                        case "EXAMINE":
                            this.AnimationToActionMapping.Add(Action.EXAMINE, pair.Animation);
                            break;
                        case "EXCHANGE":
                            this.AnimationToActionMapping.Add(Action.EXCHANGE, pair.Animation);
                            break;
                        case "FOLLOW":
                            this.AnimationToActionMapping.Add(Action.FOLLOW, pair.Animation);
                            break;
                        case "IDLE":
                            this.AnimationToActionMapping.Add(Action.IDLE, pair.Animation);
                            break;
                        case "MOVE":
                            this.AnimationToActionMapping.Add(Action.MOVE, pair.Animation);
                            break;
                        case "OPEN":
                            this.AnimationToActionMapping.Add(Action.OPEN, pair.Animation);
                            break;
                        case "PICK":
                            this.AnimationToActionMapping.Add(Action.PICK, pair.Animation);
                            break;
                        case "TO_IDLE":
                            this.AnimationToActionMapping.Add(Action.TO_IDLE, pair.Animation);
                            break;
                        default:
                            this.AnimationToActionMapping.Add(Action.IDLE, pair.Animation);
                            break;
                    }
                }

            }
            #endregion
            this.HP = HP;
            this.MaxHP = MaxHP;

            this.AnglePrecision = AnglePrecision;
            this.DistancePrecision = DistancePrecision;
            this.MovingSpeed = MovingSpeed;
            this.RotationSpeed = RotationSpeed;
            
            this.SightRange = (float)100.0;
            //TODO: zrobić poprawne ustawianie ataków.
            this.attack = new Attack((float)(0.0), (float)(4.0), 1, 1, 30);
            this.controlledObject = being;

        }

        /*protected virtual void useAttack()
        {
            if (isDisposed) return;
            action = Action.ATTACK;
            //TakeDamage dmg = new TakeDamage(attack.minInflictedDamage, this.controlledObject);
            //this.controlledObject.SendEvent(dmg, Priority.Normal, this.attackTarget);
        }*/

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
                if (controlledObject.PathfinderComponent.GetPath(controlledObject.World.Translation, moveToPointCommandEvent.point))
                {
                    target = controlledObject.PathfinderComponent.NextNode();
                }
                Diagnostics.PushLog(LoggingLevel.WARN, "Position:" + controlledObject.World.Translation.ToString());
                Diagnostics.PushLog(LoggingLevel.WARN, "Target:" + target.ToString());
                action = Action.MOVE;
                #endregion
            }
            else if (e.GetType().Equals(typeof(TakeDamage)))
            {
                 #region Take Damage
                TakeDamage evt = e as TakeDamage;
                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(controlledObject);
                    SendEvent(args, Priority.Normal, AbstractAIController.ai);
                    Dispose();
                }
                else
                {
                    HP -= (uint)evt.amount;
                    if (attackTarget == null)
                    {
                        attackTarget = evt.attacker;
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
                    controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                    attackTarget = null;
                    action = Action.IDLE;
                    controlledObject.Controller.StopMoving();
                    controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(AnimationEndEvent)))
            {
               
                #region Attack or Chase or Idle
               if (action == Action.ATTACK_IDLE)
                {
                    if (attackTarget != null)
                    {
                        double currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                           new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Z));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            action = Action.ATTACK;
                        }
                        else
                        {
                            controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                            action = Action.ENGAGE;
                        }
                    }
                    else
                    {
                        controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                        action = Action.IDLE;
                    }
                }
                else
                {
                    controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                }
                #endregion
            }
            /*************************************/

        }

        public virtual void Update(TimeSpan deltaTime)
        {
            Vector3 direction;
            Vector2 v1;
            Vector2 v2;

            float det;
            float angle;
                        
            //Diagnostics.PushLog("AKCJA: " + action.ToString());
            if (isDisposed) return;

            if (IsBleeding)
            {
                if (HP < BleedingIntensity)
                {
                    EnemyKilled evt = new EnemyKilled(controlledObject);
                    SendEvent(evt, Priority.Normal, AbstractAIController.ai);
                    Dispose();
                }
                else
                {
                    HP -= BleedingIntensity;
                }
            }

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
                    if (currentDistance < DistancePrecision)
                    {
                        if (controlledObject.PathfinderComponent.isEmpty)
                        {
                            action = Action.IDLE;
                            controlledObject.Controller.StopMoving();
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                            controlledObject.SendEvent(new ActionDoneEvent(), Priority.High, receiver);
                        }
                        target = controlledObject.PathfinderComponent.NextNode();
                        
                    }
                    else
                    {
                        direction = controlledObject.World.Translation - target;
                        v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        det = v1.X * v2.Y - v1.Y * v2.X;
                        angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0) angle = -angle;

                        if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                        controlledObject.Controller.MoveForward(MovingSpeed);

                        if (controlledObject.Mesh.CurrentClip != this.AnimationToActionMapping[Action.MOVE])
                        {
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.MOVE], TimeSpan.FromSeconds(0.5f));
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
                            if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[Action.IDLE])
                            {
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                            }
                            return;
                            #endregion
                        }
                        else if (controlledObject.Mesh.CurrentClip == AnimationToActionMapping[Action.IDLE] && currDistance > 8)
                        {
                            #region Resume Chase
                            direction = controlledObject.World.Translation - objectTarget.World.Translation;
                            v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                            det = v1.X * v2.Y - v1.Y * v2.X;
                            angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                            if (det < 0) angle = -angle;

                            if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                            controlledObject.Controller.MoveForward(MovingSpeed);

                            if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[Action.MOVE])
                            {
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.MOVE], TimeSpan.FromSeconds(0.3f));
                            }
                            #endregion
                        }
                        else if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[Action.IDLE])
                        {
                            direction = controlledObject.World.Translation - objectTarget.World.Translation;
                            v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                            v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                            det = v1.X * v2.Y - v1.Y * v2.X;
                            angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                            if (det < 0) angle = -angle;

                            if (Math.Abs(angle) > AnglePrecision) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle) * RotationSpeed * (float)deltaTime.TotalSeconds);

                            controlledObject.Controller.MoveForward(MovingSpeed);

                            if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[Action.MOVE])
                            {
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.MOVE], TimeSpan.FromSeconds(0.3f));
                            }
                        }
                    }
                    #endregion
                    return;
                case Action.ENGAGE:
                    #region Engage to Enemy
                    if (attackTarget == null) break;
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                               new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Z));
                    if (currentDistance < attack.maxAttackDistance)
                    {
                        action = Action.ATTACK;
                        controlledObject.Controller.StopMoving();
                    }
                    else if (currentDistance > this.SightRange)
                    {
                        this.attackTarget = null;
                        this.action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        this.controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                    }
                    else
                    {
                        direction = controlledObject.World.Translation - attackTarget.World.Translation;
                        v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));

                        det = v1.X * v2.Y - v1.Y * v2.X;
                        angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

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
                    #region Attack Enemy
                    {
                        direction = controlledObject.World.Translation - attackTarget.World.Translation;
                        v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                        
                        det = v1.X * v2.Y - v1.Y * v2.X;
                        
                        angle = (float)Math.Acos((double)Vector2.Dot(v1, v2)); 
                        
                        if (det < 0) angle = -angle; 
                        
                        if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle)); 
                        
                        TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); }); 
                        

                        currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                           new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Z));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            controlledObject.Mesh.StartClip(AnimationToActionMapping[Action.ATTACK]);
                            controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action.ATTACK]);
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
                    direction = controlledObject.World.Translation - attackTarget.World.Translation;
                    v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                        
                    det = v1.X * v2.Y - v1.Y * v2.X;
                        
                    angle = (float)Math.Acos((double)Vector2.Dot(v1, v2)); 
                        
                    if (det < 0) angle = -angle; 
                        
                    if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle)); 
                        
                    TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); }); 
                        
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
            this.AnimationToActionMapping = null;
            this.objectTarget = null;
            this.controlledObject.SendEvent(new DestroyObjectEvent(this.controlledObject.ID),Priority.Normal, GlobalGameObjects.GameController);
            this.isDisposed = true;
        }
    }
}
