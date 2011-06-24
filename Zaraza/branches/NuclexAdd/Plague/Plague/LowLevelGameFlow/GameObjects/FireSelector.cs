using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.Input.Components;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// FireSelector
    /********************************************************************************/
    class FireSelector : GameObjectInstance
    {
        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private FrontEndComponent frontEndComponent = null;
        private MouseListenerComponent mouse = null;

        private Firearm Firearm = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init 
        /****************************************************************************/
        public void Init(FrontEndComponent frontEndComponent,
                         MouseListenerComponent mouse,
                         Firearm firearm)
        {
            this.frontEndComponent = frontEndComponent;
            this.mouse = mouse;
            Firearm = firearm;

            frontEndComponent.Draw = Draw;
            mouse.SubscribeMouseMove(delegate(MouseMoveAction mouseMoveAction, ref ExtendedMouseMovementState mouseMovementState)
                                    {
                                        if (mouseMovementState.ScrollDifference > 0)
                                        {
                                            int i = Firearm.SelectiveFire.IndexOf(Firearm.SelectiveFireMode);
                                            ++i;
                                            if (i < Firearm.SelectiveFire.Count)
                                            {
                                                Firearm.SelectiveFireMode = Firearm.SelectiveFire.ElementAt(i);
                                            }
                                        }
                                        else if (mouseMovementState.ScrollDifference < 0)
                                        {
                                            int i = Firearm.SelectiveFire.IndexOf(Firearm.SelectiveFireMode);
                                            --i;
                                            if (i > -1)
                                            {
                                                Firearm.SelectiveFireMode = Firearm.SelectiveFire.ElementAt(i);
                                            }
                                        }
                                    },
                                    MouseMoveAction.Scroll);

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw 
        /****************************************************************************/
        public void Draw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            switch (Firearm.SelectiveFireMode)
            { 
                case 1:
                    spriteBatch.Draw(frontEndComponent.Texture,
                                     new Vector2(screenWidth - 128, screenHeight - 128),
                                     new Rectangle(256, 0, 128, 128),
                                     Color.White);
                    break;
                case -1:
                    spriteBatch.Draw(frontEndComponent.Texture,
                                     new Vector2(screenWidth - 128, screenHeight - 128),
                                     new Rectangle(0, 0, 128, 128),
                                     Color.White);
                    break;
                default:
                    spriteBatch.Draw(frontEndComponent.Texture,
                                     new Vector2(screenWidth - 128, screenHeight - 128),
                                     new Rectangle(128, 0, 128, 128),
                                     Color.White);
                    break;
            }

            foreach (int fireMode in Firearm.SelectiveFire)
            {
                switch (fireMode)
                { 
                    case 1:
                        spriteBatch.Draw(frontEndComponent.Texture,
                                         new Vector2(screenWidth - 85, screenHeight - 110),
                                         new Rectangle(0, 128, 50, 22),
                                         Color.White);
                        break;
                    case -1: 
                        spriteBatch.Draw(frontEndComponent.Texture,
                                          new Vector2(screenWidth - 40, screenHeight - 75),
                                          new Rectangle(256, 128, 50, 22),
                                          Color.White);
                        break;
                    default:
                        spriteBatch.Draw(frontEndComponent.Texture,
                                         new Vector2(screenWidth - 45, screenHeight - 95),
                                         new Rectangle(128, 128, 50, 22),
                                         Color.White);
                        break;
                }
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
            Firearm = null;
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// FireSelectorData
    /********************************************************************************/
    public class FireSelectorData : GameObjectInstanceData
    {
        public FireSelectorData()
        {
            Type = typeof(FireSelector);
        }

        public int Firearm { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/