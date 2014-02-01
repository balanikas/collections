using System;

namespace Collections.Messages
{
    public class UIMessage : Message
    {
        public UIMessage(Type objectType, MethodExecution methodExecution, TimeSpan timeElapsed, int progress,
            ObjectState state)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;
            Progress = progress;

            ObjectState = state;
            MethodExecution = methodExecution;
        }

        public ObjectState ObjectState { get; private set; }
        public Type ObjectType { get; private set; }

        public MethodExecution MethodExecution { get; private set; }


        public TimeSpan ExecutionTime { get; private set; }

        public override string ToString()
        {
            return
                "Type: " + ObjectType + "\n" +
                "Method: " + MethodExecution.Name + "\n" +
                "ExecutionTime: " + ExecutionTime.Seconds + ":" + ExecutionTime.Milliseconds + "\n" +
                "Progress: " + Progress + "\n";
        }
    }
}