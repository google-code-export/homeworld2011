using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.GUI.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlagueEngine.Rendering.Components;

namespace PlagueEngine.LowLevelGameFlow.GameObjects
{
    class OptionsMenu : GameObjectInstance
    {
        public ButtonComponent BackButton { get; set; }
        public ListComponent ChooseLanguage { get; set; }
        
        public LabelComponent LanguageChoiceLabel {get; set; }
        public LabelComponent TitleLabel { get; set; }

        private bool isRendered = false;

        public void Enable()
        {
            isRendered = true;
            BackButton.Register();
            ChooseLanguage.Register();
            LanguageChoiceLabel.Register();            
        }

        public void Disable()
        {
            isRendered = false;
            BackButton.Unregister();
            ChooseLanguage.Unregister();
            LanguageChoiceLabel.Unregister();
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        FrontEndComponent frame;

        public OptionsMenu(string languageLabelKey, string title, string buttonLabelKey, int x, int y, int width, int height, FrontEndComponent frame)
        {
            this.ChooseLanguage = new ListComponent(new List<string> { "OptionsMenu.language_english", "OptionsMenu.language_polish" },
                                                         x + 150,
                                                         y + 80,
                                                         200, 170, Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single);
            this.LanguageChoiceLabel = new LabelComponent(languageLabelKey, x, y , 100, 70);
            this.BackButton          = new ButtonComponent(buttonLabelKey, x, y + height - 250, 80, 50, "");
            this.frame = frame;
            
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public void OnDraw(SpriteBatch spriteBatch, ref Matrix ViewProjection, int screenWidth, int screenHeight)
        {
            if (isRendered)
            {
                spriteBatch.Draw(frame.Texture, new Rectangle(X, Y, Width, Height), Color.White);
            }
        }
    }
}
