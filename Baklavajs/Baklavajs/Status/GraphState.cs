namespace Baklavajs
{
    public class GraphState
    {
        public string id { get; set; }
        public NodeState[] nodes { get; set; }
        public ConnectionState[] connections { get; set; }
        public GraphInterface[] inputs { get; set; }
        public GraphInterface[] outputs { get; set; }
        public NodeStatePanning panning { get; set; }
        public float scaling { get; set; }
        public object graphTemplates { get; set; }
    }

    public class NodeStatePanning
    {
        public float x { get; set; }
        public float y { get; set; }
    }
}