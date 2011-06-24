using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Pathfinder
{
    class HManhattan :Heuristic
    {

        public override void ComputeNodeValue(Node node, Node endNode)
        {

            node.Value = Math.Abs(node.x - endNode.x) + Math.Abs(node.y - endNode.y);

        }
    }
}
