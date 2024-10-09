using System;

namespace Baklavajs
{
  public class EngineHook
  {
    public Func<object, ConnectionState, object> transferData { get; set; }
  }
}