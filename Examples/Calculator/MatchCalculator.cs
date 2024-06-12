using System.Text.RegularExpressions;
using Baklavajs;
namespace Examples;

public class MatchCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate<T>(Dictionary<string, object> inputs, NodeState state, EngineContext<T> context)
  {
    if (inputs.ContainsKey("pattern")
    && inputs.ContainsKey("text"))
    {
      string pattern = inputs["pattern"] as string;
      string text = inputs["text"] as string;
      if (!string.IsNullOrEmpty(pattern)
      && !string.IsNullOrEmpty(text))
      {

        Match match = Regex.Match(text, pattern);
        return new Dictionary<string, object>{
              {"true", match.Success ? match.Value : text},
              {"false", text},
            };
      }
    }

    return new Dictionary<string, object>{
      {"true", ""},
      {"false", inputs.TryGetValue("text",out object r) ? r : ""}
    };
  }
}
