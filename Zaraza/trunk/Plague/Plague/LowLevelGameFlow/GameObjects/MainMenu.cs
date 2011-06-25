using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using PlagueEngine.Rendering.Components;
using PlagueEngine.GUI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlagueEngine.HighLevelGameFlow;

using System.ComponentModel.Design;
using System.Drawing.Design;
using PlagueEngine.Audio.Components;
using PlagueEngine.Audio;


namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// MainMenu
    /********************************************************************************/
    class MainMenu:GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/

        BackgroundMusicComponent music = new BackgroundMusicComponent();
        SoundEffectComponent waves = new SoundEffectComponent();
        SoundEffectComponent wind = new SoundEffectComponent();


        FrontEndComponent frame;

        /***********************************/
        /// window
        /***********************************/
        private FrontEndComponent window;
        private int windowx;
        private int windowy;
        private int windowheight;
        private int windowwidth;
        /***********************************/


        /***********************************/
        /// newGame button
        /***********************************/
        private ButtonComponent newGame;

        private String newGametext { get; set; }
        private String newGametag { get; set; }
        private string levelToLoad { get; set; }
        /***********************************/

        /***********************************/
        /// options button
        /***********************************/
        private ButtonComponent options;

        private String optionstext { get; set; }
        private String optionstag { get; set; }
        /***********************************/

        /***********************************/
        /// credits button
        /***********************************/
        private ButtonComponent credits;

        private String creditstext { get; set; }
        private String creditstag { get; set; }
        /***********************************/

        /***********************************/
        /// exit button
        /***********************************/
        private ButtonComponent exit;

        private String exittext { get; set; }
        private String exittag { get; set; }
        /***********************************/

        /***********************************/
        /// credits window
        /***********************************/
        private LabelComponent creditslabel;
        private int creditswindowx { get; set; }
        private int creditswindowy { get; set; }
        private int creditswindowwidth { get; set; }
        private int creditswindowheight { get; set; }
        private String creditswindowtext { get; set; }
        private int creditswindowtextx { get; set; }
        private int creditswindowtexty { get; set; }
        private bool creditswindowregistered { get; set; }
        /***********************************/



        /***********************************/
        /// options window
        /***********************************/
        private LabelComponent optionslabel;
        private int optionswindowx { get; set; }
        private int optionswindowy { get; set; }
        private int optionswindowwidth { get; set; }
        private int optionswindowheight { get; set; }
        private String optionswindowtext { get; set; }
        private int optionswindowtextx { get; set; }
        private int optionswindowtexty { get; set; }
        private bool optionswindowregistered { get; set; }
        /***********************************/

        OptionsMenu optionsMenu;

        FrontEndComponent splashScreen;
        bool drawSplashScreen = false;
        /****************************************************************************/

        public void refresh()
        {
            newGame.refresh();

            options.refresh();
            credits.refresh();

            exit.refresh();
            creditslabel.refresh();
            optionslabel.refresh();
            optionsMenu.refresh();
        }

        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(ButtonComponent newGame, String newGametext, String newGametag, String levelToLoad,
                        ButtonComponent options, String optionstext, String optionstag,
                        ButtonComponent credits, String creditstext, String creditstag,
                        ButtonComponent exit, String exittext, String exittag,
                        FrontEndComponent window, int windowx, int windowy, int windowheight, int windowwidth,
                        LabelComponent creditslabel, int creditswindowx, int creditswindowy, int creditswindowwidth, int creditswindowheight, String creditswindowtext, int creditswindowtextx, int creditswindowtexty,
                        LabelComponent optionslabel, int optionswindowx, int optionswindowy, int optionswindowwidth, int optionswindowheight, String optionswindowtext, int optionswindowtextx, int optionswindowtexty,
                        FrontEndComponent frame,
            FrontEndComponent splash)
        {
            this.RequiresUpdate = true;


            //music.LoadFolder("Music", 0.4f); ;  //music.
            //music.AutomaticMode = false;
            //music.PlaySong("default", "cautious-path", true);

            waves.LoadFolder("Menu",1, 0, 0, false);
            waves.SetPosition(new Vector3(0, 3, -60));
            waves.PlaySound("Menu", "ocean-wave-2", true);

            wind.LoadFolder("Menu", 0.5f, 0, 0, false);
            wind.SetPosition(new Vector3(0, 3, -60));
            wind.PlaySound("Menu", "windhowl", true);    
            
            //AudioManager.GetInstance.BackgroundMusicComponent.PlaySong("default", "cautious-path");
            
            splashScreen = splash;
            splashScreen.Draw = OnDraw;

            optionsMenu = new OptionsMenu(this, "OptionsMenu.language_change","OptionsMenu.main","OptionsMenu.back",optionswindowx, optionswindowy, optionswindowwidth, optionswindowheight, frame);
            this.frame = frame;
            this.frame.Draw = OnDraw;
            this.window = window;
            this.windowx = windowx;
            this.windowy = windowy;
            this.windowheight = windowheight;
            this.windowwidth = windowwidth;

            this.window.Draw = OnDraw;

            this.newGame = newGame;

            this.newGametext = newGametext;
            this.newGametag = newGametag;
            this.levelToLoad = levelToLoad;
            this.newGame.Register();
            this.newGame.setDelegate(newGameClick);

            this.options = options;

            this.optionstext = optionstext;
            this.optionstag = optionstag;

            this.options.Register();
            this.options.setDelegate(optionsClick);


            this.credits = credits;

            this.creditstext = creditstext;
            this.creditstag = creditstag;

            this.credits.Register();
            this.credits.setDelegate(creditsClick);

            this.exit = exit;

            this.exittext = exittext;
            this.exittag = exittag;

            this.exit.Register();

            this.exit.setDelegate(exitClick);

            this.creditslabel = creditslabel;
            this.creditswindowx = creditswindowx;
            this.creditswindowy = creditswindowy;
            this.creditswindowwidth = creditswindowwidth;
            this.creditswindowheight = creditswindowheight;
            this.creditswindowtext = creditswindowtext;
            this.creditswindowtextx = creditswindowtextx;
            this.creditswindowtexty = creditswindowtexty;
            this.creditswindowregistered = false;



            this.optionslabel = optionslabel;
            this.optionswindowx = optionswindowx;
            this.optionswindowy = optionswindowy;
            this.optionswindowwidth = optionswindowwidth;
            this.optionswindowheight = optionswindowheight;
            this.optionswindowtext = optionswindowtext;
            this.optionswindowtextx = optionswindowtextx;
            this.optionswindowtexty = optionswindowtexty;
            this.optionswindowregistered = false;

            
        }        
        /****************************************************************************/





        /****************************************************************************/
        /// newGameClick
        /****************************************************************************/
        public void newGameClick(object sender, EventArgs e)
        {
            if (levelToLoad != String.Empty)
            {
                drawSplashScreen = true;
                TimeControlSystem.TimeControl.CreateFrameCounter(1, 1, SendNextLevelEvent);
                HideAll();
            }
        }
        /****************************************************************************/

        private void SendNextLevelEvent()
        {
            this.SendEvent(new ChangeLevelEvent(levelToLoad), EventsSystem.Priority.High, GlobalGameObjects.GameController);
           
        }

        private void HideAll()
        {
        
        
        newGame.Unregister();

        options.Unregister();
        credits.Unregister();

        exit.Unregister();
        creditslabel.Unregister();
        optionslabel.Unregister();
        optionsMenu.Disable();
        }

        /****************************************************************************/
        /// exitClick
        /****************************************************************************/
        public void exitClick(object sender, EventArgs e)
        {
            this.SendEvent(new ExitGameEvent(), EventsSystem.Priority.High, GlobalGameObjects.GameController);  
        }
        /****************************************************************************/

       

        /****************************************************************************/
        /// creditsClick
        /****************************************************************************/
        public void creditsClick(object sender, EventArgs e)
        {
            this.optionsMenu.Disable();
            optionswindowregistered = false;

            if (!creditswindowregistered)
            {
                this.creditslabel.Register();
            }
            if (creditswindowregistered)
            {
                this.creditslabel.Unregister();
            }

            creditswindowregistered = !creditswindowregistered;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// optionsClick
        /****************************************************************************/
        public void optionsClick(object sender, EventArgs e)
        {
            this.creditslabel.Unregister();
            creditswindowregistered = false;

            if (!optionswindowregistered)
            {
                optionsMenu.Enable();

            }
            if (optionswindowregistered)
            {
                optionsMenu.Disable();
            }


            optionswindowregistered = !optionswindowregistered;
        }
        /****************************************************************************/

        public override void Update(TimeSpan elapsed)
        {
            AudioManager.GetInstance.BackgroundMusicComponent.Update(elapsed);
            
            //waves.Emitter.Position = new Vector3(0,3,-60) + Vector3.Multiply(Vector3.Left, (float)(Math.Cos((double)elapsed.Milliseconds)));
            //wind.Emitter.Position = new Vector3(0,3,-60) + 2 * Vector3.Left + Vector3.Multiply(Vector3.Left, (float)(Math.Cos((double)elapsed.Milliseconds)));
        }

        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            if (drawSplashScreen)
            {
                spriteBatch.Draw(splashScreen.Texture, new Rectangle(0, 0, screenWidth, screenHeight), Color.Wheat);
                
            }
            if (drawSplashScreen==false)
            {
                spriteBatch.Draw(window.Texture, new Rectangle(windowx, windowy, windowwidth, windowheight), Color.White);

                optionsMenu.OnDraw(spriteBatch, ref ViewProjection, screenWidth, screenHeight);

                if (creditswindowregistered)
                {
                    spriteBatch.Draw(frame.Texture, new Rectangle(creditswindowx, creditswindowy, creditswindowwidth, creditswindowheight), Color.White);
                }
            }

        }


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            newGame.ReleaseMe();
            options.ReleaseMe();
            credits.ReleaseMe();
            exit.ReleaseMe();
            window.ReleaseMe();

            creditslabel.ReleaseMe();

            optionslabel.ReleaseMe();

            frame.ReleaseMe();

            optionsMenu.ReleaseComponents();
            splashScreen.ReleaseMe();
        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            MainMenuData data = new MainMenuData();
            GetData(data);

            data.newGametext = newGametext;
            data.newGametag = newGametag;
            data.newGamex = newGame.X;
            data.newGamey = newGame.Y;
            data.newGamewidth = newGame.Width;
            data.newGameheight = newGame.Height;
            data.levelToLoad = levelToLoad;


            data.optionstext = optionstext;
            data.optionstag = optionstag;
            data.optionsx = options.X;
            data.optionsy = options.Y;
            data.optionswidth = options.Width;
            data.optionsheight = options.Height;


            data.creditstext = creditstext;
            data.creditstag = creditstag;
            data.creditsx = credits.X;
            data.creditsy = credits.Y;
            data.creditswidth = credits.Width;
            data.creditsheight = credits.Height;


            data.exittext = exittext;
            data.exittag = exittag;
            data.exitx = exit.X;
            data.exity = exit.Y;
            data.exitwidth = exit.Width;
            data.exitheight = exit.Height;



            data.windowx = windowx;
            data.windowy = windowy;
            data.windowheight = windowheight;
            data.windowwidth = windowwidth;



            data.creditswindowx = creditswindowx;
            data.creditswindowy = creditswindowy;
            data.creditswindowwidth = creditswindowwidth;
            data.creditswindowheight = creditswindowheight;
            data.creditswindowtext = creditswindowtext;
            data.creditswindowtextx = creditswindowtextx;
            data.creditswindowtexty = creditswindowtexty;

            data.optionswindowx = optionswindowx;
            data.optionswindowy = optionswindowy;
            data.optionswindowwidth = optionswindowwidth;
            data.optionswindowheight = optionswindowheight;
            data.optionswindowtext = optionswindowtext;
            data.optionswindowtextx = optionswindowtextx;
            data.optionswindowtexty = optionswindowtexty;

            return data;
        }
        /********************************************************************************/



    }



    /********************************************************************************/
    /// MainMenuData
    /********************************************************************************/
    [Serializable]
    public class MainMenuData : GameObjectInstanceData
    {
        public MainMenuData()
        {
            Type = typeof(MainMenu);
        }

        [CategoryAttribute("new game button")]
        public String newGametext { get; set; }

        [CategoryAttribute("new game button")]
        public String newGametag { get; set; }

        [CategoryAttribute("new game button")]
        public int newGamex { get; set; }

        [CategoryAttribute("new game button")]
        public int newGamey { get; set; }

        [CategoryAttribute("new game button")]
        public int newGamewidth { get; set; }

        [CategoryAttribute("new game button")]
        public int newGameheight { get; set; }

        [CategoryAttribute("new game button")]
        public String levelToLoad { get; set; }
        


        [CategoryAttribute("options button")]
        public String optionstext { get; set; }

        [CategoryAttribute("options button")]
        public String optionstag { get; set; }

        [CategoryAttribute("options button")]
        public int optionsx { get; set; }

        [CategoryAttribute("options button")]
        public int optionsy { get; set; }

        [CategoryAttribute("options button")]
        public int optionswidth { get; set; }

        [CategoryAttribute("options button")]
        public int optionsheight { get; set; }




        [CategoryAttribute("credits button")]
        public String creditstext { get; set; }

        [CategoryAttribute("credits button")]
        public String creditstag { get; set; }

        [CategoryAttribute("credits button")]
        public int creditsx { get; set; }

        [CategoryAttribute("credits button")]
        public int creditsy { get; set; }

        [CategoryAttribute("credits button")]
        public int creditswidth { get; set; }

        [CategoryAttribute("credits button")]
        public int creditsheight { get; set; }




        [CategoryAttribute("exit button")]
        public String exittext { get; set; }

        [CategoryAttribute("exit button")]
        public String exittag { get; set; }

        [CategoryAttribute("exit button")]
        public int exitx { get; set; }

        [CategoryAttribute("exit button")]
        public int exity { get; set; }

        [CategoryAttribute("exit button")]
        public int exitwidth { get; set; }

        [CategoryAttribute("exit button")]
        public int exitheight { get; set; }



        [CategoryAttribute("window label")]
        public int windowx { get; set; }

        [CategoryAttribute("window label")]
        public int windowy { get; set; }

        [CategoryAttribute("window label")]
        public int windowwidth { get; set; }

        [CategoryAttribute("window label")]
        public int windowheight { get; set; }






        [CategoryAttribute("window credits")]
        public int creditswindowx { get; set; }

        [CategoryAttribute("window credits")]
        public int creditswindowy { get; set; }

        
        [CategoryAttribute("window credits")]
        public int creditswindowwidth { get; set; }
        
        [CategoryAttribute("window credits")]
        public int creditswindowheight { get; set; }


        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [CategoryAttribute("window credits")]
        public String creditswindowtext { get; set; }

        [CategoryAttribute("window credits")]
        public int creditswindowtextx { get; set; }

        [CategoryAttribute("window credits")]
        public int creditswindowtexty { get; set; }


        [CategoryAttribute("window options")]
        public int optionswindowx { get; set; }

        [CategoryAttribute("window options")]
        public int optionswindowy { get; set; }


        [CategoryAttribute("window options")]
        public int optionswindowwidth { get; set; }

        [CategoryAttribute("window options")]
        public int optionswindowheight { get; set; }


        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [CategoryAttribute("window options")]
        public String optionswindowtext { get; set; }

        [CategoryAttribute("window options")]
        public int optionswindowtextx { get; set; }

        [CategoryAttribute("window options")]
        public int optionswindowtexty { get; set; }

    }
    /********************************************************************************/


}
