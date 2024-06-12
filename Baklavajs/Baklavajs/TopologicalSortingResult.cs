namespace Baklavajs
{
  public class TopologicalSortingResult
  {
    public List<NodeState> calculationOrder { get; } = new List<NodeState>();
    public Dictionary<NodeState, List<ConnectionState>> connectionsFromNode { get; } = new Dictionary<NodeState, List<ConnectionState>>();
    public Dictionary<string, string> interfaceIdToNodeId { get; } = new Dictionary<string, string>();
  }
}