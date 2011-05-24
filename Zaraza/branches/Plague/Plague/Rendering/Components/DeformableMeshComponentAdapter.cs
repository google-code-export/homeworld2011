using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlagueEngine.Rendering.Components
{
    class DeformableMeshComponentAdapter : GameObjectComponent, IDeformable
    {
        private MeshComponent packedComponent;

        public DeformableMeshComponentAdapter(GameObjectInstance gameObject, 
                             MeshComponent comp)
            : base(gameObject)
    {
        packedComponent = comp;
    }

        private float logistic(double x)
        {
            return (float) (1.0/(1.0+Math.Exp(-x)));
        }

        public void deform()
        {
            List<Vector3> vertices = packedComponent.Model.VertexList;
            //List<Vector3> normals;
            Dictionary<uint, List<uint>> adjacency = packedComponent.Model.Adjacency;
            List<uint> affectedPoints = new List<uint>();
            Random rnd = new Random();
            Vector3 point = vertices[rnd.Next(packedComponent.Model.VertexCount)];
            Vector3 middle = Vector3.Divide(packedComponent.BoundingBox.Max - packedComponent.BoundingBox.Min, 2);
            Vector3 dir = middle - point;
            
            for (int i = 0; i < packedComponent.Model.VertexCount; i++)
            {
                double currDist = Vector3.Distance(vertices[i], point);
                if (currDist < 0.5)
                {
                    vertices[i] += Vector3.Multiply(dir, (float) 0.1 * logistic(currDist));
                    //affectedPoints.Add((uint) vertices.IndexOf(pt));
                }
            }
            packedComponent.Model.VertexBuffer.SetData(0, vertices.ToArray(), 0, packedComponent.Model.VertexCount, packedComponent.Model.VertexBuffer.VertexDeclaration.VertexStride);
        }
        public void deform(Vector3 dir, Vector3 point, double rad, double strength)
        {
            if (dir == null)
            {
                //Teoretycznie powinno wyliczać wektor do środka
                Vector3 middle = Vector3.Divide(packedComponent.BoundingBox.Max - packedComponent.BoundingBox.Min, 2);
                dir = middle - point;
            }
            List<Vector3> vertices = packedComponent.Model.VertexList;
            //List<Vector3> normals;
            Dictionary<uint, List<uint>> adjacency = packedComponent.Model.Adjacency;
            List<uint> affectedPoints = new List<uint>();
            for (int i = 0; i < packedComponent.Model.VertexCount; i++)
            {
                double currDist = Vector3.Distance(vertices[i], point);
                if (currDist < rad)
                {
                    vertices[i] += Vector3.Multiply(dir, (float) strength * logistic(currDist));
                    //affectedPoints.Add((uint) vertices.IndexOf(pt));
                }
            }
            packedComponent.Model.VertexBuffer.SetData(vertices.ToArray());
            

            /*foreach(Vector3 pt in packedComponent.Model.VertexList)
            {
                if (Vector3.Distance(pt, point) < rad)
                {
                    pt += Vector3.Multiply(dir, logistic(Vector3.Distance(pt, point)) );
                    //affectedPoints.Add((uint) vertices.IndexOf(pt));
                }
            }*/
            //VertexBuffer myBuf = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, packedComponent.Model.VertexCount, BufferUsage.WriteOnly);
            //packedComponent.Model.VertexBuffer = 
            
        }

        /****************************************************************************/
        /// Release Me
        /****************************************************************************/
        public override void ReleaseMe()
        {
            packedComponent.ReleaseMe();
            base.ReleaseMe();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Bounding Box
        /****************************************************************************/
        public BoundingBox BoundingBox
        {
            get
            {
                return packedComponent.BoundingBox;
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public PlagueEngineModel Model { get { return packedComponent.Model; } }
        public TexturesPack Textures { get { return packedComponent.Textures; } }
        public InstancingModes InstancingMode { get { return packedComponent.InstancingMode; } }
        /****************************************************************************/

    }
}
