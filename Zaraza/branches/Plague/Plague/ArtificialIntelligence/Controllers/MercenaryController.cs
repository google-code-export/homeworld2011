using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Physics;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow.GameObjects;

enum MoveState{STOP, MOVE, FOLLOW, TO_GRAB};

namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    class MercenaryController 

    {

        public AbstractPerson Mercenary { get; private set; }
        
        private GameObjectInstance objectTarget;
        private TacticalAction moving;
        private Vector3 target;

        private float rotationSpeed = 0;
        private float movingSpeed = 0;
        private float distance = 0;
        private float anglePrecision = 0;

        private GameObjectInstance currentObject;

        //TODO: change temporary constructor.
        public MercenaryController(AbstractPerson person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle)
        {
            this.Mercenary = person;
            this.rotationSpeed = rotationSpeed;
            this.movingSpeed = movingSpeed;
            this.distance = distance;
            this.anglePrecision = angle;
        }


        public  void OnEvent(EventsSystem.EventsSender sender, EventArgs e)
        {

            #region Move to point (1)
            /*************************************/
            /// MoveToPointCommandEvent
            /*************************************/
            if (e.GetType().Equals(typeof(MoveToPointCommandEvent)))
            {
                MoveToPointCommandEvent moveToPointCommandEvent = e as MoveToPointCommandEvent;

                target = moveToPointCommandEvent.point;
                moving = TacticalAction.MOVE;
            }
            #endregion

            #region Grab Object (2)
            /*************************************/
            /// GrabObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(GrabObjectCommandEvent)))
            {
                GrabObjectCommandEvent moveToObjectCommandEvent = e as GrabObjectCommandEvent;

                if (moveToObjectCommandEvent.gameObject != this.Mercenary)
                {
                    objectTarget = moveToObjectCommandEvent.gameObject;
                    moving = TacticalAction.GRAB;
                    Mercenary.Body.SubscribeCollisionEvent(objectTarget.ID);
                }
            }
            #endregion

            #region Collision detected (ends 2 & 4)
            /*************************************/
            /// CollisionEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(CollisionEvent)))
            {
                CollisionEvent collisionEvent = e as CollisionEvent;

                if (collisionEvent.gameObject == objectTarget)
                {
                    Mercenary.Body.CancelSubscribeCollisionEvent(objectTarget.ID);
                    if (moving == TacticalAction.GRAB)
                    {
                        #region Pick object if pickable
                        if (objectTarget.Status == GameObjectStatus.Pickable)
                        {
                            #region Drop current object if present
                            if (currentObject != null)
                            {
                                currentObject.World.Translation += Vector3.Normalize(Mercenary.World.Forward) * 2;
                                currentObject.Owner = null;
                                currentObject.OwnerBone = -1;
                            }
                            #endregion

                            currentObject = objectTarget;
                            objectTarget.Owner = this.Mercenary;
                            objectTarget.OwnerBone = Mercenary.Mesh.BoneMap[Mercenary.gripBone];
                        }
                        #endregion
                    }
                    else if (moving == TacticalAction.EXAMINE)
                    {
                        #region Examine object
                        //TODO: zmienić na wysyłkę przez kontroler, nie przez GO
                        Mercenary.SendEvent(new ExamineEvent(), EventsSystem.Priority.Normal, objectTarget);
                        #endregion
                    }
                    #region Turn to idle
                    objectTarget = null;
                    moving = 0;
                    Mercenary.Controller.StopMoving();
                    Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                    #endregion
                }
            }
            #endregion

            #region Follow object (3)
            /*************************************/
            /// FollowObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(FollowObjectCommandEvent)))
            {
                FollowObjectCommandEvent FollowObjectCommandEvent = e as FollowObjectCommandEvent;

                if (FollowObjectCommandEvent.gameObject != this.Mercenary)
                {
                    objectTarget = FollowObjectCommandEvent.gameObject;
                    moving = TacticalAction.FOLLLOW;
                }
            }
            #endregion

            #region Examine Object (4)
            /*************************************/
            /// ExamineObjectCommandEvent
            /*************************************/
            else if (e.GetType().Equals(typeof(ExamineObjectCommandEvent)))
            {
                ExamineObjectCommandEvent ExamineObjectCommandEvent = e as ExamineObjectCommandEvent;

                objectTarget = ExamineObjectCommandEvent.gameObject;
                moving = TacticalAction.EXAMINE;
                Mercenary.Body.SubscribeCollisionEvent(objectTarget.ID);
            }
            #endregion
        }


        private void move(TimeSpan deltaTime)
        {
            if (Vector2.Distance(new Vector2(Mercenary.World.Translation.X,
                                                 Mercenary.World.Translation.Z),
                                     new Vector2(target.X,
                                                 target.Z)) < distance)
            {
                moving = 0;
                Mercenary.Controller.StopMoving();
                Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
            }
            else
            {
                Vector3 direction = Mercenary.World.Translation - target;
                Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                Vector2 v2 = Vector2.Normalize(new Vector2(Mercenary.World.Forward.X, Mercenary.World.Forward.Z));

                float det = v1.X * v2.Y - v1.Y * v2.X;
                float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                if (det < 0)
                {
                    angle = -angle;
                }

                if (Math.Abs(angle) > anglePrecision)
                {
                    Mercenary.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                }

                Mercenary.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                #region Blend to Run
                if (Mercenary.Mesh.CurrentClip != "Run")
                {
                    Mercenary.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
                }
                #endregion
            }
        }

        private void grabOrExamine(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed() || objectTarget.Owner != null)
            {
                #region Cancel Grab or Examine
                objectTarget = null;
                moving = 0;
                Mercenary.Controller.StopMoving();
                Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }

            Vector3 direction = Mercenary.World.Translation - objectTarget.World.Translation;
            Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
            Vector2 v2 = Vector2.Normalize(new Vector2(Mercenary.World.Forward.X, Mercenary.World.Forward.Z));

            float det = v1.X * v2.Y - v1.Y * v2.X;
            float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

            if (det < 0)
            {
                angle = -angle;
            }

            if (Math.Abs(angle) > anglePrecision)
            {
                Mercenary.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
            }

            Mercenary.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

            #region Blend to Run
            if (Mercenary.Mesh.CurrentClip != "Run")
            {
                Mercenary.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.5f));
            }
            #endregion
        }

        private void follow(TimeSpan deltaTime)
        {
            if (objectTarget.IsDisposed())
            {
                #region Turn to Idle
                objectTarget = null;
                moving = 0;
                Mercenary.Controller.StopMoving();
                Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                #endregion
                return;
            }
            else
            {
                double currentDistance = Vector2.Distance(new Vector2(Mercenary.World.Translation.X, Mercenary.World.Translation.Z),
                                     new Vector2(objectTarget.World.Translation.X, objectTarget.World.Translation.Z));
                if (Mercenary.Mesh.CurrentClip == "Idle" && currentDistance > 8)
                {
                    #region Resume Chase
                    Vector3 direction = Mercenary.World.Translation - objectTarget.World.Translation;
                    Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                    Vector2 v2 = Vector2.Normalize(new Vector2(Mercenary.World.Forward.X, Mercenary.World.Forward.Z));

                    float det = v1.X * v2.Y - v1.Y * v2.X;
                    float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                    if (det < 0)
                    {
                        angle = -angle;
                    }

                    if (Math.Abs(angle) > anglePrecision)
                    {
                        Mercenary.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                    }

                    Mercenary.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                    Mercenary.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));

                    #endregion
                }
                else if (Mercenary.Mesh.CurrentClip != "Idle")
                {
                    if (currentDistance < 4)
                    {
                        #region Pause Chase
                        Mercenary.Controller.StopMoving();
                        Mercenary.Mesh.BlendTo("Idle", TimeSpan.FromSeconds(0.3f));
                        #endregion
                        return;
                    }
                    else
                    {
                        #region Continue Running
                        Vector3 direction = Mercenary.World.Translation - objectTarget.World.Translation;
                        Vector2 v1 = Vector2.Normalize(new Vector2(direction.X, direction.Z));
                        Vector2 v2 = Vector2.Normalize(new Vector2(Mercenary.World.Forward.X, Mercenary.World.Forward.Z));

                        float det = v1.X * v2.Y - v1.Y * v2.X;
                        float angle = (float)Math.Acos((double)Vector2.Dot(v1, v2));

                        if (det < 0)
                        {
                            angle = -angle;
                        }

                        if (Math.Abs(angle) > anglePrecision)
                        {
                            Mercenary.Controller.Rotate(MathHelper.ToDegrees(angle) * rotationSpeed * (float)deltaTime.TotalSeconds);
                        }

                        Mercenary.Controller.MoveForward(movingSpeed * (float)deltaTime.TotalSeconds);

                        if (Mercenary.Mesh.CurrentClip != "Run")
                        {
                            Mercenary.Mesh.BlendTo("Run", TimeSpan.FromSeconds(0.3f));
                        }
                        #endregion
                    }
                }
            }
        }


        /****************************************************************************/
        /// Update
        /****************************************************************************/
        public  void Update(TimeSpan deltaTime)
        {
            switch (moving)
            {
                case TacticalAction.MOVE:
                    move(deltaTime);
                    break;
                case TacticalAction.GRAB:
                case TacticalAction.EXAMINE:
                    grabOrExamine(deltaTime);
                    break;
                case TacticalAction.FOLLLOW:
                    follow(deltaTime);
                    break;
            }
        }
        /****************************************************************************/
    }
}
