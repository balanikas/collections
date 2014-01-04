using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public enum ObjectState {
        Running,
        Finished,
        Unknown
    }

    public class Message
    {
        public int Progress { get; protected set; }
    }

    public class ErrorMessage : Message
    {
        
        public string Message { get; private set; }

        public ErrorMessage(string message, int progress)
        {
            Message = message;
            Progress = progress;
        }
        public override string ToString()
        {
            return Message;
                
        }
    }


}
