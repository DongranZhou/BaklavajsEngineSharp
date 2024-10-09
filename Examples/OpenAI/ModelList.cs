using System.Collections.Generic;

namespace OpenAI
{
  public class ModelList
  {
    public string @object { get; set; }
    public List<ModelCard> data { get; set; } = new List<ModelCard>();
  }
}