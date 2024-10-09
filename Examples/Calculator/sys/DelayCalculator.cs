using Baklavajs;

namespace Examples;

public class DelayCalculator : ICalculator
{
  public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
  {
    long? delay = inputs["delay"] as long?;
    await Task.Delay(delay.HasValue ? (int)delay.Value : 0);
    return inputs;
  }
  public static string GetTimeStamp()
  {
    TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
    return Convert.ToInt64(ts.TotalMilliseconds).ToString();
  }
}