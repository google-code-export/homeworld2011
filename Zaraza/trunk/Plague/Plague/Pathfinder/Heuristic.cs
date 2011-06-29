
namespace PlagueEngine.Pathfinder
{
    public abstract class Heuristic
    {
        abstract public void ComputeNodeValue(Node node, Node endNode);
    }
}
