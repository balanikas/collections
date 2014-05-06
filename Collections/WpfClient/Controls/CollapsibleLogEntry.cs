using System.Collections.Generic;

namespace WpfClient.Controls
{
    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }
}