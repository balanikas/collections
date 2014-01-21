using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    class TplBasedRunner : IRunner
    {
        IBehavior _behavior;
        int _loopCount;
        CancellationTokenSource _cts;
        Task _task;
        ILogger _logger;
        List<IGui> _uiListeners;
        public string Id { get; private set; }
        public TplBasedRunner(IBehavior behavior, IGui gui,ILogger logger, int loopCount = 1000000)
        {
            _uiListeners = new List<IGui>();
            
            _behavior = behavior;
            _loopCount = loopCount;
            _logger = logger;
            _cts = new CancellationTokenSource();

            Id = Guid.NewGuid().ToString();
            gui.Id = Id;

          
            _uiListeners.Add(gui);
        }
        public void AddUIListener(IGui listener)
        {
            if (!_uiListeners.Contains(listener))
            {
                listener.Id = Id;
                _uiListeners.Add(listener);
            }
        }

         public void Start()
        {
            foreach (var listener in _uiListeners)
            {
                listener.Draw();
            }
            


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

                 MethodExecution methodExecution;
               

                //check if end of loop, or check every now and then
                if (i % ( _loopCount / 10) == 0 || i == _loopCount)
                {
                    methodExecution = _behavior.Update();

                    ObjectState state = ObjectState.Running;
                    if (i == _loopCount)
                    {
                        watch.Stop();
                        state = ObjectState.Finished;
                        _logger.Flush();
                    }

                    var progressCount = (int)(i / (double)_loopCount * 100);
                    _logger.Info(Id + ": " + progressCount.ToString());

                    var msg = new UIMessage(
                        _behavior.GetObjectType(), 
                        methodExecution,
                        watch.Elapsed,
                        progressCount, 
                        state);

                    foreach (var listener in _uiListeners)
                    {
                        listener.Update(msg);
                    }
                }
                else
                {
                    _behavior.Update();
                }

                 
             }
             watch.Stop();
         }

        public void Destroy()
        {
            _cts.Cancel();
            foreach (var listener in _uiListeners)
            {
                listener.Destroy();
            }
        }

        public bool IsAlive()
        {
            if(_task.Status == TaskStatus.Running)
            {
                return true;
            }

            return false;
        }


        public RunSummaryMessage GetState()
        {
            return null;
        }
    }
}
