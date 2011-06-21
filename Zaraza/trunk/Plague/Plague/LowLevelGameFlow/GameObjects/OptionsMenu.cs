using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.GUI.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;

using PlagueEngine.Resources;
using PlagueEngine.Rendering;
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class OptionsMenu : GameObjectInstance
    {
        public ButtonComponent BackButton { get; set; }
        //public ListComponent ChooseLanguage { get; set; }
        
        //public LabelComponent LanguageChoiceLabel {get; set; }
        //public LabelComponent TitleLabel { get; set; }

        private bool isRendered = false;

        internal static Renderer renderer;
        internal static ContentManager contentManager;
 
        public LabelComponent BrightnessLabel1;
        public LabelComponent BrightnessLabel2;
        public ButtonComponent BrightnessButton1;
        public ButtonComponent BrightnessButton2;

        public LabelComponent ContrastLabel1;
        public LabelComponent ContrastLabel2;
        public ButtonComponent ContrastButton1;
        public ButtonComponent ContrastButton2;


        public LabelComponent SSAOLabel1;
        public LabelComponent SSAOLabel2;
        public ButtonComponent SSAOButton1;

        public LabelComponent FullScreenLabel1;
        public LabelComponent FullScreenLabel2;
        public ButtonComponent FullScreenButton1;


        public LabelComponent ScreenSizeLabel1;
        public LabelComponent ScreenSizeLabel2;
        public ButtonComponent ScreenSizeButton1;
        public ButtonComponent ScreenSizeButton2;

        List<int[]> resolutions = renderer.EnumerateAvailableResolutions();
        int current;

        public void Enable()
        {
            isRendered = true;
            BackButton.Register();

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

            FullScreenButton1.Register();
            FullScreenLabel1.Register();
            FullScreenLabel2.Register();



            ScreenSizeButton1.Register();
            ScreenSizeLabel1.Register();
            ScreenSizeButton2.Register();
            ScreenSizeLabel2.Register();
        }

        public void Disable()
        {
            isRendered = false;
            BackButton.Unregister();

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

            FullScreenButton1.Unregister();
            FullScreenLabel1.Unregister();
            FullScreenLabel2.Unregister();

            ScreenSizeButton1.Unregister();
            ScreenSizeLabel1.Unregister();
            ScreenSizeButton2.Unregister();
            ScreenSizeLabel2.Unregister();



            //ChooseLanguage.Unregister();
            //LanguageChoiceLabel.Unregister();
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        FrontEndComponent frame;

        public OptionsMenu(string languageLabelKey, string title, string buttonLabelKey, int x, int y, int width, int height, FrontEndComponent frame)
        {
            //this.ChooseLanguage = new ListComponent(new List<string> { "OptionsMenu.language_english", "OptionsMenu.language_polish" },
            //                                             x + 150,
            //                                             y + 80,
            //                                             200, 170, Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single);
            //this.LanguageChoiceLabel = new LabelComponent(languageLabelKey, x, y , 100, 70);
            this.BackButton          = new ButtonComponent("Save", 400, 200 + height - 250, 120, 50, "");

     

            this.BrightnessLabel1 = new LabelComponent("Brightness", 300, 60, 50, 25);
            this.BrightnessLabel2 = new LabelComponent(Math.Round(renderer.Brightness, 2).ToString(), 550, 60, 50, 25);
            this.BrightnessButton1 = new ButtonComponent("<", 450, 65, 20, 25,"");
            this.BrightnessButton2 = new ButtonComponent(">", 500, 65, 20, 25, "");
            this.BrightnessButton1.setDelegate(click);
            this.BrightnessButton2.setDelegate(click);
           


            this.ContrastLabel1 = new LabelComponent("Contrast", 300, 110, 50, 25);
            this.ContrastLabel2 = new LabelComponent(Math.Round(renderer.Contrast, 2).ToString(), 550, 110, 50, 25);
            this.ContrastButton1 = new ButtonComponent("<", 450, 115, 20, 25, "");
            this.ContrastButton2 = new ButtonComponent(">", 500, 115, 20, 25, "");
            this.ContrastButton1.setDelegate(click);
            this.ContrastButton2.setDelegate(click);


          
            this.SSAOLabel1 = new LabelComponent("SSAO", 300, 160, 50, 25);
            this.SSAOLabel2 = new LabelComponent(renderer.ssaoEnabled ? "ON" : "OFF", 550, 160, 50, 25);
            this.SSAOButton1 = new ButtonComponent("TOGGLE", 450, 165, 70, 25, "");
            this.SSAOButton1.setDelegate(click);



            this.FullScreenLabel1 = new LabelComponent("FullScreen", 300, 210, 50, 25);
            this.FullScreenLabel2 = new LabelComponent(renderer.CurrentConfiguration.FullScreen ? "ON" : "OFF", 550, 210, 50, 25);
            this.FullScreenButton1 = new ButtonComponent("TOGGLE", 450, 215, 70, 25, "");
            this.FullScreenButton1.setDelegate(click);



            FindCurrentResolution();

            this.ScreenSizeLabel1 = new LabelComponent("ScreenSize", 300, 260, 50, 25);
            this.ScreenSizeLabel2 = new LabelComponent(resolutions[current][0].ToString() + "X" + resolutions[current][1].ToString(), 550, 260, 50, 25);
            this.ScreenSizeButton1 = new ButtonComponent("<", 450, 265, 20, 25, "");
            this.ScreenSizeButton2 = new ButtonComponent(">", 500, 265, 20, 25, "");
            this.ScreenSizeButton1.setDelegate(click);
            this.ScreenSizeButton2.setDelegate(click);

            this.frame = frame;
            
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }



        private void FindCurrentResolution()
        {
            int width = renderer.CurrentConfiguration.Width;
            int height = renderer.CurrentConfiguration.Height;
            int i = -1;
            foreach (int[] res in resolutions)
            {
                i++;

                if (res[0] == width && res[1] == height)
                {
                    current = i;
                    break;
                }
            }
        }

        public void click(object sender, EventArgs e)
        {
            RenderConfig config = renderer.CurrentConfiguration;

            if (sender == BrightnessButton1.Control)
            {
                if ((Math.Round(renderer.Brightness - 0.05f,2)) >= -10)
                {
                    renderer.Brightness -= 0.05f;
                    BrightnessLabel2.Text = (float.Parse(BrightnessLabel2.Text) - 0.05f).ToString();
                }
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
                    SSAOLabel2.Text = "OFF";
                }
                else
                {
                    renderer.ssaoEnabled = true;
                    config.SSAO = true;
                    SSAOLabel2.Text = "ON";
                    contentManager.SaveConfiguration(config);
                }
            }

            if (sender == FullScreenButton1.Control)
            {
                Diagnostics.PushLog(renderer.CurrentConfiguration.FullScreen.ToString());
                if (FullScreenLabel2.Text == "ON")
                {
                 
                    config.FullScreen = false;
                    FullScreenLabel2.Text = "OFF";
                }
                else
                {
                    config.FullScreen = true;
                    FullScreenLabel2.Text = "ON";
                }
            }

            if (sender == ScreenSizeButton1.Control)
            {
                if (current > 0)
                {
                    current--;
                    this.ScreenSizeLabel2.Text = resolutions[current][0].ToString() + "X" + resolutions[current][1].ToString();
                    config.Width = resolutions[current][0];
                    config.Height = resolutions[current][1];
                }
            }

            if (sender == ScreenSizeButton2.Control)
            {
                if (current < (resolutions.Count-1))
                {
                    current++;
                    this.ScreenSizeLabel2.Text = resolutions[current][0].ToString() + "X" + resolutions[current][1].ToString();
                   config.Width = resolutions[current][0];
                    config.Height = resolutions[current][1];
                }
            }


            contentManager.SaveConfiguration(config);
        }
        public void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            if (isRendered)
            {
                spriteBatch.Draw(frame.Texture, new Rectangle(X, Y, Width, Height), Color.White);
            }
        }


        /********************************************************************************/
        /// Release Components
        /********************************************************************************/
        public override void ReleaseComponents()
        {
            frame.ReleaseMe();
            BackButton.ReleaseMe();



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


            FullScreenButton1.ReleaseMe();
            FullScreenLabel1.ReleaseMe();
            FullScreenLabel2.ReleaseMe();



            ScreenSizeButton1.ReleaseMe();
            ScreenSizeLabel1.ReleaseMe();
            ScreenSizeButton2.ReleaseMe();
            ScreenSizeLabel2.ReleaseMe();
            //ChooseLanguage.ReleaseMe();
            //LanguageChoiceLabel.ReleaseMe();
        }
        /********************************************************************************/
    }
}
