using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace CollectionsSOLID
{

    public class BWBasedRunner : IRunner
    {
        private IBehavior _behavior;
        private IGui _gui;
        private BackgroundWorker _bw = new BackgroundWorker();
        int _loopCount;

        public string Id { get; private set; }

        public BWBasedRunner(IBehavior behavior, IGui gui, int loopCount = 1000000)
        {

            _gui = gui;
            _behavior = behavior;
            _loopCount = loopCount;
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
            _gui.Init();

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
            BackgroundWorker worker = sender as BackgroundWorker;

          
            int sizeOfObject = 1;// Utils.GetObjectSize(Activator.CreateInstance(_behavior.GetObjectType()));

           
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 1; (i <= _loopCount); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    _behavior.Update();

                    if (i % 100 == 0 || i == _loopCount)
                    {
                        ObjectState state = ObjectState.Running;
                        if (i == _loopCount)    
                        {
                            watch.Stop();
                            state = ObjectState.Finished;
                        }

                        var msg = new Message(
                            _behavior.GetObjectType(), 
                            _behavior.GetCollectionType(), 
                            watch.Elapsed,
                            (int)(i / (double)_loopCount * 100), 
                            sizeOfObject * i,
                            state);

                        worker.ReportProgress(msg.Progress, msg);
                    }

                }
            }
            watch.Stop();
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
            _gui.Update((Message)e.UserState);

        }
    }
}
