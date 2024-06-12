using System.Collections.Generic;

namespace Baklavajs
{
    public class NodeState
    {
        public string type { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public Dictionary<string, NodeInterfaceState> inputs { get; set; }
        public Dictionary<string, NodeInterfaceState> outputs { get; set; }
        public NodeStatePosition position {get;set;}
        public int width { get; set; } = 200;
        public bool twoColumn { get; set; } = false;
    }

    public class NodeStatePosition
    {
        public float x { get; set; }
        public float y { get; set; }
    }
}