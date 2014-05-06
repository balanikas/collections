using System;
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
}