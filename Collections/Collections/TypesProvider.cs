using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            //todo enable support for roslyn compiler
            _activeCompilerService = _services.First(x => x.Type == CompilerType.Default);
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
                types.Add(new LoadedType(definedType, filePath, fileContent));
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
                    types.Add(new LoadedType(definedType, Path.Combine(filePath, file), fileContent));
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
                types.Add(new LoadedType(definedType, filePath, "N/A"));
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
                    types.Add(new LoadedType(definedType, "", source));
                }
            }
           
            return types;

        }
    }
}