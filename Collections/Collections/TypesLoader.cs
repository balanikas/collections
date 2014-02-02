using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Collections
{
    public class TypesLoader
    {
        readonly ILogger _logger;
        public TypesLoader(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<List<LoadedType>> FromDiscAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromDisc(filePath); });
        }

        public async Task<List<LoadedType>> FromBCLAsync()
        {
            return await Task.Factory.StartNew(() => { return FromBCL(); });
        }

        public async Task<List<LoadedType>> FromAssemblyAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromAssembly(filePath); });
        }

        public List<LoadedType> FromDisc(string filePath)
        {
            bool isEmpty = String.IsNullOrEmpty(filePath);
            bool isValid = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if (isEmpty || !isValid)
            {
                throw new ArgumentException("bad args: " + filePath);
            }

            bool isFile = filePath.EndsWith(".cs") || filePath.EndsWith(".vb");

            var types = new List<LoadedType>();

            if (isFile)
            {
                string fileContent = File.ReadAllText(filePath);
                CompilerResults results = CompileFromFile(filePath);
                foreach (TypeInfo definedType in results.CompiledAssembly.DefinedTypes)
                {
                    if (definedType.Name == filePath)
                    {
                        types.Add(new LoadedType
                        {
                            FilePath = filePath,
                            Source = fileContent,
                            TypeInfo = definedType,
                            IsCompilable = true
                        });
                    }
                }
                return types;
            }

            IEnumerable<string> files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".cs") || s.EndsWith(".vb"));
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileContent = File.ReadAllText(file);

                CompilerResults results = CompileFromFile(file);
               
         
                foreach (TypeInfo definedType in results.CompiledAssembly.DefinedTypes)
                {


                    //if (definedType.Name == fileName) //for vb files My.MyApplication is generated...wtf
                    {
                        types.Add(new LoadedType
                        {
                            FilePath = Path.Combine(filePath, file),
                            Source = fileContent,
                            TypeInfo = definedType,
                            IsCompilable = true
                        });
                    }
                }
            }
            return types;
        }

        public List<LoadedType> FromBCL()
        {
            var data = new List<LoadedType>();


            data.Add(new LoadedType
            {
                TypeInfo = typeof (int).GetTypeInfo(),
                Source = "N/A",
                FilePath = "N/A",
                IsCompilable = false
            });
            data.Add(new LoadedType
            {
                TypeInfo = typeof (string).GetTypeInfo(),
                Source = "N/A",
                FilePath = "N/A",
                IsCompilable = false
            });

            return data;
        }

        public List<LoadedType> FromAssembly(string filePath)
        {
            var data = new List<LoadedType>();

            bool isEmpty = String.IsNullOrEmpty(filePath);
            bool isValid = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if (isEmpty || !isValid)
            {
                throw new ArgumentException("bad args: " + filePath);
            }


            bool isFile = filePath.EndsWith(".dll") || filePath.EndsWith(".exe");

            var filePaths = new List<string>();
            if (isFile)
            {
                filePaths.Add(filePath);
            }
            else
            {
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
                filePaths.AddRange(files);
            }

            foreach (string path in filePaths)
            {
                Assembly assembly = Assembly.LoadFile(filePath);
                foreach (TypeInfo definedType in assembly.DefinedTypes)
                {
                    if (definedType.IsInterface ||
                        definedType.IsAbstract)
                    {
                        continue;
                    }
                    data.Add(new LoadedType
                    {
                        TypeInfo = definedType,
                        FilePath = filePath,
                        Source = "N/A",
                        IsCompilable = false
                    });
                }
            }


            return data;
        }

        private CompilerResults CompileFromSource(string source, string language = "CSharp")
        {
            CodeDomProvider compiler = CodeDomProvider.CreateProvider(language);

            var parms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                CompilerOptions = "/t:library /d:TRACE"
            };
            
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("System.Core.dll");
            CompilerResults compilationResults = compiler.CompileAssemblyFromSource(parms, source);

            if (compilationResults.Errors.Count > 0)
            {
                string message = String.Empty;
                foreach (CompilerError error in compilationResults.Errors)
                {
                    message += error.ErrorText + Environment.NewLine;
                }
                throw new Exception(message);
            }

            return compilationResults;
        }

        private CompilerResults CompileFromSource(string[] sources, string language = "CSharp")
        {
            CodeDomProvider compiler = CodeDomProvider.CreateProvider(language);

            var parms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };

            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("System.Core.dll");

            CompilerResults compilationResults = compiler.CompileAssemblyFromSource(parms, sources);

            if (compilationResults.Errors.Count > 0)
            {
                string message = String.Empty;
                foreach (CompilerError error in compilationResults.Errors)
                {
                    message += error.ErrorText + Environment.NewLine;
                }
                throw new Exception(message);
            }

            return compilationResults;
        }

        private  CompilerResults CompileFromFile(string filePath)
        {
            string language;

            if (filePath.EndsWith(".cs"))
            {
                language = "CSharp";
            }
            else if (filePath.EndsWith(".vb"))
            {
                language = "VisualBasic";
            }
            else
            {
                throw new ArgumentException();
            }

            string source = File.ReadAllText(filePath);

            return CompileFromSource(source, language);
        }


        public void SaveType(LoadedType type)
        {
            if (File.Exists(type.FilePath))
            {
                File.Delete(type.FilePath);
            }
            using (var file = new StreamWriter(type.FilePath, true))
            {
                file.Write(type.Source);
            }
        }

        public bool TryCompileFromSource(string source)
        {
            try
            {
                CompilerResults compilationResult = CompileFromSource(source);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}