using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Pathfinder
{
    public abstract class NodeComperer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            return x.Value.CompareTo(y.Value);
        }

    }
}
