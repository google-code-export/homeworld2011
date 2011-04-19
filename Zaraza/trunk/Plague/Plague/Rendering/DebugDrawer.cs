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
        private bool                      drawHeightmapSkin = false;
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
        /// EnableHeightmapDrawing
        /****************************************************************************/
        public void EnableHeightmapDrawing()
        {
            drawHeightmapSkin = true;
        }
        /****************************************************************************/





        /****************************************************************************/
        /// DisableHeightmapDrawing
        /****************************************************************************/
        public void DisableHeightmapDrawing()
        {
            drawHeightmapSkin = false;
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
        public void Draw(Matrix view, Matrix projection)
        {
            if (!enabled) return;

            basicEffect = new BasicEffect(renderer.Device);

            this.basicEffect.AmbientLightColor = Vector3.One;
            this.basicEffect.View = view;
            this.basicEffect.Projection = projection;
            this.basicEffect.VertexColorEnabled = true;

            if (selectiveDrawing)
            {
                if (physicsManager.rigidBodies.ContainsKey(gameObjectID))
                {
                    RigidBodyComponent rbc = physicsManager.rigidBodies[gameObjectID];
                    AddShape(BodyRenderExtensions.GetLocalSkinWireframe(rbc.Skin));
                    Matrix skinWorld = rbc.Body.Orientation;
                    skinWorld.Translation = rbc.Body.Position;
                    basicEffect.World = skinWorld;
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                            vertexData.ToArray(),
                                                                            0,
                                                                            vertexData.Count - 1);
                    vertexData.Clear();
                }
                else if (physicsManager.collisionSkins.ContainsKey(gameObjectID))
                {
                    if (( physicsManager.collisionSkins[gameObjectID].GetType().Equals(typeof(Physics.Components.TerrainSkinComponent)) && drawHeightmapSkin) || 
                        (!physicsManager.collisionSkins[gameObjectID].GetType().Equals(typeof(Physics.Components.TerrainSkinComponent))))
                    {
                        CollisionSkinComponent csc = physicsManager.collisionSkins[gameObjectID];
                        AddShape(BodyRenderExtensions.GetLocalSkinWireframe(csc.Skin));
                        Matrix skinWorld = csc.Skin.NewOrient;
                        skinWorld.Translation = csc.Skin.NewPosition;
                        basicEffect.World = skinWorld;
                        basicEffect.CurrentTechnique.Passes[0].Apply();
                        renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                                vertexData.ToArray(),
                                                                                0,
                                                                                vertexData.Count - 1);
                        vertexData.Clear();
                    }
                }
            }




            if (!selectiveDrawing)
            {
                foreach (CollisionSkinComponent skin in physicsManager.collisionSkins.Values)
                {

                    if ((skin.GetType().Equals(typeof(Physics.Components.TerrainSkinComponent)) && drawHeightmapSkin) || (!skin.GetType().Equals(typeof(Physics.Components.TerrainSkinComponent))))
                    {
                        AddShape(BodyRenderExtensions.GetLocalSkinWireframe(skin.Skin));
                        Matrix skinWorld = skin.Skin.NewOrient;
                        skinWorld.Translation = skin.Skin.NewPosition;
                        basicEffect.World = skinWorld;
                        basicEffect.CurrentTechnique.Passes[0].Apply();
                        renderer.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                                                                                vertexData.ToArray(),
                                                                                0,
                                                                                vertexData.Count - 1);
                        vertexData.Clear();
                    }

                }


                foreach (RigidBodyComponent body in physicsManager.rigidBodies.Values)
                {

                    AddShape(BodyRenderExtensions.GetLocalSkinWireframe(body.Skin));
                    Matrix skinWorld = body.Body.Orientation;
                    skinWorld.Translation = body.Body.Position;
                    basicEffect.World = skinWorld;
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

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public bool IsEnabled
        {
            get { return enabled; }
        }
        /****************************************************************************/



        
    }
    /********************************************************************************/

}
/************************************************************************************/