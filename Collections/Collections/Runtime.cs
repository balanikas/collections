using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace Collections
{
   

    public class Runtime : IRuntime
    {
        private readonly CancellationTokenSource _cts;
        private readonly IDictionary<string, IRunner> _runners;
        private bool _isRunning;

        public Runtime()
        {
            _runners = new Dictionary<string, IRunner>();
            _cts = new CancellationTokenSource();

          
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Start()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                while (_isRunning)
                {
                    Thread.Sleep(10000);
                }

                Clear();
            }, _cts.Token);

            _isRunning = true;
        }

        public void Clear()
        {
            foreach (IRunner go in _runners.Values)
            {
                go.Destroy();
            }
        }

        public void Add(IRunner runner)
        {
            _runners.Add(runner.Id, runner);
        }

        public IRunner GetById(string id)
        {
            IRunner obj;
            if (_runners.TryGetValue(id, out obj))
            {
                return obj;
            }
            return null;
        }

        public void Remove(string runnerId)
        {
            IRunner obj;
            if (_runners.TryGetValue(runnerId, out obj))
            {
                _runners.Remove(runnerId);
                obj.Destroy();
            }
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}