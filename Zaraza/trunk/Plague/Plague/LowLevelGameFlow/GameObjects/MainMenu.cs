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
        private int newGamex { get; set; }
        private int newGamey { get; set; }
        private int newGamewidth { get; set; }
        private int newGameheight { get; set; }
        private string levelToLoad { get; set; }
        /***********************************/

        /***********************************/
        /// options button
        /***********************************/
        private ButtonComponent options;

        private String optionstext { get; set; }
        private String optionstag { get; set; }
        private int optionsx { get; set; }
        private int optionsy { get; set; }
        private int optionswidth { get; set; }
        private int optionsheight { get; set; }
        /***********************************/

        /***********************************/
        /// credits button
        /***********************************/
        private ButtonComponent credits;

        private String creditstext { get; set; }
        private String creditstag { get; set; }
        private int creditsx { get; set; }
        private int creditsy { get; set; }
        private int creditswidth { get; set; }
        private int creditsheight { get; set; }
        /***********************************/

        /***********************************/
        /// exit button
        /***********************************/
        private ButtonComponent exit;

        private String exittext { get; set; }
        private String exittag { get; set; }
        private int exitx { get; set; }
        private int exity { get; set; }
        private int exitwidth { get; set; }
        private int exitheight { get; set; }
        /***********************************/

        /***********************************/
        /// credits window
        /***********************************/
        private WindowComponent creditswindow;
        private LabelComponent creditslabel;
        private int creditswindowx { get; set; }
        private int creditswindowy { get; set; }
        private int creditswindowwidth { get; set; }
        private int creditswindowheight { get; set; }
        private String creditswindowtext { get; set; }
        private bool creditswindowregistered { get; set; }
        /***********************************/



        /***********************************/
        /// options window
        /***********************************/
        private WindowComponent optionswindow;
        private LabelComponent optionslabel;
        private int optionswindowx { get; set; }
        private int optionswindowy { get; set; }
        private int optionswindowwidth { get; set; }
        private int optionswindowheight { get; set; }
        private String optionswindowtext { get; set; }
        private bool optionswindowregistered { get; set; }
        /***********************************/



        /****************************************************************************/

        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(ButtonComponent newGame, String newGametext, String newGametag, int newGamex, int newGamey, int newGamewidth, int newGameheight,String levelToLoad,
                        ButtonComponent options, String optionstext, String optionstag, int optionsx, int optionsy, int optionswidth, int optionsheight,
                        ButtonComponent credits, String creditstext, String creditstag, int creditsx, int creditsy, int creditswidth, int creditsheight,
                        ButtonComponent exit, String exittext, String exittag, int exitx, int exity, int exitwidth, int exitheight,
                        FrontEndComponent window, int windowx, int windowy, int windowheight, int windowwidth,
                        WindowComponent creditswindow, LabelComponent creditslabel, int creditswindowx, int creditswindowy, int creditswindowwidth, int creditswindowheight, String creditswindowtext,
                        WindowComponent optionswindow, LabelComponent optionslabel, int optionswindowx, int optionswindowy, int optionswindowwidth, int optionswindowheight, String optionswindowtext)
   {


           this.window = window;
           this.windowx = windowx;
           this.windowy = windowy;
           this.windowheight = windowheight;
           this.windowwidth = windowwidth;

           this.window.Draw = OnDraw;
            

            this.newGame = newGame;

            this.newGametext = newGametext;
            this.newGametag = newGametag;
            this.newGamex = newGamex;
            this.newGamey = newGamey;
            this.newGamewidth = newGamewidth;
            this.newGameheight = newGameheight;
            this.levelToLoad = levelToLoad;
            this.newGame.Register();

            this.newGame.setDelegate(newGameClick);


            this.options = options;

            this.optionstext = optionstext;
            this.optionstag = optionstag;
            this.optionsx = optionsx;
            this.optionsy = optionsy;
            this.optionswidth = optionswidth;
            this.optionsheight = optionsheight;

            this.options.Register();
            this.options.setDelegate(optionsClick);



            this.credits = credits;

            this.creditstext = creditstext;
            this.creditstag = creditstag;
            this.creditsx = creditsx;
            this.creditsy = creditsy;
            this.creditswidth = creditswidth;
            this.creditsheight = creditsheight;

            this.credits.Register();
            this.credits.setDelegate(creditsClick);




            this.exit = exit;

            this.exittext = exittext;
            this.exittag = exittag;
            this.exitx = exitx;
            this.exity = exity;
            this.exitwidth = exitwidth;
            this.exitheight = exitheight;

            this.exit.Register();

            this.exit.setDelegate(exitClick);





          this.creditswindow = creditswindow;
          this.creditslabel=creditslabel;
          this.creditswindowx=creditswindowx;
          this.creditswindowy =creditswindowy;
          this.creditswindowwidth=creditswindowwidth;
          this.creditswindowheight =creditswindowheight;
          this.creditswindowtext =creditswindowtext;
          this.creditswindow.AddControl(creditslabel.Control);
          this.creditswindow.Unregister();

          this.creditswindowregistered = false;



          this.optionswindow = optionswindow;
          this.optionslabel = optionslabel;
          this.optionswindowx = optionswindowx;
          this.optionswindowy = optionswindowy;
          this.optionswindowwidth = optionswindowwidth;
          this.optionswindowheight = optionswindowheight;
          this.optionswindowtext = optionswindowtext;
          this.optionswindow.AddControl(optionslabel.Control);
          this.optionswindow.Unregister();

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
                this.SendEvent(new ChangeLevelEvent(levelToLoad), EventsSystem.Priority.High, GlobalGameObjects.GameController);
               
            }
        }
        /****************************************************************************/


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
            this.optionswindow.Unregister();
            optionswindowregistered = false;

            if (!creditswindowregistered)
            {
                
                this.creditswindow.Register();
            }
            if (creditswindowregistered)
            {

                this.creditswindow.Unregister();
            }


            creditswindowregistered = !creditswindowregistered;
        }
        /****************************************************************************/

        /****************************************************************************/
        /// optionsClick
        /****************************************************************************/
        public void optionsClick(object sender, EventArgs e)
        {
            this.creditswindow.Unregister();
            creditswindowregistered = false;

            if (!optionswindowregistered)
            {

                this.optionswindow.Register();
            }
            if (optionswindowregistered)
            {

                this.optionswindow.Unregister();
            }


            optionswindowregistered = !optionswindowregistered;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            spriteBatch.Draw(window.Texture, new Rectangle(windowx, windowy, windowwidth, windowheight), Color.White);
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

            creditswindow.ReleaseMe();
            creditslabel.ReleaseMe();

            optionswindow.ReleaseMe();
            optionslabel.ReleaseMe();
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
            data.newGamex = newGamex;
            data.newGamey = newGamey;
            data.newGamewidth = newGamewidth;
            data.newGameheight = newGameheight;
            data.levelToLoad = levelToLoad;


            data.optionstext = optionstext;
            data.optionstag = optionstag;
            data.optionsx = optionsx;
            data.optionsy = optionsy;
            data.optionswidth = optionswidth;
            data.optionsheight = optionsheight;


            data.creditstext = creditstext;
            data.creditstag = creditstag;
            data.creditsx = creditsx;
            data.creditsy = creditsy;
            data.creditswidth = creditswidth;
            data.creditsheight = creditsheight;


            data.exittext = exittext;
            data.exittag = exittag;
            data.exitx = exitx;
            data.exity = exity;
            data.exitwidth = exitwidth;
            data.exitheight = exitheight;



            data.windowx = windowx;
            data.windowy = windowy;
            data.windowheight = windowheight;
            data.windowwidth = windowwidth;



            data.creditswindowx = creditswindowx;
            data.creditswindowy = creditswindowy;
            data.creditswindowwidth = creditswindowwidth;
            data.creditswindowheight = creditswindowheight;
            data.creditswindowtext = creditswindowtext;


            data.optionswindowx = optionswindowx;
            data.optionswindowy = optionswindowy;
            data.optionswindowwidth = optionswindowwidth;
            data.optionswindowheight = optionswindowheight;
            data.optionswindowtext = optionswindowtext;


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

    }
    /********************************************************************************/


}
