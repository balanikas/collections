using System;

namespace Collections.Messages
{
    public class MethodExecutionMessage : Message
    {
        public MethodExecutionMessage(Type objectType, MethodExecutionResult methodExecutionResult, TimeSpan timeElapsed, int progress)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;
            Progress = progress;

            MethodExecutionResult = methodExecutionResult;
        }

        public Type ObjectType { get; private set; }

        public MethodExecutionResult MethodExecutionResult { get; private set; }


        public TimeSpan ExecutionTime { get; private set; }

        public override string ToString()
        {
            return
                "Type: " + ObjectType + "\n" +
                "Method: " + MethodExecutionResult.Name + "\n" +
                "ExecutionTime: " + ExecutionTime.Seconds + ":" + ExecutionTime.Milliseconds + "\n" +
                "Progress: " + Progress + "\n";
        }
    }
}