using System.Collections.Generic;

namespace Collections.Compiler
{
    public class CompilerServiceMessage
    {
        public string Source;
        public List<string> CompilerErrors;
        public List<LoadedType> Types;

        public CompilerServiceMessage(string source = null)
        {
            CompilerErrors = new List<string>();
            Source = source;
            Types = new List<LoadedType>();
        }

        public override string ToString()
        {
            return "errors: " + CompilerErrors.Count + " source: " + Source;
        }
    }
}