using System;

namespace Baklavajs
{
  public class BeforeNodeCalculationEventData
  {
    public NodeState node { get; set; }
    public Dictionary<string, object> inputValues { get; set; }
    public BeforeNodeCalculationEventData(NodeState node, Dictionary<string, object> inputValues)
    {
      this.node = node;
      this.inputValues = inputValues;
    }
  }
}