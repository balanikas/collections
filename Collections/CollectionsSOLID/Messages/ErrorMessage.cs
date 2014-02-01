namespace Collections.Messages
{
    public class ErrorMessage : Message
    {
        public ErrorMessage(string message, int progress)
        {
            Message = message;
            Progress = progress;
        }

        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}