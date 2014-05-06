using Collections.Messages;

namespace Collections
{
    public interface IRunner
    {
        string Id { get; }
        void Start();
        void Destroy();
        bool IsAlive();
        RunSummaryMessage GetCurrentState();

        void AddUiListener(IGui listener);
    }
}