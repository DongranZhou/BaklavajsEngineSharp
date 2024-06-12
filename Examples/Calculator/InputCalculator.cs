using System.Text.RegularExpressions;
using Baklavajs;
namespace Examples;

public class InputCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate<T>(Dictionary<string, object> inputs, NodeState state, EngineContext<T> context)
  {

    return new Dictionary<string, object>{
      {"string", context.globalValues},
      {"number", 0},
      {"boolean",false}
    };
  }
}
