using System;
using System.Collections.Generic;

namespace PlagueEngine.Pathfinder
{
    public enum NodeType {Static, Dynamic, None, Navigation}
    [Serializable]
    public class Node : IComparable
    {

        public readonly int X;
        public readonly int Y;
        public float Value;
        public float Distance;
        public NodeType NodeType;
        public Node Parent;
        public int Direction=-1;

        public Node(){}

        public Node(int x, int y, NodeType nodeType,int direction=-1)
        {
            X = x;
            Y = y;
            NodeType = nodeType;
            Direction = direction;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var other = obj as Node;
            return other != null && (other.X == X && other.Y == Y);
        }
        public List<Node> GenerateChildren()
        {
            var result = new List<Node>();
            var direction = 0;
            for (var newX = X - 1; newX <= X + 1; newX++)
            {
                for (var newY = Y - 1; newY <= Y + 1; newY++)
                {
                    if (newX == X && newY == Y) continue;
                    var child = PathfinderManager.Pm.CheckNode(new Node(newX, newY, NodeType.Navigation,direction));
                    if (child.NodeType != NodeType.Navigation) continue;
                    child.Parent = this;
                    result.Add(child);
                    direction++;
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

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode * 7) + X;
            hashCode = (hashCode * 7) + Y;
            return hashCode;
        }

        public int DirectionToNode(Node node)
        {
            if (node != null && !node.Equals(this))
            {
                var xD = X - node.X;
                var yD = Y - node.Y;
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
        
        public override string ToString()
        {
            return "Node["+X+","+Y+"] type "+ NodeType;
        }
    }
}
