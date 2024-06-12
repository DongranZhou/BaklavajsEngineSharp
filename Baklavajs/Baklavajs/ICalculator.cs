namespace Baklavajs
{
    public interface ICalculator
    {
        public Task<Dictionary<string, object>> Calculate<T>(Dictionary<string, object> inputs,NodeState state, EngineContext<T> context);
    }
}