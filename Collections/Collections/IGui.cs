using Collections.Messages;

namespace Collections
{
    public interface IGui
    {
        string Id { get; set; }
        void Draw();
        void Update(MethodExecutionMessage message);
        void Destroy();


    }
}