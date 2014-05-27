using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Logging;
using Collections.Messages;

namespace Collections.Runtime
{
    public class TplBasedRunner : IRunner
    {
        private readonly IRunnable _runnableItem;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;
        private readonly List<IGui> _uiListeners;
        private readonly RunnerSettings _settings;
        private Task _task;
        private readonly Stopwatch _watch;
        private readonly MethodExecutionResultAggregation _aggregation;
        private MethodExecutionMessage _lastMessage;

        public string Id { get; private set; }

        public TplBasedRunner(IRunnable runnableItem, ILogger logger, RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();
            _aggregation = new MethodExecutionResultAggregation();
            _lastMessage = new MethodExecutionMessage();
            _runnableItem = runnableItem;
            _settings = settings;
            _logger = logger;
            _cts = new CancellationTokenSource();

            Id = Guid.NewGuid().ToString();
            _watch = new Stopwatch();
        }

       

        public void AddUiListener(IGui listener)
        {
            if (!_uiListeners.Contains(listener))
            {
                listener.Id = Id;
                _uiListeners.Add(listener);
            }
        }

        public void RemoveUiListener(IGui listener)
        {
            _uiListeners.Remove(listener);
        }

        public void Start()
        {
            if (_task != null)
            {
                if (_task.Status == TaskStatus.Running ||
                _task.Status == TaskStatus.WaitingToRun)
                {
                    return;
                }
            }
            

            foreach (IGui listener in _uiListeners)
            {
                listener.Initialize();
            }


            _task = Task.Factory.StartNew(() => { DoWork(); }, _cts.Token,
                TaskCreationOptions.LongRunning,TaskScheduler.Default);

            _task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.Canceled:
                        {
                            foreach (IGui listener in _uiListeners.ToList())
                            {
                                listener.Destroy();
                            }
                            _uiListeners.Clear();
                        }
                        break;
                    case TaskStatus.RanToCompletion:
                        break;
                    case TaskStatus.Faulted:
                        if (t.Exception != null)
                        {
                            _logger.ErrorNow("runner faulted: \n" + t.Exception.InnerException.Message);
                            foreach (IGui listener in _uiListeners.ToList())
                            {
                                listener.Destroy();
                            }
                            _uiListeners.Clear();
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


        public MethodExecutionMessage GetCurrentState()
        {
            return _lastMessage;
        }

        private void DoWork()
        {
            _logger.InfoNow(string.Format("started executing {0}.{1}", _runnableItem.ObjectType, _runnableItem.Method));
            _watch.Start();

            var methodExecution = new MethodExecutionResult();
            ReportProgress(methodExecution, 0);

            for (int execCount = 1; execCount <= _settings.Iterations; execCount++)
            {
                

                bool log = _settings.Iterations >= 100 && execCount % (_settings.Iterations / 100) == 0 || execCount == _settings.Iterations;

                TimeSpan beforeExecution = _watch.Elapsed;
                methodExecution = _runnableItem.Update(true);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _aggregation.Add(methodExecution);

                    var progressCount = (int)(execCount / (double)_settings.Iterations * 100);
                    if (log)
                    {
                        ReportProgress(methodExecution, progressCount);
                    }

                }

                if (execCount == _settings.Iterations)
                {
                    var progressCount = (int)(execCount / (double)_settings.Iterations * 100);
                    ReportProgress(methodExecution, progressCount);
                }
            }
            _watch.Stop();
        }

        private void ReportProgress(MethodExecutionResult methodExecution, int progress)
        {

            _lastMessage = new MethodExecutionMessage(
                _runnableItem.ObjectType,
                _runnableItem.Method,
                methodExecution,
                _watch.Elapsed,
                progress);

            _lastMessage.Aggregation = _aggregation;
           
            foreach (IGui listener in _uiListeners)
            {
                listener.Update(_lastMessage);
            }
        }
    }
}