using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Input.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// ActionSwitch
    /********************************************************************************/
    class ActionSwitch : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private FrontEndComponent frontEndComponent = null;
        private MouseListenerComponent    mouse     = null;
        private KeyboardListenerComponent keyboard  = null;
        private Vector3 Position;
        private Action[] actions = new Action[8];
        private int selectedAction = 0;
        private GameObjectInstance feedback;

        private Rectangle switchRect = new Rectangle(0, 0, 40, 84);        
        private float rotation = 0;
        private Vector2 mousePosition;
        private Color fade = Color.FromNonPremultiplied(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));

        private SpriteFont front = null;        

        private String objectName;
        private Vector2 nameOffset;
        /****************************************************************************/


        /****************************************************************************/
        /// Action
        /****************************************************************************/
        private struct Action
        {
            public String    Name;
            public Vector2   Position;
            public Vector2   DescriptionPosition;
            public Rectangle Rect;

            public Action(String name, Vector2 position,Vector2 descriptionPosition,Rectangle rect)
            {
                Name                = name;
                Position            = position;
                DescriptionPosition = descriptionPosition;
                Rect                = rect;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent         frontEndComponent,
                         MouseListenerComponent    mouse,
                         KeyboardListenerComponent keyboard,
                         Vector3                   position,
                         String[]                  actions,
                         GameObjectInstance        feedback,
                         String                    objectName)
        {
            this.frontEndComponent = frontEndComponent;
            this.mouse             = mouse;
            this.keyboard          = keyboard;
            this.Position          = position;
            this.objectName        = objectName;
            this.feedback          = feedback;            
            
            mouse.SubscribeKeys(OnMouseKey, MouseKeyAction.RightClick);
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);

            mouse.Modal = true;
            keyboard.SubscibeKeys(OnKey, Keys.Escape);
            keyboard.Modal = true;

            mouse.SetCursor("Default");

            frontEndComponent.Draw = Draw;

            front = frontEndComponent.GetFont("Courier New");

            if (!String.IsNullOrEmpty(objectName))
            {
                nameOffset = front.MeasureString(objectName);
                nameOffset.X *= -0.5f;
                nameOffset.Y += 64;
            }

            for(int i = 0 ; i < actions.Length && i < 8; i++)
            {
                this.actions[i] = new Action(actions[i], 
                                             GetPosition(i),
                                             GetDescriptionPosition(i,actions[i]),
                                             GetRect(actions[i]));
            }

            SendEvent(new ChangeSpeedEvent(0.5f), EventsSystem.Priority.High, GlobalGameObjects.GameController);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        public void OnMouseKey(MouseKeyAction mouseKeyAction, ref ExtendedMouseKeyState mouseKeyState)
        {
            if (mouseKeyAction == MouseKeyAction.RightClick && mouseKeyState.WasReleased())
            {
                SendEvent(new SelectedActionEvent(actions[selectedAction].Name), EventsSystem.Priority.Normal, feedback);
                SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                mouse.Modal    = false;
                keyboard.Modal = false;
                SendEvent(new ChangeSpeedEvent(2), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        public void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
        {
            if (mouseMovementState.Moved)
            {
                mousePosition.X = mouseMovementState.Position.X;
                mousePosition.Y = mouseMovementState.Position.Y;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        public void OnKey(Keys key, ExtendedKeyState state)
        {
            if (key == Keys.Escape && state.WasPressed())
            {
                SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.High, GlobalGameObjects.GameController);
                mouse.Modal    = false;
                keyboard.Modal = false;
                SendEvent(new ChangeSpeedEvent(2), EventsSystem.Priority.High, GlobalGameObjects.GameController);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw 
        /****************************************************************************/
        public void Draw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {            
            Vector4 position;
            Vector2 pos2;

            position = Vector4.Transform(Vector3.Transform(Position, World), ViewProjection);

            pos2.X = MathHelper.Clamp(0.5f * ((position.X / Math.Abs(position.W)) + 1.0f), 0.01f, 0.99f);
            pos2.X *= screenWidth;

            pos2.Y = MathHelper.Clamp(1.0f - (0.5f * ((position.Y / Math.Abs(position.W)) + 1.0f)), 0.01f, 0.99f);
            pos2.Y *= screenHeight;

            if (!(mousePosition.X == 0 && mousePosition.Y == 0))
            {
                Vector2 direction = Vector2.Normalize(pos2 - mousePosition);

                float angle = (float)Math.Acos((double)Vector2.Dot(Vector2.UnitY, direction));

                angle *= Math.Sign(-direction.X);                

                rotation = (float)Math.Round(angle / MathHelper.PiOver4,0) * MathHelper.PiOver4;

                selectedAction = (int)Math.Round(angle / MathHelper.PiOver4, 0);
                if (-direction.X < 0 && selectedAction != 0) selectedAction += 8;
            }
            else
            {
                rotation = 0;
            }

            spriteBatch.Draw(frontEndComponent.Texture, pos2, switchRect, Color.White,rotation ,new Vector2(20,64),1,SpriteEffects.None,0);


            for(int i = 0; i < actions.Length ; i++)
            {
                if (!String.IsNullOrEmpty(actions[i].Name))
                {
                    spriteBatch.Draw(frontEndComponent.Texture, 
                                     pos2 + actions[i].Position, 
                                     actions[i].Rect, 
                                     (i == selectedAction ? Color.White : fade),
                                     0, 
                                     new Vector2(16, 16), 
                                     1, 
                                     SpriteEffects.None, 
                                     0);
                }
            }


            if (!String.IsNullOrEmpty(objectName))
            {
                spriteBatch.DrawString(front, objectName, pos2 + nameOffset, Color.DarkRed);
            }

            if (!String.IsNullOrEmpty(actions[selectedAction].Name))
            {                
                spriteBatch.DrawString(front, actions[selectedAction].Name, pos2 + actions[selectedAction].DescriptionPosition, Color.Gray);
            }
            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Position
        /****************************************************************************/
        private Vector2 GetPosition(int i)
        {
            switch (i)
            {
                case 0:  return new Vector2(        0.0f, -44.0f);
                case 1:  return new Vector2( 31.1126976f, -31.1126976f);
                case 2:  return new Vector2(       44.0f,  0.0f);
                case 3:  return new Vector2( 31.1126976f,  31.1126976f);
                case 4:  return new Vector2(        0.0f,  44.0f);
                case 5:  return new Vector2(-31.1126976f,  31.1126976f);
                case 6:  return new Vector2(      -44.0f,  0.0f);
                case 7:  return new Vector2(-31.1126976f, -31.1126976f);
                default: return new Vector2(0, 0);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Rect
        /****************************************************************************/
        private Rectangle GetRect(String action)
        {
            Rectangle rect = new Rectangle(0, 0, 32, 32);

            switch (action)
            {
                case "Grab"          : rect.X = 64;  rect.Y = 0; break;
                case "Examine"       : rect.X = 96;  rect.Y = 0; break;
                case "Follow"        : rect.X = 128; rect.Y = 0; break;
                case "Inventory"     : rect.X = 160; rect.Y = 0; break;
                case "Exchange Items": rect.X = 192; rect.Y = 0; break;
            }

            return rect;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Description Position
        /****************************************************************************/
        private Vector2 GetDescriptionPosition(int i, String description)
        {
            Vector2 stringSize = front.MeasureString(description);
            
            switch(i)
            {
                case 0: return new Vector2(     -stringSize.X/2, -64 - stringSize.Y);
                case 1: return new Vector2(         45.2548332f, -45.2548332f - stringSize.Y);
                case 2: return new Vector2(                  68, -stringSize.Y/2);
                case 3: return new Vector2(         45.2548332f, 45.2548332f);
                case 4: return new Vector2(   -stringSize.X / 2, 64);
                case 5: return new Vector2(     -45.2548332f - stringSize.X, 45.2548332f);
                case 6: return new Vector2(-68 - stringSize.X, -stringSize.Y / 2);
                case 7: return new Vector2(-45.2548332f - stringSize.X, -45.2548332f - stringSize.Y);
                default: return new Vector2(0, 0);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            frontEndComponent.ReleaseMe();
            frontEndComponent = null;
            mouse.ReleaseMe();
            mouse = null;
            keyboard.ReleaseMe();
            keyboard = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// ActionSwitchData
    /********************************************************************************/
    [Serializable]
    class ActionSwitchData : GameObjectInstanceData
    {
        public ActionSwitchData()
        {
            Type = typeof(ActionSwitch);
        }

        public Vector3  Position   { get; set; }
        public String[] Actions    { get; set; }
        public String   ObjectName { get; set; }
        public int      Feedback   { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/