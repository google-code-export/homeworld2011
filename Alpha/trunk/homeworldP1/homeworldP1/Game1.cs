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
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// 
        /// </summary>
        public GraphicsDeviceManager graphics;
        /// <summary>
        /// 
        /// </summary>
        SpriteBatch spriteBatch;
        /// <summary>
        /// 
        /// </summary>
        public GameSpace gamespace;
        /// <summary>
        /// 
        /// </summary>
        public Camera camera;
        /// <summary>
        /// 
        /// </summary>
        public Model TestModel001;

        /// <summary>
        /// 
        /// </summary>
        private TestStarSphere starsphere;
        /// <summary>
        /// 
        /// </summary>
        private MovableObject movableObject;

        /// <summary>
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            starsphere = new TestStarSphere(this);
            movableObject =  new MovableObject(this);
        }

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
            gamespace = new GameSpace(100, 50, Color.Red, this);
			//camera = new Camera(graphics, new Vector3(0, 50, -250), new Vector3(0, 0, 0));
			camera = new Camera(graphics, new Vector3(0, 50, -250), Quaternion.Identity);
            base.Initialize();
        }

        /// <summary>
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TestModel001 = Content.Load<Model>("model");
            starsphere.LoadContent();
            movableObject.LoadContent();
        }

        /// <summary>
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            camera.Update(gameTime);
            movableObject.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            base.Update(gameTime);
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gamespace.Draw(gameTime);
            starsphere.Draw(gameTime);
            movableObject.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
