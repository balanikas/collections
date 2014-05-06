using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Collections
{
    public static class Utils
    {
        public static bool MethodsUseSupportedTypes(IEnumerable<MethodInfo> methods)
        {
            foreach (MethodInfo method in methods)
            {
                foreach (ParameterInfo p in method.GetParameters())
                {
                    Type isValidType = GetValidMethodTypes().
                        FirstOrDefault(t => t.FullName == p.ParameterType.FullName);
                    if (isValidType == null)
                    {
                        return false;
                    }
                }


                Type isValidReturnType = GetValidMethodTypes().
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
            validTypes.AddRange(new[]
            {
                typeof (SByte),
                typeof (Byte),
                typeof (Int16),
                typeof (UInt16),
                typeof (Int32),
                typeof (UInt32),
                typeof (Int64),
                typeof (UInt64),
                typeof (Single),
                typeof (Double),
                typeof (Decimal),
                typeof (Boolean),
                typeof (Char),
                typeof (Object),
                typeof (Char*),
                typeof (String),
                typeof (void)
            });

            return validTypes;
        }

        public static  bool IsValidAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return false;
            }
            string extension = Path.GetExtension(assemblyPath);
            if (extension == ".dll" ||
                extension == ".exe")
            {
                return true;
            }
            return false;
        }
    }
}