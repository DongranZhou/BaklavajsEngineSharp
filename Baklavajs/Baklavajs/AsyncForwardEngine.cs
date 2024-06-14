namespace Baklavajs;

public class AsyncForwardEngine<T> : BaseEngine<T>
{
  public AsyncForwardEngine(EditorState editor) : base(editor) { }
  public async Task<Dictionary<string, Dictionary<string, object>>> RunOnce(T calculationData, NodeState startingNode)
  {
    try
    {
      isRunning = true;
      return await Execute(calculationData, startingNode);
    }
    catch (Exception ex)
    {

    }
    finally
    {
      isRunning = false;
    }
    return new Dictionary<string, Dictionary<string, object>>();
  }

  public Task<Dictionary<string, Dictionary<string, object>>> Execute(T calculationData, NodeState startingNode)
  {
    TaskCompletionSource<Dictionary<string, Dictionary<string, object>>> tcs = new TaskCompletionSource<Dictionary<string, Dictionary<string, object>>>();
    Dictionary<string, Dictionary<string, object>> result = new Dictionary<string, Dictionary<string, object>>();
    if (startingNode == null)
    {
      tcs.SetResult(result);
      return tcs.Task;
    }

    GraphState graph = editor.graph;
    Dictionary<string, object> inputs = GetInputValues(graph);
    Dictionary<string, NodeRuntimeStatus> nodesToCalculate = new Dictionary<string, NodeRuntimeStatus>();
    Dictionary<string, NodeState> interfaceToNode = new Dictionary<string, NodeState>();

    foreach (NodeState node in graph.nodes)
    {
      foreach (NodeInterfaceState intf in node.outputs.Values)
      {
        interfaceToNode.Add(intf.id, node);
      }
      foreach (NodeInterfaceState intf in node.inputs.Values)
      {
        interfaceToNode.Add(intf.id, node);
      }
    }

    List<NodeState> GetBackNodes(NodeState node)
    {
      List<NodeState> backNodes = new List<NodeState>();
      foreach (NodeInterfaceState input in node.inputs.Values)
      {
        foreach (ConnectionState c in graph.connections)
        {
          if (c.to == input.id)
          {
            backNodes.Add(interfaceToNode[c.from]);
          }
        }
      }
      return backNodes;
    }

    List<NodeState> GetForwardNodes(NodeState node)
    {
      List<NodeState> forwardNodes = new List<NodeState>();
      foreach (NodeInterfaceState output in node.outputs.Values)
      {
        foreach (ConnectionState c in graph.connections)
        {
          if (c.from == output.id)
          {
            forwardNodes.Add(interfaceToNode[c.to]);
          }
        }
      }
      return forwardNodes;
    }
    List<NodeState> GetConnectionNodes(NodeState node)
    {
      List<NodeState> backNodes = GetBackNodes(node);
      List<NodeState> forwardNodes = GetForwardNodes(node);
      return backNodes.Concat(forwardNodes).ToList();
    }
    void GetLeafNodes(NodeState node)
    {
      if (!nodesToCalculate.ContainsKey(node.id))
      {
        nodesToCalculate[node.id] = NodeRuntimeStatus.none;
        foreach (NodeState n in GetConnectionNodes(node))
        {
          GetLeafNodes(n);
        }
      }
    }
    async void RunDeepLeaf(NodeState n)
    {
      if (nodesToCalculate.ContainsKey(n.id) && nodesToCalculate[n.id] == NodeRuntimeStatus.none)
      {
        List<NodeState> backNodes = GetBackNodes(n);
        List<NodeState> backCalc = backNodes.Where(n => nodesToCalculate.ContainsKey(n.id) && nodesToCalculate[n.id] == NodeRuntimeStatus.none).ToList();
        if (backCalc.Count > 0)
        {
          foreach (NodeState _n in backCalc)
          {
            RunDeepLeaf(_n);
          }
        }
        else
        {
          try
          {
            nodesToCalculate[n.id] = NodeRuntimeStatus.started;
            Dictionary<string, object> inputsForNode = new Dictionary<string, object>();
            foreach (KeyValuePair<string, NodeInterfaceState> kv in n.inputs)
            {
              if (inputs.TryGetValue(kv.Value.id, out object v))
              {
                inputsForNode[kv.Key] = v;
              }
            }
            Dictionary<string, object> r = new Dictionary<string, object>();
            if (calculators.TryGetValue(n.type, out ICalculator calculator))
            {
              r = await calculator.Calculate(inputsForNode, n, new EngineContext<T> { globalValues = calculationData, engine = this });
            }
            else
            {
              foreach (KeyValuePair<string, NodeInterfaceState> kv in n.outputs)
              {
                if (inputs.TryGetValue(kv.Value.id, out object v))
                {
                  r[kv.Key] = v;
                }
              }
            }

            foreach (KeyValuePair<string, object> kv in r)
            {
              if (!result.ContainsKey(n.id))
                result[n.id] = new Dictionary<string, object>();
              result[n.id][kv.Key] = kv.Value;
            }

            foreach (KeyValuePair<string, NodeInterfaceState> kv in n.outputs)
            {
              foreach (ConnectionState c in graph.connections)
              {
                if (c.from == kv.Value.id)
                {
                  inputs[c.to] = r[kv.Key];
                }
              }
            }

            nodesToCalculate[n.id] = NodeRuntimeStatus.stoped;
            List<NodeState> forwardNodes = GetForwardNodes(n);
            foreach (NodeState _n in forwardNodes)
            {
              RunDeepLeaf(_n);
            }
          }
          catch (Exception ex)
          {
            tcs.SetException(ex);
          }
          finally
          {
            nodesToCalculate[n.id] = NodeRuntimeStatus.stoped;
            if (nodesToCalculate.Values.All(x => x == NodeRuntimeStatus.stoped))
            {
              tcs.SetResult(result);
            }
          }
        }
      }
    }

    GetLeafNodes(startingNode);
    RunDeepLeaf(startingNode);
    return tcs.Task;
  }
  public Dictionary<string, object> GetInputValues(GraphState graph)
  {
    Dictionary<string, object> inputValues = new Dictionary<string, object>();
    foreach (NodeState n in graph.nodes)
    {
      foreach (NodeInterfaceState intf in n.inputs.Values)
      {
        if (!graph.connections.Any(c => c.to == intf.id))
        {
          inputValues[intf.id] = intf.value;
        }
      }
      if (!calculators.ContainsKey(n.type))
      {
        foreach (NodeInterfaceState intf in n.outputs.Values)
        {
          inputValues[intf.id] = intf.value;
        }
      }
    }
    return inputValues;
  }
}
public enum NodeRuntimeStatus
{
  none,
  started,
  stoped,
}