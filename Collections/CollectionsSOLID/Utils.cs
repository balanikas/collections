using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Threading;


namespace CollectionsSOLID
{
    

    public static class Utils
    {
       

        public static List<LoadedType> LoadTypesFromDisc(string filePath)
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
                var fileContent = File.ReadAllText(filePath);
                var results = CompileFromFile(filePath);
                foreach (var definedType in results.CompiledAssembly.DefinedTypes)
                {
                    if (definedType.Name == filePath)
                    {
                        types.Add(new LoadedType { FilePath = filePath, Source = fileContent, TypeInfo = definedType,  IsCompilable = true });
                    }
                }
                return types;
            }

            var files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".cs") || s.EndsWith(".vb"));
            foreach (string file in files) 
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                var fileContent = File.ReadAllText(file);

                var results = CompileFromFile(file);
               
                foreach(var definedType in results.CompiledAssembly.DefinedTypes)
                {
                    if(definedType.Name == fileName) //for vb files My.MyApplication is generated...wtf
                    {
                        types.Add(new LoadedType { FilePath = Path.Combine(filePath, file), Source = fileContent, TypeInfo = definedType, IsCompilable = true });
                    }
                }
                 
            }
            return types;
           
        }

        public static List<LoadedType> LoadBclTypes()
        {
            var data = new List<LoadedType>();


            data.Add(new LoadedType { TypeInfo = typeof(int).GetTypeInfo(), Source = "N/A", FilePath = "N/A", IsCompilable = false });
            data.Add(new LoadedType { TypeInfo = typeof(string).GetTypeInfo(), Source = "N/A", FilePath = "N/A", IsCompilable = false });
          
            return data;
        }

        public static List<LoadedType> LoadTypesFromAssembly(string filePath)
        {
            var data = new List<LoadedType>();

            bool isEmpty = String.IsNullOrEmpty(filePath);
            bool isValid = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if(isEmpty || !isValid)
            {
                throw new ArgumentException("bad args: " + filePath);

            }


            bool isFile = filePath.EndsWith(".dll") || filePath.EndsWith(".exe");

            var filePaths = new List<string>();
            if(isFile)
            {
                filePaths.Add(filePath);
            }
            else
            {
                var files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
                filePaths.AddRange(files);
            }

            foreach (var path in filePaths)
            {
                var assembly = Assembly.LoadFile(filePath);
                foreach (var definedType in assembly.DefinedTypes)
                {
                    if (definedType.IsInterface ||
                       definedType.IsAbstract)
                    {
                        continue;
                    }
                    data.Add(new LoadedType { TypeInfo = definedType, FilePath = filePath, Source = "N/A" , IsCompilable = false});
                }
            }

            

            return data;
        }

        private static CompilerResults CompileFromSource(string source, string language = "CSharp")
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var compiler =  CodeDomProvider.CreateProvider(language);
           
            var parms = new System.CodeDom.Compiler.CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
               
            };

            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("System.Core.dll");
            var compilationResults = compiler.CompileAssemblyFromSource(parms, source);

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

        private static CompilerResults CompileFromFile(string filePath)
        {
            string language;
            
            if(filePath.EndsWith(".cs"))
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

            var source = File.ReadAllText(filePath);

            return CompileFromSource(source, language);
        }

        public static void SaveType(LoadedType type)
        {

            if (File.Exists(type.FilePath))
            {
                File.Delete(type.FilePath);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(type.FilePath, true))
            {
                file.Write(type.Source);
            }
        }

        public static bool TryCompileFromSource(string source)
        {
            try
            {
                var compilationResult = CompileFromSource(source);
            }
            catch 
            {

                return false;
            }

            return true;

        }

        public static bool MethodsUseSupportedTypes(IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                foreach (var p in method.GetParameters())
                {
                    var isValidType = GetValidMethodTypes().
                        FirstOrDefault(t => t.FullName == p.ParameterType.FullName);
                    if (isValidType == null)
                    {
                        return false;
                    }
                }

                var isValidReturnType = GetValidMethodTypes().
                    FirstOrDefault(t => t.FullName == method.ReturnType.FullName);
                if (isValidReturnType == null)
                {
                    return false;
                }

            }

            return true;
        }

        private static IEnumerable<Type> GetValidMethodTypes()
        {
            var validTypes = new List<Type>();
            validTypes.AddRange(new[] { 
                typeof(SByte),
                typeof(Byte),
                typeof(Int16),
                typeof(UInt16),
                typeof(Int32),
                typeof(UInt32),
                typeof(Int64),
                typeof(UInt64),
                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(Boolean),
                typeof(Char),
                typeof(Object),
                typeof(Char*),
                typeof(String),
                typeof(void)
            
            });

            return validTypes;
        }

    }
}
