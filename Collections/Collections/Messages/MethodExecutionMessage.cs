using System;
using System.Reflection;
using System.Text;

namespace Collections.Messages
{
    public class MethodExecutionMessage : Message
    {
        public MethodExecutionMessage()
        {
            
        }
        public MethodExecutionMessage(Type objectType, MethodInfo method, MethodExecutionResult methodExecutionResult, 
            TimeSpan timeElapsed, int progress)
        {
            ObjectType = objectType;
            Method = method;
            TotalExecutionTime = timeElapsed;
            Progress = progress;

            MethodExecutionResult = methodExecutionResult;
        }
        public int Progress { get; private set; }
        public Type ObjectType { get; private set; }
        public MethodInfo Method { get; private set; }
        public TimeSpan TotalExecutionTime { get; private set; }
        public MethodExecutionResult MethodExecutionResult { get; private set; }

        public MethodExecutionResultAggregation Summary { get; set; }

       

        public override string ToString()
        {

            var sb = new StringBuilder(50);
            sb.AppendFormat("Type: {0}\nMethod: {1}\nExecutionTime: {2}\nProgress: {3}\nReturnValue: {4}\n",
                ObjectType,
                Method.Name,
                TotalExecutionTime.Seconds + ":" + TotalExecutionTime.Milliseconds,
                Progress,
                MethodExecutionResult.ReturnValue);
            return sb.ToString();

        }
    }
}