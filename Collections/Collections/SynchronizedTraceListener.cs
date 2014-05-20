using System.Diagnostics;

namespace Collections
{
        public delegate void TraceWriterHandler(string message);

        public class SynchronizedTraceListener : TraceListener
    {
        private readonly TraceWriterHandler _messageHandler;

        public SynchronizedTraceListener(TraceWriterHandler writeHandler)
        {
            _messageHandler = writeHandler;
        }

        public override void Write(string message)
        {
            _messageHandler(message);
        }

        public override void WriteLine(string message)
        {
            _messageHandler(message + System.Environment.NewLine);
            
        }
    }
}
