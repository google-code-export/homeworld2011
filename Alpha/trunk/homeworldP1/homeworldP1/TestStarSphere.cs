using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace homeworldP1
{
    /// <summary>
    /// 
    /// </summary>
    public class TestStarSphere : DrawableGameComponent
    {
        Game1 game;
        Model star;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_game"></param>
        public TestStarSphere(Game1 _game): base (_game)
        {
            game = _game;
        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadContent()
        {
            star = game.TestModel001;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            for (float j = 0; j < MathHelper.TwoPi; j += MathHelper.Pi / 16)
            {
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 16)
                {
                    foreach (ModelMesh mesh in star.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = Matrix.CreateScale(0.05f, 0.05f, 0.05f) * 
                                Matrix.CreateTranslation(
                                (float)Math.Sin(i) * (float)Math.Sin(j) * 100,
                                (float)Math.Cos(j) * 100,
                                (float)Math.Cos(i) * (float)Math.Sin(j) * 100);
                            effect.View = game.camera.View();
                            effect.Projection = game.camera.Projection();
                        }
                        mesh.Draw();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
