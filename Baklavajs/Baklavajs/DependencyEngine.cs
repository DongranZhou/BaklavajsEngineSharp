
namespace Baklavajs
{
  public class DependencyEngine : BaseEngine
  {
    public DependencyEngine(EditorState editor) : base(editor) { }
    public Dictionary<string, TopologicalSortingResult> order { get; } = new Dictionary<string, TopologicalSortingResult>();
    public override async Task<CalculationResult> RunGraph(GraphState graph, Dictionary<string, object> inputs, CalculationData calculationData)
    {
      if (!order.ContainsKey(graph.id))
        order[graph.id] = TopologicalSorting.SortTopologically(graph);

      TopologicalSortingResult sorted = order[graph.id];
      var calculationOrder = sorted.calculationOrder;
      var connectionsFromNode = sorted.connectionsFromNode;

      CalculationResult result = new CalculationResult();

      foreach (NodeState n in calculationOrder)
      {
        Dictionary<string, object> inputsForNode = new Dictionary<string, object>();
        foreach (KeyValuePair<string, NodeInterfaceState> kv in n.inputs)
        {
          string k = kv.Key;
          NodeInterfaceState v = kv.Value;
          inputsForNode[k] = getInterfaceValue(inputs, v.id);
        }

        events.beforeNodeCalculation?.Invoke(new BeforeNodeCalculationEventData(n, inputsForNode));

        var r = new Dictionary<string, object>();
        if (calculators.TryGetValue(n.type, out ICalculator calculator))
        {
          r = await calculator.Calculate(inputsForNode, n, new EngineContext { globalValues = calculationData, engine = this });
        }
        else if(defaultCalculator != null)
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

        if (connectionsFromNode.ContainsKey(n))
        {
          foreach (ConnectionState c in connectionsFromNode[n])
          {
            string intfKey = "";
            foreach (KeyValuePair<string, NodeInterfaceState> kv in n.outputs)
            {
              NodeInterfaceState intf = kv.Value;
              if (intf.id == c.from)
              {
                intfKey = kv.Key;
              }
            }
            if (string.IsNullOrEmpty(intfKey))
            {
              throw new Exception($"Could not find key for interface {c.from}\nThis is likely a Baklava internal issue. Please report it on GitHub.");

            }
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
      return result;
    }
    bool AllowMultipleConnections(string to, ConnectionState[] connections)
    {
      return connections.Where(x => x.to == to).Count() > 1;
    }
    public override async Task<CalculationResult> Execute(CalculationData calculationData)
    {
      order.Clear();
      var inputValues = GetInputValues(editor.graph);
      return await RunGraph(editor.graph, inputValues, calculationData);
    }

    public Dictionary<string, object> GetInputValues(GraphState graph)
    {
      var inputValues = new Dictionary<string, object>();
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
    private object getInterfaceValue(Dictionary<string, object> values, string id)
    {
      if (!values.ContainsKey(id))
      {
        throw new Exception($"Could not find value for interface {id}\nThis is likely a Baklava internal issue. Please report it on GitHub.");
      }
      return values[id];
    }
  }
}