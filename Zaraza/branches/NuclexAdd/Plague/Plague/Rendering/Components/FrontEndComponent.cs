using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna;

using PlagueEngine.LowLevelGameFlow;


/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// Front End Component
    /********************************************************************************/
    class FrontEndComponent : GameObjectComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public Texture2D  Texture { get; private set; }
        
        internal static Renderer renderer = null;

        public delegate void OnDraw(SpriteBatch spriteBatch,ref Matrix ViewProjection,int screenWidth,int screenHeight);
        public OnDraw Draw;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public FrontEndComponent(GameObjectInstance gameObject,Texture2D texture) 
            : base(gameObject)
        {
            Texture = texture;
            renderer.frontEndComponents.Add(this);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Font
        /****************************************************************************/
        public SpriteFont GetFont(String font)
        {
            return renderer.fonts[font];
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.frontEndComponents.Remove(this);            
            base.ReleaseMe();
        }
        /****************************************************************************/
        
    }
    /********************************************************************************/

}
/************************************************************************************/
