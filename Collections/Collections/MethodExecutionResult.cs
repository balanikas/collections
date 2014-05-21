using System;

namespace Collections
{
    public class MethodExecutionResult
    {
        public object[] ArgsValues { get; set; }
        public object ReturnValue { get; set; }
        public bool Failed { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}