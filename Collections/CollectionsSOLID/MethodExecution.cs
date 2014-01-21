using System;
using System.Collections.Generic;

namespace CollectionsSOLID
{
    public class MethodExecution
    {
      
        public string Name {get;set;}
        public List<Object> ArgsValues {get;set;}

        public object ReturnValue {get;set;}

        public bool Success {get;set;}
        public TimeSpan ExecutionTime {get;set;}
        public string ErrorMessage {get;set;}

        public MethodExecution()
        {
            ArgsValues = new List<object>();
            Success = true;
        }
    }
}
