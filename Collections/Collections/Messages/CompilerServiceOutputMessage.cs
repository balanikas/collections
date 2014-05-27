using System.Collections.Generic;
using System.Linq;

namespace Collections.Messages
{
    public class CompilerServiceOutputMessage
    {
       
        public List<string> CompilerErrors {get; private set; }
        public List<LoadedType> Types { get; private set; }

        public ServiceMessageState State { get; private set; }
        public CompilerServiceOutputMessage(List<string> errors, List<LoadedType> types,ServiceMessageState state = ServiceMessageState.NotHandled)
        {
            CompilerErrors = errors;

            Types = types;
            State = state;
        }
    }
}