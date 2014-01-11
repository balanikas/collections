using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public class UIMessage : Message
    {
        public ObjectState ObjectState { get; private set; }
        public Type ObjectType { get; private set; }

        public string MethodName { get; set; }


        public TimeSpan ExecutionTime { get; private set; }

        public UIMessage(Type objectType, string methodName, TimeSpan timeElapsed, int progress,ObjectState state)
        {
            ObjectType = objectType;
            ExecutionTime = timeElapsed;
            Progress = progress;

            ObjectState = state;
            MethodName = methodName;
        }

        public override string ToString()
        {
            return
                "Type: " + ObjectType + "\n" +
                "Method: " + MethodName + "\n" +
                "ExecutionTime: " + ExecutionTime.Seconds + ":" + ExecutionTime.Milliseconds + "\n" +
                "Progress: " + Progress + "\n";
        }
    }
}
