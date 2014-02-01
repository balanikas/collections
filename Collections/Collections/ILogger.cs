namespace Collections
{
    public interface ILogger
    {
        int Count { get; }

        int ErrorCount { get; }
        void Info(string message);
        void InfoNow(string message);
        void Error(string message);

        void ErrorNow(string message);
        void Flush();
    }
}