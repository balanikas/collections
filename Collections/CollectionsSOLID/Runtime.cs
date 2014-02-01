using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Collections
{
    public class Runtime
    {
        private readonly CancellationTokenSource _cts;
        private readonly IDictionary<string, IRunner> _gObjects;
        private bool _isRunning;

        public Runtime()
        {
            _gObjects = new Dictionary<string, IRunner>();
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
            foreach (IRunner go in _gObjects.Values)
            {
                go.Destroy();
            }
        }

        public void Add(IRunner runner)
        {
            _gObjects.Add(runner.Id, runner);
        }

        public IRunner GetById(string id)
        {
            IRunner obj;
            if (_gObjects.TryGetValue(id, out obj))
            {
                return obj;
            }
            return null;
        }

        public void Remove(string runnerId)
        {
            IRunner obj;
            if (_gObjects.TryGetValue(runnerId, out obj))
            {
                _gObjects.Remove(runnerId);
                obj.Destroy();
            }
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}