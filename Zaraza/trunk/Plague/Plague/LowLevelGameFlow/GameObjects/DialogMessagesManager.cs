using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;


using PlagueEngine.EventsSystem;
using PlagueEngine.Rendering.Components;
using PlagueEngine.GUI.Components;

using PlagueEngine.TimeControlSystem;
using PlagueEngine.Input.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    struct Message
    {
        public String name;
        public Rectangle icon;
        public String text;
    }

    class DialogMessagesManager : GameObjectInstance
    {


        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        List<Message> messages = new List<Message>();
        int currentMessage = -1;
        bool newMessage = false;

        EventsSnifferComponent sniffer = new EventsSnifferComponent();
        bool draw = false;


        LabelComponent text;
        FrontEndComponent texture;
        Rectangle windowRect;
        Vector2 windowPos;
        int windowHeight, windowWidth;

        Vector2 iconPosition;
        uint timerID;


        MouseListenerComponent mouse;
        int mousex, mousey;
        /****************************************************************************/




        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent face,LabelComponent text, Rectangle windowRect,Vector2 windowPos, int windowHeight, int windowWidth,Vector2 iconPos)
        {
            this.text = text;
            this.texture = face;
            this.windowPos = windowPos;
            this.windowRect = windowRect;
            Diagnostics.PushLog(text.X.ToString());
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            face.Draw = OnDraw;
            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(NewDialogMessageEvent));
            this.iconPosition = iconPos;
       


            mouse = new MouseListenerComponent(this, true);
            mouse.SubscribeMouseMove(OnMouseMove, MouseMoveAction.Move);
            mouse.SubscribeKeys(OnMouseKey,MouseKeyAction.LeftClick);
             
        }
        /****************************************************************************/




        /****************************************************************************/
        /// On Mouse Key
        /****************************************************************************/
        private void OnMouseKey(MouseKeyAction mouseKeyAction,ref ExtendedMouseKeyState mouseKeyState)
        {
            if (mouseKeyState.WasPressed() && mouseKeyAction == MouseKeyAction.LeftClick)
            {
                if (mousex >= windowPos.X && mousex <= (windowPos.X + windowWidth) && mousey >= windowPos.Y && mousey <= (windowPos.Y + windowHeight))
                {
                 
                    UpdateMessages();
                }
            }
        }

            
        /****************************************************************************/
        /// On Mouse Move
        /****************************************************************************/
        private void OnMouseMove(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
        {
            mousex = (int)mouseMovementState.Position.X;
            mousey = (int)mouseMovementState.Position.Y;

            
        }





        private void UpdateMessages()
        {
            if ((messages.Count - 1) == currentMessage)
            {
                 draw = false;
                text.Unregister();
                TimeControl.ReleaseTimer(timerID);

            }
            if ((messages.Count - 1) > currentMessage)
            {
                 
                currentMessage++;
                //window.Title = messages[currentMessage].name;
                text.Text = messages[currentMessage].text;
            }
        }





        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {


            if (draw && messages.Count!=0)
            {
                spriteBatch.Draw(texture.Texture, new Rectangle((int)windowPos.X, (int)windowPos.Y, windowWidth, windowHeight), windowRect, Color.Wheat);
                spriteBatch.Draw(texture.Texture, iconPosition, messages[currentMessage].icon, Color.Wheat);
                
            }
        }
        /****************************************************************************/
    




        /****************************************************************************/
        /// OnSniffedEvent
        /****************************************************************************/
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            if (e.GetType().Equals(typeof(NewDialogMessageEvent)))
            {
                

                    

                    Message m = new Message();
                    m.text = ((NewDialogMessageEvent)e).text;
                    m.name = ((NewDialogMessageEvent)e).name;
                    m.icon = ((NewDialogMessageEvent)e).icon;
                    messages.Add(m);

                    newMessage = true;

                    if (draw == false)
                    {
                        currentMessage++;
                        //window.Title = messages[currentMessage].name;
                        text.Text = messages[currentMessage].text;
                        text.Register();
                        draw = true;
                        timerID = TimeControl.CreateTimer(TimeSpan.FromSeconds(4), -1, UpdateMessages);
                    }
            }
        }
        /****************************************************************************/


        /********************************************************************************/
        /// ReleaseComponents
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            sniffer.ReleaseMe();
            texture.ReleaseMe();
            text.ReleaseMe();
            mouse.ReleaseMe();
        }
        /********************************************************************************/



        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            DialogMessagesManagerData data = new DialogMessagesManagerData();
            GetData(data);

            data.IconPosition = iconPosition;
            data.windowHeight = windowHeight;
            data.windowWidth =  windowWidth;
            data.WindowPosition = new Vector2(windowPos.X,windowPos.Y);
            data.TextPosition = new Vector2(text.X, text.Y);
            Diagnostics.PushLog(text.X.ToString());
            return data;
        }
        /********************************************************************************/

    }



    /********************************************************************************/
    /// DialogMessagesManagerData
    /********************************************************************************/
    [Serializable]
    public class DialogMessagesManagerData : GameObjectInstanceData
    {

        public Vector2 IconPosition {get;set;}
        public Vector2 WindowPosition{get;set;}
        public int windowWidth{get;set;}
        public int windowHeight{get;set;}
        public Vector2 TextPosition { get; set; }

    }
    /********************************************************************************/

}
