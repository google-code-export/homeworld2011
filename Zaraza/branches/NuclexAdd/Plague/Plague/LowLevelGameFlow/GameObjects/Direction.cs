using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Particles.Components;
using PlagueEngine.Input.Components;
using PlagueEngine.Rendering.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.TimeControlSystem;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class Direction : GameObjectInstance
    {
        ParticleEmitterComponent emitter = null;
        MouseListenerComponent mouse = null;
        FrontEndComponent frontEnd = null;

        Vector2 mousePosition;
        Matrix  viewProjection;
        Vector2 screenSize;
        Vector3 orientation;
        Clock clock;

        GameObjectInstance feedback;

        public void Init(ParticleEmitterComponent emitter,
                         FrontEndComponent frontEnd,
                         MouseListenerComponent mouse,
                        GameObjectInstance feedback)
        {
            this.emitter  = emitter;
            this.mouse    = mouse;
            this.frontEnd = frontEnd;
            this.feedback = feedback;

            clock = TimeControlSystem.TimeControl.CreateClock();

            frontEnd.Draw = delegate(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight) 
                                    {                                        
                                        viewProjection = ViewProjection;
                                        screenSize.X = screenWidth;
                                        screenSize.Y = screenHeight;
                                    };

            mouse.Modal = true;

            mouse.SubscribeKeys(delegate(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
                                {
                                    if (mouseKeyAction == MouseKeyAction.RightClick && mouseKeyState.WasReleased())
                                    {
                                        if (clock.Time.TotalSeconds > 0.25f)
                                        {
                                            SendEvent(new LookAtPointEvent(World.Translation + orientation), EventsSystem.Priority.High, feedback);
                                        }
                                        else
                                        {
                                            SendEvent(new LookAtPointEvent(Vector3.Zero), EventsSystem.Priority.High, feedback);
                                        }

                                        SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                                        mouse.Modal = false;
                                        TimeControl.ReleaseClock(clock);
                                    }
                                }, 
                                MouseKeyAction.RightClick);

            mouse.SubscribeMouseMove(delegate(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
                                     {
                                         mousePosition.X = mouseMovementState.Position.X;
                                         mousePosition.Y = mouseMovementState.Position.Y;

                                         Vector2 pos2;

                                         Vector4 position = Vector4.Transform(World.Translation, viewProjection);

                                         pos2.X = MathHelper.Clamp(0.5f * ((position.X / Math.Abs(position.W)) + 1.0f), 0.01f, 0.99f);
                                         pos2.X *= screenSize.X;

                                         pos2.Y = MathHelper.Clamp(1.0f - (0.5f * ((position.Y / Math.Abs(position.W)) + 1.0f)), 0.01f, 0.99f);
                                         pos2.Y *= screenSize.Y;



                                         orientation = Vector3.Transform(new Vector3(mousePosition.X, -mousePosition.Y,1),Matrix.Invert(viewProjection)) -
                                                       Vector3.Transform(new Vector3(pos2.X, -pos2.Y,1), Matrix.Invert(viewProjection));
                                         
                                         emitter.particleSystem.SetOrientation(orientation); 
                                     }, 
                                     MouseMoveAction.Move);
        }


        public override void ReleaseComponents()
        {
            emitter.ReleaseMe();
            emitter = null;
            mouse.ReleaseMe();
            mouse = null;
            frontEnd.ReleaseMe();
            frontEnd = null;
            base.ReleaseComponents();
        }




    }

    /********************************************************************************/
    /// DirectionData
    /********************************************************************************/
    [Serializable]
    class DirectionData : GameObjectInstanceData
    {
        public DirectionData()
        {
            Type = typeof(Direction);
        }

        public int Feedback { get; set; }
    }
    /********************************************************************************/
}
