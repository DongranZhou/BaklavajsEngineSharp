using System.Collections.Generic;

namespace OpenAI
{
  public class ModelCard
  {
    public string id { get; set; }
    public string @object { get; set; }
    public int created { get; set; }
    public string owned_by { get; set; }
    public string root { get; set; }
    public string parent { get; set; }
    public List<string> permission { get; set; }
  }
}