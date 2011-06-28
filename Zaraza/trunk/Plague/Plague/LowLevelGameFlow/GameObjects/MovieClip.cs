using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.Rendering.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// MovieClip
    /********************************************************************************/
    class MovieClip : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        Video video;
        VideoPlayer player;
        FrontEndComponent texture;
        Texture2D texture2;
        string videoName;
        /****************************************************************************/

        /// Init
        /****************************************************************************/
        public void Init(Video video,string videoName,FrontEndComponent front)
        {
            this.videoName = videoName;
            this.texture = front;
            this.video = video;
            this.RequiresUpdate = true;
            player = new VideoPlayer();
            texture.Draw = OnDraw;
            
        }
        /****************************************************************************/



        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {

            if (player.State != MediaState.Stopped)
                texture2 = player.GetTexture();
          

            if (texture2 != null)
            {

                spriteBatch.Draw(texture2, new Rectangle(0,0,screenWidth,screenHeight), Color.White);
          
            }

    

        }
        /****************************************************************************/





        /****************************************************************************/
        // Update
        /****************************************************************************/
        public override void Update(TimeSpan deltaTime)
        {
            if (player.State == MediaState.Stopped)
            {
                player.IsLooped = true;
                player.Play(video);
            }

        }
        /****************************************************************************/


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            texture.ReleaseMe();

        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MovieClipData data = new MovieClipData();
            GetData(data);
            data.videoName = videoName;
            return data;
        }
        /********************************************************************************/



    }



    /********************************************************************************/
    /// MovieClipData
    /********************************************************************************/
    [Serializable]
    public class MovieClipData : GameObjectInstanceData
    {

        public MovieClipData()
        {
            Type = typeof(MovieClip);
        }
        public string videoName { get; set; }
    }
    /********************************************************************************/


}
