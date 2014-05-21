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
        private readonly BackgroundWorker _bw;
        private readonly ILogger _logger;
        private readonly RunnerSettings _settings;
        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;
        private readonly MethodExecutionResultAggregation _aggregation;
        private int _execCount;

        public BWBasedRunner(IRunnable runnableItem, ILogger logger, RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();
            _aggregation = new MethodExecutionResultAggregation();
            _runnableItem = runnableItem;
            _settings = settings;
            _logger = logger;
            _watch = new Stopwatch();
            Console.SetOut(new StringWriter());

            _bw = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

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

        public string Id { get; private set; }


        public void Start()
        {
            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();
                foreach (IGui listener in _uiListeners.ToList())
                {
                    listener.Initialize();
                }
            }
        }

        public void Destroy()
        {
            _bw.CancelAsync();
            foreach (IGui listener in _uiListeners.ToList())
            {
                listener.Destroy();
            }
        }

        public bool IsAlive()
        {
            if (_bw.IsBusy)
            {
                return true;
            }

            return false;
        }

        public MethodExecutionResultAggregation GetCurrentState()
        {
            return new MethodExecutionResultAggregation(_aggregation);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {

            var worker = sender as BackgroundWorker;

            _watch.Start();

            var methodExecution = new MethodExecutionResult();
            worker.ReportProgress(0, methodExecution);

            for (_execCount = 1; _execCount <= _settings.Iterations; _execCount++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

               
                bool log =  _settings.Iterations >= 100 && _execCount % (_settings.Iterations / 100) == 0 || _execCount == _settings.Iterations;
                
                TimeSpan beforeExecution = _watch.Elapsed;
                methodExecution = _runnableItem.Update(true);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _aggregation.Add(methodExecution);

                    var progressCount = (int)(_execCount / (double)_settings.Iterations * 100);
                    if (log)
                    {
                        worker.ReportProgress(progressCount, methodExecution);
                    }
                    
                }

                if (_execCount == _settings.Iterations)
                {
                    var progressCount = (int)(_execCount / (double)_settings.Iterations * 100);
                    worker.ReportProgress(progressCount, methodExecution);
                }
            }
            _watch.Stop();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
          
            var methodExecution = e.UserState as MethodExecutionResult;
         
            var msg = new MethodExecutionMessage(
                _runnableItem.ObjectType,
                _runnableItem.Method,
                methodExecution,
                _watch.Elapsed,
                e.ProgressPercentage);

            msg.Summary = GetCurrentState();

            foreach (IGui listener in _uiListeners)
            {
                listener.Update(msg);
            }
        }
    }
}