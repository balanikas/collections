using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public class RunnerMessage : Message
    {
        public ObjectState ObjectState { get; private set; }
        public Type ObjectType { get; private set; }
        public Type CollectionType { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }
        public int AllocatedMemory { get; private set; }
        public RunnerMessage(Type objectType, Type collectionType, TimeSpan timeElapsed, int progress, int allocatedMemory, ObjectState state)
        {
            ObjectType = objectType;
            CollectionType = collectionType;
            ExecutionTime = timeElapsed;
            Progress = progress;
            AllocatedMemory = allocatedMemory;
            ObjectState = state;
        }

        public override string ToString()
        {
            return
                "Object: " + ObjectType + "\n" +
                "Collection: " + CollectionType + "\n" +
                "ExecutionTime: " + ExecutionTime.Seconds + ":" + ExecutionTime.Milliseconds + "\n" +
                "Progress: " + Progress + "\n" +
                "AllocatedMemory: " + AllocatedMemory + "\n";
        }
    }
}
