using Collections.Logging;

namespace Collections.Runtime
{
    public interface IRuntime
    {
        void Stop();
        void Start();
        void Reset();
        IRunnerCollection Runners { get; }

        bool IsRunning();

        IRunner CreateAndAddRunner(IRunnable runnable, RunnerSettings settings);
        ILogger Logger { get;  }
    }
}
