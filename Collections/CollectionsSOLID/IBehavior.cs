using System;

namespace CollectionsSOLID
{
    public interface IBehavior
    {
        MethodExecution Update();

        Type GetObjectType();
       

    }

    
}
