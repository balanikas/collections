namespace Collections
{
    public interface IRunner
    {
        string Id { get; }
        void Start();
        void Destroy();
        bool IsAlive();
        RunSummaryMessage GetState();

        void AddUiListener(IGui listener);
    }
}