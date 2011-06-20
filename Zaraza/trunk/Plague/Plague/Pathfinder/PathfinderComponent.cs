using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Pathfinder
{
    public enum PathType { TOTARGET, CLOSEST, EMPTY }
    class PathfinderComponent
    {
        private PathfinderManager _pathfindermanager;
        private Queue<Vector3> _nodes;
        private Vector3 _previousNode;
        private Vector3 _curentNode;
        private PathType _pathType;

        bool GetPath(Vector3 startPoint, Vector3 destinationPoint)
        {
            return false;
        }

        public Vector3 NextNode()
        {
            if (_nodes.Count > 0)
            {
                _previousNode = _curentNode;
                return _curentNode = _nodes.Dequeue();
            }
            return Vector3.Zero;
        }
        public Vector3 CurentNode()
        {
            if (_curentNode != null)
            {
                return _curentNode;
            }
            return Vector3.Zero;
        }
        public Vector3 PreviousNode()
        {
            if (_previousNode != null)
            {
                return _previousNode;
            }
            return Vector3.Zero;
        }
    }
}
