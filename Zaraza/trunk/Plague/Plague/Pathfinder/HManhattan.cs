using System;

namespace PlagueEngine.Pathfinder
{
    class HManhattan :Heuristic
    {

        public override void ComputeNodeValue(Node node, Node endNode)
        {
            if (node != null && endNode != null)
            {
                node.Value = Math.Abs(node.X - endNode.X) + Math.Abs(node.Y - endNode.Y);
            }   
        }
    }
}
