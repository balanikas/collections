using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

    public class KeyPressedEventArgs 
    {
        public string EventInfo { get; private set; }
        public System.Windows.Input.Key Key { get; private set; }
        public KeyPressedEventArgs(string info, Key key)
        {
            EventInfo = info;
            Key = key;
        }

    }

    public delegate void RightClickEventHandler(object source, RightClickEventArgs e);
    public delegate void MouseOverEventHandler(object source, MouseOverEventArgs e);
    public delegate void KeyPressedEventHandler(object source, KeyPressedEventArgs e);
    
}
