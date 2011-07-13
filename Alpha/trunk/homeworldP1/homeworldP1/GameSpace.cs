using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace homeworldP1
{
    /// <summary>
    /// przestrzen w jakiej odbywac sie bedzie rozgrywka
    /// </summary>
    public class GameSpace : DrawableGameComponent
    {
        /// <summary>
        /// promien przestrzeni
        /// </summary>
        private float radius;
		/// <summary>
		/// 
		/// </summary>
        private Vector3 position;
        /// <summary>
        /// kolor granicy przestrzeni
        /// </summary>
        private Color color;
        /// <summary>
        /// 
        /// </summary>
        private BasicEffect effect;
        /// <summary>
        /// 
        /// </summary>
        List<VertexPositionColor> vertices;
        /// <summary>
        /// 
        /// </summary>
        VertexPositionColor[] vertexArray;
        /// <summary>
        /// 
        /// </summary>
        GraphicsDeviceManager graphics;
        /// <summary>
        /// 
        /// </summary>
        Game1 game;

        /// <summary>
        /// konstruktor jaki jest kazdy widzi
        /// </summary>
        /// <param name="radius">promien przestrzeni</param>
        /// <param name="height">wysokosc przestrzeni</param>
        /// <param name="color"> kolor kola ogranoiczajacego przestrzen</param>
        /// <param name="_game"></param>
        public GameSpace(float radius, float height, Color color, Game1 _game) : base(_game)
        {
            this.game = _game;
            this.radius = radius;
            this.position = new Vector3(0,height,0);
            this.color = color;
            this.graphics = game.graphics;
			this.Create();
        }

        /// <summary>
        /// aktualizacja kola ograniczajacego przestrzen
        /// </summary>
        public void Create()
        {
            this.position.Y = 0;
            vertices = new List<VertexPositionColor>();
            float angle = 0.0f;
            float jump = 0.1f;
            while (angle<6.30f)
            {
                vertices.Add(new VertexPositionColor(
                        new Vector3((float)Math.Sin(angle)*this.radius,
                                0,
                                (float)Math.Cos(angle) * this.radius),
                        this.color));
                angle += jump;
            }
            vertexArray = vertices.ToArray();
            effect = new BasicEffect(this.graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{

		}

        /// <summary>
        /// rysowanie koła ograniczającego przestrzen
        /// </summary>
		public override void Draw(GameTime gameTime)
		{
			effect.CurrentTechnique.Passes[0].Apply();
			effect.View = game.camera.View();
			effect.Projection = game.camera.Projection();
			effect.World = Matrix.CreateTranslation(position);
			game.graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertexArray,0,vertexArray.Length-1);
 
        }
    }
}