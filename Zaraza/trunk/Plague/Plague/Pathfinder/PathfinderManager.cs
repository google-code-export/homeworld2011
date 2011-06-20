using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace PlagueEngine.Pathfinder
{
    [Serializable()]
    public class PathfinderManager
    {
        [NonSerialized]
        public static PathfinderManager PM;

        private float _boxWidth;
        private float _boxHeight;
        private int _numberOfBoxesInLength;
        private int _numberOfBoxesInWidth;
        private float _distanceBeetwenBoxes;
        private Vector3 _boxStartPosition;
        private HashSet<Node> _blockedNodes;
        private float _boxSpace;


        public float BoxSpace
        {
            get { return _boxSpace; }
            set { _boxSpace = value; }
        }
        public float BoxWidth
        {
            get { return _boxWidth; }
            set { _boxWidth = value; computeBoxSpace(); }
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
            set { _distanceBeetwenBoxes = value; computeBoxSpace(); }
        }
        public Vector3 BoxStartPosition
        {
            get { return _boxStartPosition; }
            set { _boxStartPosition = value; }
        }
        public HashSet<Node> BlockedNodes
        {
            get { return _blockedNodes; }
            set { _blockedNodes = value; }
        }
        public PathfinderManager()
        {
            BlockedNodes = new HashSet<Node>();
        }
        private void computeBoxSpace()
        {
            _boxSpace = _distanceBeetwenBoxes + _boxWidth;
        }
        public Node getNode(Vector3 position)
        {
            Vector3 newPos = Vector3.Subtract(position, _boxStartPosition);
            if (newPos.X < 0 || newPos.Z < 0)
            {
                return new Node(0, 0, NodeType.NONE);
            }
            int x = (int)Math.Ceiling(newPos.X / (_boxSpace));
            int y = (int)Math.Ceiling(newPos.Z / (_boxSpace));
            
            return checkNode(new Node(x, y, NodeType.NAVIGATION));
        }
        public Node checkNode(Node node)
        {
            if (node == null || (node.x > _numberOfBoxesInLength - 1 || node.y > _numberOfBoxesInWidth - 1))
            {
                return new Node(0, 0, NodeType.NONE);
            }
            if (_blockedNodes.Contains(node))
            {
                foreach (Node n in _blockedNodes)
                {
                    if (node.Equals(n))
                    {
                        return n;
                    }
                }
            }
            return node;
        }
        public Vector3 NodeToVector(Node node)
        {
            if (node == null) return Vector3.Zero;
            return new Vector3(node.x * _boxSpace, _boxStartPosition.Y, node.y * _boxSpace);
        }

    }
}
