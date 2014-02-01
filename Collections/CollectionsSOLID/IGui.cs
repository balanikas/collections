namespace Collections
{
    public interface IGui
    {
        string Id { get; set; }
        void Draw();
        void Update(UIMessage message);
        void Destroy();
    }
}