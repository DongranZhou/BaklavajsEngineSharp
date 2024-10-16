using System.Text.RegularExpressions;
using Baklavajs;
namespace Examples;

public class InputCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
  {
    return new Dictionary<string, object>{
      { "message" , context.globalValues["message"] }
    };
  }
}
