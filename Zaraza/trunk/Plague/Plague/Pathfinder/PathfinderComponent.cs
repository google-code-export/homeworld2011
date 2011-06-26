using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Pathfinder
{
    public enum PathType { Totarget, Closest, Empty }
    class PathfinderComponent
    {
        private Stack<Node> _nodes;
        private Node _previousNode;
        private Node _curentNode;
        private PathType _pathType;
        private DateTime _startTime;
        public PathType PathType
        {
            get { return _pathType; }
            set { _pathType = value; }
        }
        private static readonly Heuristic H = new HManhattan();
        public PathfinderComponent()
        {
            _nodes = new Stack<Node>();
        }
       public bool GetPath(Vector3 startPoint, Vector3 destinationPoint)
        {
            Clear();
            _startTime = DateTime.Now;
            if (PathfinderManager.Pm != null)
            {
               var start = PathfinderManager.Pm.GetNode(startPoint);

               if (start.NodeType.Equals(NodeType.Navigation))
               {
                   var end = PathfinderManager.Pm.GetNode(destinationPoint);

                   _pathType = (end.NodeType == NodeType.Navigation)?PathType.Totarget:PathType.Closest;

                   if (end.NodeType.Equals(NodeType.Navigation))
                   {
#if DEBUG
                       Diagnostics.PushLog(LoggingLevel.INFO, this, "Start node: " + start);
                       Diagnostics.PushLog(LoggingLevel.INFO, this, "End node: " + end);
#endif
                       var visited = new HashSet<Node>();
                       var tempNodes = new PriorityQueue<Node>(_pathType==PathType.Totarget);
                       tempNodes.Enqueue(start);
                       H.ComputeNodeValue(start, end);
                       
                       while (tempNodes.Count > 0)
                       {
                           if (DateTime.Now.Subtract(_startTime).Seconds > 0)
                           {
                               Clear();
                               return false;
                           }
                           var active = tempNodes.Dequeue();
                           visited.Add(active);
                           if (_pathType == PathType.Totarget && active.Equals(end))
                           {
                               while (active.Parent != null)
                               {
                                   _nodes.Push(active);
                                   active = active.Parent;
                               }
                               SimplifiePath();
                               return true;
                           }
                           foreach (var child in active.GenerateChildren())
                           {
                               if (visited.Contains(child)) continue;
                               H.ComputeNodeValue(child,end);
                               child.Distance = active.Distance + 1;
                               tempNodes.Enqueue(child);
                           }
                       }
                       if (_pathType == PathType.Closest)
                       {
                           var closest = start;
                           foreach (var node in visited)
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
                           SimplifiePath();
                           return true;
                       }
                   }
               }
            
            }
            return false;
        }
        private void SimplifiePath()
        {
            if (_nodes != null && _nodes.Count > 0)
            {
                var tempNodes = new List<Node>();
                var curentDirection=-1;
                tempNodes.Add(_nodes.Pop());
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, this, "Path before simplification: ");
                Diagnostics.PushLog(LoggingLevel.INFO, this, tempNodes[0].ToString());
#endif
                while (_nodes.Count > 0)
                {
                    var temp = _nodes.Pop();
#if DEBUG
                    //PathfinderManager.PM.generateBox(temp);
                    Diagnostics.PushLog(LoggingLevel.INFO,this, temp.ToString());
#endif
                    var nodeDirection = tempNodes[tempNodes.Count - 1].DirectionToNode(temp);
                    if (nodeDirection == curentDirection)
                    {
                        tempNodes[tempNodes.Count - 1] = temp;
                    }
                    else
                    {
                        curentDirection = nodeDirection;
                        tempNodes.Add(temp);
                    }
                    
                }
                tempNodes.Reverse(0, tempNodes.Count);
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, this, "Path after simplification:");

                for (var i = 0; i < tempNodes.Count; i++)
                {
                    Diagnostics.PushLog(LoggingLevel.INFO,this, i + ": " + tempNodes[i]);
                }
#endif                  
                _nodes= new Stack<Node>(tempNodes);
            }
        }
        public bool IsEmpty{
            get { return _nodes.Count == 0; }
        }
        private void Clear()
        {
            _pathType = PathType.Empty;
            _nodes = new Stack<Node>();
            _curentNode = null;
            _previousNode = null;
        }
        public Vector3 NextNode()
        {
            if (_nodes.Count <= 0)
            {
                return Vector3.Zero;
            }
            _previousNode = _curentNode;
            _curentNode = _nodes.Pop();
#if DEBUG
            Diagnostics.PushLog(LoggingLevel.INFO, this, "Next node was returned:" + _curentNode);
#endif
            return PathfinderManager.Pm.NodeToVector(_curentNode);
        }
        public Vector3 CurentNode()
        {
            return _curentNode != null ? PathfinderManager.Pm.NodeToVector(_curentNode) : Vector3.Zero;
        }

        public Vector3 PreviousNode()
        {
            return _previousNode != null ? PathfinderManager.Pm.NodeToVector(_previousNode) : Vector3.Zero;
        }
    }
}
