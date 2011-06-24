using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Pathfinder
{
    abstract class Heuristic
    {
        abstract public void ComputeNodeValue(Node node, Node endNode);
    }
}
