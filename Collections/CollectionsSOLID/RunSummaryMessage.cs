using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections
{
    public class RunSummaryMessage : Message
    {
        public RunSummaryMessage(Type objectType, TimeSpan timeElapsed, IEnumerable<MethodExecution> methodExecutions)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;


            Summarize(methodExecutions);
        }

        public ObjectState ObjectState { get; private set; }
        public Type ObjectType { get; private set; }
        public string MethodName { get; set; }
        public TimeSpan ExecutionTime { get; private set; }
        public double AvgMethodExecutionTimeInMs { get; private set; }
        public int ExecutionsCount { get; private set; }
        public int FailedExecutionsCount { get; private set; }

        public double MaxMethodExecutionTime { get; private set; }

        public double MinMethodExecutionTime { get; private set; }

        private void Summarize(IEnumerable<MethodExecution> methodExecutions)
        {
            List<MethodExecution> items = methodExecutions.ToList();
            if (!items.Any())
            {
                return;
            }
            AvgMethodExecutionTimeInMs = items.Average(x => x.ExecutionTime.TotalMilliseconds);
            MinMethodExecutionTime = items.Min(x => x.ExecutionTime.TotalMilliseconds);
            MaxMethodExecutionTime = items.Max(x => x.ExecutionTime.TotalMilliseconds);
            ExecutionsCount = items.Count();
            FailedExecutionsCount = items.Count(x => !x.Success);
            MethodName = items.First().Name;
        }
    }
}