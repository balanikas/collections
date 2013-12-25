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

        public static Dictionary<TypeInfo, string> LoadTypesFromDisc(string filePath = null)
        {
            
            var data = new Dictionary<TypeInfo, string>();

            var compiler = new CSharpCodeProvider();
            
            var parms = new System.CodeDom.Compiler.CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                 
            };
            parms.ReferencedAssemblies.Add("System.dll");
            //parms.ReferencedAssemblies.Add("System.Collections.Generic.dll");

            if (String.IsNullOrEmpty(filePath))
            {
                filePath = _defaultResourceDirectory;
            }

            var files = new Dictionary<string, string>();
            foreach (string file in Directory.EnumerateFiles(filePath, "*.cs"))
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                var fileContent = File.ReadAllText(file);
                files.Add(fileName, File.ReadAllText(file));

                var results = compiler.CompileAssemblyFromSource(parms, fileContent);


                
                if (results.Errors.Count == 0)
                {
                    foreach(var definedType in results.CompiledAssembly.DefinedTypes)
                    {
                        if(definedType.Name == fileName)
                        {
                            if(!String.IsNullOrEmpty(definedType.Namespace))
                            {
                                
                                //data.Add(results.CompiledAssembly.CreateInstance(definedType.FullName), fileContent);
                                data.Add(definedType, fileContent);
                            }
                            else
                            {
                                
                                //data.Add(results.CompiledAssembly.CreateInstance(definedType.Name), fileContent);
                                data.Add(definedType, fileContent);
                            }
                            
                        }
                    }
                    
                }


            }
            return data;
           
        }

        public static Dictionary<TypeInfo, string> LoadBclTypes()
        {
            var data = new Dictionary<TypeInfo, string>();
            

            data.Add(typeof(int).GetTypeInfo(), "N/A");
            data.Add(typeof(string).GetTypeInfo(), "N/A");
            data.Add(typeof(DateTime).GetTypeInfo(), "N/A");
            data.Add(typeof(Uri).GetTypeInfo(), "N/A");
            data.Add(typeof(Console).GetTypeInfo(), "N/A");
            data.Add(typeof(Debug).GetTypeInfo(), "N/A");
            return data;
        }

        public static Dictionary<TypeInfo, string> LoadTypesFromAssembly(string filePath)
        {
            var data = new Dictionary<TypeInfo, string>();

            bool isEmpty = String.IsNullOrEmpty(filePath);
            bool isValid = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            if(isEmpty || !isValid)
            {
                throw new ArgumentException("bad args: " + filePath);

            }


            bool isFile = filePath.EndsWith(".dll");

            var filePaths = new List<string>();
            if(isFile)
            {
                filePaths.Add(filePath);
            }
            else
            {
                filePaths.AddRange(Directory.GetFiles(filePath, "*.dll"));
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
                    data.Add(definedType, "N/A");
                }
            }

            

            return data;
        }

        private const string _defaultResourceDirectory = @"C:\dev\collections\Collections\CollectionsSOLID\resources\sampletypes";
        public static void SaveType(string content, string fileName, string filePath = null)
        {
            if(String.IsNullOrEmpty(filePath))
            {
                filePath = _defaultResourceDirectory;
            }


            if(File.Exists(Path.Combine(filePath, fileName + ".cs")))
            {
                File.Delete(Path.Combine(filePath, fileName + ".cs"));
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(filePath, fileName + ".cs"), true))
            {
                file.Write(content);
            }
        }

        public static int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
    }
}
