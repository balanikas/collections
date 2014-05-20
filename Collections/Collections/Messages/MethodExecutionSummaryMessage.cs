﻿using System;
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
            AvgMethodExecutionTimeInMs = items.Average(x => x.ExecutionTime.TotalMilliseconds);
            MinMethodExecutionTime = items.Min(x => x.ExecutionTime.TotalMilliseconds);
            MaxMethodExecutionTime = items.Max(x => x.ExecutionTime.TotalMilliseconds);
            
            //FailedExecutionsCount = items.Count(x => !x.Success);
            MethodName = items.First().Name;
        }
    }
}