using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Collections
{
    public class Utils
    {
        private static readonly List<Type> _supportedTypes = new List<Type>();
        static Utils()
        {
            _supportedTypes.AddRange(new[]
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
                typeof (SByte[]),
                typeof (Byte[]),
                typeof (Int16[]),
                typeof (UInt16[]),
                typeof (Int32[]),
                typeof (UInt32[]),
                typeof (Int64[]),
                typeof (UInt64[]),
                typeof (Single[]),
                typeof (Double[]),
                typeof (Decimal[]),
                typeof (Boolean[]),
                typeof (Char[]),
                typeof (Object[]),
                typeof (Char*[]),
                typeof (String[]),
                typeof (void)
            });

        }

        public static bool MethodsUseSupportedTypes(IEnumerable<MethodInfo> methods)
        {
            foreach (MethodInfo method in methods)
            {
                if (!IsMethodSupported(method))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsMethodSupported(MethodInfo method)
        {
 
            foreach (ParameterInfo p in method.GetParameters())
            {
                Type isValidType = _supportedTypes.
                    FirstOrDefault(t => t.FullName == p.ParameterType.FullName);
                if (isValidType == null)
                {
                    return false;
                }
            }


            Type isValidReturnType = _supportedTypes.
                FirstOrDefault(t => t.FullName == method.ReturnType.FullName);
            if (isValidReturnType == null)
            {
                return false;
            }
            

            return true;
        }

  


        //private static IEnumerable<Type> GetValidMethodTypes()
        //{
        //    var validTypes = new List<Type>();
        //    validTypes.AddRange(new[]
        //    {
        //        typeof (SByte),
        //        typeof (Byte),
        //        typeof (Int16),
        //        typeof (UInt16),
        //        typeof (Int32),
        //        typeof (UInt32),
        //        typeof (Int64),
        //        typeof (UInt64),
        //        typeof (Single),
        //        typeof (Double),
        //        typeof (Decimal),
        //        typeof (Boolean),
        //        typeof (Char),
        //        typeof (Object),
        //        typeof (Char*),
        //        typeof (String),
        //        typeof (SByte[]),
        //        typeof (Byte[]),
        //        typeof (Int16[]),
        //        typeof (UInt16[]),
        //        typeof (Int32[]),
        //        typeof (UInt32[]),
        //        typeof (Int64[]),
        //        typeof (UInt64[]),
        //        typeof (Single[]),
        //        typeof (Double[]),
        //        typeof (Decimal[]),
        //        typeof (Boolean[]),
        //        typeof (Char[]),
        //        typeof (Object[]),
        //        typeof (Char*[]),
        //        typeof (String[]),
        //        typeof (void)
        //    });

        //    return validTypes;
        //}

    
        public static TimeSpan MeasureExecutionTime<T>(Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            return watch.Elapsed;
        }

        public static TimeSpan MeasureExecutionTime<T>(Action<T> action, T obj)
        {
            var watch = new Stopwatch();
            watch.Start();
            action.Invoke(obj);
            watch.Stop();
            return watch.Elapsed;
        }
    }
}