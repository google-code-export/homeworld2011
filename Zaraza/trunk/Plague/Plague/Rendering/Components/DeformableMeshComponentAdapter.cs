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
        public bool Enabled { protected set { ; } get { return this.packedComponent.Enabled; } }
        public bool Static { protected set { ; } get { return this.packedComponent.Static; } }

        public DeformableMeshComponentAdapter(GameObjectInstance gameObject,
                             MeshComponent comp)
            : base(gameObject)
        {
            packedComponent = comp;
        }

        private float logistic(double x)
        {
            return (float)(1.0 / (1.0 + Math.Exp(-x)));
        }

        public void deform()
        {
            List<Vector3> vertices = packedComponent.Model.VertexList;
            Random rnd = new Random();
            Vector3 point = vertices[rnd.Next(packedComponent.Model.VertexCount)];
            Vector3 middle = Vector3.Divide(packedComponent.BoundingBox.Max - packedComponent.BoundingBox.Min, 2);
            Vector3 dir = middle - point;

            for (int i = 0; i < packedComponent.Model.VertexCount; i++)
            {
                double currDist = Vector3.Distance(vertices[i], point);
                if (currDist < 3)
                {
                    vertices[i] += Vector3.Multiply(dir, 0.1f * logistic(currDist));
                }
            }
            packedComponent.Model.VertexBuffer.SetData(0, vertices.ToArray(), 0, packedComponent.Model.VertexCount, packedComponent.Model.VertexBuffer.VertexDeclaration.VertexStride);
        }

        

        public void deform(Vector3 hitPoint)
        {
            List<Vector3> vertices = packedComponent.Model.VertexList;
            Vector3 point = vertices[0];
            double currDist = 0.0;
            double smallestDist = Vector3.Distance(point, hitPoint);
            foreach (Vector3 pt in vertices)
            {
                currDist = Vector3.Distance(pt, hitPoint);
                if (currDist < smallestDist)
                {
                    smallestDist = currDist;
                    point = pt;
                }
            }

            Vector3 middle = Vector3.Divide(packedComponent.BoundingBox.Max - packedComponent.BoundingBox.Min, 2);
            Vector3 direction = middle - point;
            for (int i = 0; i < packedComponent.Model.VertexCount; i++)
            {
                currDist = Vector3.Distance(vertices[i], point);
                if (currDist < 3)
                {
                    vertices[i] += Vector3.Multiply(direction, 0.1f * logistic(currDist));
                }
            }
            packedComponent.Model.VertexBuffer.SetData(0, vertices.ToArray(), 0, packedComponent.Model.VertexCount, packedComponent.Model.VertexBuffer.VertexDeclaration.VertexStride);
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
