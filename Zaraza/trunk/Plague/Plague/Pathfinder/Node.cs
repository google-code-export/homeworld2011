using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Pathfinder
{
    public enum NodeType {STATIC, DYNAMIC}
    [Serializable()]
    public class Node
    {
        public int x;
        public int y;
        public NodeType NodeType;

        public Node()
        {

        }
        public Node(int x, int y, NodeType nodeType)
        {
            this.x = x;
            this.y = y;
            NodeType = nodeType;
        }
    }
}
