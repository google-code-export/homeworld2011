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
        WindowComponent window;
        FrontEndComponent face;


        Vector2 iconPosition;
        uint timerID;
        /****************************************************************************/




        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent face,LabelComponent text,WindowComponent window,Vector2 iconPos)
        {
            this.text = text;
            this.face = face;
            this.window = window;
            window.AddControl(text.Control);

            face.Draw = OnDraw;
            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(NewDialogMessageEvent));
            window.Unregister();
            this.iconPosition = iconPos;
            RequiresUpdate = true;


            timerID= TimeControl.CreateTimer(TimeSpan.FromSeconds(4), -1, UpdateMessages);
        }
        /****************************************************************************/


        private void UpdateMessages()
        {
            if ((messages.Count - 1) == currentMessage)
            {
                draw = false;
                window.Unregister();
                TimeControl.ReleaseTimer(timerID);

            }
            if ((messages.Count - 1) > currentMessage)
            {
                currentMessage++;
                window.Title = messages[currentMessage].name;
                text.Text = messages[currentMessage].text;
            }
        }


        public override void Update(TimeSpan deltaTime)
        {

        }



        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {

            if (draw && messages.Count!=0)
            {
                spriteBatch.Draw(face.Texture, iconPosition, messages[currentMessage].icon, Color.Wheat);
                
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
                        window.Title = messages[currentMessage].name;
                        text.Text = messages[currentMessage].text;
                        window.Register();
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
            face.ReleaseMe();
            text.ReleaseMe();
            window.ReleaseMe();
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
            data.windowHeight = window.Height;
            data.windowWidth = window.Width;
            data.WindowPosition = new Vector2(window.X, window.Y);
            data.TextPosition = new Vector2(text.X, text.Y);

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
