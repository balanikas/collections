namespace Collections
{
    public interface ILogSubscriber
    {
        void Notify(LogMessage message);
    }
}