namespace Baklavajs
{
  public class BaseEngine
  {
    public ICalculator defaultCalculator;
    public Dictionary<string, ICalculator> calculators { get; private set; } = new Dictionary<string, ICalculator>();
    public void AddCalculator(string name, ICalculator calculator)
    {
      calculators[name] = calculator;
    }
    public void RemoveCalculator(string name)
    {
      calculators.Remove(name);
    }
    public EditorState editor { get; private set; }
    public BaseEngine(EditorState editor)
    {
      this.editor = editor;
    }
    public EngineEvent events { get; set; } = new EngineEvent();
    public EngineHook hooks { get; set; } = new EngineHook();
    public bool isRunning { get; set; } = false;
    public async Task<CalculationResult> RunOnce(CalculationData calculationData)
    {
      events.beforeRun?.Invoke(calculationData);
      try
      {
        isRunning = true;
        var result = await Execute(calculationData);
        events.afterRun?.Invoke(result);
        return result;
      }
      catch (Exception ex)
      {

      }
      finally
      {
        isRunning = false;
      }
      return new CalculationResult();
    }
    protected void ValidateNodeCalculationOutput(NodeState node, Dictionary<string, object> output)
    {
      foreach (string k in node.outputs.Keys)
      {
        if (!output.ContainsKey(k))
        {
          throw new Exception($"Calculation return value from node {node.id} (type {node.type}) is missing key \"{k}\"");
        }
      }
    }
    public virtual Task<CalculationResult> Execute(CalculationData calculationData) => Task.FromResult(new CalculationResult());
    public virtual Task<CalculationResult> RunGraph(GraphState graph, Dictionary<string, object> inputs, CalculationData calculationData) => Task.FromResult(new CalculationResult());
  }
}