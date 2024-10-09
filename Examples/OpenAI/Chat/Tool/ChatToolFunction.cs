namespace OpenAI
{
  public class ChatToolFunction
  {
    public string name { get; set; }
    public string description { get; set; }
    public ChatToolFunctionParameter parameters { get; set; }
  }
}