using System.Collections.Generic;

namespace OpenAI
{
  public class EmbeddingResponse
  {
    public List<EmbeddingData> data {get;set;}
    public string model {get;set;}
    public string @object {get;set;}
    public ChatUsage usage {get;set;}
  }
}