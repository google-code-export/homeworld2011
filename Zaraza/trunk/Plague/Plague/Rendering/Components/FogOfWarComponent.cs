using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna;

using PlagueEngine.LowLevelGameFlow;

namespace PlagueEngine.Rendering.Components
{
    class FogOfWarComponent : GameObjectComponent
    {

        public Texture2D Texture { get; private set; }
        public RenderTarget2D Fog { get; private set; }
        public RenderTarget2D FogSmall { get; private set; }
        public float SpotSize = 1.0f;
        public float FogScale = 1.0f;
        public Vector2 FogSize;
        public bool Enabled;

        internal static Renderer renderer = null;
        private List<Vector2> spots = new List<Vector2>();

        public FogOfWarComponent(GameObjectInstance gameObject, 
                                 Texture2D texture, 
                                 float spotSize, 
                                 float fogScale, 
                                 Vector2 fogSize,
                                 bool enabled)
            : base(gameObject)
        {
            Texture  = texture;
            SpotSize = spotSize;
            FogSize  = fogSize;
            FogScale = fogScale;
            Enabled  = enabled;

            //renderer.fogOfWar = this;
            if (FogSize != Vector2.Zero && FogScale != 0)
            {
                Fog      = new RenderTarget2D(renderer.Device, (int)(FogSize.X / FogScale), (int)(FogSize.Y / FogScale), false, SurfaceFormat.Color, DepthFormat.None);
                FogSmall = new RenderTarget2D(renderer.Device, (int)(FogSize.X / FogScale), (int)(FogSize.Y / FogScale), false, SurfaceFormat.Color, DepthFormat.None,1,RenderTargetUsage.PreserveContents);

                renderer.Device.SetRenderTarget(Fog);
                renderer.Device.Clear(Color.Black);
                renderer.Device.SetRenderTarget(null);
            }
        }


        public void DrawSpot(Vector2 position)
        {
            spots.Add(position);
        }

        public void Update(SpriteBatch spriteBatch)
        {
            renderer.Device.SetRenderTarget(Fog);
            renderer.Device.Clear(Color.Black);
            spriteBatch.Begin();

            foreach (var spot in spots)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)(spot.X - SpotSize / 2),
                                                        (int)(spot.Y - SpotSize / 2),
                                                        (int)(SpotSize),
                                                        (int)(SpotSize)), Color.White);
            }

            spriteBatch.End();

            renderer.Device.SetRenderTarget(FogSmall);
            spriteBatch.Begin();

            foreach (var spot in spots)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)(spot.X - SpotSize / 2),
                                                        (int)(spot.Y - SpotSize / 2),
                                                        (int)(SpotSize),
                                                        (int)(SpotSize)), Color.White);
            }

            spriteBatch.End();


            renderer.Device.SetRenderTarget(null);
            spots.Clear();
        }

        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            renderer.fogOfWar = null;
            base.ReleaseMe();
        }
        /****************************************************************************/
    }
}
