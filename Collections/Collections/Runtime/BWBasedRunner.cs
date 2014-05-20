﻿using System;
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
        private readonly IRunnable _runnableObject;
        private readonly BackgroundWorker _bw;
        private readonly ILogger _logger;
        private readonly ConcurrentBag<MethodExecutionResult> _methodExecutions;
        private readonly RunnerSettings _settings;
        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;
        private int _execCount;

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
            //foreach (IGui listener in _uiListeners)
            //{
            //    listener.Destroy();
            //}
        }

        public bool IsAlive()
        {
            if (_bw.IsBusy)
            {
                return true;
            }

            return false;
        }

        public MethodExecutionSummaryMessage GetCurrentState()
        {
            return new MethodExecutionSummaryMessage(_runnableObject.ObjectType, _watch.Elapsed, _methodExecutions, _execCount);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //_logger.InfoNow(string.Format("id:{0} {1}%",Id,0));
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

                TimeSpan beforeExecution = _watch.Elapsed;
                bool log =  _settings.Iterations >= 100 && _execCount % (_settings.Iterations / 100) == 0 || _execCount == _settings.Iterations;
               
                methodExecution = _runnableObject.Update(log);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _methodExecutions.Add(methodExecution);

                    var progressCount = (int)(_execCount / (double)_settings.Iterations * 100);
                    worker.ReportProgress(progressCount, methodExecution);
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
                _runnableObject.ObjectType,
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