using System;
using System.Threading;
using System.Threading.Tasks;
using Collections.Logging;

namespace Collections.Runtime
{
    public class Runtime : IRuntime
    {
        private readonly CancellationTokenSource _cts;

        private bool _isRunning;

        public Runtime(ILogger logger)
        {
            Runners = new RunnerCollection();
            _cts = new CancellationTokenSource();
            Logger = logger;
        }

        public ILogger Logger { get; private set; }
        public IRunnerCollection Runners { get; private set; }

        public IRunner CreateAndAddRunner(IRunnable runnable, RunnerSettings settings)
        {
            IRunner newRunner;
            switch (settings.RunnerType)
            {
                case RunnerType.BackgroundWorkerBased:
                    newRunner = new BWBasedRunner(runnable, Logger, settings);
                    break;
                case RunnerType.ParallelTaskBased:
                    newRunner = new TplBasedRunner(runnable, Logger, settings);
                    break;
                default:
                    throw new NotImplementedException();
            }

            Runners.Add(newRunner);
            return newRunner;
        }

        public void Stop()
        {
            _isRunning = false;
            Runners.RemoveAll();

            _cts.Cancel();
        }

        public void Reset()
        {
            Stop();
            Start();
        }

       

        public void Start()
        {
            if (_isRunning)
            {
                return;
            }


            Task task = Task.Factory.StartNew(() =>
            {
                while (_isRunning)
                {
                    Thread.Sleep(10000);
                }

                Runners.RemoveAll();
            }, _cts.Token);

            _isRunning = true;
        }


        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}