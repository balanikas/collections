using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Collections
{
    public class BWBasedRunner : IRunner
    {
        private readonly IBehavior _behavior;
        private readonly BackgroundWorker _bw = new BackgroundWorker();
        private readonly ILogger _logger;
        private readonly int _loopCount;
        private readonly ConcurrentBag<MethodExecution> _methodExecutions;

        private readonly List<IGui> _uiListeners;
        private readonly Stopwatch _watch;
        private UIMessage _lastMessage;

        public BWBasedRunner(IBehavior behavior, IGui gui, ILogger logger, int loopCount = 1000000)
        {
            _uiListeners = new List<IGui>();

            _behavior = behavior;
            _loopCount = loopCount;
            _logger = logger;
            _watch = new Stopwatch();
            _methodExecutions = new ConcurrentBag<MethodExecution>();

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
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

            for (int i = 1; i <= _loopCount; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                TimeSpan beforeExecution = _watch.Elapsed;
                bool log = i%(_loopCount/100) == 0 || i == _loopCount;
                methodExecution = _behavior.Update(log);
                if (methodExecution != null)
                {
                    methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;
                    _methodExecutions.Add(methodExecution);

                    var progressCount = (int) (i/(double) _loopCount*100);
                    worker.ReportProgress(progressCount, methodExecution);
                }

                if (i == _loopCount)
                {
                    var progressCount = (int) (i/(double) _loopCount*100);
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