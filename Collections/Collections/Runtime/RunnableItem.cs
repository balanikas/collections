using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Collections.Utilities;

namespace Collections.Runtime
{
    public class RunnableItem : IRunnable
    {
        private readonly List<ExecutionInfo> _executionInfos;
        private readonly object _objectInstance;


        private ThreadSafeRandom _randomizer;
        public Type ObjectType { get; private set; }
        public MethodInfo Method { get; private set; }

        public RunnableItem(Type type, List<MethodInfo> methods)
        {
            var methodValidator = new MethodValidator();

            foreach (var methodInfo in methods)
            {
                methodValidator.ValidateParametersTypes(methodInfo);
                methodValidator.ValidateReturnType(methodInfo);
                methodValidator.ValidateMethodKind(methodInfo);

            }
            
            _objectInstance = CreateInstanceFromType(type);
            if (_objectInstance == null)
            {
                IEnumerable<MethodInfo> staticMethods = methods.Where(m => !m.IsStatic);
                if (staticMethods.Any())
                {
                    throw new ArgumentException(string.Format("cannot invoke non-static method of static class {0}", type));
                }
            }

            ObjectType = type;
            Method = methods[0];

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
                        object paramValue;
                        if (p.ParameterType.IsGenericParameter)
                        {
                            paramValue = new object();
                        }
                        else
                        {
                            paramValue = Randomizer.RandomizeParamValue(p.ParameterType.Name);
                        }
                        
                        parameters[index] = paramValue;
                    }
                }


                try
                {
                    //if (execInfo.Cached != null)
                    //{
                    //    //special optimization when method has no return value and no params
                    //    execInfo.Cached();
                    //    result = null;
                    //}

                    object result = execInfo.Execute(_objectInstance, parameters);


                    if (log)
                    {
                        methodExecutionResult = new MethodExecutionResult();
                        methodExecutionResult.ReturnValue = result;
                        methodExecutionResult.Failed = false;
                        methodExecutionResult.ArgsValues = parameters;
                    }
                }
                catch (Exception e)
                {
                    methodExecutionResult = new MethodExecutionResult();
                    methodExecutionResult.Failed = true;
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

            if (type.IsInterface)
            {
                throw new Exception("interfaces are not allowed");
            }

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