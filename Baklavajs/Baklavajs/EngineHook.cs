using System;

namespace Baklavajs
{
  public class EngineHook
  {
    public Func<CalculationData, CalculationData> gatherCalculationData { get; set; }
    public Func<object, ConnectionState, object> transferData { get; set; }
  }
}