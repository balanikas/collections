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
        void Update();
        Type GetObjectType();
        Type GetCollectionType();
      
    }

    
}
