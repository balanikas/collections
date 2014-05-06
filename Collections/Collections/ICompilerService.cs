using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace Collections
{
    public interface ICompilerService
    {
        Assembly Compile(string sourceCode);
        bool TryCompile(string sourceCode, out List<string> errors);
    }
}