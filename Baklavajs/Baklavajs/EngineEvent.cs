using System;

namespace Baklavajs
{
  public class EngineEvent
  {
    public Action<CalculationData> beforeRun { get; set; }
    public Action<Dictionary<string,Dictionary<string,object>>> afterRun { get; set; }
    public Action<BeforeNodeCalculationEventData> beforeNodeCalculation { get; set; }
    public Action<AfterNodeCalculationEventData> afterNodeCalculation { get; set; }
  }
}