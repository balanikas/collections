using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Collections.Compiler;
using Collections.Logging;

namespace Collections
{
    public class TypesProvider
    {
        private ICompiler _activeCompilerService;
        private readonly IEnumerable<ICompiler> _services;
        private readonly ILogger _logger;

        public TypesProvider(IEnumerable<ICompiler> services , ILogger logger)
        {
            _services = services.ToList();
            _logger = logger;
        }

      
        public void SetActiveCompilerService(CompilerType type)
        {
            _activeCompilerService = _services.First(x => x.Type == type);
        }
        public async Task<List<LoadedType>> FromSourceFolderAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromSourceFolder(filePath); });
        }

        public async Task<List<LoadedType>> FromSourceFileAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromSourceFile(filePath); });
        }

        public async Task<List<LoadedType>> FromAssemblyFileAsync(string filePath)
        {
            return await Task.Factory.StartNew(() => { return FromAssemblyFile(filePath); });
        }


        public List<LoadedType> FromSourceFile(string filePath)
        {
            var types = new List<LoadedType>();
            string fileContent = File.ReadAllText(filePath);
            Assembly compiledAssembly;
            if (!_activeCompilerService.TryCompile(fileContent, out compiledAssembly))
            {
                return types;
            }
            foreach (TypeInfo definedType in compiledAssembly.DefinedTypes)
            {

                types.Add(new LoadedType
                {
                    MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
                    FilePath = filePath,
                    Source = fileContent,
                    TypeInfo = definedType,
                    IsCompilable = true,
                    AllowEditSource = true
                });

            }
            return types;
        }

        public List<LoadedType> FromSourceFolder(string filePath)
        {
            var types = new List<LoadedType>();
            var files = Directory.EnumerateFiles(filePath, "*.cs", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                string fileContent = File.ReadAllText(file);

                Assembly compiledAssembly;


                if (!_activeCompilerService.TryCompile(fileContent, out compiledAssembly))
                {
                    continue;
                }

                foreach (TypeInfo definedType in compiledAssembly.DefinedTypes)
                {

                    types.Add(new LoadedType
                    {
                        MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
                        FilePath = Path.Combine(filePath, file),
                        Source = fileContent,
                        TypeInfo = definedType,
                        IsCompilable = true,
                        AllowEditSource = true
                    });
                }
            }
            return types;
        }


        public List<LoadedType> FromAssemblyFile(string filePath)
        {
            var types = new List<LoadedType>();

            Assembly assembly = Assembly.LoadFile(filePath);

            try
            {
                assembly.DefinedTypes.Any();
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var le in e.LoaderExceptions)
                {
                    _logger.ErrorNow(le.Message);

                }
                return types;
            }

            foreach (TypeInfo definedType in assembly.DefinedTypes)
            {
                if (definedType.IsInterface ||
                    definedType.IsAbstract)
                {
                    continue;
                }
                types.Add(new LoadedType
                {
                    MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
                    TypeInfo = definedType,
                    FilePath = filePath,
                    Source = "N/A",
                    IsCompilable = false
                });
            }
            
            return types;
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

        public List<LoadedType> TryCompileFromText(string source, out List<string> errors)
        {
            var types = new List<LoadedType>();
            Assembly compiledAssembly;
            if( _activeCompilerService.TryCompile(source, out compiledAssembly,out errors))
            {
                foreach (TypeInfo definedType in compiledAssembly.DefinedTypes)
                {
                    types.Add(new LoadedType
                    {
                        MethodsInfos = new List<MethodInfo>(definedType.GetMethods()),
                        FilePath = "",
                        Source = source,
                        TypeInfo = definedType,
                        IsCompilable = true
                    });
                }
            }
           
            return types;

        }
    }
}