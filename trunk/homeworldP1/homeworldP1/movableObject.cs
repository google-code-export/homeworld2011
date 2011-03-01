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
    public class MovableObject : DrawableGameComponent
    {
        /// <summary>
        /// 
        /// </summary>
        Model model;
        /// <summary>
        /// 
        /// </summary>
        Game1 game;
        /// <summary>
        /// 
        /// </summary>
        Vector3 position;
        /// <summary>
        /// 
        /// </summary>
        Logi logi;
        /// <summary>
        /// 
        /// </summary>
        bool selected;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_game"></param>
        public MovableObject(Game1 _game)
            : base(_game)
        {
            game = _game;
            logi = new Logi();
            logi.setTitle("logiObjektu");
            logi.Visible = true;
            position = new Vector3(0, 0, 0);
            selected = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadContent()
        {
            model = game.TestModel001;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateScale(1f, 1f, 1f) * Matrix.CreateTranslation(position);
                    effect.View = game.camera.View();
                    effect.Projection = game.camera.Projection();
                }
                mesh.Draw();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.X += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.X -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.Z += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.Z -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                position.Y += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                position.Y -= 1;
            }


            MouseState MS = Mouse.GetState();
            if (MS.LeftButton == ButtonState.Pressed)
            {
                BoundingSphere boundingSphere;
                boundingSphere = new BoundingSphere(position, 10);
				if (boundingSphere.Intersects(game.camera.GetMouseRay(new Vector2(MS.X, MS.Y), this.game.graphics.GraphicsDevice.Viewport)) != null)
				{
					selected = true;
				}
				else
				{
					if (selected)
					{
					}
					else
					{
						/*nothing*/
					}
				}
				logi.addText("left  : " + selected + "\n");
            }
            if (MS.RightButton == ButtonState.Pressed)
            {
                {
                    selected = false;
                }
                logi.addText("right : " + selected + "\n");
            }
        }
    }
}
