
namespace Baklavajs
{
  public class DependencyEngine<T> : BaseEngine<T>
  {
    public DependencyEngine(EditorState editor) : base(editor) { }
    public Dictionary<string, TopologicalSortingResult> order { get; } = new Dictionary<string, TopologicalSortingResult>();
    public override async Task<Dictionary<string,Dictionary<string,object>>> RunGraph(GraphState graph, Dictionary<string, object> inputs, T calculationData)
    {
      if (!order.ContainsKey(graph.id))
        order[graph.id] = TopologicalSorting.SortTopologically(graph);

      TopologicalSortingResult sorted = order[graph.id];

      Dictionary<string,Dictionary<string,object>> result = new Dictionary<string,Dictionary<string,object>>();

      foreach (NodeState n in sorted.calculationOrder)
      {
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

        result[n.id] = new Dictionary<string, object>();
        foreach(KeyValuePair<string,object> kv in r)
        {
          result[n.id][kv.Key] = kv.Value;
        }

        if (sorted.connectionsFromNode.ContainsKey(n))
        {
          foreach (ConnectionState c in sorted.connectionsFromNode[n])
          {
            foreach (KeyValuePair<string, NodeInterfaceState> kv in n.outputs)
            {
              if (kv.Value.id == c.from)
              {
                inputs[c.to] = r[kv.Key];
              }
            }
          }
        }
      }
      return result;
    }
    public override async Task<Dictionary<string,Dictionary<string,object>>> Execute(T calculationData)
    {
      Dictionary<string, object> inputValues = GetInputValues(editor.graph);
      return await RunGraph(editor.graph, inputValues, calculationData);
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
}