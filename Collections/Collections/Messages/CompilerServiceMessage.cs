using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections.Compiler;

namespace Collections.Messages
{
    public class CompilerServiceMessage
    {
        public string Source { get; private set; }
        public ServiceMessageState State { get; private set; }

        public CompilerServiceMessage(string source, ServiceMessageState state = ServiceMessageState.NotHandled)
        {
            Source = source;
            State = state;
        }
    }
}
