using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;


namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    abstract class Controller
    {
        static public AI ai { get; set; }
        public LivingBeing GameObject { get; protected set; }
        protected GameObjectInstance objectTarget;
        protected TacticalAction action;
        protected Vector3 target;
        
        protected float rotationSpeed = 0;
        protected float movingSpeed = 0;
        protected float distance = 0;
        protected float anglePrecision = 0;

        protected List<Attack> availableAttacks;
        protected Attack currentAttack;

        protected Timer cooldownTimer;
        protected LivingBeing attackTarget;

        public Controller(LivingBeing lb)
        {
            this.GameObject = lb;
            Controller.ai.addController(this);
            PlagueEngine.TimeControlSystem.Timer.CallbackDelegate2 deleg = new PlagueEngine.TimeControlSystem.Timer.CallbackDelegate2(this.Attack);
            this.cooldownTimer = new Timer(new TimeSpan(), 0, deleg);
        }


        /****************************************************************************/
        /// VIRTUAL
        /****************************************************************************/
        public virtual bool OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                moveEvent(e);
                return true;
            }
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                followEvent(e);
                return true;
            }
            return false;
        }

        public virtual void Update(TimeSpan deltaTime)
        {
            switch (action)
            {
                case TacticalAction.MOVE:
                    move(deltaTime);
                    break;
                case TacticalAction.FOLLLOW:
                    follow(deltaTime);
                    break;
                case TacticalAction.IDLE:
                    break;
                case TacticalAction.STOP_TO_IDLE:
                default:
                    toIdle();
                    break;
            }
        }

        /****************************************************************************/
        /// EVENTS HANDLING
        /****************************************************************************/        
        private void moveEvent(EventArgs e)
        {
            MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

            target = moveToPointCommandEvent.point;
            action = TacticalAction.MOVE;
        }

        private void followEvent(EventArgs e)
        {
            FollowObjectCommandEvent FollowObjectCommandEvent = e as FollowObjectCommandEvent;

            if (FollowObjectCommandEvent.gameObject != this.GameObject)
            {
                objectTarget = FollowObjectCommandEvent.gameObject;
                action = TacticalAction.FOLLLOW;
            }
        }

        protected void takeDamageEvent(EventArgs e)
        {
            TakeDamage evtArg = e as TakeDamage;
            if (this.GameObject.HP < evtArg.amount)
            {
                this.GameObject.HP = 0;
                this.GameObject.SendEvent(new EnemyKilled(this.GameObject), Priority.Normal, ai);
            }
            else
            {
                this.GameObject.HP -= (uint)evtArg.amount;
            }

        }
        /****************************************************************************/
        /// ACTIONS HANDLING
        /****************************************************************************/
        protected void move(TimeSpan deltaTime)
        {
            if (Vector2.Distance(new Vector2(GameObject.World.Translation.X,
                                                 GameObject.World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
            {
                action = 0;
                GameObject.Controller.StopMoving();
                GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
            }
            else
            {
                Vector3 direction = GameObject.World.Translation - target;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(GameObject.World.Forward.X, GameObject.World.Forward.Z));

                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                if (det < 0)
                {
                    angle = -angle;
                }

                if (Math.Abs(angle) > anglePrecision)
                {
                    GameObject.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                }

                GameObject.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                #region Blend to Run
                if (GameObject.Mesh.CurrentClip != "Run")
                {
                    GameObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                }
                #endregion
            }
        }

        protected void follow(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed())
            {
                #region Turn to Idle
                objectTarget = null;
                action = 0;
                GameObject.Controller.StopMoving();
                GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }
            else
            {
                double currentDistance = Vector2.Distance(new Vector2(GameObject.World.Translation.X, GameObject.World.Translation.Z),
                                     new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z));
                if (GameObject.Mesh.CurrentClip == "Idle" && currentDistance > 8)
                {
                    #region Resume Chase
                    Vector3 direction = GameObject.World.Translation - objectTarget.World.Translation;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(GameObject.World.Forward.X, GameObject.World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0)
                    {
                        angle = -angle;
                    }

                    if (Math.Abs(angle) > anglePrecision)
                    {
                        GameObject.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                    }

                    GameObject.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                    GameObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));

                    #endregion
                }
                else if (GameObject.Mesh.CurrentClip != "Idle")
                {
                    if (currentDistance < 4)
                    {
                        #region Pause Chase
                        GameObject.Controller.StopMoving();
                        GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        #endregion
                        return;
                    }
                    else
                    {
                        #region Continue Running
                        Vector3 direction = GameObject.World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(GameObject.World.Forward.X, GameObject.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0)
                        {
                            angle = -angle;
                        }

                        if (Math.Abs(angle) > anglePrecision)
                        {
                            GameObject.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                        }

                        GameObject.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                        if (GameObject.Mesh.CurrentClip != "Run")
                        {
                            GameObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                        }
                        #endregion
                    }
                }
            }
        }

        private void toIdle()
        {
            objectTarget = null;
            action = TacticalAction.IDLE;
            GameObject.Controller.StopMoving();
            GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
        }

        protected void Engage()
        {
            if (attackTarget != null)
            {
                double currentDistance = Vector2.Distance(new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Z),
                                     new Vector2(attackTarget.World.Translation.X, attackTarget.World.Translation.Z));

                if (currentDistance > currentAttack.minAttackDistance)
                {
                    target = attackTarget.World.Translation;
                }
                else
                {
                    Attack();
                }
            }
        }

        protected abstract void Attack();

        protected void AttackIdle()
        {
            //DO NOTHING?
        }
    }
}
