using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    public class UsingTrace
    {
        public void Multiply(int value)
        {
            var result = value*2;
            Trace.Write(string.Format("Multiplied {0} with 2 and got {1}",value, result));
        }
    }
}
