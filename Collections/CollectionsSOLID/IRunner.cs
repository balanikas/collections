namespace CollectionsSOLID
{
    public interface IRunner
    {
        string Id { get; }
        void Start();
        void Destroy();
        bool IsAlive();
        RunSummaryMessage GetState();

        void AddUIListener(IGui listener);
    }



}
