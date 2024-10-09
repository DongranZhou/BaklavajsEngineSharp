using System.Text.RegularExpressions;
using Baklavajs;
namespace Examples;

public class ReplaceCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
  {
    if (inputs.ContainsKey("pattern")
    && inputs.ContainsKey("text"))
    {
      string pattern = inputs["pattern"] as string;
      string replacement = inputs.TryGetValue("replacement", out object _replacement) ? _replacement as string : "";
      string text = inputs["text"] as string;
      if (!string.IsNullOrEmpty(pattern)
      && !string.IsNullOrEmpty(text))
      {
        return new Dictionary<string, object>{
              {"text", Regex.Replace(text,pattern,replacement)}
            };
      }
    }

    return new Dictionary<string, object>{
      {"text", inputs.TryGetValue("text",out object _text) ? _text : ""}
    };
  }
}
