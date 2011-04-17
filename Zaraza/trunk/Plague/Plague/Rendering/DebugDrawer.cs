using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JigLibX.Physics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PlagueEngine.Physics;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// Debug Drawer
    /********************************************************************************/
    class DebugDrawer
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        private BasicEffect               basicEffect    = null;
        private List<VertexPositionColor> vertexData     = null;
        private Renderer                  renderer       = null;
        private PhysicsManager            physicsManager = null;
        private bool                      enabled        = false;
        private bool                      selectiveDrawing = false;
        private uint                      gameObjectID;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public DebugDrawer(Renderer renderer,PhysicsManager physicsManager)
        {
            this.renderer       = renderer;
            this.physicsManager = physicsManager;

            vertexData  = new List<VertexPositionColor>();
        }
        /****************************************************************************/



        /****************************************************************************/
        /// StartSelectiveDrawing
        /****************************************************************************/
        public void StartSelectiveDrawing(uint gameObjectID)
        {
          selectiveDrawing=true;
          this.gameObjectID=gameObjectID;
        }
        /****************************************************************************/



        /****************************************************************************/
        /// StopSelectiveDrawing
        /****************************************************************************/
        public void StopSelectiveDrawing()
        {
            selectiveDrawing = false;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// Draw
        /****************************************************************************/
        public void Draw(Matrix view,Matrix projection)
        {
            
            
            basicEffect = new BasicEffect(renderer.Device);

            this.basicEffect.AmbientLightColor = Vector3.One;
            this.basicEffect.View = view;
            this.basicEffect.Projection = projection;
            this.basicEffect.VertexColorEnabled = true;


            foreach (CollisionSkinComponent skin in physicsManager.collisionSkins)
            {
                if (enabled || (!enabled &&  selectiveDrawing && ((uint)(skin.Skin.ExternalData) == gameObjectID)))
                {
                    AddShape(BodyRenderExtensions.GetLocalSkinWireframe(skin.Skin));
                    basicEffect.World = skin.GameObject.World;
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                            vertexData.ToArray(),
                                                                            0,
                                                                            vertexData.Count - 1);
                    vertexData.Clear();
                }
            }


            foreach (RigidBodyComponent body in physicsManager.rigidBodies)
            {
                if (enabled || (!enabled && selectiveDrawing && ((uint)(body.Skin.ExternalData) == gameObjectID)))
                {
                    AddShape(BodyRenderExtensions.GetLocalSkinWireframe(body.Skin));
                    basicEffect.World = body.GameObject.World;
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                            vertexData.ToArray(),
                                                                            0,
                                                                            vertexData.Count - 1);
                    vertexData.Clear();
                }
            }

        }
        /****************************************************************************/


        /****************************************************************************/
        /// Add Shape
        /****************************************************************************/
        private void AddShape(VertexPositionColor[] shape)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                vertexData.Add(shape[i]);
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Enable
        /****************************************************************************/
        public void Enable()
        {
            enabled = true;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Disable
        /****************************************************************************/
        public void Disable()
        {
            enabled = false;
        }
        /****************************************************************************/
        
    }
    /********************************************************************************/

}
/************************************************************************************/