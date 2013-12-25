using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient
{
    public class RightClickEventArgs : EventArgs
    {
        public string EventInfo { get; private set; }

        public RightClickEventArgs(string info)
        {
            EventInfo = info;
        }
      
    }

    public class MouseOverEventArgs : EventArgs
    {
        public string EventInfo { get; private set; }

        public MouseOverEventArgs(string info)
        {
            EventInfo = info;
        }

    }

    public delegate void RightClickEventHandler(object source, RightClickEventArgs e);
    public delegate void MouseOverEventHandler(object source, MouseOverEventArgs e);
    
}
