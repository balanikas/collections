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
using System.Linq;
using System.CodeDom.Compiler;

namespace CollectionsSOLID
{
    

    public static class Utils
    {
        static Random rnd = new Random();
        public static object RandomizeParamValue(string typeName) 
        {

            switch (typeName)
	        {
                case "SByte":
                    {
                        var bytes = new byte[sizeof(sbyte)];
                        rnd.NextBytes(bytes);
                        return (sbyte)bytes[0];
                    }
                case "Byte":
                    {
                        var bytes = new byte[sizeof(byte)];
                        rnd.NextBytes(bytes);
                        return bytes[0];
                    }
                case "Int16":
                    {

                        var bytes = new byte[sizeof(Int16)];
                        rnd.NextBytes(bytes);
                        return BitConverter.ToInt16(bytes, 0);
                    }
                case "UInt16":
                     {
                         var bytes = new byte[sizeof(UInt16)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToUInt16(bytes,0);
                    }
                case "Int32":
                     {
                         var bytes = new byte[sizeof(Int32)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToInt32(bytes, 0);
                     }
                case "UInt32":
                     {
                         var bytes = new byte[sizeof(UInt32)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToUInt32(bytes, 0);
                     }
                case "Int64":
                     {
                         var bytes = new byte[sizeof(Int64)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToInt64(bytes, 0);
                     }
                case "UInt64":
                     {
                         var bytes = new byte[sizeof(UInt64)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToUInt64(bytes, 0);
                     }
                case "Single":
                     {
                         var bytes = new byte[sizeof(float)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToSingle(bytes, 0);
                     }
                case "Double":
                     {
                         var bytes = new byte[sizeof(double)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToDouble(bytes, 0);
                     }
                case "Decimal":
                     {
                         byte scale = (byte)rnd.Next(29);
                         bool sign = rnd.Next(2) == 1;
                         return new decimal(NextInt32(),
                                            NextInt32(),
                                            NextInt32(),
                                            sign,
                                            scale);
                     }
                case "Boolean":
                     {
                         var bytes = new byte[sizeof(bool)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToBoolean(bytes, 0);
                     }
                case "Char":
                     {
                         var bytes = new byte[sizeof(char)];
                         rnd.NextBytes(bytes);
                         return BitConverter.ToChar(bytes, 0);
                     }
                case "Object":
                     {
                         return new object();
                     }
                case "Char*":
                case "String":
                     {
                         var size = 10;
                         StringBuilder builder = new StringBuilder();
                         char ch;
                         for (int i = 0; i < size; i++)
                         {
                             ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
                             builder.Append(ch);
                         }

                         return builder.ToString();
                     }
		        default:
                    throw new NotSupportedException();
	        }
            
        }

        private static int NextInt32()
        {
            unchecked
            {
                int firstBits = rnd.Next(0, 1 << 4) << 28;
                int lastBits = rnd.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

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
                        types.Add(new LoadedType { FilePath = filePath, Source = fileContent, TypeInfo = definedType });
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
                        types.Add(new LoadedType { FilePath = Path.Combine(filePath,file), Source = fileContent, TypeInfo = definedType});
                    }
                }
                 
            }
            return types;
           
        }

        public static List<LoadedType> LoadBclTypes()
        {
            var data = new List<LoadedType>();


            data.Add(new LoadedType { TypeInfo = typeof(int).GetTypeInfo() , Source = "N/A", FilePath = "N/A"});
            data.Add(new LoadedType { TypeInfo = typeof(string).GetTypeInfo(), Source = "N/A", FilePath = "N/A" });
          
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
                    data.Add(new LoadedType { TypeInfo = definedType, FilePath = filePath, Source = "N/A" });
                }
            }

            

            return data;
        }

        private static CompilerResults CompileFromSource(string source, string language = "CSharp")
        {
            var compiler =  CodeDomProvider.CreateProvider(language);
           
            var parms = new System.CodeDom.Compiler.CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,

            };

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

        public static void CompileAndSaveType(LoadedType type)
        {

            var compilationResults = CompileFromFile(type.FilePath);

            if (File.Exists(type.FilePath))
            {
                File.Delete(type.FilePath);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(type.FilePath, true))
            {
                file.Write(type.Source);
            }
        }

    }
}
