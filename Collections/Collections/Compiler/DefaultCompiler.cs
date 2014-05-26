using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Collections.Logging;

namespace Collections.Compiler
{
    public class DefaultCompiler : ICompiler
    {
        private readonly CompilerParameters _compilerParams;
        private CodeDomProvider _compiler;
        private string _language;
        private readonly ILogger _logger;


        public DefaultCompiler(ILogger logger, string language = "CSharp")
        {
            Type = CompilerType.Default;
            Language = language;
            _logger = logger;

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

        public bool TryCompile(string sourceCode, out Assembly compiledAssembly)
        {
            compiledAssembly = null;

            

            CompilerResults compilationResults = _compiler.CompileAssemblyFromSource(_compilerParams, sourceCode);

            if (compilationResults.Errors.Count > 0)
            {
                foreach (var error in compilationResults.Errors)
                {
                    
                    _logger.ErrorNow(error.ToString());
                    
                }
                
                return false;
            }

            compiledAssembly = compilationResults.CompiledAssembly;
            return true;
        }

        public bool TryCompile(string [] files, out Assembly compiledAssembly)
        {
            compiledAssembly = null;



            CompilerResults compilationResults = _compiler.CompileAssemblyFromFile(_compilerParams, files);

            if (compilationResults.Errors.Count > 0)
            {
                foreach (var error in compilationResults.Errors)
                {

                    _logger.ErrorNow(error.ToString());

                }

                return false;
            }

            compiledAssembly = compilationResults.CompiledAssembly;
            return true;
        }


        public bool TryCompile(string sourceCode, out Assembly compiledAssembly, out List<string> errors)
        {
            compiledAssembly = null;
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

            compiledAssembly = compilationResults.CompiledAssembly;
            return true;
        }

        public CompilerType Type
        {
            get; private set;
        }
    }
}