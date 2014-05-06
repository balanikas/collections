using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Collections
{
    public class DefaultCompilerService : ICompilerService
    {
        private readonly CompilerParameters _compilerParams;
        private CodeDomProvider _compiler;
        private string _language;

        public DefaultCompilerService(string language = "CSharp")
        {
            Language = language;

            _compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                CompilerOptions = "/t:library /d:TRACE"
            };

            _compilerParams.ReferencedAssemblies.Add("System.dll");
            _compilerParams.ReferencedAssemblies.Add("System.Core.dll");
        }

        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                _compiler = CodeDomProvider.CreateProvider(_language);
            }
        }

        public Assembly Compile(string sourceCode)
        {
            Assert.IsNotNull(sourceCode);
          
            CompilerResults compilationResults = _compiler.CompileAssemblyFromSource(_compilerParams, sourceCode);

            if (compilationResults.Errors.Count > 0)
            {
                string message = String.Empty;
                foreach (CompilerError error in compilationResults.Errors)
                {
                    message += error.ErrorText + Environment.NewLine;
                }
                throw new Exception(message);
            }
            return compilationResults.CompiledAssembly;
        }

        public bool TryCompile(string sourceCode, out List<string> errors)
        {
            Assert.IsNotNull(sourceCode);

            errors = new List<string>();

            CompilerResults compilationResults = _compiler.CompileAssemblyFromSource(_compilerParams, sourceCode);

            if (compilationResults.Errors.Count > 0)
            {
                foreach (var error in compilationResults.Errors)
                {
                    errors.Add(error.ToString());
                }
                
                return false;
            }

            return true;
        }

    }
}