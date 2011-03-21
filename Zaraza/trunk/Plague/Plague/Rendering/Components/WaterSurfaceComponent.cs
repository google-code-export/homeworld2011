using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.Rendering;
using PlagueEngine.Rendering.Components;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.TimeControlSystem;

// TODO: tworzyć nowe RenderTarget2D za każdym razem gdy zmienia się wielkość ekranu

/************************************************************************************/
/// PlagueEngine.Rendering.Components
/************************************************************************************/
namespace PlagueEngine.Rendering.Components
{

    /********************************************************************************/
    /// WaterSurfaceComponent
    /********************************************************************************/
    class WaterSurfaceComponent : RenderableComponent
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private RenderTarget2D      renderTarget        = null;
        private VertexBuffer        vertexBuffer        = null;
        private IndexBuffer         indexBuffer         = null;
        private float               width               = 0;
        private float               length              = 0;
        private float               level               = 0;
        private Vector3             waterColor          = Vector3.Zero;
        private float               colorAmount         = 0;
        private float               waveLength          = 0;
        private float               waveHeight          = 0;
        private float               waveSpeed           = 0;
        private Texture2D           normalMap           = null;

        private RasterizerState     rasterizerState     = new RasterizerState();
        private Matrix              reflectionMatrix    = Matrix.Identity;
        private float               surfacePosition     = 0;
        private Clock               clock               = TimeControl.CreateClock();
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public WaterSurfaceComponent(GameObjectInstance gameObject,
                                     Renderer           renderer,
                                     float              width,
                                     float              length,
                                     float              level,
                                     Vector3            waterColor,
                                     float              colorAmount,
                                     float              waveLength,
                                     float              waveHeight,
                                     float              waveSpeed,                                  
                                     Texture2D          normalMap,
                                     Effect             effect) : base(gameObject,renderer,effect)
        {
            this.width       = width;
            this.length      = length;
            this.level       = level;
            this.waterColor  = waterColor;
            this.colorAmount = colorAmount;
            this.normalMap   = normalMap;
            this.waveLength  = waveLength;
            this.waveHeight  = waveHeight;
            this.waveSpeed   = waveSpeed;
            
            this.renderTarget = new RenderTarget2D(device,
                                                   device.Viewport.Width,
                                                   device.Viewport.Height,
                                                   true,
                                                   SurfaceFormat.Color,
                                                   DepthFormat.Depth24Stencil8);

            this.preRender = true;

            rasterizerState.CullMode = CullMode.CullClockwiseFace;

            surfacePosition  = gameObject.World.Translation.Y + level;
            
            reflectionMatrix = Matrix.CreateTranslation(0f, -surfacePosition, 0f) *
                               Matrix.CreateScale(1f, -1f, 1f) *
                               Matrix.CreateTranslation(0f, surfacePosition, 0f);

            ComputeMesh();
            SetupEffect();            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Compute Mesh
        /****************************************************************************/
        private void ComputeMesh()
        { 
            if (vertexBuffer != null) vertexBuffer.Dispose();
            if (indexBuffer  != null) indexBuffer.Dispose();


            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            vertices[0].Position          = new Vector3(0, 0, 0);
            vertices[0].TextureCoordinate = new Vector2(0, 0);

            vertices[1].Position          = new Vector3(width, 0, 0);
            vertices[1].TextureCoordinate = new Vector2(width/1000, 0);
            
            vertices[2].Position          = new Vector3(0, 0, length);
            vertices[2].TextureCoordinate = new Vector2(0, length/1000);

            vertices[3].Position          = new Vector3(width, 0, length);
            vertices[3].TextureCoordinate = new Vector2(width/1000, length/1000);

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(vertices);

            int[] indices = new int[] { 0, 1, 2, 3 };
            indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, 4, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public override void Draw(Matrix view, Matrix projection)
        {
            effect.Parameters["World"].SetValue(gameObject.World * Matrix.CreateTranslation(0, level, 0));
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["Time"].SetValue((float)clock.Time.TotalSeconds);
            
            effect.CurrentTechnique.Passes[0].Apply();
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, 4, 0, 2);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Render Reflection
        /****************************************************************************/
        public override void PreRender(Matrix view,Matrix projection)
        {
            Matrix reflectedView =  reflectionMatrix * view;
                        
            effect.Parameters["ReflectedView"].SetValue(reflectedView);

            Vector4 clipPlane = new Vector4(0, 1, 0, -surfacePosition + 10);

            RasterizerState defaultRasterizerState = device.RasterizerState;
            device.RasterizerState = rasterizerState;

            device.SetRenderTarget(renderTarget);

            device.Clear(Color.CornflowerBlue);
                
            foreach (RenderableComponent renderableComponent in renderer.renderableComponents)
            {
                if (renderableComponent != this)
                {
                    renderableComponent.SetClipPlane(clipPlane);
                    renderableComponent.Draw(reflectedView, projection);
                    renderableComponent.DisableClipPlane();
                }
            }

            device.SetRenderTarget(null);
            device.RasterizerState = defaultRasterizerState;

            effect.Parameters["ReflectionMap"].SetValue(renderTarget);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Setup Effect
        /****************************************************************************/
        private void SetupEffect()
        {
            effect.Parameters["Color"].SetValue(waterColor);
            effect.Parameters["ColorAmount"].SetValue(colorAmount);
            effect.Parameters["ViewportWidth"].SetValue((float)device.Viewport.Width);
            effect.Parameters["ViewportHeight"].SetValue((float)device.Viewport.Height);
            effect.Parameters["WaterNormalMap"].SetValue(normalMap);
            effect.Parameters["WaveLength"].SetValue(waveLength);
            effect.Parameters["WaveHeight"].SetValue(waveHeight);
            effect.Parameters["WaveSpeed"].SetValue(waveSpeed);
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public float    Width       { get { return width;       } }
        public float    Length      { get { return length;      } }
        public float    Level       { get { return level;       } }
        public Vector3  WaterColor  { get { return waterColor;  } }
        public float    ColorAmount { get { return colorAmount; } }

        public float  WaveLength    { get { return waveLength;     } }
        public float  WaveHeight    { get { return waveHeight;     } }
        public float  WaveSpeed     { get { return waveSpeed;      } }
        public String NormalMap     { get { return normalMap.Name; } }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/