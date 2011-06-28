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
using PlagueEngine.Input.Components;
using System.ComponentModel.Design;
using System.Drawing.Design;
using PlagueEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlagueEngine.Resources;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{


    /********************************************************************************/
    /// InGameMenu
    /********************************************************************************/
    class InGameMenu : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        KeyboardListenerComponent keyboard;
        MouseListenerComponent mouse;
        FrontEndComponent frame;
        internal static Renderer renderer;
        internal static ContentManager contentManager;

        ButtonComponent exitGamebutton;
        ButtonComponent resumeGamebutton;
        ButtonComponent exitMainMenuButton;
        ButtonComponent optionsbutton;



        public LabelComponent BrightnessLabel1;
        public LabelComponent BrightnessLabel2;
        public ButtonComponent BrightnessButton1;
        public ButtonComponent BrightnessButton2;

        public LabelComponent ContrastLabel1;
        public LabelComponent ContrastLabel2;
        public ButtonComponent ContrastButton1;
        public ButtonComponent ContrastButton2;

        public LabelComponent LanguageLabel1;
        public LabelComponent LanguageLabel2;
        public ButtonComponent LanguageButton1;
        public ButtonComponent LanguageButton2;

        public LabelComponent SSAOLabel1;
        public LabelComponent SSAOLabel2;
        public ButtonComponent SSAOButton1;

        ButtonComponent backbutton;
        /****************************************************************************/

        /// Init
        /****************************************************************************/
        public void Init(FrontEndComponent frame)
        {
            keyboard = new KeyboardListenerComponent(this, true);
            mouse = new MouseListenerComponent(this, true);
            keyboard.Modal = true;
            mouse.Modal = true;
            keyboard.SubscibeKeys(OnKey, Keys.Escape);
            this.frame = frame;
            frame.Draw = OnDraw;


            resumeGamebutton = new ButtonComponent("MainMenu.resumetext", 340, 160, 150, 45, "");
            optionsbutton = new ButtonComponent("MainMenu.optionstext", 340, 245, 150, 45, "");
            exitMainMenuButton = new ButtonComponent("MainMenu.mainmenutext", 340, 330, 150, 45, "");
            exitGamebutton = new ButtonComponent("MainMenu.exittext", 340, 415, 150, 45, "");


            this.BrightnessLabel1 = new LabelComponent("OptionsMenu.brightness_change", 260, 160, 50, 25);
            this.BrightnessLabel2 = new LabelComponent(Math.Round(renderer.Brightness, 2).ToString(), 500, 160, 50, 25);
            this.BrightnessButton1 = new ButtonComponent("<", 400, 165, 20, 25, "");
            this.BrightnessButton2 = new ButtonComponent(">", 450, 165, 20, 25, "");
            this.BrightnessButton1.SetDelegate(click);
            this.BrightnessButton2.SetDelegate(click);

            this.LanguageButton1 = new ButtonComponent("<", 400, 310, 20, 25, "");
            this.LanguageButton2 = new ButtonComponent(">", 450, 310, 20, 25, "");
            this.LanguageButton1.SetDelegate(click);
            this.LanguageButton2.SetDelegate(click);
            this.LanguageLabel1 = new LabelComponent("OptionsMenu.language_change", 260, 305, 50, 25);
            this.LanguageLabel2 = new LabelComponent(GlobalGameObjects.StringManager.Language, 500, 305, 50, 25);

            this.ContrastLabel1 = new LabelComponent("OptionsMenu.contrast_change", 260, 210, 50, 25);
            this.ContrastLabel2 = new LabelComponent(Math.Round(renderer.Contrast, 2).ToString(), 500, 210, 50, 25);
            this.ContrastButton1 = new ButtonComponent("<", 400, 215, 20, 25, "");
            this.ContrastButton2 = new ButtonComponent(">", 450, 215, 20, 25, "");
            this.ContrastButton1.SetDelegate(click);
            this.ContrastButton2.SetDelegate(click);



            this.SSAOLabel1 = new LabelComponent("OptionsMenu.ssao", 260, 260, 50, 25);
            this.SSAOLabel2 = new LabelComponent(renderer.ssaoEnabled ? "OptionsMenu.on" : "OptionsMenu.off", 500, 260, 50, 25);
            this.SSAOButton1 = new ButtonComponent(renderer.ssaoEnabled ? "OptionsMenu.toggle_off" : "OptionsMenu.toggle_on", 400, 265, 70, 25, "");
            this.SSAOButton1.SetDelegate(click);

            backbutton = new ButtonComponent("OptionsMenu.back2", 340, 430, 150, 45, "");
            backbutton.SetDelegate(click);

            resumeGamebutton.Register();
            optionsbutton.Register();
            exitMainMenuButton.Register();
            exitGamebutton.Register();


            resumeGamebutton.SetDelegate(click);
            optionsbutton.SetDelegate(click);
            exitMainMenuButton.SetDelegate(click);
            exitGamebutton.SetDelegate(click);

        }
        /****************************************************************************/


        public void click(object sender, EventArgs e)
        {

            if (sender == resumeGamebutton.Control)
            {
                this.Broadcast(new InGameMenuClose(), EventsSystem.Priority.Normal);
                this.SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);
            }
            if (sender == exitGamebutton.Control)
            {
                this.SendEvent(new ExitGameEvent(), EventsSystem.Priority.High, GlobalGameObjects.GameController);  
            }
            if (sender == exitMainMenuButton.Control)
            {
                this.SendEvent(new ChangeLevelEvent("Menu.lvl"), EventsSystem.Priority.High, GlobalGameObjects.GameController);
           
            }


            if (sender == backbutton.Control)
            {
                resumeGamebutton.Register();
                optionsbutton.Register();
                exitMainMenuButton.Register();
                exitGamebutton.Register();

                BrightnessButton1.Unregister();
                BrightnessLabel1.Unregister();
                BrightnessButton2.Unregister();
                BrightnessLabel2.Unregister();


                ContrastButton1.Unregister();
                ContrastLabel1.Unregister();
                ContrastButton2.Unregister();
                ContrastLabel2.Unregister();


                SSAOButton1.Unregister();
                SSAOLabel1.Unregister();
                SSAOLabel2.Unregister();



                LanguageButton1.Unregister();
                LanguageButton2.Unregister();
                LanguageLabel1.Unregister();
                LanguageLabel2.Unregister();
                backbutton.Unregister();
            }
            if (sender == optionsbutton.Control)
            {
                resumeGamebutton.Unregister();
                optionsbutton.Unregister();
                exitMainMenuButton.Unregister();
                exitGamebutton.Unregister();

                BrightnessButton1.Register();
                BrightnessLabel1.Register();
                BrightnessButton2.Register();
                BrightnessLabel2.Register();


                ContrastButton1.Register();
                ContrastLabel1.Register();
                ContrastButton2.Register();
                ContrastLabel2.Register();


                SSAOButton1.Register();
                SSAOLabel1.Register();
                SSAOLabel2.Register();



                LanguageButton1.Register();
                LanguageButton2.Register();
                LanguageLabel1.Register();
                LanguageLabel2.Register();
                backbutton.Register();
            }


            RenderConfig config = renderer.CurrentConfiguration;

            if (sender == BrightnessButton1.Control)
            {
                if ((Math.Round(renderer.Brightness - 0.05f, 2)) >= -10)
                {
                    renderer.Brightness -= 0.05f;
                    BrightnessLabel2.Text = (float.Parse(BrightnessLabel2.Text) - 0.05f).ToString();
                }
            }

            
            if (sender == LanguageButton2.Control || sender == LanguageButton1.Control)
            {
                if (GlobalGameObjects.StringManager.Language.Equals("English"))
                {
                    GlobalGameObjects.StringManager.Language = "Polski";
                }
                else
                {
                    GlobalGameObjects.StringManager.Language = "English";
                }
                LanguageLabel2.Text = GlobalGameObjects.StringManager.Language;
                resumeGamebutton.Refresh();
                optionsbutton.Refresh();
                exitMainMenuButton.Refresh();
                exitGamebutton.Refresh();

                backbutton.Refresh();

                BrightnessButton1.Refresh();
                BrightnessLabel1.Refresh();
                BrightnessButton2.Refresh();
                BrightnessLabel2.Refresh();

                ContrastButton1.Refresh();
                ContrastLabel1.Refresh();
                ContrastButton2.Refresh();
                ContrastLabel2.Refresh();


                SSAOButton1.Refresh();
                SSAOLabel1.Refresh();
                SSAOLabel2.Refresh();


                LanguageButton1.Refresh();
                LanguageButton2.Refresh();
                LanguageLabel1.Refresh();
                LanguageLabel2.Refresh();
            }

            if (sender == BrightnessButton2.Control)
            {
                if ((Math.Round(renderer.Brightness + 0.05f, 2)) <= 10)
                {
                    renderer.Brightness += 0.05f;
                    BrightnessLabel2.Text = (float.Parse(BrightnessLabel2.Text) + 0.05f).ToString();
                }
            }


            if (sender == ContrastButton1.Control)
            {
                if ((Math.Round(renderer.Contrast - 0.05f, 2)) >= 0)
                {
                    renderer.Contrast -= 0.05f;
                    ContrastLabel2.Text = (float.Parse(ContrastLabel2.Text) - 0.05f).ToString();
                }
            }
            if (sender == ContrastButton2.Control)
            {
                if ((Math.Round(renderer.Contrast + 0.05f, 2)) <= 10)
                {
                    renderer.Contrast += 0.05f;
                    ContrastLabel2.Text = (float.Parse(ContrastLabel2.Text) + 0.05f).ToString();

                    contentManager.SaveConfiguration(config);
                }
            }

            if (sender == SSAOButton1.Control)
            {
                if (renderer.ssaoEnabled == true)
                {
                    renderer.ssaoEnabled = false;
                    config.SSAO = false;
                    SSAOLabel2.Text = "OptionsMenu.off";
                }
                else
                {
                    renderer.ssaoEnabled = true;
                    config.SSAO = true;
                    SSAOLabel2.Text = "OptionsMenu.on";
                    contentManager.SaveConfiguration(config);
                }
            }





            contentManager.SaveConfiguration(config);



        }

        
        /****************************************************************************/
        /// On Key
        /****************************************************************************/
        private void OnKey(Keys key, ExtendedKeyState state)
        {
            if (key == Keys.Escape && state.WasPressed())
            {
                this.SendEvent(new DestroyObjectEvent(this.ID), EventsSystem.Priority.Normal, GlobalGameObjects.GameController);

            }
        }
        /****************************************************************************/

        /****************************************************************************/
        /// OnDraw
        /****************************************************************************/
        private void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {

            spriteBatch.Draw(frame.Texture, new Rectangle(0, 0, screenWidth, screenHeight), new Rectangle(50, 50, 2, 2), Color.Wheat);
            spriteBatch.Draw(frame.Texture, new Rectangle(screenWidth / 2 - frame.Texture.Width / 2, screenHeight / 2 - frame.Texture.Height / 2, frame.Texture.Width, frame.Texture.Height), Color.Wheat);
                

        }


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            keyboard.Modal = false;
            mouse.Modal = false;

            resumeGamebutton.ReleaseMe();
            exitMainMenuButton.ReleaseMe();
            exitGamebutton.ReleaseMe();
            optionsbutton.ReleaseMe();

            keyboard.ReleaseMe();
            frame.ReleaseMe();


            backbutton.ReleaseMe();

            LanguageLabel2.ReleaseMe();
            LanguageLabel1.ReleaseMe();
            LanguageButton2.ReleaseMe();
            LanguageButton1.ReleaseMe();

            BrightnessButton1.ReleaseMe();
            BrightnessLabel1.ReleaseMe();
            BrightnessButton2.ReleaseMe();
            BrightnessLabel2.ReleaseMe();


            ContrastButton1.ReleaseMe();
            ContrastLabel1.ReleaseMe();
            ContrastButton2.ReleaseMe();
            ContrastLabel2.ReleaseMe();



            SSAOButton1.ReleaseMe();
            SSAOLabel1.ReleaseMe();
            SSAOLabel2.ReleaseMe();


        }
        /********************************************************************************/




        /********************************************************************************/
        /// GetData
        /********************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            InGameMenuData data = new InGameMenuData();
            GetData(data);

            return data;
        }
        /********************************************************************************/



    }



    /********************************************************************************/
    /// InGameMenu
    /********************************************************************************/
    [Serializable]
    public class InGameMenuData : GameObjectInstanceData
    {

        public InGameMenuData()
        {
            Type = typeof(InGameMenu);
        }

    }
    /********************************************************************************/


}
