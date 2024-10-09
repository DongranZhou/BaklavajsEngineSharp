using System;
using Baklavajs;

namespace Examples.Calculator;

public class OutputCalculator : ICalculator
{
    public async Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs, NodeState state, EngineContext context)
    {
        return new Dictionary<string, object>{
          { "message" , inputs["message"] }
        };
    }
}
