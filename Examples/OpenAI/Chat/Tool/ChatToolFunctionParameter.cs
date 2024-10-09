using System.Collections.Generic;

namespace OpenAI
{
  public class ChatToolFunctionParameter
  {
    public string type { get; set; }
    public Dictionary<string,ChatToolFunctionParameterProperty> properties { get; set; }
    public List<string> required { get; set; } = new List<string>();
  }
}