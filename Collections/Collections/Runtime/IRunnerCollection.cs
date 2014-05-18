using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections.Runtime
{
    public interface  IRunnerCollection
    {
        void Add(IRunner runner);
        IRunner GetById(string id);
        void RemoveById(string runnerId);
        void Remove(IRunner runner);

        void RemoveAll();
        IEnumerable<IRunner> GetActiveRunners();
    }
}
