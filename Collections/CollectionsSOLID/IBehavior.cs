using System;

namespace Collections
{
    public interface IBehavior
    {
        MethodExecution Update(bool log);

        Type GetObjectType();
    }
}