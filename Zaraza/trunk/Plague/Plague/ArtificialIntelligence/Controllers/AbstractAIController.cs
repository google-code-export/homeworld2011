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
    public enum Action { 
        IDLE, WOUNDED_IDLE,
        MOVE, TACTICAL_MOVE_SIDEARM, TACTICAL_MOVE_CARABINE, WOUNDED_MOVE,
        TO_IDLE,
        PICK, EXAMINE, OPEN, ACTIVATE,
        FOLLOW, EXCHANGE,
        ENGAGE, ATTACK_IDLE, ATTACK,
        SWITCH_TO_SIDEARM, SWITCH_TO_CARABINE, SWITCH,
        RELOAD, RELOAD_SIDEARM, RELOAD_CARABINE, LOAD_CARTRIDGE,
        DIE
    };
    abstract class AbstractAIController : EventsSender, IAIController, IAttackable, IEventsReceiver
    {
        /****************************************************************************/
        /// STATIC
        /****************************************************************************/
        public static AI ai;
        /****************************************************************************/

        /****************************************************************************/
        /// PROPERTIES
        /****************************************************************************/
        public GameObjectInstance AttackTarget
        {
            get
            {
                return attackTarget;
            }
            protected set
            {
                if (value == null && (Action == Action.ATTACK || Action == Action.ATTACK_IDLE))
                {
#if DEBUG
                    Diagnostics.PushLog(LoggingLevel.ERROR, this.controlledObject.ToString() +
                        " controller tried to nullify attack target while in ATTACK or ATTACK_IDLE state");
#endif
                    //TODO: zakomentować przed prezentacją.
                    throw new ArgumentException("Ustawianie NULL na celu ataku w trakcie ataku lub oczekiwania ataku");
                }
                else
                {
                    attackTarget = value;
                }

            }
        }


        /***ACTION*******************************************************************/
        protected virtual Action            Action
        {
            get
            {
                return action;
            }
            set
            {
                switch (value)
                {
                    case Action.ATTACK:
                    case Action.ATTACK_IDLE:
                        if (AttackTarget == null)
                        {
#if DEBUG
                            Diagnostics.PushLog(LoggingLevel.ERROR, this.controlledObject.ToString() +
                                " controller tried to reach illegal state. Reason: entering ATTACK while"
                                + "attackTarget was null");
#endif
                            //TODO: zakomentować przed prezentacją.
                            throw new ArgumentException("Przejście w atak/oczekiwanie ataku z celem ataku ustawionym na NULL");
                        }
                        break;
                    default:
                        break;
                }
                action = value;
            }
        }
        protected Action                    MoveAction
        {
            get
            {
                return moveAction;
            }
            set
            {
                switch (value)
                {
                    case Action.MOVE:
                    case Action.TACTICAL_MOVE_CARABINE:
                    case Action.TACTICAL_MOVE_SIDEARM:
                    case Action.WOUNDED_MOVE:
                        moveAction = value;
                        break;
                }
            }
        }
        
        /***MOVEMENT*****************************************************************/
        public float                        RotationSpeed { get; protected set; }
        public float                        MovingSpeed { get; protected set; }
        public float                        DistancePrecision { get; protected set; }
        public float                        AnglePrecision { get; protected set; }

        /***BLEEDING*****************************************************************/
        public float                        SightRange { get; protected set; }
        public float                        SightAngle { get; protected set; }
        public Dictionary<Action, String>   AnimationToActionMapping { get; protected set; }

        /***HP***********************************************************************/
        public uint                         MaxHP { get; protected set; }
        public uint                         HP { get; set; }

        /***BLEEDING*****************************************************************/
        public bool                         IsBleeding
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
                    TimeControlSystem.TimeControl.ReleaseTimer(bleedingTimerID);
                }
                else
                {
                    if (isBleeding)
                    {
                        BleedingIntensity++;

                    }
                    else
                    {
                        BleedingIntensity = 5;
                        bleedingTimerID = TimeControl.CreateTimer(new TimeSpan(0, 0, 0, 1, 750), -1, delegate() { bleed(); });

                    }
                }
                isBleeding = value;
            }
        }
        public ushort                       BleedingIntensity
        {
            get
            {
                return bleedingIntensity;
            }
            set
            {
                TimeControl.ReleaseTimer(bleedingSlowDownTimerID);
                if (value < 0)
                {
                }
                else if (value == 0 && IsBleeding)
                {
                    isBleeding = false;
                }
                else
                {
                    bleedingIntensity = value;
                    bleedingSlowDownTimerID = TimeControl.CreateTimer(
                                new TimeSpan(0,
                                             0,
                                             20 + (bleedingIntensity)),
                                -1,
                                delegate() { BleedingIntensity = (ushort)(bleedingIntensity / 2); });
                }
            }
        }

        /***BLINDING*****************************************************************/
        public bool IsBlinded { get; set; }
        public bool IsBlind { get; protected set; }
        /****************************************************************************/
        
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

        /// <summary>
        /// Default constructor; sets up default values for some fields
        /// </summary>
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
            
            #region CREATE Actions FROM STRINGS
            
            this.AnimationToActionMapping = new Dictionary<Action, string>();
            if(AnimationMapping != null){
                foreach (AnimationBinding pair in AnimationMapping)
                {
                    try
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
                            case "WOUNDED_IDLE":
                                this.AnimationToActionMapping.Add(Action.WOUNDED_IDLE, pair.Animation);
                                break;
                            case "MOVE":
                                this.AnimationToActionMapping.Add(Action.MOVE, pair.Animation);
                                break;
                            case "WOUNDED_MOVE":
                                this.AnimationToActionMapping.Add(Action.WOUNDED_MOVE, pair.Animation);
                                break;
                            case "TACTICAL_MOVE_SIDEARM":
                                this.AnimationToActionMapping.Add(Action.TACTICAL_MOVE_SIDEARM, pair.Animation);
                                break;
                            case "TACTICAL_MOVE_CARABINE":
                                this.AnimationToActionMapping.Add(Action.TACTICAL_MOVE_CARABINE, pair.Animation);
                                break;
                            case "SWITCH_TO_PISTOL":
                                //this.AnimationToActionMapping.Add(Action.OPEN, pair.Animation);
                                break;
                            case "SWITCH_TO_SIDEARM":
                                //this.AnimationToActionMapping.Add(Action.OPEN, pair.Animation);
                                break;
                            case "RELOAD_SIDEARM":
                                this.AnimationToActionMapping.Add(Action.RELOAD_SIDEARM, pair.Animation);
                                break;
                            case "RELOAD_CARABINE":
                                this.AnimationToActionMapping.Add(Action.RELOAD_CARABINE, pair.Animation);
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
                                //this.AnimationToActionMapping.Add(Action.IDLE, pair.Animation);
#if DEBUG
                                Diagnostics.PushLog(LoggingLevel.ERROR, "Unknown action: " + pair.Action + " encountered during binding animation " + pair.Animation);
#endif
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
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
            //TODO: zrobić poprawne ustawianie ataków. in progress
            this.attack = new Attack((float)(0.0), (float)(4.0), 1, 1, 30);
            this.controlledObject = being;

        }

        public void bleed()
        {
            if (IsDisposed()) return;
            if (IsBleeding)
            {
                //TODO: tu jakiś particle effect
                //TODO tu jakiś dźwięk
                if (HP < BleedingIntensity)
                {
                    OnEvent(null, new TakeDamage(HP, null, true, false));
                }
                else
                {
                    HP -= BleedingIntensity;
                }
            }
        }

        /// <summary>
        /// Processes Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (isDisposed) return;
            if (e.GetType().Equals(typeof(TakeDamage)))
            {
                 #region Take Damage
                TakeDamage evt = e as TakeDamage;

                if (evt.causesBleeding)
                {
                    IsBleeding = true;
                }                

                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(controlledObject);
                    SendEvent(args, Priority.Normal, AbstractAIController.ai);
                    Dispose();
                }
                else
                {
                    HP -= (uint)evt.amount;
                    if (evt.attacker !=null && AttackTarget == null)
                    {
                        AttackTarget = evt.attacker;
                        Action = PlagueEngine.ArtificialIntelligence.Controllers.Action.ENGAGE;
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
                    Action = Action.FOLLOW;
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(StopActionEvent)))
            {
                #region StopEvent
                Action = Action.IDLE;
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
                AttackTarget = evt.ClosestNoticedEnemy;
                Action = Action.ENGAGE;
                #endregion
            }
            else if (e.GetType().Equals(typeof(EnemyKilled)))
            {
                #region Stop Attacking Killed Enemy
                EnemyKilled evt = e as EnemyKilled;
                if (evt.DeadEnemy.Equals(AttackTarget))
                {
                    controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                    Action = Action.IDLE;
                    AttackTarget = null;
                    controlledObject.Controller.StopMoving();
                    controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                }
                #endregion
            }
            else if (e.GetType().Equals(typeof(AnimationEndEvent)))
            {
                #region Attack or Chase or Idle
               if (Action == Action.ATTACK_IDLE)
                {
                    if (AttackTarget != null)
                    {
                        double currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                           new Vector2(AttackTarget.World.Translation.X, AttackTarget.World.Translation.Z));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            Action = Action.ATTACK;
                        }
                        else
                        {
                            controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                            Action = Action.ENGAGE;
                        }
                    }
                    else
                    {
                        controlledObject.Mesh.CancelAnimationsEndSubscription(AnimationToActionMapping[Action.ATTACK]);
                        Action = Action.IDLE;
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

        

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(TimeSpan deltaTime)
        {
            if (isDisposed) return;

            #region LOCALS
            Vector3 direction;
            Vector2 v1;
            Vector2 v2;
            float det;
            float angle;
            double currentDistance;
            #endregion

            controlledObject.SoundEffectComponent.SetPosition(controlledObject.World.Translation);
            
            switch (Action)
            {
                case Action.MOVE:
                case Action.WOUNDED_MOVE:
                case Action.TACTICAL_MOVE_CARABINE:
                case Action.TACTICAL_MOVE_SIDEARM:
                    #region MoveAction
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X,
                                                 controlledObject.World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)); 
                    if (currentDistance < DistancePrecision)
                    {
                        if (controlledObject.PathfinderComponent.isEmpty)
                        {
                            Action = Action.IDLE;
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

                        if (controlledObject.Mesh.CurrentClip != this.AnimationToActionMapping[Action])
                        {
                            controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action], TimeSpan.FromSeconds(0.5f));
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
                        Action = Action.IDLE;
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

                            if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[MoveAction])
                            {
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[MoveAction], TimeSpan.FromSeconds(0.3f));
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

                            if (controlledObject.Mesh.CurrentClip != AnimationToActionMapping[MoveAction])
                            {
                                controlledObject.Mesh.BlendTo(AnimationToActionMapping[MoveAction], TimeSpan.FromSeconds(0.3f));
                            }
                        }
                    }
                    #endregion
                    return;
                case Action.ENGAGE:
                    #region Engage to Enemy
                    if (AttackTarget == null) break;
                    currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                               new Vector2(AttackTarget.World.Translation.X, AttackTarget.World.Translation.Z));
                    if (currentDistance < attack.maxAttackDistance)
                    {
                        Action = Action.ATTACK;
                        controlledObject.Controller.StopMoving();
                    }
                    else if (currentDistance > this.SightRange)
                    {
                        this.AttackTarget = null;
                        this.Action = Action.IDLE;
                        controlledObject.Controller.StopMoving();
                        this.controlledObject.Mesh.BlendTo(AnimationToActionMapping[Action.IDLE], TimeSpan.FromSeconds(0.3f));
                    }
                    else
                    {
                        direction = controlledObject.World.Translation - AttackTarget.World.Translation;
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
                        direction = controlledObject.World.Translation - AttackTarget.World.Translation;
                        v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                        
                        det = v1.X * v2.Y - v1.Y * v2.X;
                        
                        angle = (float)Math.Acos((double)Vector2.Dot(v1, v2)); 
                        
                        if (det < 0) angle = -angle; 
                        
                        if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle)); 
                        
                        TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); }); 
                        

                        currentDistance = Vector2.Distance(new Vector2(controlledObject.World.Translation.X, controlledObject.World.Translation.Z),
                                                           new Vector2(AttackTarget.World.Translation.X, AttackTarget.World.Translation.Z));
                        if (currentDistance < attack.maxAttackDistance)
                        {
                            controlledObject.Mesh.StartClip(AnimationToActionMapping[Action.ATTACK]);
                            controlledObject.Mesh.SubscribeAnimationsEnd(AnimationToActionMapping[Action.ATTACK]);
                            controlledObject.SendEvent(new TakeDamage(attack.maxInflictedDamage, this.controlledObject, true, false), Priority.Normal, this.AttackTarget);
                            Action = Action.ATTACK_IDLE;
                        }
                        else
                        {
                            Action = Action.ENGAGE;
                        }
                    }
                    #endregion
                    return;
                case Action.ATTACK_IDLE:
                    #region Attack Idle - rotate towards enemy
                    direction = controlledObject.World.Translation - AttackTarget.World.Translation;
                    v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    v2 = Vector2.Normalize(new Vector2(controlledObject.World.Forward.X, controlledObject.World.Forward.Z));
                        
                    det = v1.X * v2.Y - v1.Y * v2.X;
                        
                    angle = (float)Math.Acos((double)Vector2.Dot(v1, v2)); 
                        
                    if (det < 0) angle = -angle; 
                        
                    if (Math.Abs(angle) > 0.01f) controlledObject.Controller.Rotate(MathHelper.ToDegrees(angle)); 
                        
                    TimeControlSystem.TimeControl.CreateFrameCounter(1, 0, delegate() { controlledObject.Controller.StopMoving(); }); 
                    #endregion
                    return;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// Informs if the object was disposed
        /// </summary>
        /// <returns></returns>
        public bool IsDisposed()
        {
            return isDisposed;
        }

        /// <summary>
        /// Prepares Object to be removed
        /// </summary>
        public virtual void Dispose()
        {
            this.objectTarget = null;
            this.receiver = null;
            this.attack = null;
            this.Action = Action.IDLE;
            this.AttackTarget = null;
            this.AnimationToActionMapping = null;
            this.objectTarget = null;
            this.controlledObject.SendEvent
                (
                new DestroyObjectEvent(this.controlledObject.ID),
                Priority.Normal, 
                GlobalGameObjects.GameController
                );
            this.isDisposed = true;
        }


        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        protected Vector3 target;
        protected GameObjectInstance objectTarget;
        private GameObjectInstance attackTarget;
        private Action action = Action.IDLE;
        private Action moveAction = Action.MOVE;
        protected Attack attack;
        public AbstractLivingBeing controlledObject;
        protected IEventsReceiver receiver = null;
        protected bool isDisposed = false;
        protected bool isBleeding;
        protected uint bleedingTimerID;
        protected uint bleedingSlowDownTimerID;
        private ushort bleedingIntensity;
        /****************************************************************************/

    }
    /********************************************************************************/
}
