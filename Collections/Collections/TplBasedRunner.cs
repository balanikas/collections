using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Collections.Messages;

namespace Collections
{
    public class TplBasedRunner : IRunner
    {
        private readonly IRunnable _runnable;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;
        private readonly List<IGui> _uiListeners;
        private readonly RunnerSettings _settings;
        private Task _task;

        public TplBasedRunner(IRunnable runnable, ILogger logger,RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();

            _runnable = runnable;
            _settings = settings;
            _logger = logger;
            _cts = new CancellationTokenSource();

            Id = Guid.NewGuid().ToString();
          
        }

        public string Id { get; private set; }

        public void AddUiListener(IGui listener)
        {
            if (!_uiListeners.Contains(listener))
            {
                listener.Id = Id;
                _uiListeners.Add(listener);
            }
        }

        public void Start()
        {
            foreach (IGui listener in _uiListeners)
            {
                listener.Draw();
            }


            _task = Task.Factory.StartNew(() => { DoWork(); }, _cts.Token);

            _task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.Canceled:
                        break;
                    case TaskStatus.RanToCompletion:
                        break;
                    case TaskStatus.Faulted:
                        if (t.Exception != null)
                        {
                            Debugger.Break();
                        }
                        break;
                    default:
                        break;
                }
            }, _cts.Token);
        }

        public void Destroy()
        {
            _cts.Cancel();
            foreach (IGui listener in _uiListeners)
            {
                listener.Destroy();
            }
        }

        public bool IsAlive()
        {
            if (_task.Status == TaskStatus.Running)
            {
                return true;
            }

            return false;
        }


        public RunSummaryMessage GetCurrentState()
        {
            return null;
        }

        private void DoWork()
        {
            //todo: not having following line, fucks up loop..sometimes...wtf investigate 
            Debug.WriteLine(_task);

            var watch = new Stopwatch();
            watch.Start();

            for (int i = 1; (i <= _settings.Iterations); i++)
            {
                CancellationToken ct = _cts.Token;
                ct.ThrowIfCancellationRequested();

                MethodExecutionResult methodExecutionResult;


                //check if end of loop, or check every now and then
                if (i % (_settings.Iterations / 10) == 0 || i == _settings.Iterations)
                {
                    methodExecutionResult = _runnable.Update(false);

                    if (i == _settings.Iterations)
                    {
                        watch.Stop();
                        _logger.Flush();
                    }

                    var progressCount = (int)(i / (double)_settings.Iterations * 100);
                    _logger.Info(Id + ": " + progressCount);

                    var msg = new MethodExecutionMessage(
                        _runnable.GetObjectType(),
                        methodExecutionResult,
                        watch.Elapsed,
                        progressCount);

                    foreach (IGui listener in _uiListeners)
                    {
                        listener.Update(msg);
                    }
                }
                else
                {
                    _runnable.Update(false);
                }
            }
            watch.Stop();
        }
    }
}