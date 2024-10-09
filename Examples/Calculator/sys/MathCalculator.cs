using System.Text.RegularExpressions;
using Baklavajs;
namespace Examples;

public class MathCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
  {
    return new Dictionary<string, object>{
      {"output", (long)inputs["number1"] + (long)inputs["number2"]},
    };
  }
}
