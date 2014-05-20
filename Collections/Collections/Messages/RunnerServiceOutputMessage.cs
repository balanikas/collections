namespace Collections.Messages

{


    public class RunnerServiceOutputMessage
    {
        public ServiceMessageState State { get; private set; }
        public string ErrorMessage { get; private set; }

        public RunnerServiceOutputMessage(ServiceMessageState state = ServiceMessageState.NotHandled, string error = null)
        {
            State = state;
            ErrorMessage = error;
        }
    }
}