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
    public interface IRunner
    {
        string Id { get; }
        void Start();
        void Destroy();
        bool IsAlive();
    }



}
