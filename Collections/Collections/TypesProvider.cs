using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace Collections
{
    public class TypesProvider
    {
        private readonly ICompilerService _compilerService;
        public TypesProvider(ICompilerService compilerService)
        {
            _compilerService = compilerService;
        }
        public async Task<List<LoadedType>> FromDiscAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromDisc(filePath); });
        }

        public async Task<List<LoadedType>> FromBCLAsync()
        {
            return await Task.Factory.StartNew(() => { return FromBaseClassLibrary(); });
        }

        public async Task<List<LoadedType>> FromAssemblyAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromAssembly(filePath); });
        }

        public List<LoadedType> FromDisc(string filePath)
        {
            //bool isEmpty = String.IsNullOrEmpty(filePath);
            //bool isValid = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            //if (isEmpty || !isValid)
            //{
            //    throw new ArgumentException("bad args: " + filePath);
            //}

            bool isFile = filePath.EndsWith(".cs") || filePath.EndsWith(".vb");

            var types = new List<LoadedType>();

            if (isFile)
            {
                string fileContent = File.ReadAllText(filePath);
                var compiledAssembly = CompileFromFile(filePath);
                foreach (TypeInfo definedType in compiledAssembly.DefinedTypes)
                {

                    types.Add(new LoadedType
                    {
                        MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
                        FilePath = filePath,
                        Source = fileContent,
                        TypeInfo = definedType,
                        IsCompilable = true
                    });
                    
                }
               
            }
            else
            {
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
               .Where(s => s.EndsWith(".cs")/* || s.EndsWith(".vb")*/);
                foreach (string file in files)
                {
                    //string fileName = Path.GetFileNameWithoutExtension(file);
                    string fileContent = File.ReadAllText(file);

                    var compiledAssembly = CompileFromFile(file);


                    foreach (TypeInfo definedType in compiledAssembly.DefinedTypes)
                    {

                        types.Add(new LoadedType
                        {
                            MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
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


        public List<LoadedType> FromBaseClassLibrary()
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



        private  Assembly CompileFromFile(string filePath)
        {
            //string language;

            //if (filePath.EndsWith(".cs"))
            //{
            //    language = "CSharp";
            //}
            //else if (filePath.EndsWith(".vb"))
            //{
            //    language = "VisualBasic";
            //}
            //else
            //{
            //    throw new ArgumentException();
            //}

            string source = File.ReadAllText(filePath);

            return _compilerService.Compile(source);
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

        public bool TryCompileFromSource(string source, out List<string> errors)
        {
            return _compilerService.TryCompile(source, out errors);
        }
    }
}