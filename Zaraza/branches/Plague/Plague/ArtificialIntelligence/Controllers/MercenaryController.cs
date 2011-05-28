using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow.GameObjects;


namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MercenaryController : Controller
    {


        private GameObjectInstance currentObject;

        //TODO: change temporary constructor
        public MercenaryController(LivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle):base(person)
        {
            this.GameObject = person;
            this.rotationSpeed = rotationSpeed;
            this.movingSpeed = movingSpeed;
            this.distance = distance;
            this.anglePrecision = angle;
        }

        private Firearm currentWeapon;
        private int ammoLeft;

        override public bool OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {
            if (base.OnEvent(sender, e))
            {
                return true;
            }
            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                grabEvent(e);
                return true;
            }
            else if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                collisionEvent(e);
                return true;
            }
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                examineEvent(e);
                return true;
            }
            return false;
        }

        public override void Update(TimeSpan deltaTime)
        {
            switch (action)
            {
                case TacticalAction.GRAB:
                case TacticalAction.EXAMINE:
                    grabOrExamine(deltaTime);
                    break;
                case TacticalAction.DROP:
                    drop();
                    break;
                default:
                    base.Update(deltaTime);
                    break;
            }
        }

        /****************************************************************************/
        /// ACTIONS HANDLING
        /****************************************************************************/
        private void grabOrExamine(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed() || objectTarget.Owner != null)
            {
                #region Cancel Grab or Examine
                objectTarget = null;
                action = TacticalAction.IDLE;
                GameObject.Controller.StopMoving();
                GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }

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

            #region Blend to Run
            if (GameObject.Mesh.CurrentClip != "Run")
            {
                GameObject.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
            }
            #endregion
        }

        private void drop()
        {
            if (currentObject != null)
            {
                currentObject.World.Translation += Vector3.Normalize(GameObject.World.Forward) * 2;
                currentObject.Owner = null;
                currentObject.OwnerBone = -1;
            }
            action = TacticalAction.IDLE;
        }

        /****************************************************************************/
        /// EVENTS HANDLING
        /****************************************************************************/
        private void grabEvent(EventArgs e)
        {
            GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

            if (moveToObjectCommandEvent.gameObject != this.GameObject)
            {
                objectTarget = moveToObjectCommandEvent.gameObject;
                action = TacticalAction.GRAB;
                GameObject.Body.SubscribeCollisionEvent(objectTarget.ID);
            }
        }

        private void collisionEvent(EventArgs e)
        {
            CollisionEvent collisionEvent = e as CollisionEvent;

            if (collisionEvent.gameObject == objectTarget)
            {
                GameObject.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                if (action == TacticalAction.GRAB)
                {
                    #region Pick object if pickable
                    if (objectTarget.Status == GameObjectStatus.Pickable)
                    {
                        #region Drop current object if present
                        if (currentObject != null)
                        {
                            currentObject.World.Translation += Vector3.Normalize(GameObject.World.Forward) * 2;
                            currentObject.Owner = null;
                            currentObject.OwnerBone = -1;
                        }
                        #endregion

                        currentObject = objectTarget;
                        objectTarget.Owner = this.GameObject;
                        objectTarget.OwnerBone = GameObject.Mesh.BoneMap[GameObject.gripBone];
                    }
                    #endregion
                }
                else if (action == TacticalAction.EXAMINE)
                {
                    #region Examine object
                    //TODO: zmienić na wysyłkę przez kontroler, nie przez GO
                    GameObject.SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);
                    #endregion
                }
                #region Turn to idle
                objectTarget = null;
                action = 0;
                GameObject.Controller.StopMoving();
                GameObject.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
            }
        }

        private void examineEvent(EventArgs e)
        {
            ExamineObjectCommandEvent ExamineObjectCommandEvent = e as ExamineObjectCommandEvent;

            objectTarget = ExamineObjectCommandEvent.gameObject;
            action = TacticalAction.EXAMINE;
            GameObject.Body.SubscribeCollisionEvent(objectTarget.ID);
        }

        protected override void Attack()
        {
            TakeDamage attack = new TakeDamage(currentAttack.minInflictedDamage, this.GameObject);
            GameObject.SendEvent(attack, EventsSystem.Priority.Normal, this.attackTarget);
            ammoLeft -= currentWeapon.ammoPerShot;
            if (ammoLeft >= currentWeapon.ammoPerShot)
            {
                action = TacticalAction.ATTACK_IDLE;
            }
            else
            {
                action = TacticalAction.RELOAD;
            }
        }

        protected void Reload()
        {

        }
        /*****************************************************************************************/
    }
}
