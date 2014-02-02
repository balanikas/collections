﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Collections.Messages;

namespace Collections
{
    public class BWBasedRunner : IRunner
    {
        private readonly IBehavior _behavior;
        private readonly BackgroundWorker _bw;
        private readonly ILogger _logger;
        private readonly ConcurrentBag<MethodExecution> _methodExecutions;
        private readonly RunnerSettings _settings;
        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;
        private UIMessage _lastMessage;

        public BWBasedRunner(IBehavior behavior, IGui gui, ILogger logger, RunnerSettings settings)
        {
            _uiListeners = new List<IGui>();

            _behavior = behavior;
            _settings = settings;
            _logger = logger;
            _watch = new Stopwatch();
            _methodExecutions = new ConcurrentBag<MethodExecution>();
            Console.SetOut(new StringWriter());

            _bw = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            Id = Guid.NewGuid().ToString();
            gui.Id = Id;
            _uiListeners.Add(gui);
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
            foreach (IGui listener in _uiListeners)
            {
                listener.Draw();
            }

            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();
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

        public RunSummaryMessage GetState()
        {
            var msg = new RunSummaryMessage(_behavior.GetObjectType(), _watch.Elapsed, _methodExecutions);
            return msg;
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var worker = sender as BackgroundWorker;


            _watch.Start();

            var methodExecution = new MethodExecution();
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
                methodExecution = _behavior.Update(log);
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
            _logger.Info("RUNNERID " + Id + ": " + e.ProgressPercentage + "%");

            ObjectState state = e.ProgressPercentage == 100 ? ObjectState.Finished : ObjectState.Running;

            var methodExecution = e.UserState as MethodExecution;

            _lastMessage = new UIMessage(
                _behavior.GetObjectType(),
                methodExecution,
                _watch.Elapsed,
                e.ProgressPercentage,
                state);
            foreach (IGui listener in _uiListeners)
            {
                listener.Update(_lastMessage);
            }
        }
    }
}