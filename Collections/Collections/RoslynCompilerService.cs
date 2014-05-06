﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Collections
{
    class RoslynCompilerService : ICompilerService
    {
        public Assembly Compile(string sourceCode)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(sourceCode);
            var compilation = CSharpCompilation.Create(
                "default.dll",
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees: new[] { tree },
                references: new[] { new MetadataFileReference(typeof(object).Assembly.Location) });

            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                compiledAssembly = Assembly.Load(stream.GetBuffer());
            }

            return compiledAssembly;
        }

        public bool TryCompile(string sourceCode, out List<string> errors)
        {
            errors = new List<string>();

            var tree = SyntaxFactory.ParseSyntaxTree(sourceCode);
            var compilation = CSharpCompilation
                .Create("default.dll",
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location));

            IEnumerable<Diagnostic> errorsAndWarnings = compilation.GetDiagnostics();


            if (errorsAndWarnings.Any())
            {
                foreach (var error in errorsAndWarnings)
                {
                    if (error.IsWarningAsError)
                    {
                        errors.Add(error.GetMessage());
                    }

                }
                return false;
            }
            else
            {
                return true;
            }
          


        }
    }
}