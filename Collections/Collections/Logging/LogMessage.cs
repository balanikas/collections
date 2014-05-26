namespace Collections
{
    public class LogMessage
    {
        public LogMessage(string message, bool isError)
        {
            Message = message;
            IsError = isError;
        }
        public string Message { get; private set; }
        public bool IsError { get; private set; }
    }
}