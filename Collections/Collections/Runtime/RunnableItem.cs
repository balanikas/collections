using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Collections.Runtime
{
    public class RunnableItem : IRunnable
    {
        private readonly List<ExecutionInfo> _executionInfos;
        private readonly object _objectInstance;


        private ThreadSafeRandom _randomizer;
        public Type ObjectType { get; private set; }

        public RunnableItem(Type type, List<MethodInfo> methods)
        {
            ObjectType = type;
            bool areMethodsValid = Utils.MethodsUseSupportedTypes(methods);
            if (!areMethodsValid)
            {
                throw new ArgumentException(string.Format("method {0}  in type {1} contains unsupported types",methods[0], type));
            }

            _objectInstance = CreateInstanceFromType(ObjectType);
            if (_objectInstance == null)
            {
                IEnumerable<MethodInfo> staticMethods = methods.Where(m => !m.IsStatic);
                if (staticMethods.Any())
                {
                    throw new ArgumentException(string.Format("cannot invoke non-static method of static class {0}", type));
                }
            }

            _executionInfos = new List<ExecutionInfo>();
            foreach (MethodInfo methodInfo in methods)
            {
                var executionInfo = new ExecutionInfo(methodInfo, _objectInstance);
                _executionInfos.Add(executionInfo);
            }
        }

        private ThreadSafeRandom Randomizer
        {
            get { return _randomizer ?? (_randomizer = new ThreadSafeRandom()); }
        }

        public MethodExecutionResult Update(bool log)
        {
            MethodExecutionResult methodExecutionResult = null;

            foreach (ExecutionInfo execInfo in _executionInfos)
            {
                object[] parameters = null;
                
                ParameterInfo[] reflectedParams = execInfo.ParameterInfos;
                int paramCount = reflectedParams.Length;
                if (paramCount > 0)
                {
                    parameters = new object[paramCount];
                    for (int index = 0; index < paramCount; index++)
                    {
                        ParameterInfo p = reflectedParams[index];
                        object paramValue = Randomizer.RandomizeParamValue(p.ParameterType.Name);
                        parameters[index] = paramValue;
                    }
                }


                try
                {
                    object result;
                    if (execInfo.Cached != null)
                    {
                        //special optimization when method has no return value and no params
                        execInfo.Cached();
                        result = null;
                    }
                    else
                    {
                        result = execInfo.MethodInfo.Invoke(_objectInstance, parameters);
                    }

                    if (log)
                    {
                        methodExecutionResult = new MethodExecutionResult();
                        methodExecutionResult.ReturnValue = result;
                        methodExecutionResult.Success = true;
                        methodExecutionResult.ArgsValues = parameters;
                        methodExecutionResult.Name = execInfo.MethodInfo.Name;
                    }
                }
                catch (Exception e)
                {
                    methodExecutionResult = new MethodExecutionResult();
                    methodExecutionResult.Success = false;
                    methodExecutionResult.Name = execInfo.MethodInfo.Name;
                    string errorMsg = e.InnerException != null ? e.InnerException.Message : e.Message;
                    methodExecutionResult.ErrorMessage = errorMsg;
                    return methodExecutionResult;
                }
            }

            return methodExecutionResult;
        }

        

        private object CreateInstanceFromType(Type type)
        {
            object obj;

            if (type.IsValueType)
            {
                obj = Activator.CreateInstance(type);
            }
            else
            {
                ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                {
                    if (!type.IsSealed && type.IsAbstract)
                    {
                        //abstract class
                        throw new Exception("abstract classes are not allowed");
                    }
                    if (type.IsSealed && type.IsAbstract)
                    {
                        //static class
                        return null;
                    }
                    throw new Exception("only types with empty constructors are allowed");
                }
                //constructor is paramless
                obj = ci.Invoke(new object[] {});
            }
            return obj;
        }
    }
}