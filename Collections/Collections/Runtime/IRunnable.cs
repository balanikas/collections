using System;
using System.Reflection;

namespace Collections.Runtime
{
    public interface IRunnable
    {
        MethodExecutionResult Update(bool log);

        Type ObjectType { get; }
        MethodInfo Method { get; }
    }
}