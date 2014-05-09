using System;

namespace Collections.Runtime
{
    public interface IRunnable
    {
        MethodExecutionResult Update(bool log);

        Type ObjectType { get; }
    }
}