namespace Collections.Logging
{
    public interface ILogSubscriber
    {
        void Notify(LogMessage message);
    }
}