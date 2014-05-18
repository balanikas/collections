using System;
using System.Windows.Input;

namespace WpfClient
{
    public class RightClickEventArgs : EventArgs
    {
        public RightClickEventArgs(string info)
        {
            EventInfo = info;
        }

        public string EventInfo { get; private set; }
    }

    public class LeftClickEventArgs : EventArgs
    {
        public LeftClickEventArgs(string info)
        {
            EventInfo = info;
        }

        public string EventInfo { get; private set; }
    }

    public class MouseOverEventArgs : EventArgs
    {
        public MouseOverEventArgs(string info)
        {
            EventInfo = info;
        }

        public string EventInfo { get; private set; }
    }

    public class KeyPressedEventArgs
    {
        public KeyPressedEventArgs(string info, Key key)
        {
            EventInfo = info;
            Key = key;
        }

        public string EventInfo { get; private set; }
        public Key Key { get; private set; }
    }

    public delegate void RightClickEventHandler(object source, RightClickEventArgs e);

    public delegate void LeftClickEventHandler(object source, LeftClickEventArgs e);

    public delegate void MouseOverEventHandler(object source, MouseOverEventArgs e);

    public delegate void KeyPressedEventHandler(object source, KeyPressedEventArgs e);
}