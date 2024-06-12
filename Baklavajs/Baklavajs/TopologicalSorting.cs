namespace Baklavajs
{
  public class TopologicalSorting
  {
    public static TopologicalSortingResult SortTopologically(GraphState graph)
    {
      TopologicalSortingResult result = new TopologicalSortingResult();
      Dictionary<string, List<string>> adjacency = new Dictionary<string, List<string>>();
      foreach (NodeState n in graph.nodes)
      {
        foreach (NodeInterfaceState intf in n.inputs.Values)
        {
          result.interfaceIdToNodeId[intf.id] = n.id;
        }
        foreach (NodeInterfaceState intf in n.outputs.Values)
        {
          result.interfaceIdToNodeId[intf.id] = n.id;
        }
      }
      foreach (NodeState n in graph.nodes)
      {
        List<ConnectionState> connectionsFromCurrentNode = graph.connections.Where(c => !string.IsNullOrEmpty(c.from)
          && result.interfaceIdToNodeId.ContainsKey(c.from)
          && result.interfaceIdToNodeId[c.from] == n.id
        ).ToList();

        List<string> adjacentNodes = connectionsFromCurrentNode.Select(c => result.interfaceIdToNodeId[c.to]).ToList();
        adjacency[n.id] = adjacentNodes;
        result.connectionsFromNode[n] = connectionsFromCurrentNode;
      }

      List<NodeState> startNodes = graph.nodes.ToList();
      foreach (ConnectionState c in graph.connections)
      {
        int index = startNodes.FindIndex(n => result.interfaceIdToNodeId[c.to] == n.id);
        if (index >= 0)
          startNodes.RemoveAt(index);
      }

      while (startNodes.Count > 0)
      {
        NodeState n = startNodes[startNodes.Count - 1];
        startNodes.RemoveAt(startNodes.Count - 1);

        result.calculationOrder.Add(n);
        List<string> nodesConnectedFromN = adjacency[n.id];
        while (nodesConnectedFromN.Count > 0)
        {
          string mId = nodesConnectedFromN[0];
          nodesConnectedFromN.RemoveAt(0);
          if (adjacency.Values.All(connectedNodes => !connectedNodes.Contains(mId)))
          {
            NodeState m = graph.nodes.FirstOrDefault(node => node.id == mId);
            startNodes.Add(m);
          }
        }
      }
      if (adjacency.Values.Any(c => c.Count > 0))
      {
        throw new Exception("有未处理链接");
      }
      return result;
    }
  }
}