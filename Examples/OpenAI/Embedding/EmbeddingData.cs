using System.Collections.Generic;

namespace OpenAI
{
  public class EmbeddingData
  {
    public int index {get;set;}
    public string @object {get;set;}
    public List<float> embedding {get;set;}
  }
}