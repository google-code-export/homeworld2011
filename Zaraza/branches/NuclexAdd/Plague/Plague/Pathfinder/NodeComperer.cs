using System.Collections.Generic;

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
