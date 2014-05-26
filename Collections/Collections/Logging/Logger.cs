using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Collections.Logging
{
    public class Logger : ILogger
    {
        private readonly ConcurrentQueue<LogMessage> _logBuffer;
        private readonly List<ILogSubscriber> _subscribers; 
        private static readonly object _syncLock = new object();
        public Logger()
        {
            _subscribers = new List<ILogSubscriber>();
            _logBuffer = new ConcurrentQueue<LogMessage>();

            var traceListener = new SynchronizedTraceListener(InfoNow);
            Trace.Listeners.Add(traceListener);
        }

        public void Subscribe(ILogSubscriber subscriber)
        {
            lock (_syncLock)
            {
                if (!_subscribers.Contains(subscriber))
                {
                    _subscribers.Add(subscriber);
                }
            }
        }

        public void Info(string message)
        {
            _logBuffer.Enqueue(new LogMessage ( message,false ));
        }

        public void InfoNow(string message)
        {
            lock (_syncLock)
            {
                NotifySubscribers(new LogMessage(message, false));
            }
        }

        public void Error(string message)
        {
            _logBuffer.Enqueue(new LogMessage (message,true));
        }

        public void ErrorNow(string message)
        {
            lock (_syncLock)
            {
                NotifySubscribers(new LogMessage(message, true));
            }
        }

        public void Flush()
        {
            lock (_syncLock)
            {
                while (_logBuffer.Count > 0)
                {
                    LogMessage logMessage;
                    if (_logBuffer.TryDequeue(out logMessage))
                    {
                        NotifySubscribers(logMessage);
                    }
                }
            }
        }

        private void NotifySubscribers(LogMessage message)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Notify(message);
            }
        }
    }
}
