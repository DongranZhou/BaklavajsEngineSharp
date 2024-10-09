
using System.Collections.Generic;

namespace OpenAI
{
    public class VisionMessage
    {
        public string role { get; set; }
        public List<ChatVisionContent> content { get; set; }
    }
}