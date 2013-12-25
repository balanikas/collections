using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public enum ObjectType
    {
        BackgroundWorkerBased,
        ParallelTaskBased
    }
    public static class ObjectFactory
    {


        public static IRunner Get (ObjectType type,IBehavior behavior, IGui gui, int loopCount = 1000000)
        {
            switch (type)
            {
                case ObjectType.BackgroundWorkerBased:
                    return new BWBasedRunner(behavior, gui, loopCount);
                case ObjectType.ParallelTaskBased:
                    return new TplBasedRunner(behavior, gui, loopCount);
                default:
                    throw new NotImplementedException();
                    
            }
        }
    }
}
