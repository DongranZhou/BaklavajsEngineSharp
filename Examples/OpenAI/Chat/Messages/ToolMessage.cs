﻿namespace OpenAI
{
  public class ToolMessage : ChatMessage
  {
    public string tool_call_id { get; set; }
  }
}