﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Collections
{
    internal class ExecutionInfo
    {
        public ExecutionInfo(MethodInfo methodInfo, object objectInstance)
        {
            Debug.Assert(methodInfo != null);

            MethodInfo = methodInfo;
            ParameterInfos = methodInfo.GetParameters();
            if (!methodInfo.GetParameters().Any() && methodInfo.ReturnParameter.ParameterType == typeof (void))
            {
                if (methodInfo.IsStatic)
                {
                    Cached = (Action) Delegate.CreateDelegate(typeof (Action), null, methodInfo);
                }
                else
                {
                    Cached = (Action) Delegate.CreateDelegate(typeof (Action), objectInstance, methodInfo);
                }
            }
        }

        public MethodInfo MethodInfo { get; private set; }
        public ParameterInfo[] ParameterInfos { get; private set; }
        public Action Cached { get; private set; }
    }

    public class ObjectBehavior : IBehavior
    {
        private readonly List<ExecutionInfo> _executionInfos;
        private readonly object _objectInstance;
        private readonly Type _objectType;

        private ThreadSafeRandom _randomizer;

        public ObjectBehavior(Type type, List<MethodInfo> methods)
        {
            _objectType = type;
            bool areMethodsValid = Utils.MethodsUseSupportedTypes(methods);
            if (!areMethodsValid)
            {
                throw new ArgumentException("method(s) contains unsupported types");
            }

            _objectInstance = CreateInstanceFromType(_objectType);
            if (_objectInstance == null)
            {
                IEnumerable<MethodInfo> staticMethods = methods.Where(m => !m.IsStatic);
                if (staticMethods.Any())
                {
                    throw new ArgumentException("cannot invoke non-static method of static class");
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

        public MethodExecution Update(bool log)
        {
            MethodExecution methodExecution = null;

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
                    if (paramCount == 0)
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
                        methodExecution = new MethodExecution();
                        methodExecution.ReturnValue = result;
                        methodExecution.Success = true;
                        methodExecution.ArgsValues = parameters;
                        methodExecution.Name = execInfo.MethodInfo.Name;
                    }
                }
                catch (Exception e)
                {
                    methodExecution = new MethodExecution();
                    methodExecution.Success = false;
                    methodExecution.Name = execInfo.MethodInfo.Name;
                    string errorMsg = e.InnerException != null ? e.InnerException.Message : e.Message;
                    methodExecution.ErrorMessage = errorMsg;
                    return methodExecution;
                }
            }

            return methodExecution;
        }


        public Type GetObjectType()
        {
            return _objectType;
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
                    //todo: this doesnt work because lets say a type has many ctors with params,
                    //which one to choose? lets limit now to only allow empty constructors...until a better solution
                    //var parameters = new List<object>();


                    //var constructors = type.GetConstructors();
                    //foreach (var c in constructors)
                    //{
                    //    var constructorParameters = c.GetParameters();
                    //    ci = type.GetConstructor(constructorParameters.Select(p => p.ParameterType).ToArray());

                    //    foreach (var item in constructorParameters)
                    //    {
                    //        parameters.Add(Randomizer.RandomizeParamValue(item.ParameterType.Name));
                    //    }
                    //    obj = ci.Invoke(parameters.ToArray());

                    //}
                }
                //constructor is paramless
                obj = ci.Invoke(new object[] {});
            }
            return obj;
        }
    }
}