using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.Pathfinder
{
    [Serializable]
    public class PathfinderManager
    {
        [NonSerialized]
        public static PathfinderManager Pm;
        [NonSerialized]
        internal GameObjectsFactory Factory;

        private float _boxWidth;
        private float _distanceBeetwenBoxes;

        public float BoxWidth
        {
            get { return _boxWidth; }
            set { _boxWidth = value; ComputeBoxSpace(); }
        }

        public float DistanceBeetwenBoxes
        {
            get { return _distanceBeetwenBoxes; }
            set { _distanceBeetwenBoxes = value; ComputeBoxSpace(); }
        }

        public float BoxSpace { get; set; }

        public float BoxHeight { get; set; }

        public int NumberOfBoxesInLength { get; set; }

        public int NumberOfBoxesInWidth { get; set; }

        public Vector3 BoxStartPosition { get; set; }

        public HashSet<Node> BlockedNodes { get; set; }

        public PathfinderManager()
        {
            BlockedNodes = new HashSet<Node>();
        }
        private void ComputeBoxSpace()
        {
            BoxSpace = _distanceBeetwenBoxes + _boxWidth;
        }
        public Node GetNode(Vector3 position)
        {
            var newPos = Vector3.Subtract(position, BoxStartPosition);
            if (newPos.X < 0 || newPos.Z < 0)
            {
                return new Node(0, 0, NodeType.None);
            }
            var x = (int)Math.Ceiling(newPos.X / (BoxSpace));
            var y = (int)Math.Ceiling(newPos.Z / (BoxSpace));
            
            return CheckNode(new Node(x, y, NodeType.Navigation));
        }
        public Node CheckNode(Node node)
        {
            if (node == null || BlockedNodes == null || (node.X > NumberOfBoxesInLength - 1 || node.Y > NumberOfBoxesInWidth - 1) || node.X < 0 || node.Y < 0)
            {
                return new Node(0, 0, NodeType.None);
            }
            return BlockedNodes.Contains(node) ? new Node(0, 0, NodeType.None) : node;
        }
        public Vector3 NodeToVector(Node node)
        {
            return node == null ? Vector3.Zero : new Vector3(node.X * BoxSpace + BoxStartPosition.X, BoxStartPosition.Y, node.Y * BoxSpace + BoxStartPosition.Z);
        }

        public  void GenerateBox(Node n)
        {
            if (n == null) return;
            var move = Pm.BoxStartPosition;
            
            move.X += (Pm.BoxSpace) * n.X;
            move.Z += (Pm.BoxSpace) * n.Y;
            var dddtata = new SquareBodyMeshData
                              {
                                  Definition = "PathNode",
                                  Type = (typeof (SquareBodyMesh)),
                                  World = Matrix.CreateTranslation(move)
                              };
            Factory.Create(dddtata);
        }

    }
}
