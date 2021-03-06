using System;
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

        public object Execute(object instance, object[] parameters)
        {
           
            if (MethodInfo.IsGenericMethod)
            {
                return null;
            }
            else
            {
                return MethodInfo.Invoke(instance, parameters);
            }
        }

        public MethodInfo MethodInfo { get; private set; }
        public ParameterInfo[] ParameterInfos { get; private set; }
        public Action Cached { get; private set; }
    }
}