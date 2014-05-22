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
            var execTime = result.ExecutionTime.TotalMilliseconds;
            AvgMethodExecutionTime = (AvgMethodExecutionTime + execTime) / 2;
            if (MinMethodExecutionTime == default(double))
            {
                MinMethodExecutionTime = execTime;
            }
            else
            {
                if (MinMethodExecutionTime > execTime)
                {
                    MinMethodExecutionTime = execTime;
                }
            }
            if (MaxMethodExecutionTime == default(double))
            {
                MaxMethodExecutionTime = execTime;
            }
            else
            {
                if (MaxMethodExecutionTime < execTime)
                {
                    MaxMethodExecutionTime = execTime;
                }
            }
            ExecutionsCount++;

            if (result.Failed)
            {
                FailedExecutionsCount ++;
            }
            
        }

    }
}