using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Collections
{
    public class CompiledResultsMessage
    {
        public string Source;
        public List<string> CompilerErrors;
        public bool HasCompiled;

        public CompiledResultsMessage(string source = null)
        {
            CompilerErrors = new List<string>();
            Source = source;

        }

        public override string ToString()
        {
            return "errors: " + CompilerErrors.Count + " hasCompiled: " + HasCompiled + " source: " + Source;
        }
    }
}