using System.Collections.Generic;

namespace OpenAI
{
  public class ImageResponse
  {
    public string created { get; set; }
    public List<ImageInfo> data { get; set; }
  }
}