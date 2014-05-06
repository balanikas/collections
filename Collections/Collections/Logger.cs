using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public class Logger : ILogger
    {
        private readonly Queue<LogMessage> _logBuffer;
        private List<ILogSubscriber> _subscribers; 
        private int _errorCount;

        public Logger()
        {
            _subscribers = new List<ILogSubscriber>();
            _logBuffer = new Queue<LogMessage>();

            var traceListener = new SynchronizedTraceListener(Info);
            Trace.Listeners.Add(traceListener);
        }

        public void Subscribe(ILogSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        private void NotifySubscribers(LogMessage message)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Notify(message);
            }
        }
        public void AddTraceListener(TraceWriterHandler handler)
        {

        }
        public int Count
        {
            get { return _logBuffer.Count; }
        }

        public int ErrorCount
        {
            get { return _errorCount; }
        }

        public void Info(string message)
        {
            _logBuffer.Enqueue(new LogMessage { IsError = false, Message = message });
        }

        public void InfoNow(string message)
        {

            NotifySubscribers(new LogMessage
            {
                IsError = false,
                Message = message
            });
           
        }

        public void Error(string message)
        {
            _errorCount++;
            _logBuffer.Enqueue(new LogMessage { IsError = true, Message = message });
        }

        public void ErrorNow(string message)
        {
            _errorCount++;
            NotifySubscribers(new LogMessage
            {
                IsError = true,
                Message = message
            });
        }

        public void Flush()
        {
           
            while (_logBuffer.Count > 0)
            {
                LogMessage logMessage = _logBuffer.Dequeue();
                NotifySubscribers(logMessage);
            }
        }
    }
}
