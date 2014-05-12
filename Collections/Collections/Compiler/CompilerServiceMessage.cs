using System;
using System.Collections.Generic;

namespace Collections.Compiler
{

    public enum ServiceMessageState
    {
        NotHandled,
        Succeeded,
        Failed
    }

    public class CompilerServiceMessage
    {
        public string Source { get; private set; }
        public List<string> CompilerErrors {get; set; }
        public List<LoadedType> Types { get; set; }

        public ServiceMessageState State { get;  set; }
        public CompilerServiceMessage( string source = null,ServiceMessageState state = ServiceMessageState.NotHandled)
        {
            CompilerErrors = new List<string>();
            Source = source;
            Types = new List<LoadedType>();
            State = state;
        }

        public CompilerServiceMessage(CompilerServiceMessage copyFrom, ServiceMessageState state = ServiceMessageState.NotHandled)
        {
            State = state;
            Source = copyFrom.Source;
            CompilerErrors = copyFrom.CompilerErrors;
            Types = copyFrom.Types;
        }

        public override string ToString()
        {
            return "errors: " + CompilerErrors.Count + " source: " + Source;
        }
    }
}