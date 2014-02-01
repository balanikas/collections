using System.ComponentModel;
using System.Windows.Input;
using Collections;
using MahApps.Metro.Controls;

namespace WpfClient
{
    public partial class RunnerInfoFlyout : Flyout, INotifyPropertyChanged
    {
        private bool canCloseFlyout;

        private ICommand closeCmd;

        public RunnerInfoFlyout()
        {
            InitializeComponent();
        }

        public bool CanCloseFlyout
        {
            get { return canCloseFlyout; }
            set
            {
                if (Equals(value, canCloseFlyout))
                {
                    return;
                }
                canCloseFlyout = value;
                RaisePropertyChanged("CanCloseFlyout");
            }
        }

        public ICommand CloseCmd
        {
            get
            {
                return closeCmd ?? (closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => CanCloseFlyout,
                    ExecuteDelegate = x => IsOpen = false
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddContent(RunSummaryMessage message)
        {
            if (message == null)
            {
                return;
            }

            txtType.Text = message.ObjectType.ToString();
            txtExecutionTime.Text = message.ExecutionTime.ToString();
            txtMethod.Text = message.MethodName;
            string failpercentage = ((message.FailedExecutionsCount/(double) message.ExecutionsCount)*100) + "%";

            txtFailRate.Text = failpercentage + " (" + message.FailedExecutionsCount + "/" + message.ExecutionsCount +
                               ")";
            txtAvgMethodExecutionTime.Text = message.AvgMethodExecutionTimeInMs + " ms";
            txtMinMethodExecutionTime.Text = message.MinMethodExecutionTime + " ms";
            txtMaxMethodExecutionTime.Text = message.MaxMethodExecutionTime + " ms";
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}