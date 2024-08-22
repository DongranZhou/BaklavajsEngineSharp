namespace Baklavajs
{
    public interface ICalculator
    {
        public Task<Dictionary<string, object>> Calculate(Dictionary<string, object> inputs,NodeState state, EngineContext context);
    }
}