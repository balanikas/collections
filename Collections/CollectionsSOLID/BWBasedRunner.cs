using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Collections.Concurrent;

namespace CollectionsSOLID
{

    public class BWBasedRunner : IRunner
    {
        private IBehavior _behavior;
        private BackgroundWorker _bw = new BackgroundWorker();
        int _loopCount;
        ILogger _logger;
        Stopwatch _watch;
        Message _lastMessage;
        ConcurrentBag<MethodExecution> _methodExecutions;
        List<IGui> _uiListeners;

        public string Id { get; private set; }

        public BWBasedRunner(IBehavior behavior, IGui gui, ILogger logger, int loopCount = 1000000 )
        {
            _uiListeners = new List<IGui>();

            _behavior = behavior;
            _loopCount = loopCount;
            _logger = logger;
            _watch = new Stopwatch();
            _methodExecutions = new ConcurrentBag<MethodExecution>();

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            Id =  Guid.NewGuid().ToString();
            gui.Id = Id;
            _uiListeners.Add(gui);
        }

        public void AddUIListener(IGui listener)
        {
            if(!_uiListeners.Contains(listener))
            {
                _uiListeners.Add(listener);
            }
        }

        public void Start()
        {
            foreach (var listener in _uiListeners)
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
            foreach (var listener in _uiListeners)
            {
                listener.Destroy();
            }
        }

        public bool IsAlive()
        {
            if(_bw.IsBusy)
            { 
                return true;
            }

            return false;
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            BackgroundWorker worker = sender as BackgroundWorker;

           
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

                var beforeExecution = _watch.Elapsed; 
                methodExecution = _behavior.Update();
                methodExecution.ExecutionTime = _watch.Elapsed - beforeExecution;

                if (i % (_loopCount / 10) == 0 || i == _loopCount)
                {
                    
                    var progressCount = (int)(i / (double)_loopCount * 100);
                    worker.ReportProgress(progressCount, methodExecution);
                }
               

                _methodExecutions.Add(methodExecution);
               
                
            }
            _watch.Stop();
            var c = _methodExecutions.Where(x => x.Success == false);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _logger.Flush();
            if ((e.Cancelled == true))
            {
                foreach (var listener in _uiListeners)
                {
                    listener.Destroy();
                }
            }
            else if (e.Error != null)
            {
                foreach (var listener in _uiListeners)
                {
                    listener.Destroy();
                }
            }
            else
            {
                //_gui.Destroy();
            }

        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _logger.Info("RUNNERID " + Id + ": " + e.ProgressPercentage.ToString() + "%");

            ObjectState state = e.ProgressPercentage == 100 ? ObjectState.Finished : ObjectState.Running;

            var methodExecution = e.UserState as MethodExecution;

            _lastMessage = new UIMessage(
                       _behavior.GetObjectType(),
                       methodExecution.Name,
                       _watch.Elapsed,
                       e.ProgressPercentage,
                       state);
            foreach (var listener in _uiListeners)
            {
                listener.Update(_lastMessage);
            }
            
        }

        public RunSummaryMessage GetState()
        {
            var msg = new RunSummaryMessage(_behavior.GetObjectType(), _watch.Elapsed, _methodExecutions);
            return msg;
        }
    }
}
