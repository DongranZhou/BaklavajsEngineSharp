﻿namespace OpenAI
{
  public class EmbeddingRequest
  {
    public string input { get; set;}
    public string model { get; set;} = "embedding-2";
  }
}