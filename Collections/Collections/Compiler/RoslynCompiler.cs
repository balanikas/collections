using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Collections.Compiler
{
    public class RoslynCompiler : ICompiler
    {

        public RoslynCompiler()
        {
            Type = CompilerType.Roslyn;
        }
     

        public bool TryCompile(string sourceCode, out Assembly compiledAssembly)
        {
            compiledAssembly = null;
            return false;
        }

        public bool TryCompile(string[] files, out Assembly compiledAssembly)
        {
            throw new System.NotImplementedException();
        }

        public bool TryCompile(string sourceCode, out Assembly compiledAssembly, out List<string> errors)
        {
            throw new System.NotImplementedException();
        }

        public CompilerType Type { get; private set; }
    }
}