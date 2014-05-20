using System;

namespace Collections
{
    public class MethodExecutionResult
    {
        public MethodExecutionResult()
        {
            Success = true;
        }

        public string Name { get; set; }
        public object[] ArgsValues { get; set; }

        public object ReturnValue { get; set; }

        public bool Success { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ErrorMessage { get; set; }

       
    }
}