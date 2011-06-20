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
        private Stack<Node> _nodes;
        private Node _previousNode;
        private Node _curentNode;
        private PathType _pathType;

        public PathType PathType
        {
            get { return _pathType; }
            set { _pathType = value; }
        }
        private static Heuristic H = new HManhattan();
        public PathfinderComponent()
        {
            _nodes = new Stack<Node>();
        }
        bool GetPath(Vector3 startPoint, Vector3 destinationPoint)
        {
            clear();
            if (PathfinderManager.PM != null)
            {
               Node start = PathfinderManager.PM.getNode(startPoint);
               if (start.NodeType != NodeType.NONE)
               {
                   Node end = PathfinderManager.PM.getNode(destinationPoint);
                   _pathType = (end.NodeType == NodeType.NAVIGATION)?PathType.TOTARGET:PathType.CLOSEST;

                   if (end.NodeType != NodeType.NONE)
                   {
                       HashSet<Node> _visited = new HashSet<Node>();
                       PriorityQueue<Node> _tempNodes = new PriorityQueue<Node>(_pathType==PathType.TOTARGET);
                       _tempNodes.Enqueue(start);
                       Node active;
                       H.ComputeNodeValue(start, end);
                       while (_tempNodes.Count > 0)
                       {
                           active = _tempNodes.Dequeue();
                           _visited.Add(active);
                           if (_pathType == PathType.TOTARGET && active.Equals(end))
                           {
                               while (active.Parent != null)
                               {
                                   _nodes.Push(active);
                                   active = active.Parent;
                               }
                               return true;
                           }
                           foreach (var child in active.GenerateChildren())
                           {
                               if (!_visited.Contains(child))
                               {
                                    H.ComputeNodeValue(child,end);
                                    child.Distance = active.Distance + 1;
                                   _tempNodes.Enqueue(child);
                               }
                           }
                       }
                       if (_pathType == PathType.CLOSEST)
                       {
                           Node closest = start;
                           foreach (Node node in _visited)
                           {
                               if (node.CompareTo(closest) <0)
                               {
                                   closest = node;
                               }
                           }
                           while (closest.Parent != null)
                           {
                               _nodes.Push(closest);
                               closest = closest.Parent;
                           }
                           return true;
                       }
                       _visited = null;
                       _tempNodes = null;
                   }
               }
            
            }
            return false;
        }
        private void clear()
        {
            _pathType = PathType.EMPTY;
            _nodes = new Stack<Node>();
            _curentNode = null;
            _previousNode = null;
        }
        public Vector3 NextNode()
        {
            if (_nodes.Count > 0)
            {
                _previousNode = _curentNode;
                _curentNode = _nodes.Pop();
                return PathfinderManager.PM.NodeToVector(_curentNode);
            }
            return Vector3.Zero;
        }
        public Vector3 CurentNode()
        {
            if (_curentNode != null)
            {
                return PathfinderManager.PM.NodeToVector(_curentNode);
            }
            return Vector3.Zero;
        }
        public Vector3 PreviousNode()
        {
            if (_previousNode != null)
            {
                return PathfinderManager.PM.NodeToVector(_previousNode);
            }
            return Vector3.Zero;
        }
        
    }
}
