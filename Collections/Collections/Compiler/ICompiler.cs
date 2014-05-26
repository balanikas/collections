using System.Collections.Generic;
using System.Reflection;

namespace Collections.Compiler
{
    public interface ICompiler
    {
        bool TryCompile(string sourceCode, out Assembly compiledAssembly);
        bool TryCompile(string sourceCode, out Assembly compiledAssembly, out List<string> errors);
        bool TryCompile(string[] files, out Assembly compiledAssembly);
        CompilerType Type { get; }
    }
}