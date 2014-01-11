using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CollectionsSOLID
{
    public class RunSummaryMessage : Message
    {
        public ObjectState ObjectState { get; private set; }
        public Type ObjectType { get; private set; }
        public string MethodName { get; set; }
        public TimeSpan ExecutionTime { get; private set; }
        public double AvgMethodExecutionTimeInMs { get; private set; }
        public int ExecutionsCount { get; private set; }
        public int FailedExecutionsCount { get; private set; }



        public RunSummaryMessage(Type objectType, TimeSpan timeElapsed, IEnumerable<MethodExecution> methodExecutions)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;

            
            Summarize(methodExecutions);
        }

        private void Summarize(IEnumerable<MethodExecution> methodExecutions)
        {
            AvgMethodExecutionTimeInMs = methodExecutions.Average(x => x.ExecutionTime.TotalMilliseconds);
            ExecutionsCount = methodExecutions.Count();
            FailedExecutionsCount = methodExecutions.Where(x => !x.Success).Count();
            MethodName = methodExecutions.First().Name;
            
        }
    }
}
