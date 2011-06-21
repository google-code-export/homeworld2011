﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Pathfinder
{
    public enum NodeType {STATIC, DYNAMIC, NONE, NAVIGATION}
    [Serializable()]
    public class Node : IComparable
    {
        public int x;
        public int y;
        public float Value;
        public float Distance;
        static public int HASH_HELPER = 1500;
        public NodeType NodeType;
        public Node Parent;
        public Node()
        {

        }
        public Node(int x, int y, NodeType nodeType)
        {
            this.x = x;
            this.y = y;
            NodeType = nodeType;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Node other = obj as Node;
            return other != null ? (other.x == x && other.y == y) : base.Equals(obj);
        }
        public List<Node> GenerateChildren()
        {
            List<Node> result = new List<Node>();
            for (int newX = x - 1; newX <= x + 1; newX++)
            {
                for (int newY = y - 1; newY <= y + 1; newY++)
                {
                    if (newX == x && newY == y) continue;
                    Node child = PathfinderManager.PM.checkNode(new Node(newX, newY, Pathfinder.NodeType.NAVIGATION));
                    if (child.NodeType == Pathfinder.NodeType.NAVIGATION)
                    {
                        child.Parent = this;
                        result.Add(child);
                    }
                }
            }
            return result;

        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            var n = obj as Node;
            return (object)n == null ? 0 : (Value + Distance).CompareTo((n.Value + n.Distance));
        }

        public int directionToNode(Node node)
        {
            if (node != null && !node.Equals(this))
            {
                int direction;
                int xD = x - node.x;
                int yD = y - node.y;
                if (xD < 0)
                {
                    if (yD < 0)
                    {
                        return 0;
                    }
                    if (yD == 0)
                    {
                        return 1;
                    }
                    if (yD > 0)
                    {
                        return 2;
                    }
                }
                if (xD == 0)
                {
                    if (yD > 0)
                    {
                        return 3;
                    }
                    if (yD < 0)
                    {
                        return 4;
                    }
                }
                if (xD > 0)
                {
                    if (yD < 0)
                    {
                        return 5;
                    }
                    if (yD == 0)
                    {
                        return 6;
                    }
                    if (yD > 0)
                    {
                        return 7;
                    }
                }
            }
            return 9;
        }
        public override int GetHashCode()
        {
            return x + y * HASH_HELPER;
        }
        public override string ToString()
        {
            return "Node["+x+","+y+"] type "+ NodeType;
        }
    }
}
