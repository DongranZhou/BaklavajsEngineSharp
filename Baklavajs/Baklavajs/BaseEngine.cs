namespace Baklavajs
{
    public class BaseEngine<T>
    {
        public Dictionary<string, ICalculator> calculators { get; private set; } = new Dictionary<string, ICalculator>();
        public void AddCalculator(string name, ICalculator calculator)
        {
            calculators[name] = calculator;
        }
        public void RemoveCalculator(string name)
        {
            calculators.Remove(name);
        }

        
        public EditorState editor { get; private set; }
        public BaseEngine(EditorState editor)
        {
            this.editor = editor;
        }

        public EngineStatus status
        {
            get
            {
                if (isRunning)
                    return EngineStatus.Running;
                return internalStatus;
            }
        }
        public EngineStatus internalStatus { get; private set; } = EngineStatus.Stopped;
        public bool isRunning { get; private set; } = false;
        /** Start the engine. After started, it will run everytime the graph is changed. */
        public void Start()
        {
            if (internalStatus == EngineStatus.Stopped)
            {
                internalStatus = EngineStatus.Idle;
            }
        }
        /**
         * Temporarily pause the engine.
         * Use this method when you want to update the graph with the calculation results.
         */
        public virtual void Pause()
        {
            if (internalStatus == EngineStatus.Idle)
            {
                internalStatus = EngineStatus.Paused;
            }
        }

        /** Resume the engine from the paused state */
        public void Resume()
        {
            if (internalStatus == EngineStatus.Paused)
            {
                internalStatus = EngineStatus.Idle;
            }
        }

        /** Stop the engine */
        public void Stop()
        {
            if (internalStatus == EngineStatus.Idle || internalStatus == EngineStatus.Paused)
            {
                internalStatus = EngineStatus.Stopped;
            }
        }

        public async Task<Dictionary<string,Dictionary<string,object>>> RunOnce(T calculationData)
        {
            try
            {
                isRunning = true;
                return await Execute(calculationData);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                isRunning = false;
            }
            return new Dictionary<string, Dictionary<string, object>>();
        }

        public virtual Task<Dictionary<string,Dictionary<string,object>>> Execute(T calculationData) => Task.FromResult(new Dictionary<string, Dictionary<string, object>>());
        public virtual Task RunGraph(GraphState graph, Dictionary<string, object> inputs, T calculationData) => Task.CompletedTask;
    }
}