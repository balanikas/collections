using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections.Messages
{
    public class MethodExecutionResultAggregation : Message
    {

        public MethodExecutionResultAggregation(MethodExecutionResultAggregation copy)
        {
            AvgMethodExecutionTime = copy.AvgMethodExecutionTime;
            MaxMethodExecutionTime = copy.MaxMethodExecutionTime;
            MinMethodExecutionTime = copy.MinMethodExecutionTime;
            ExecutionsCount = copy.ExecutionsCount;
            FailedExecutionsCount = copy.FailedExecutionsCount;
        }

        public MethodExecutionResultAggregation()
        {
           
        }

        public double AvgMethodExecutionTime { get; private set; }
        public int ExecutionsCount { get; private set; }
        public int FailedExecutionsCount { get; private set; }

        public double MaxMethodExecutionTime { get; private set; }

        public double MinMethodExecutionTime { get; private set; }

        public void Add(MethodExecutionResult result)
        {
            AvgMethodExecutionTime = (AvgMethodExecutionTime + result.ExecutionTime.TotalMilliseconds )/2;
            MinMethodExecutionTime = 1;
            MaxMethodExecutionTime = 1;
            ExecutionsCount++;
            FailedExecutionsCount += result.Failed ? 1 : 0;
        }



        //public void Aggregate(IEnumerable<MethodExecutionResult> methodExecutions)
        //{

        //    if (!methodExecutions.Any())
        //    {
        //        return;
        //    }
        //    AvgMethodExecutionTime = Math.Round(methodExecutions.Average(x => x.ExecutionTime.TotalMilliseconds), 3);
        //    MinMethodExecutionTime = Math.Round(methodExecutions.Min(x => x.ExecutionTime.TotalMilliseconds), 3);
        //    MaxMethodExecutionTime = Math.Round(methodExecutions.Max(x => x.ExecutionTime.TotalMilliseconds), 3);

        //    ExecutionsCount = methodExecutions.Count();
        //    FailedExecutionsCount = methodExecutions.Count(x => x.Failed);
        //}
    }
}