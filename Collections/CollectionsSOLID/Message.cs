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

   


}
