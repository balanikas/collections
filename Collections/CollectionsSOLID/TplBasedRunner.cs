using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CollectionsSOLID
{
    class TplBasedRunner : IRunner
    {
        IBehavior _behavior;
        IGui _gui;
        int _loopCount;
        CancellationTokenSource _cts;
        Task _task;
        ILogger _logger;
        public string Id { get; private set; }
        public TplBasedRunner(IBehavior behavior, IGui gui,ILogger logger, int loopCount = 1000000)
        {

            _gui = gui;
            _behavior = behavior;
            _loopCount = loopCount;
            _logger = logger;
            _cts = new CancellationTokenSource();

            Id = Guid.NewGuid().ToString();
            _gui.Id = Id;
        }

         public void Start()
        {
            _gui.Draw();


            _task = Task.Factory.StartNew(() =>
            {
                
                DoWork();
            },_cts.Token);

            _task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                               
                    case TaskStatus.Canceled:
                        break;
                    case TaskStatus.RanToCompletion:
                        break;
                    case TaskStatus.Faulted:
                        if (t.Exception != null)
                        {
                            Debugger.Break();
                        }
                        break;
                    default:
                        break;
                }
            }, _cts.Token);
             

        }
     
        private void DoWork()
         {
             
             int sizeOfObject = 1;
            //todo: not having following line, fucks up loop..sometimes...wtf investigate 
             Debug.WriteLine(_task); 

             var watch = new Stopwatch();
             watch.Start();

             for (int i = 1; (i <= _loopCount); i++)
             {
                 CancellationToken ct = _cts.Token;
                 ct.ThrowIfCancellationRequested();
                 

               

                //check if end of loop, or check every now and then
                if (i % ( _loopCount / 10) == 0 || i == _loopCount)
                {
                    _behavior.UpdateAndLog(_logger);

                    ObjectState state = ObjectState.Running;
                    if (i == _loopCount)
                    {
                        watch.Stop();
                        state = ObjectState.Finished;
                        _logger.Flush();
                    }

                    var progressCount = (int)(i / (double)_loopCount * 100);
                    _logger.Info(Id + ": " + progressCount.ToString());

                    var msg = new RunnerMessage(
                        _behavior.GetObjectType(), 
                        _behavior.GetCollectionType(), 
                        watch.Elapsed,
                        progressCount, 
                        sizeOfObject * i,
                        state);

                    _gui.Update(msg);
                }
                else
                {
                    _behavior.Update(_logger);
                }

                 
             }
             watch.Stop();
         }

        public void Destroy()
        {
            _cts.Cancel();
            _gui.Destroy();
        }

        public bool IsAlive()
        {
            if(_task.Status == TaskStatus.Running)
            {
                return true;
            }

            return false;
        }


        public Message GetState()
        {
            return null;
        }
    }
}
