using System;

namespace Baklavajs
{
  public class AfterNodeCalculationEventData
  {
    public NodeState node { get; set; }
    public Dictionary<string, object> outputValues { get; set; }
    public AfterNodeCalculationEventData(NodeState node, Dictionary<string, object> outputValues)
    {
      this.node = node;
      this.outputValues = outputValues;
    }
  }
}