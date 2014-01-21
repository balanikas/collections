namespace CollectionsSOLID
{
    public interface ILogger
    {
        void Info(string message);
        void InfoNow(string message);
        void Error(string message);

        void ErrorNow(string message);
        int Count { get; }

        int ErrorCount { get; }
        void Flush();
    }
}
