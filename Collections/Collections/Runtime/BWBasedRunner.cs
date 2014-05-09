using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Collections.Logging;
using Collections.Messages;

namespace Collections.Runtime
{
    public class BWBasedRunner : IRunner
    {
        private readonly IRunnable _runnableObject;
        private readonly BackgroundWorker _bw;
        private readonly ILogger _logger;
        private readonly ConcurrentBag<MethodExecutionResult> _methodExecutions;
        private readonly RunnerSettings _settings;
        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;

        public BWBasedRunner(IRunnable runnableObject, ILogger logger, RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();

            _runnableObject = runnableObject;
            _settings = settings;
            _logger = logger;
            _watch = new Stopwatch();
            _methodExecutions = new ConcurrentBag<MethodExecutionResult>();
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

        public string Id { get; private set; }


        public void Start()
        {
           

            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();
            }

            foreach (IGui listener in _uiListeners)
            {
                listener.Draw();
            }
        }


        public void Destroy()
        {
            _bw.CancelAsync();
            foreach (IGui listener in _uiListeners)
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

        public RunSummaryMessage GetCurrentState()
        {
            return new RunSummaryMessage(_runnableObject.ObjectType, _watch.Elapsed, _methodExecutions);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
           
            var worker = sender as BackgroundWorker;

            _watch.Start();

            var methodExecution = new MethodExecutionResult();
            worker.ReportProgress(0, methodExecution);

            for (int i = 1; i <= _settings.Iterations; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                TimeSpan beforeExecution = _watch.Elapsed;
                bool log = i % (_settings.Iterations / 100) == 0 || i == _settings.Iterations;
                methodExecution = _runnableObject.Update(log);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _methodExecutions.Add(methodExecution);

                    var progressCount = (int)(i / (double)_settings.Iterations * 100);
                    worker.ReportProgress(progressCount, methodExecution);
                }

                if (i == _settings.Iterations)
                {
                    var progressCount = (int)(i / (double)_settings.Iterations * 100);
                    worker.ReportProgress(progressCount, methodExecution);
                }
            }
            _watch.Stop();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _logger.Flush();
            if (e.Cancelled)
            {
                foreach (IGui listener in _uiListeners)
                {
                    listener.Destroy();
                }
            }
            else if (e.Error != null)
            {
                foreach (IGui listener in _uiListeners)
                {
                    listener.Destroy();
                }
            }
        }

        

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _logger.Info(string.Format("id:{0} {1}%",Id,e.ProgressPercentage));
          
            var methodExecution = e.UserState as MethodExecutionResult;
         
            var msg = new MethodExecutionMessage(
                _runnableObject.ObjectType,
                methodExecution,
                _watch.Elapsed,
                e.ProgressPercentage);
            foreach (IGui listener in _uiListeners)
            {
                listener.Update(msg);
            }
        }
    }
}