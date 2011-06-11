using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Math;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;


namespace PlagueEngine.Physics
{
    class Cone: EventsSender
    {
      
        public CollisionSkin skin;
        public Body body;
        internal static PhysicsManager physicsManager;

        
        public bool wasted = false;
        GameObjectInstance reciever = null;

        List<GameObjectInstance> collisionObject = new List<GameObjectInstance>();
        public Cone(Vector3 position, Matrix orientation, Model coneModel,GameObjectInstance rec)
        {

            reciever = rec;
            body = new Body();
            skin = new CollisionSkin(body);
       
            body.CollisionSkin = skin;
            body.Immovable = true;



            TriangleMesh mesh = new TriangleMesh();

            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();

            ExtractData(vertexList, indexList, coneModel);

            mesh.CreateMesh(vertexList, indexList, 4, 1.0f);
            skin.AddPrimitive(mesh, new MaterialProperties(0.8f, 0.7f, 0.6f));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(skin);
            body.MoveTo(position, orientation);
            body.EnableBody();


            physicsManager.cones.Add(this);


            skin.callbackFn += HandleCollisionDetection;

        }


        /****************************************************************************/
        /// Handle Collision Detection
        /****************************************************************************/
        private bool HandleCollisionDetection(CollisionSkin owner, CollisionSkin collidee)
        {
            if (collidee.ExternalData != null)
            {
                int id = ((GameObjectInstance)collidee.ExternalData).ID;
                if (!collisionObject.Contains((GameObjectInstance)collidee.ExternalData))
                {
                    collisionObject.Add((GameObjectInstance)collidee.ExternalData);
                }
            }
            return false;
        }
        /****************************************************************************/


        public void Update()
        {
         
                body.DisableBody();
                PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
                wasted = true;
                if (collisionObject.Count > 0)
                {
                    this.SendEvent(new ConeTestEvent(collisionObject.ToList()), Priority.Normal, reciever);
                }
           
            
        }

        public void ExtractData(List<Vector3> vertices, List<TriangleVertexIndices> indices, Model model)
        {
            Matrix[] bones_ = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones_);
            foreach (ModelMesh mm in model.Meshes)
            {
                int offset = vertices.Count;
                Matrix xform = bones_[mm.ParentBone.Index];
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    Vector3[] a = new Vector3[mmp.NumVertices];
                    int stride = mmp.VertexBuffer.VertexDeclaration.VertexStride;
                    //mm.VertexBuffer.GetData<Vector3>(mmp.StreamOffset + mmp.BaseVertex * mmp.VertexStride, a, 0, mmp.NumVertices, mmp.VertexStride);  //XNA 4.0 change
                    mmp.VertexBuffer.GetData(mmp.VertexOffset * stride, a, 0, mmp.NumVertices, stride);

                    for (int i = 0; i != a.Length; ++i)
                        Vector3.Transform(ref a[i], ref xform, out a[i]);
                    vertices.AddRange(a);

                    //if (mm.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)      //XNA 4.0 change
                    if (mmp.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)
                        throw new Exception(String.Format("Model uses 32-bit indices, which are not supported."));

                    short[] s = new short[mmp.PrimitiveCount * 3];
                    //mm.IndexBuffer.GetData<short>(mmp.StartIndex * 2, s, 0, mmp.PrimitiveCount * 3);      //XNA 4.0 change
                    mmp.IndexBuffer.GetData(mmp.StartIndex * 2, s, 0, mmp.PrimitiveCount * 3);

                    JigLibX.Geometry.TriangleVertexIndices[] tvi = new JigLibX.Geometry.TriangleVertexIndices[mmp.PrimitiveCount];
                    for (int i = 0; i != tvi.Length; ++i)
                    {
                        tvi[i].I0 = s[i * 3 + 2] + offset;
                        tvi[i].I1 = s[i * 3 + 1] + offset;
                        tvi[i].I2 = s[i * 3 + 0] + offset;
                    }
                    indices.AddRange(tvi);
                }
            }
        }


    }
}
