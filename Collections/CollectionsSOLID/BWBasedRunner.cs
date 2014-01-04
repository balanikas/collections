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

namespace CollectionsSOLID
{

    public class BWBasedRunner : IRunner
    {
        private IBehavior _behavior;
        private IGui _gui;
        private BackgroundWorker _bw = new BackgroundWorker();
        int _loopCount;
        ILogger _logger;
        Stopwatch _watch;
        public string Id { get; private set; }

        public BWBasedRunner(IBehavior behavior, IGui gui, ILogger logger, int loopCount = 1000000 )
        {

            _gui = gui;
            _behavior = behavior;
            _loopCount = loopCount;
            _logger = logger;
            _watch = new Stopwatch();

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            Id =  Guid.NewGuid().ToString();
            _gui.Id = Id;
        }



        public void Start()
        {
            _gui.Draw();

            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();

            }

        }


        public void Destroy()
        {
            _bw.CancelAsync();
            _gui.Destroy();
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

            int errorCount = 0;
            bool successfulUpdate;
           
            _watch.Start();

            for (int i = 1; i <= _loopCount; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (i % (_loopCount / 10) == 0 || i == _loopCount)
                {
                    successfulUpdate = _behavior.UpdateAndLog(_logger);

                    var progressCount = (int)(i / (double)_loopCount * 100);
                    worker.ReportProgress(progressCount);
                }
                else
                {
                    successfulUpdate = _behavior.Update(_logger);
                }

                if(!successfulUpdate)
                {
                    if(++errorCount >= 10)
                    {
                        _logger.Flush();
                        e.Cancel = true;
                        break;
                    }
                }
                
                
            }
            _watch.Stop();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                _gui.Destroy();
            }
            else if (e.Error != null)
            {
                _gui.Destroy();
            }
            else
            {
                //_gui.Destroy();
            }

        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _logger.Info(Id + ": " + e.ProgressPercentage.ToString());

            ObjectState state = e.ProgressPercentage == 100 ? ObjectState.Finished : ObjectState.Running;

            var msg = new RunnerMessage(
                       _behavior.GetObjectType(),
                       _behavior.GetCollectionType(),
                       _watch.Elapsed,
                       e.ProgressPercentage,
                       1 * e.ProgressPercentage,
                       state);

            _gui.Update((RunnerMessage)msg);

        }
    }
}
