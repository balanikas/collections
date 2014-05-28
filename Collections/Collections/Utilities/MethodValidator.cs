using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Collections.Utilities
{
    class MethodValidator
    {
        private static readonly List<Type> _supportedTypes = new List<Type>();
        static MethodValidator()
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

        public void ValidateParametersTypes(MethodInfo method)
        {
            
            foreach (ParameterInfo p in method.GetParameters())
            {
              
                Type isValidType = _supportedTypes.
                    FirstOrDefault(t => t.FullName == p.ParameterType.FullName);
                if (isValidType == null)
                {
                    throw new Exception(
                        string.Format("method '{0}' in type '{1}' contains unsupported type '{2}'", 
                        method, 
                        method.ReflectedType,
                        p.ParameterType.FullName));
                    
                }
            }


         


          
        }

        public void ValidateReturnType(MethodInfo method)
        {
           
            Type isValidReturnType = _supportedTypes.
             FirstOrDefault(t => t.FullName == method.ReturnType.FullName);
            if (isValidReturnType == null)
            {
                throw new Exception(
                       string.Format("method '{0}' in type '{1}' contains unsupported return type '{2}'",
                       method,
                       method.ReflectedType,
                       method.ReturnType.FullName));
            }
        }

        public void ValidateMethodKind(MethodInfo method)
        {
            if (method.IsAbstract)
            {
                throw new Exception(string.Format("method '{0}' cannot be abstract", method));    
            }
            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                throw new Exception(string.Format("method '{0}' cannot be generic", method)); 
            }
            if (method.DeclaringType != null && method.DeclaringType.IsInterface)
            {
                throw new Exception(string.Format("method '{0}' declaring type is an interface", method)); 
            }
            
        }



    }
}
