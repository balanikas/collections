using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public enum ObjectState {
        Running,
        Finished,
        Unknown
    }

    public class Message
    {
        public int Progress { get; protected set; }
    }

    public class ErrorMessage : Message
    {
        
        public string Message { get; private set; }

        public ErrorMessage(string message, int progress)
        {
            Message = message;
            Progress = progress;
        }
        public override string ToString()
        {
            return Message;
                
        }
    }


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
