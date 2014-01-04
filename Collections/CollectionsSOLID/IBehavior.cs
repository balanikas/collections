using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CollectionsSOLID
{
    public interface IBehavior
    {
        bool Update(ILogger logger);

        bool UpdateAndLog(ILogger logger);
        Type GetObjectType();
        Type GetCollectionType();
      



    }

    
}
