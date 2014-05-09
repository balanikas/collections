using System.Collections.Generic;
using System.Reflection;

namespace Collections.Compiler
{
    public interface ICompiler
    {
        Assembly Compile(string sourceCode);
        Assembly TryCompile(string sourceCode, out List<string> errors);

        CompilerType Type { get; }
    }
}