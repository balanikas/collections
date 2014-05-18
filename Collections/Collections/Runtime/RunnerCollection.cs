using System.Collections.Generic;
using System.Linq;

namespace Collections.Runtime
{
    class RunnerCollection : IRunnerCollection
    {
        private readonly IDictionary<string, IRunner> _runners;

        public RunnerCollection()
        {
            _runners = new Dictionary<string, IRunner>();
        }
        public void Add(IRunner runner)
        {
            if (!_runners.ContainsKey(runner.Id))
            {
                _runners.Add(runner.Id, runner);

            }
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

        public void RemoveById(string runnerId)
        {
            IRunner obj;
            if (_runners.TryGetValue(runnerId, out obj))
            {
                _runners.Remove(runnerId);
                obj.Destroy();
            }
        }

        public void Remove(IRunner runner)
        {
            RemoveById(runner.Id);
        }

        public void RemoveAll()
        {
            foreach (IRunner r in _runners.Values)
            {
                r.Destroy();
            }
            _runners.Clear();
        }

        public IEnumerable<IRunner> GetActiveRunners()
        {
            return _runners.Where(x => x.Value.IsAlive() == true).Select(x=> x.Value);

        }
    }
}