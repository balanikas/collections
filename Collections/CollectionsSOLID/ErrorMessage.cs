namespace CollectionsSOLID
{
    public class ErrorMessage : Message
    {

        public string Message { get; private set; }

        public ErrorMessage(string message, int progress)
        {
            Message = message;
            Progress = progress;
        }
        public override string ToString()
        {
            return Message;

        }
    }
}
