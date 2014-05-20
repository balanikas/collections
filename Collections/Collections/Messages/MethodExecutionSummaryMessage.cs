using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections.Messages
{
    public class MethodExecutionSummaryMessage : Message
    {
        public MethodExecutionSummaryMessage(Type objectType, TimeSpan timeElapsed, IEnumerable<MethodExecutionResult> methodExecutions, int executionCount)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;

            ExecutionsCount = executionCount;
            Summarize(methodExecutions);
        }

        public Type ObjectType { get; private set; }
        public string MethodName { get; set; }
        public TimeSpan ExecutionTime { get; private set; }
        public double AvgMethodExecutionTimeInMs { get; private set; }
        public int ExecutionsCount { get; private set; }
        public int FailedExecutionsCount { get; private set; }

        public double MaxMethodExecutionTime { get; private set; }

        public double MinMethodExecutionTime { get; private set; }

        private void Summarize(IEnumerable<MethodExecutionResult> methodExecutions)
        {
            List<MethodExecutionResult> items = methodExecutions.ToList();
            if (!items.Any())
            {
                return;
            }
            AvgMethodExecutionTimeInMs = Math.Round(items.Average(x => x.ExecutionTime.TotalMilliseconds),3);
            MinMethodExecutionTime = Math.Round(items.Min(x => x.ExecutionTime.TotalMilliseconds),3);
            MaxMethodExecutionTime = Math.Round(items.Max(x => x.ExecutionTime.TotalMilliseconds),3);
            
            FailedExecutionsCount = items.Count(x => !x.Success);
            MethodName = items.First().Name;
        }
    }
}