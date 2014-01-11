using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<MethodExecution> MethodExecutions { get; private set; }

        public RunSummaryMessage(Type objectType, TimeSpan timeElapsed, int progress, List<MethodExecution> methodExecutions)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;
            Progress = progress;
            MethodExecutions = methodExecutions;

            Summarize();


        }

        private void Summarize()
        {
            AvgMethodExecutionTimeInMs = MethodExecutions.Average(x => x.ExecutionTime.TotalMilliseconds);
            ExecutionsCount = MethodExecutions.Count;
            FailedExecutionsCount = MethodExecutions.Where(x => !x.Success).Count();
            
        }
    }
}
