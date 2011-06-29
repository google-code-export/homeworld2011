using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using PlagueEngine.Helpers;

namespace PlagueEngine.Pathfinder
{
    public enum PathType { Totarget, Closest, Empty }
    class PathfinderComponent
    {
        private Stack<Node> _path;
        private Node _previousNode;
        private Node _curentNode;
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }
        private DateTime _startTime;
        private Thread _thread;

        public PathType PathType { get; set; }

        public PathfinderComponent()
        {
            _path = new Stack<Node>();
        }
        public void ComputePath(Vector3 startPoint, Vector3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
#if DEBUG
            Diagnostics.PushLog(LoggingLevel.INFO, this, "Position:" + startPoint);
            Diagnostics.PushLog(LoggingLevel.INFO, this, "Target:" + endPoint);
#endif
            ThreadHelper.Kill(_thread);
            _thread = ThreadHelper.GetNewThread(ComputePath);
            _thread.Start();
        }
        public bool IsComputing
        {
            get
            {
                return _thread != null && _thread.IsAlive;
            }
        }
        private void ComputePath()
        {
            if (PathfinderManager.Pm != null)
            {
                Clear();
                _startTime = DateTime.Now;
                var start = PathfinderManager.Pm.GetNode(StartPoint);
                if (start.NodeType.Equals(NodeType.Navigation))
                {
                    var end = PathfinderManager.Pm.GetNode(EndPoint);
                    if (end.NodeType.Equals(NodeType.Navigation))
                    {
#if DEBUG
                        Diagnostics.PushLog(LoggingLevel.INFO, this, "Start node: " + start);
                        Diagnostics.PushLog(LoggingLevel.INFO, this, "End node: " + end);
#endif
                        PathType = PathType.Totarget;
                        var visited = new HashSet<Node>();
                        var tempNodes = new PriorityQueue<Node>(true);
                        tempNodes.Enqueue(start);
                        PathfinderManager.Heuristic.ComputeNodeValue(start, end);

                        while (tempNodes.Count > 0)
                        {
                            if (DateTime.Now.Subtract(_startTime) > PathfinderManager.TimeLimit)
                            {
#if DEBUG
                                Diagnostics.PushLog(LoggingLevel.WARN, this, "Path computing interapted. Time limit reached.");
#endif
                                PathType = PathType.Closest;
                                break;
                            }
                            var active = tempNodes.Dequeue();
                            visited.Add(active);
                            if (PathType.Equals(PathType.Totarget) && active.Equals(end))
                            {
                                while (active.Parent != null)
                                {
                                    _path.Push(active);
                                    active = active.Parent;
                                }
                                SimplifiePath();
                                return;
                            }

                            foreach (var child in active.GenerateChildren())
                            {
                                if (visited.Contains(child)) continue;
                                PathfinderManager.Heuristic.ComputeNodeValue(child, end);
                                child.Distance = active.Distance + 1;
                                tempNodes.Enqueue(child);
                            }
                        }
                        if (PathType.Equals(PathType.Closest))
                        {
                            var closest = start;
                            foreach (var node in visited)
                            {
                                if (node.CompareTo(closest) < 0)
                                {
                                    closest = node;
                                }
                            }
                            while (closest.Parent != null)
                            {
                                _path.Push(closest);
                                closest = closest.Parent;
                            }
                            SimplifiePath();
                            return;
                        }
                    }
                }

            }
            return;
        }
        private void SimplifiePath()
        {
            if (_path != null && _path.Count > 0)
            {
                var tempNodes = new List<Node>();
                var curentDirection = -1;
                var nodeCount = 0;
                //#if DEBUG
                //Diagnostics.PushLog(LoggingLevel.INFO, this, "Path before simplification: ");
                //Diagnostics.PushLog(LoggingLevel.INFO, this, tempNodes[0].ToString());
                //#endif
                while (_path.Count > 0)
                {
                    var temp = _path.Pop();
                    //#if DEBUG               
                    //Diagnostics.PushLog(LoggingLevel.INFO,this, temp.ToString());
                    //#endif
                    if (temp.Direction == curentDirection && nodeCount != PathfinderManager.ControlNodes)
                    {
                        nodeCount++;
                        tempNodes[tempNodes.Count - 1] = temp;
                    }
                    else
                    {
                        nodeCount = 0;
                        curentDirection = temp.Direction;
                        tempNodes.Add(temp);
#if DEBUG
                        PathfinderManager.Pm.GenerateBox(temp);
#endif
                    }

                }
                tempNodes.Reverse(0, tempNodes.Count);
                //#if DEBUG
                //Diagnostics.PushLog(LoggingLevel.INFO, this, "Path after simplification:");
                //for (var i = 0; i < tempNodes.Count; i++)
                //{
                //Diagnostics.PushLog(LoggingLevel.INFO,this, i + ": " + tempNodes[i]);
                //}
                //#endif                  
                _path = new Stack<Node>(tempNodes);
            }
        }
        public bool IsEmpty
        {
            get
            {
                if (IsComputing) return true;
                return _path.Count == 0;
            }
        }
        private void Clear()
        {
            PathType = PathType.Empty;
            _path = new Stack<Node>();
            _curentNode = null;
            _previousNode = null;
        }
        public Vector3 NextNode()
        {
            if (IsComputing) return StartPoint;
            if (_path.Count <= 0)
            {
                return Vector3.Zero;
            }
            _previousNode = _curentNode;
            _curentNode = _path.Pop();
#if DEBUG
            Diagnostics.PushLog(LoggingLevel.INFO, this, "Next node was returned:" + _curentNode);
#endif
            return PathfinderManager.Pm.NodeToVector(_curentNode);
        }
        public Vector3 CurentNode()
        {
            if (IsComputing) return StartPoint;
            return _curentNode != null ? PathfinderManager.Pm.NodeToVector(_curentNode) : Vector3.Zero;
        }

        public Vector3 PreviousNode()
        {
            if (IsComputing) return StartPoint;
            return _previousNode != null ? PathfinderManager.Pm.NodeToVector(_previousNode) : Vector3.Zero;
        }
    }
}
