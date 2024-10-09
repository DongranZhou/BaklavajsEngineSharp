namespace Baklavajs;

public class AsyncForwardEngine : BaseEngine
{
  public AsyncForwardEngine(EditorState editor) : base(editor) { }

  public override async Task<CalculationResult> RunGraph(GraphState graph, Dictionary<string, object> inputs, CalculationData calculationData)
  {
    List<NodeState> startNodes = GetStartNodes(graph);
    Dictionary<string, List<NodeState>> forwardMap = MapForwardNode(graph);
    CalculationResult result = new CalculationResult();
    IEnumerable<Task<CalculationResult>> tasks = startNodes.Select(n => CalculateNode(n, graph, inputs, calculationData, forwardMap, result));
    await Task.WhenAll(tasks);
    return result;
  }
  async Task<CalculationResult> CalculateNode(NodeState n
  , GraphState graph
  , Dictionary<string, object> inputs
  , CalculationData calculationData
  , Dictionary<string, List<NodeState>> forwardMap
  , CalculationResult result)
  {
    Dictionary<string, object> inputsForNode = new Dictionary<string, object>();
    foreach (var kv in n.inputs)
    {
      inputsForNode[kv.Key] = getInterfaceValue(inputs, kv.Value.id);
    }
    events.beforeNodeCalculation?.Invoke(new BeforeNodeCalculationEventData(n, inputsForNode));
    var r = new Dictionary<string, object>();
    if (calculators.TryGetValue(n.type, out ICalculator calculator))
    {
      r = await calculator.Calculate(inputsForNode, n, new EngineContext { globalValues = calculationData, engine = this });
    }
    else if (defaultCalculator != null)
    {
      r = await defaultCalculator.Calculate(inputsForNode, n, new EngineContext { globalValues = calculationData, engine = this });
    }
    else
    {
      foreach (KeyValuePair<string, NodeInterfaceState> kv in n.outputs)
      {
        string k = kv.Key;
        NodeInterfaceState intf = kv.Value;
        r[k] = getInterfaceValue(inputs, intf.id);
      }
    }
    ValidateNodeCalculationOutput(n, r);
    events.afterNodeCalculation?.Invoke(new AfterNodeCalculationEventData(n, r));

    result[n.id] = new Dictionary<string, object>();
    foreach (KeyValuePair<string, object> kv in r)
    {
      result[n.id][kv.Key] = kv.Value;
    }

    foreach (var kv in n.outputs)
    {
      string intfKey = kv.Key;
      NodeInterfaceState intf = kv.Value;
      foreach (var c in graph.connections.Where(c => c.from == intf.id))
      {
        if (c != null)
        {
          object v = hooks.transferData == null ? r[intfKey] : hooks.transferData.Invoke(r[intfKey], c);
          if (AllowMultipleConnections(c.to, graph.connections))
          {
            if (inputs.ContainsKey(c.to) && inputs[c.to] is List<object> list)
            {
              list.Add(v);
            }
            else
            {
              inputs[c.to] = new List<object> { v };
            }
          }
          else
          {
            inputs[c.to] = v;
          }
        }
      }
    }
    List<NodeState> forwardNodes = new List<NodeState>();
    foreach (var o in n.outputs.Values)
    {
      if (forwardMap.ContainsKey(o.id))
      {
        forwardNodes.AddRange(forwardMap[o.id]);
      }
    }
    IEnumerable<Task<CalculationResult>> tasks = forwardNodes.Select(n => CalculateNode(n, graph, inputs, calculationData, forwardMap, result));
    await Task.WhenAll(tasks);
    return result;
  }
  public override Task<CalculationResult> Execute(CalculationData calculationData)
  {
    var inputValues = GetInputValues(editor.graph);
    return RunGraph(editor.graph, inputValues, calculationData);
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
  bool AllowMultipleConnections(string to, ConnectionState[] connections)
  {
    return connections.Where(x => x.to == to).Count() > 1;
  }
  List<NodeState> GetStartNodes(GraphState graph)
  {
    List<NodeState> nodes = new List<NodeState>();
    foreach (var node in graph.nodes)
    {
      if (IsStartNode(node, graph))
      {
        nodes.Add(node);
      }
    }
    return nodes;
  }
  bool IsStartNode(NodeState node, GraphState graph)
  {
    foreach (var i in node.inputs.Values)
    {
      foreach (var c in graph.connections)
      {
        if (c.to == i.id)
          return false;
      }
    }
    return true;
  }
  Dictionary<string, List<NodeState>> MapForwardNode(GraphState graph)
  {
    Dictionary<string, List<NodeState>> map = new Dictionary<string, List<NodeState>>();
    foreach (var n in graph.nodes)
    {
      foreach (var i in n.inputs.Values)
      {
        foreach (var c in graph.connections)
        {
          if (c.to == i.id)
          {
            if (!map.ContainsKey(c.from))
            {
              map[c.from] = new List<NodeState>();
            }
            map[c.from].Add(n);
          }
        }
      }
    }
    return map;
  }
  private object getInterfaceValue(Dictionary<string, object> values, string id)
  {
    if (!values.ContainsKey(id))
    {
      throw new Exception($"Could not find value for interface {id}\nThis is likely a Baklava internal issue. Please report it on GitHub.");
    }
    return values[id];
  }
}