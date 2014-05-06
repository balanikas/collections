using System;

namespace Collections
{
    public interface IRunnable
    {
        MethodExecutionResult Update(bool log);

        Type GetObjectType();
    }
}