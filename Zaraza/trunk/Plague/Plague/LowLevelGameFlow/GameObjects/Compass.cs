using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Compass
    /********************************************************************************/
    class Compass : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private FrontEndComponent frontEnd = null;
        private LinkedCamera      camera   = null;
        private Vector3           Target;        
        private Vector2           Orientation;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent frontEnd,
                         Vector3           target,
                         LinkedCamera      camera,
                         Vector2           orientation)
        {
            this.frontEnd    = frontEnd;
            this.camera      = camera;
            this.Target      = target;
            this.Orientation = Vector2.Normalize(orientation);            

            frontEnd.Draw = OnDraw;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            Vector2 cameraOrientation = Vector2.Normalize(new Vector2(camera.World.Forward.X, camera.World.Forward.Z));

            float angle  = (float)Math.Acos((double)Vector2.Dot(Orientation, cameraOrientation));
                  angle *= Math.Sign(Orientation.X * cameraOrientation.Y - Orientation.Y * cameraOrientation.X);
                  angle /= MathHelper.Pi;                  
                        
            spriteBatch.Draw(frontEnd.Texture, new Vector2(0, screenHeight - 33), new Rectangle(192 - 64 + (int)(angle * (192 - 64)), 0, 128, 33), Color.White);

            Vector2 TargetOrientation = Vector2.Normalize( new Vector2(Target.X, Target.Z) - new Vector2(camera.World.Translation.X, camera.World.Translation.Z));
            
            angle = (float)Math.Acos((double)Vector2.Dot(TargetOrientation, cameraOrientation));
            angle *= -Math.Sign(TargetOrientation.X * cameraOrientation.Y - TargetOrientation.Y * cameraOrientation.X);
            angle /= MathHelper.PiOver2;            
            
            if (angle < -1)
            {
                spriteBatch.Draw(frontEnd.Texture, new Vector2(0, screenHeight - 42), new Rectangle(13, 33, 17, 17), Color.White);            
            }
            else if (angle > 1)
            {
                spriteBatch.Draw(frontEnd.Texture, new Vector2(111, screenHeight - 42), new Rectangle(30, 33, 17, 17), Color.White);            
            }
            else
            {
                spriteBatch.Draw(frontEnd.Texture, new Vector2(58 + (int)(angle * 64), screenHeight - 33), new Rectangle(0, 33, 13, 17), Color.White);            
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Components
        /****************************************************************************/
        public override void ReleaseComponents()
        {
            frontEnd.ReleaseMe();
            frontEnd = null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            CompassData data = new CompassData();
            GetData(data);

            data.Orientation = Orientation;
            data.LinkedCamera = camera.ID;
            data.Target = Target;

            return data;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// CompassData
    /********************************************************************************/
    [Serializable]
    public class CompassData : GameObjectInstanceData
    {
        public CompassData()
        {
            Type = typeof(Compass);
        }

        public int      LinkedCamera { get; set; }
        public Vector3  Target       { get; set; }
        public Vector2  Orientation  { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/