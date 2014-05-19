using System;
using System.Text;

namespace Collections.Messages
{
    public class MethodExecutionMessage : Message
    {
        public MethodExecutionMessage()
        {
            
        }
        public MethodExecutionMessage(Type objectType, MethodExecutionResult methodExecutionResult, TimeSpan timeElapsed, int progress)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;
            Progress = progress;

            MethodExecutionResult = methodExecutionResult;
        }
        public int Progress { get; private set; }
        public Type ObjectType { get; private set; }

        public MethodExecutionResult MethodExecutionResult { get; private set; }

        public MethodExecutionSummaryMessage Summary { get; set; }

        public TimeSpan ExecutionTime { get; private set; }

        public override string ToString()
        {

            var sb = new StringBuilder(50);
            sb.AppendFormat("Type: {0}\nMethod: {1}\nExecutionTime: {2}\nProgress: {3}\nReturnValue: {4}\n",
                ObjectType,
                MethodExecutionResult.Name,
                ExecutionTime.Seconds + ":" + ExecutionTime.Milliseconds,
                Progress,
                MethodExecutionResult.ReturnValue);
            return sb.ToString();

        }
    }
}