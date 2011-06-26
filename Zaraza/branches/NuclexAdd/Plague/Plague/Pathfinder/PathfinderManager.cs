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

        private float _boxWidth;
        private float _boxHeight;
        private int _numberOfBoxesInLength;
        private int _numberOfBoxesInWidth;
        private float _distanceBeetwenBoxes;
        private Vector3 _boxStartPosition;
        public HashSet<Node> BlockedNodes;
        private float _boxSpace;
        [NonSerialized]
        internal GameObjectsFactory Factory;

        public float BoxSpace
        {
            get { return _boxSpace; }
            set { _boxSpace = value; }
        }
        public float BoxWidth
        {
            get { return _boxWidth; }
            set { _boxWidth = value; ComputeBoxSpace(); }
        }
        public float BoxHeight
        {
            get { return _boxHeight; }
            set { _boxHeight = value; }
        }
        public int NumberOfBoxesInLength
        {
            get { return _numberOfBoxesInLength; }
            set { _numberOfBoxesInLength = value; }
        }
        public int NumberOfBoxesInWidth
        {
            get { return _numberOfBoxesInWidth; }
            set { _numberOfBoxesInWidth = value; }
        }
        public float DistanceBeetwenBoxes
        {
            get { return _distanceBeetwenBoxes; }
            set { _distanceBeetwenBoxes = value; ComputeBoxSpace(); }
        }
        public Vector3 BoxStartPosition
        {
            get { return _boxStartPosition; }
            set { _boxStartPosition = value; }
        }
        public PathfinderManager()
        {
            BlockedNodes = new HashSet<Node>();
        }
        private void ComputeBoxSpace()
        {
            _boxSpace = _distanceBeetwenBoxes + _boxWidth;
        }
        public Node GetNode(Vector3 position)
        {
            var newPos = Vector3.Subtract(position, _boxStartPosition);
            if (newPos.X < 0 || newPos.Z < 0)
            {
                return new Node(0, 0, NodeType.None);
            }
            var x = (int)Math.Ceiling(newPos.X / (_boxSpace));
            var y = (int)Math.Ceiling(newPos.Z / (_boxSpace));
            
            return CheckNode(new Node(x, y, NodeType.Navigation));
        }
        public Node CheckNode(Node node)
        {
            if (node == null || (node.X > _numberOfBoxesInLength - 1 || node.Y > _numberOfBoxesInWidth - 1))
            {
                return new Node(0, 0, NodeType.None);
            }
            return BlockedNodes.Contains(node) ? new Node(0, 0, NodeType.Static) : node;
        }
        public Vector3 NodeToVector(Node node)
        {
            return node == null ? Vector3.Zero : new Vector3(node.X * _boxSpace + _boxStartPosition.X, _boxStartPosition.Y, node.Y * _boxSpace + _boxStartPosition.Z);
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
