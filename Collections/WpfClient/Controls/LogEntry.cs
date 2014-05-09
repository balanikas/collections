using System;
using System.Windows.Media;

namespace WpfClient.Controls
{
    public class LogEntry : PropertyChangedBase
    {
        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }

        public Brush TextBrush { get; set; }
    }
}