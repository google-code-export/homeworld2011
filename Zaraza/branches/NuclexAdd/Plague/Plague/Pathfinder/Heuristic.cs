
namespace PlagueEngine.Pathfinder
{
    abstract class Heuristic
    {
        abstract public void ComputeNodeValue(Node node, Node endNode);
    }
}
