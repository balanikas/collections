using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public interface IRuntime
    {
        void Stop();
        void Start();
        void Clear();
        void Add(IRunner runner);
        IRunner GetById(string id);
        void Remove(string runnerId);
        bool IsRunning();
    }
}
