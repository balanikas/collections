using Collections.Messages;

namespace Collections
{
    public interface IGui
    {
        string Id { get; set; }
        void Initialize();
        void Update(MethodExecutionMessage message);

        void Update(MethodExecutionSummaryMessage message);
        void Destroy();


    }
}