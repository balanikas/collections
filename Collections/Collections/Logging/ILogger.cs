namespace Collections.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void InfoNow(string message);
        void Error(string message);

        void ErrorNow(string message);
        void Flush();

        void Subscribe(ILogSubscriber subscriber);
    }
}