using System;

namespace CollectionsSOLID
{
    public interface IBehavior
    {
        MethodExecution Update(bool log);

        Type GetObjectType();
       

    }

    
}
