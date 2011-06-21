﻿using System;
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
        private DateTime startTime;
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
       public bool GetPath(Vector3 startPoint, Vector3 destinationPoint)
        {
            clear();
            startTime = DateTime.Now;
            if (PathfinderManager.PM != null)
            {
               Node start = PathfinderManager.PM.getNode(startPoint);
               Diagnostics.PushLog(LoggingLevel.INFO, "Wezeł startowy: " + start);
               if (start.NodeType != NodeType.NONE || start.NodeType != NodeType.STATIC)
               {
                   Node end = PathfinderManager.PM.getNode(destinationPoint);
                   Diagnostics.PushLog(LoggingLevel.INFO, "Wezeł końcowy: " + end);
                   _pathType = (end.NodeType == NodeType.NAVIGATION)?PathType.TOTARGET:PathType.CLOSEST;

                   if (end.NodeType != NodeType.NONE || end.NodeType != NodeType.STATIC)
                   {
                       HashSet<Node> _visited = new HashSet<Node>();
                       PriorityQueue<Node> _tempNodes = new PriorityQueue<Node>(_pathType==PathType.TOTARGET);
                       _tempNodes.Enqueue(start);
                       Node active;
                       H.ComputeNodeValue(start, end);
                       
                       while (_tempNodes.Count > 0)
                       {
                           if (DateTime.Now.Subtract(startTime).Seconds > 0)
                           {
                               clear();
                               return false;
                           }
                           active = _tempNodes.Dequeue();
                           _visited.Add(active);
                           if (_pathType == PathType.TOTARGET && active.Equals(end))
                           {
                               while (active.Parent != null)
                               {
                                   _nodes.Push(active);
                                   active = active.Parent;
                               }
                               simplifiePath();
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
                           simplifiePath();
                           return true;
                       }
                       _visited = null;
                       _tempNodes = null;
                   }
               }
            
            }
            return false;
        }
        private void simplifiePath()
        {
            if (_nodes != null && _nodes.Count > 0)
            {
                List<Node> tempNodes = new List<Node>();
                int curentDirection=-1;
                int nodeDirection;
                tempNodes.Add(_nodes.Pop());
                Diagnostics.PushLog(LoggingLevel.INFO, "Ścieżka przed uproszeczeniem:");
                Diagnostics.PushLog(LoggingLevel.INFO,  tempNodes[0].ToString());
                while (_nodes.Count > 0)
                {
                    Node temp = _nodes.Pop();
                    //PathfinderManager.PM.generateBox(temp);
                    Diagnostics.PushLog(LoggingLevel.INFO, temp.ToString());
                    nodeDirection = tempNodes[tempNodes.Count - 1].directionToNode(temp);
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
                Diagnostics.PushLog(LoggingLevel.INFO, "Ścieżka po uproszeczeniu:");
                for (int i = 0; i < tempNodes.Count; i++)
                {
                    Diagnostics.PushLog(LoggingLevel.INFO, i + ": " + tempNodes[i]);
                }
                   
                _nodes= new Stack<Node>(tempNodes);
            }
        }
        public bool isEmpty{
            get { return _nodes.Count == 0; }
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
                Diagnostics.PushLog(LoggingLevel.INFO, "Zwrócono następny węzeł" + _curentNode);
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
