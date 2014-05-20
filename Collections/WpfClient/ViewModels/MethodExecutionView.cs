using Collections;
using Collections.Messages;
using Collections.Runtime;
using GalaSoft.MvvmLight;

namespace WpfClient.ViewModels
{
    public class MethodExecutionView :  ViewModelBase, IGui
    {
        private MethodExecutionMessage _message;
        private IRunner _runner;
        private bool _isExpanded;
        public string Id { get; set; }

        public MethodExecutionView()
        {
            Message = new MethodExecutionMessage();
        }
        public void Initialize()
        {
           
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        public void Register(IRunner runner)
        {
            if (_runner != null)
            {
                _runner.RemoveUiListener(this);
            }
          
           
            _runner = runner;
            
            Message.Summary = _runner.GetCurrentState();
            _runner.AddUiListener(this);
        }

        public void Update(MethodExecutionMessage message)
        {
            Message = message;
            
        }


        public void Destroy()
        {
            Message = new MethodExecutionMessage();
        }

        public MethodExecutionMessage Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }


    }
}