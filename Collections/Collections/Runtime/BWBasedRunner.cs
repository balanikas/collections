using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Collections.Logging;
using Collections.Messages;

namespace Collections.Runtime
{
    public class BWBasedRunner : IRunner
    {
        private readonly IRunnable _runnableItem;
        private readonly BackgroundWorker _worker;
        private readonly ILogger _logger;
        private readonly RunnerSettings _settings;
        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;
        private readonly MethodExecutionResultAggregation _aggregation;
        private MethodExecutionMessage _lastMessage;

        public string Id { get; private set; }
        public BWBasedRunner(IRunnable runnableItem, ILogger logger, RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();
            _aggregation = new MethodExecutionResultAggregation();
            _lastMessage = new MethodExecutionMessage();
            _runnableItem = runnableItem;
            _settings = settings;
            _logger = logger;
            _watch = new Stopwatch();

            _worker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            Id = Guid.NewGuid().ToString();
           
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
            if (_worker.IsBusy != true)
            {
                _worker.RunWorkerAsync();
                foreach (IGui listener in _uiListeners.ToList())
                {
                    listener.Initialize();
                }
            }
        }

        public void Destroy()
        {
            _worker.CancelAsync();
            foreach (IGui listener in _uiListeners.ToList())
            {
                listener.Destroy();
            }
        }

        public bool IsAlive()
        {
            if (_worker.IsBusy)
            {
                return true;
            }

            return false;
        }

        public MethodExecutionMessage GetCurrentState()
        {
            return _lastMessage;
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            var worker = sender as BackgroundWorker;
         
            _watch.Start();

            var methodExecution = new MethodExecutionResult();
            worker.ReportProgress(0, methodExecution);

            for (int execCount = 1; execCount <= _settings.Iterations; execCount++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

               
                bool log =  _settings.Iterations >= 100 && execCount % (_settings.Iterations / 100) == 0 || execCount == _settings.Iterations;
                
                TimeSpan beforeExecution = _watch.Elapsed;
                methodExecution = _runnableItem.Update(true);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _aggregation.Add(methodExecution);

                    var progressCount = (int)(execCount / (double)_settings.Iterations * 100);
                    if (log)
                    {
                        worker.ReportProgress(progressCount, methodExecution);
                    }
                    
                }

                if (execCount == _settings.Iterations)
                {
                    var progressCount = (int)(execCount / (double)_settings.Iterations * 100);
                    worker.ReportProgress(progressCount, methodExecution);
                }
            }
            _watch.Stop();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _logger.Flush();
            if (e.Cancelled || e.Error != null)
            {
                foreach (IGui listener in _uiListeners.ToList())
                {
                    listener.Destroy();
                }
                _uiListeners.Clear();
            }
        }

        

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
          
            var methodExecution = e.UserState as MethodExecutionResult;

            _lastMessage = new MethodExecutionMessage(
                _runnableItem.ObjectType,
                _runnableItem.Method,
                methodExecution,
                _watch.Elapsed,
                e.ProgressPercentage);

            _lastMessage.Aggregation = _aggregation;
       
            foreach (IGui listener in _uiListeners)
            {
                listener.Update(_lastMessage);
            }
        }
    }
}