using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

using Microsoft.Xna.Framework.Input;

using PlagueEngine.Audio.Components;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Physics.Components;
using PlagueEngine.Physics;
using PlagueEngine.TimeControlSystem;

using PlagueEngine.HighLevelGameFlow;
using System.Collections.Generic;
using PlagueEngine.EventsSystem;
using PlagueEngine.Particles.Components;
using PlagueEngine.Input.Components;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class FadeInOut :GameObjectInstance
    {
        enum State { None, Out, In };

        /****************************************************************************/
        // Fields
        /****************************************************************************/
        FrontEndComponent front;
        float time;
        float currentTime;
        EventsSnifferComponent sniffer;
        State state = State.None;
        FrontEndComponent gameover;
        bool isgameover = false;
        KeyboardListenerComponent keyboard;
        /****************************************************************************/



        /****************************************************************************/
        // Init
        /****************************************************************************/
        public void Init(FrontEndComponent fr,float t,FrontEndComponent gameover)
        {
            front = fr;
            this.gameover = gameover;
            time = t;
            front.Draw = OnDraw;
            RequiresUpdate = true;
            sniffer = new EventsSnifferComponent();
            gameover.Draw = OnDraw;
            sniffer.SetOnSniffedEvent(OnSniffedEvent);
            sniffer.SubscribeEvents(typeof(FadeInEvent),typeof(FadeOutEvent));
            keyboard = new KeyboardListenerComponent(this, true);
            keyboard.SubscibeKeys(OnKey, Keys.Space, Keys.Enter, Keys.Escape);

            state = State.Out;

        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (isgameover && state.WasPressed() && (key == Keys.Escape || key == Keys.Space || key == Keys.Enter))
            {

             
                this.state = FadeInOut.State.None;
                isgameover = false;
                SendEvent(new ChangeLevelEvent("Menu.lvl"), Priority.High, GlobalGameObjects.GameController);
                currentTime = 0;
                
            }
        }




        /****************************************************************************/
        /// On Sniffed Event
        /****************************************************************************/
        private void OnSniffedEvent(EventsSender sender, IEventsReceiver receiver, EventArgs e)
        {
            if (e.GetType().Equals(typeof(FadeInEvent)))
            {
                state = State.In;
                currentTime = 0;
                isgameover = true;
            }

            if (e.GetType().Equals(typeof(FadeOutEvent)))
            {
                state = State.Out;
                currentTime = 0;
            }

        }





        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {

            if (state == State.In)
            {
                spriteBatch.Draw(front.Texture, new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, (currentTime / time) ));
                spriteBatch.Draw(gameover.Texture, new Vector2(screenWidth / 2 - gameover.Texture.Width / 2, screenHeight / 2 - gameover.Texture.Height / 2), new Color(255, 255, 255, (currentTime / time)));
                
            }
            if (state == State.Out)
            {
                spriteBatch.Draw(front.Texture, new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, (1.0f-(currentTime / time)) ));
            }

            
 
        }
        /****************************************************************************/



        public override void Update(TimeSpan deltaTime)
        {
            base.Update(deltaTime);
            currentTime += (float)deltaTime.TotalSeconds;
            if (currentTime > time)
            {
                if (state != State.In)
                {

                    state = State.None;
                }
            }
        }


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            
            front.ReleaseMe();
            gameover.ReleaseMe();
        }
        /****************************************************************************/



        /****************************************************************************/
        /// GetData
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            var data = new FadeInOutData();
            GetData(data);



            data.Time = time;

            return data;
        }
        /****************************************************************************/


    }



    /********************************************************************************/
    /// FadeInOutData
    /********************************************************************************/
    [Serializable]
    public class FadeInOutData : ActivableObjectData
    {
        public FadeInOutData()
        {
            Type = typeof(FadeInOut);

        }

        public float Time { get; set; }


    }
    /********************************************************************************/

}
/************************************************************************************/

