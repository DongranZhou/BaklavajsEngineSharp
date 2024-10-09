namespace OpenAI
{
  public class ChatToolCall
  {
    public string id {get;set;}
    public int index {get;set;}
    public string type {get;set;}
    public ChatToolCallFunction function {get;set;}
  }
}