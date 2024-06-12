namespace Baklavajs
{
  public class GraphTemplateState
  {
    public string name { get; set; }
    public string id { get; set; }
    public NodeState nodes {get;set;}
    public ConnectionState[] connections { get; set; }
    public GraphInterface[] inputs {get;set;}
    public GraphInterface[] outputs{get;set;}

  }
}