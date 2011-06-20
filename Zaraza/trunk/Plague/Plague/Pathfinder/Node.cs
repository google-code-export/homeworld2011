using System;
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
    }
}
