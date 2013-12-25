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
        public string Id { get; private set; }
        public TplBasedRunner(IBehavior behavior, IGui gui, int loopCount = 1000000)
        {

            _gui = gui;
            _behavior = behavior;
            _loopCount = loopCount;

            _cts = new CancellationTokenSource();

            Id = Guid.NewGuid().ToString();
            _gui.Id = Id;
        }

         public void Start()
        {
            _gui.Init();


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
                 

                _behavior.Update();

                //check if end of loop, or check every now and then
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
                        (int)(i/ (double)_loopCount * 100), 
                        sizeOfObject * i,
                        state);

                    _gui.Update(msg);
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
    }
}
